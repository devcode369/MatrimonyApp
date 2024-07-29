using API.Entities;
using API.Services.Inerfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenServices : ITokenServices
    {
        //Encrypt,Decrypt key & sign token; will store in server not to client 
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly UserManager<AppUser> _userManager;

        public TokenServices(IConfiguration configs, UserManager<AppUser> userManager)
        {
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configs["SecurityToken"]));
            _userManager = userManager;
        }
        public async Task<string> GenerateToken(AppUser appUser)
        {
            var claims = new List<Claim>{

                new Claim(JwtRegisteredClaimNames.NameId,appUser.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.UniqueName,appUser.UserName)
            };
            var roles = await _userManager.GetRolesAsync(appUser);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var signCred = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(60),
                SigningCredentials = signCred
            };


            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDesc);

            return tokenHandler.WriteToken(token);
        }
    }
}