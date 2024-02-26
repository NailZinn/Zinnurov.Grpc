using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using JWT.Server.Protos.Secrets;
using Microsoft.AspNetCore.Authorization;

namespace JWT.Server.Services;

[Authorize]
public class SecretService : Secrets.SecretsBase
{
    public override Task<Response> GetSecret(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new Response
        {
            Secret = "SUPER SECRET"
        });
    }
}