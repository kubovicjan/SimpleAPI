using Refit;
using SimpleAPI.Domain.Cards;

namespace SimpleAPI.Services.ExternalServices;

public interface IExternalCardsDataProvider
{
    [Get("/{id}/state")]
    Task<ApiResponse<CardState>> GetCardState(string id);

    [Get("/{id}/validity")]
    Task<ApiResponse<CardValidity>> GetCardValidity(string id);
}
