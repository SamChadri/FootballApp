namespace Football.Auth;

public sealed record AuthenticatedUser(int UserId, string Username, string Access);

public sealed record AuthResult(bool Success, string? Error, AuthenticatedUser? User)
{
    public static AuthResult Ok(AuthenticatedUser user) => new(true, null, user);
    public static AuthResult Fail(string error) => new(false, error, null);
}

public interface IAuthService
{
    Task<AuthResult> SignInAsync(CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
}

