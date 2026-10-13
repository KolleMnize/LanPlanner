namespace LanPlanner.Api.Authentication;

public interface ICurrentUser
{
    string? Email { get; }
}