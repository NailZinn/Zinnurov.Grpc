using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Models;
using Server.Protos.WeatherForecast;

namespace Server.Services;

public class WeatherForecastStreamingService : WeatherForecast.WeatherForecastBase
{
    public override async Task GetTemperatureStream(Request request, IServerStreamWriter<Response> responseStream, ServerCallContext context)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://api.open-meteo.com");
        var startDate = request.StartDate.ToDateTime();
        var endDate = DateTime.UtcNow;

        while (startDate <= endDate && !context.CancellationToken.IsCancellationRequested)
        {
            await ProcessRequestAsync(startDate, httpClient, responseStream);
            await Task.Delay(1000);
            startDate = startDate.AddHours(2);
        }
    }

    private static async Task ProcessRequestAsync(DateTime date, HttpClient httpClient, IServerStreamWriter<Response> responseStream)
    {
        var stringDate = date.ToString("s")[..^3];
        var apiResponse = await httpClient.GetAsync($"/v1/forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m&start_hour={stringDate}&end_hour={stringDate}");
        
        if (!apiResponse.IsSuccessStatusCode) return;

        var temperatureInfo = await apiResponse.Content.ReadFromJsonAsync<TemperatureInfo>();

        var response = new Response
        {
            Date = Timestamp.FromDateTime(DateTime.Parse(temperatureInfo!.Hourly.Times[0]).ToUniversalTime()),
            Temperature = temperatureInfo.Hourly.Temperatures[0]
        };
        
        await responseStream.WriteAsync(response);
    }
}