using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWT.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            
            ValidateLifetime = true,
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Key"]!))
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<SecretService>();

app.MapGet("/token/{username}", ([FromServices] IConfiguration configuration, string username) =>
{
    var now = DateTime.UtcNow;
    var jwt = new JwtSecurityToken(
        issuer: configuration["JwtSettings:Issuer"],
        audience: configuration["JwtSettings:Audience"],
        notBefore: now,
        claims: [new Claim(ClaimTypes.NameIdentifier, username), new Claim(ClaimTypes.Role, "admin")],
        expires: now.Add(TimeSpan.FromMinutes(10)),
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(configuration["JwtSettings:Key"]!)),
            SecurityAlgorithms.HmacSha256));

    return Results.Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
});

app.Run();