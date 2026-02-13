using System.Text.Json.Serialization;

namespace SimpleAPI.Domain.Cards;

public class CardState
{
    [JsonPropertyName("state_id")]
    public int? Id { get; set; }

    [JsonPropertyName("state_description")]
    public string? Description { get; set; }
}
