using Microsoft.EntityFrameworkCore;
using TaskmgmtAPI.Db;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaskmgmtAPI.Authmanager
{
    public class JwtAuthenticationManager
    {
        private readonly string key;

        public JwtAuthenticationManager(string key)
        {
            this.key = key;
        }


        public string authenticate(int id,string email)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(id.ToString())) return null;


            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, id.ToString())
                }
                ),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);


            return tokenHandler.WriteToken(token);
        }
    }
}
