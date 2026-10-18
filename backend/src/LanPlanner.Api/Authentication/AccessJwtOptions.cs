namespace LanPlanner.Api.Authentication;

public class AccessJwtOptions
{
    public const string SectionName = "AccessJwt";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string SigningKey { get; set; } = "";
    public string JwksUrl { get; set; } = "";
}
