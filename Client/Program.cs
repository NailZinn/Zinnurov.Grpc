using Client.Protos.WeatherForecast;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("http://localhost:5285");

var client = new WeatherForecast.WeatherForecastClient(channel);

var result = client.GetTemperatureStream(new Request
{
    StartDate = Timestamp.FromDateTime(DateTime.UtcNow.AddHours(-int.Parse(args[0])))
});

while (await result.ResponseStream.MoveNext(new CancellationToken()) && !Console.KeyAvailable)
{
    // Date уже в UTC формате, но .ToDateTime() вычитает ещё 3 часа, конвертируя снова в UTC, поэтому .AddHours(3)
    var dateTime = result.ResponseStream.Current.Date.ToDateTime().AddHours(3);
    
    Console.WriteLine($"{DateTime.UtcNow:T} погода на {dateTime:g} = {result.ResponseStream.Current.Temperature}C");
}