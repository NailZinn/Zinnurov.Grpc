using System.Text.Json.Serialization;

namespace Server.Models;

public class Hourly
{
    [JsonPropertyName("time")]
    public string[] Times { get; set; } = default!;

    [JsonPropertyName("temperature_2m")]
    public double[] Temperatures { get; set; } = default!;
}