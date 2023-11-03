using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskmgmtAPI.Db;
using TaskmgmtAPI.Models;

namespace TaskmgmtAPI.Controllers.Auth
{
    public  class AuthHelper
    {

        private  readonly Context _context;

        public AuthHelper(Context context) => _context = context;

        public  async Task<User> GetAuthenticatedUser(HttpContext httpContext)
        {
            ClaimsIdentity identity = httpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                Claim userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                Claim userEmailClaim = identity.FindFirst(ClaimTypes.Email);

                if (userIdClaim != null && userEmailClaim != null)
                {
                    string userId = userIdClaim.Value;
                    string email = userEmailClaim.Value;

              
                    var authUser = await FindUserAsync(email, userId);

                    return authUser;
                }
            }

            return null; 
        }

        public  async Task<User> FindUserAsync(string email, string userId)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userId)) { return null; };

            var authUser = await _context.User.FirstOrDefaultAsync(u => u.email == email && u.id == int.Parse(userId));

            return authUser; 
        }
    }
}
