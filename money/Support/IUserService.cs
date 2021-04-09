using System.Security.Claims;

namespace Money.Support
{
    public interface IUserService
    {
        ClaimsPrincipal GetClaimsPrincipal(int id, string email);

        (bool Valid, int? ID) ValidateLogin(string email, string password);
    }
}
