using System.Text.Json.Serialization;

namespace SimpleAPI.Domain.Cards;

public class CardValidity
{
    [JsonPropertyName("validity_start")]
    public DateTime? ValidityStart { get; set; }

    [JsonPropertyName("validity_end")]
    public DateTime? ValidityEnd { get; set; }
}
