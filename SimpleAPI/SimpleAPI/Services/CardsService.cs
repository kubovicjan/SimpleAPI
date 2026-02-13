using Microsoft.Extensions.Caching.Memory;
using SimpleAPI.Domain.Cards;
using SimpleAPI.Services.ExternalServices;

namespace SimpleAPI.Services;

public class CardsService : ICardsService
{
    private const int CardIdLength = 10;
    private const int CacheDurationInMinutes = 5;

    private IExternalCardsDataProvider ExternalCardsData { get; }
    private ILogger<CardsService> Logger { get; }
    public IMemoryCache MemoryCache { get; }

    public CardsService(
        IExternalCardsDataProvider externalCardsData,
        ILogger<CardsService> logger,
        IMemoryCache memoryCache
        )
    {
        ExternalCardsData = externalCardsData;
        Logger = logger;
        MemoryCache = memoryCache;
    }


    public async Task<CardInfo> GetCardInfo(string id)
    {
        Logger.LogInformation("Getting card info for card with id {CardId}", id);

        ValidateCardId(id);

        if (MemoryCache.TryGetValue(id, out CardInfo? cachedCardInfo))
        {
            Logger.LogInformation("Card info for card with id {CardId} retrieved from cache", id);

            return cachedCardInfo!;
        }

        var stateTask = ExternalCardsData.GetCardState(id);
        var validityTask = ExternalCardsData.GetCardValidity(id);

        await Task.WhenAll(stateTask, validityTask);

        var state = await stateTask;
        var validity = await validityTask;

        CheckStateResult(id, state);
        ExtractValidityResult(id, validity);

        Logger.LogInformation("Card info for card with id {CardId} retrieved successfully", id);

        var cardInfo = new CardInfo(
            id,
            state!.Content?.Description ?? string.Empty,
            validity.Content?.ValidityEnd?.ToString("dd.MM.yyyy") ?? string.Empty);

        MemoryCache.Set(id, cardInfo, TimeSpan.FromMinutes(CacheDurationInMinutes));

        return cardInfo;
    }

    private void ExtractValidityResult(string id, Refit.ApiResponse<CardValidity> validity)
    {
        if (!validity.IsSuccessful)
        {
            var message = $"Failed to get card validity for card with id {id}. Status code: {validity.StatusCode}";
            Logger.LogError("{Message}", message);
            throw new InvalidOperationException(message);
        }
    }

    private void CheckStateResult(string id, Refit.ApiResponse<CardState> state)
    {
        if (!state.IsSuccessful)
        {
            var message = $"Failed to get card state for card with id {id}. Status code: {state.StatusCode}";
            Logger.LogError("{Message}", message);
            throw new InvalidOperationException(message);
        }
    }

    private void ValidateCardId(string id)
    {
        if (string.IsNullOrWhiteSpace(id) && id.Length == CardIdLength)
        {
            var message = $"Invalid card id: {id}. Card id must be a non-empty string with a length of {CardIdLength} characters.";
            Logger.LogError("{Message}", message);
            throw new InvalidDataException(message);
        }
    }
}
