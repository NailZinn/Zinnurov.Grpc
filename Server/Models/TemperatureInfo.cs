using System.Text.Json.Serialization;
using Server.Services;

namespace Server.Models;

public class TemperatureInfo
{
    [JsonPropertyName("hourly")]
    public Hourly Hourly { get; set; } = default!;
}