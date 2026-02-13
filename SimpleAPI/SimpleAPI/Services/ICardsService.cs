
using SimpleAPI.Domain.Cards;

namespace SimpleAPI.Services;

public interface ICardsService
{
    Task<CardInfo> GetCardInfo(string id);
}
