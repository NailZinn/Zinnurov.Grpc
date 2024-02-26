using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using JWT.Client.Protos.Secrets;

using var channel = GrpcChannel.ForAddress("http://localhost:5037");

var client = new Secrets.SecretsClient(channel);
using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5037");
httpClient.DefaultRequestVersion = new Version("2.0");

var token = await httpClient.GetAsync("/token/nail");

var headers = new Metadata();
headers.Add("Authorization", $"{token}");
var response = await client.GetSecretAsync(new Empty(), headers);

Console.WriteLine(response.Secret);