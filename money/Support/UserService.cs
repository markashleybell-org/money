using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using money.Entities;

namespace money.Support
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryHelper _db;

        public UserService(
            IUnitOfWork unitOfWork,
            IQueryHelper db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public ClaimsPrincipal GetClaimsPrincipal(int id, string email)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Sid, id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Member")
            };

            var identity = new ClaimsIdentity(
                claims: claims,
                authenticationType: CookieAuthenticationDefaults.AuthenticationScheme,
                nameType: ClaimTypes.Email,
                roleType: ClaimTypes.Role
            );

            return new ClaimsPrincipal(identity);
        }

        public (bool valid, int? id) ValidateLogin(string email, string password)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";

            var user = _db.Query(conn => conn.QuerySingleOrDefault<User>(sql, new { email }));

            if (user == null)
            {
                return (false, default);
            }

            var hasher = new PasswordHasher<User>();

            var verificationResult = hasher.VerifyHashedPassword(user, user.Password, password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return (false, default);
            }

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Upgrade the password hash
                var newHash = hasher.HashPassword(user, password);

                user.SetPassword(password: newHash);

                _db.InsertOrUpdate(user);

                _unitOfWork.CommitChanges();
            }

            return (true, user.ID);
        }
    }
}
