using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LanPlanner.Api.Authentication;

public static class AuthenticationExtensions
{
    public const string AccessJwtHeader = "Cf-Access-Jwt-Assertion";

    public static IServiceCollection AddAccessJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(AccessJwtOptions.SectionName).Get<AccessJwtOptions>()
            ?? throw new InvalidOperationException($"Konfiguration '{AccessJwtOptions.SectionName}' fehlt.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.MapInboundClaims = false; // Claim bleibt "email"

                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
                };

                jwt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Headers.TryGetValue(AccessJwtHeader, out var token))
                        {
                            context.Token = token.ToString();
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
