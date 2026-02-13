namespace SimpleAPI.Domain.Cards;

public record CardInfo(
    string CardId,
    string StateDescription,
    string ValidityEnd);
