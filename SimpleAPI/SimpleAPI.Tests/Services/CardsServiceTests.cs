using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Refit;
using SimpleAPI.Domain.Cards;
using SimpleAPI.Services;
using SimpleAPI.Services.ExternalServices;
using Xunit;

namespace SimpleAPI.Tests.Services;

public class CardsServiceTests
{
    [Fact]
    public async Task GetCardInfo_CacheHit_ReturnsCachedValue()
    {
        var id = "1234567890";
        var cached = new CardInfo(id, "Cached", DateTime.UtcNow.AddDays(1).ToString("dd.MM.yyyy"));

        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set(id, cached);

        var external = new Mock<IExternalCardsDataProvider>(MockBehavior.Strict);
        var service = CreateService(external, cache);

        var result = await service.GetCardInfo(id);

        Assert.Same(cached, result);
        external.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetCardInfo_ExternalSuccess_ReturnsMappedAndCaches()
    {
        var id = "1234567890";
        var state = new CardState { Description = "Active" };
        var validity = new CardValidity
        {
            ValidityStart = DateTime.UtcNow.Date,
            ValidityEnd = DateTime.UtcNow.Date.AddDays(30)
        };

        var external = new Mock<IExternalCardsDataProvider>();
        external.Setup(x => x.GetCardState(id))
            .ReturnsAsync(CreateApiResponse(state, HttpStatusCode.OK));
        external.Setup(x => x.GetCardValidity(id))
            .ReturnsAsync(CreateApiResponse(validity, HttpStatusCode.OK));

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = CreateService(external, cache);

        var result = await service.GetCardInfo(id);

        var expected = new CardInfo(
            id,
            state.Description ?? string.Empty,
            validity.ValidityEnd?.ToString("dd.MM.yyyy") ?? string.Empty);

        Assert.Equal(expected, result);

        Assert.True(cache.TryGetValue(id, out CardInfo? cached));
        Assert.Equal(expected, cached);

        external.Verify(x => x.GetCardState(id), Times.Once);
        external.Verify(x => x.GetCardValidity(id), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetCardInfo_ExternalFailure_ThrowsInvalidOperationException(bool stateFails)
    {
        var id = "1234567890";
        var external = new Mock<IExternalCardsDataProvider>();

        external.Setup(x => x.GetCardState(id))
            .ReturnsAsync(stateFails
                ? CreateApiResponse<CardState>(null, HttpStatusCode.InternalServerError)
                : CreateApiResponse(new CardState(), HttpStatusCode.OK));

        external.Setup(x => x.GetCardValidity(id))
            .ReturnsAsync(stateFails
                ? CreateApiResponse(new CardValidity(), HttpStatusCode.OK)
                : CreateApiResponse<CardValidity>(null, HttpStatusCode.InternalServerError));

        var service = CreateService(external);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetCardInfo(id));
    }

    [Fact]
    public async Task GetCardInfo_InvalidCardId_ThrowsInvalidDataException()
    {
        var id = "          ";

        var external = new Mock<IExternalCardsDataProvider>(MockBehavior.Strict);
        var service = CreateService(external);

        await Assert.ThrowsAsync<InvalidDataException>(() => service.GetCardInfo(id));
    }

    private static CardsService CreateService(Mock<IExternalCardsDataProvider> external, IMemoryCache? cache = null)
    {
        return new CardsService(external.Object, NullLogger<CardsService>.Instance, cache ?? new MemoryCache(new MemoryCacheOptions()));
    }

    private static ApiResponse<T> CreateApiResponse<T>(T? content, HttpStatusCode statusCode)
    {
        var response = new HttpResponseMessage(statusCode);
        return new ApiResponse<T>(response, content, new RefitSettings());
    }
}