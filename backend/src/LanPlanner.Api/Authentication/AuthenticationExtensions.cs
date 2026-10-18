using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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

        if (string.IsNullOrWhiteSpace(options.Issuer) || string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new InvalidOperationException("AccessJwt: Issuer und Audience müssen gesetzt sein.");
        }

        var useSigningKey = !string.IsNullOrWhiteSpace(options.SigningKey);
        var useJwks = !string.IsNullOrWhiteSpace(options.JwksUrl);

        if (useSigningKey == useJwks)
        {
            throw new InvalidOperationException(
                "AccessJwt: Genau eines von SigningKey (lokal, HS256) oder JwksUrl (Cloudflare, RS256) muss gesetzt sein.");
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.MapInboundClaims = false; // Claim bleibt "email"

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAlgorithms = [useSigningKey ? SecurityAlgorithms.HmacSha256 : SecurityAlgorithms.RsaSha256]
                };

                if (useSigningKey)
                {
                    parameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
                }
                else
                {
                    jwt.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                        options.JwksUrl,
                        new AccessJwksRetriever(options.Issuer),
                        new HttpDocumentRetriever { RequireHttps = true });
                }

                jwt.TokenValidationParameters = parameters;

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
