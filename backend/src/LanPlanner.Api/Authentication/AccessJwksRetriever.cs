using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace LanPlanner.Api.Authentication;

/// <summary>
/// Retrieves the OpenID Connect configuration from a JWKS endpoint.
/// </summary>
/// <param name="issuer">The issuer identifier for the OpenID Connect configuration.</param>
public sealed class AccessJwksRetriever(string issuer) : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(
        string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        var json = await retriever.GetDocumentAsync(address, cancel);
        var jwks = new JsonWebKeySet(json);

        var configuration = new OpenIdConnectConfiguration { Issuer = issuer };
        foreach (var key in jwks.GetSigningKeys())
        {
            configuration.SigningKeys.Add(key);
        }

        return configuration;
    }
}