using System.Security.Claims;

namespace Money.Support
{
    public interface IUserService
    {
        ClaimsPrincipal GetClaimsPrincipal(int id, string email);

        (bool valid, int? id) ValidateLogin(string email, string password);
    }
}
