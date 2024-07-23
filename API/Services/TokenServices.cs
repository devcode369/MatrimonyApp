using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Services.Inerfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenServices : ITokenServices
    {
        //Encrypt,Decrypt key & sign token; will store in server not to client 
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        public TokenServices(IConfiguration configs)
        {
            _symmetricSecurityKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configs["SecurityToken"]));
        }
        public string GenerateToken(AppUser appUser)
        {
            var claims =new List<Claim>{

                new Claim(JwtRegisteredClaimNames.NameId,appUser.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.UniqueName,appUser.UserName)
            };

            var signCred =new SigningCredentials(_symmetricSecurityKey,SecurityAlgorithms.HmacSha512Signature);

            var tokenDesc=new SecurityTokenDescriptor{
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(5),
                SigningCredentials=signCred
            };


            var tokenHandler=new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDesc);

            return tokenHandler.WriteToken(token);
        }
    }
}