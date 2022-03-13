using Friends_Date_API.Entities;
using Friends_Date_API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace Friends_Date_API.Services
{
    public class TokenService : ITokenService
    {
        // this key will remain on the server and dosesn't go anywhere 
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            //used indexer from IConfiguration to store key
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            //what claims [what type of data we want to store like
            //{"nameId": "local", role:"Member","nbf(in which time token will be available)":15551, "expiredate":15597,"issuDat":156587}]
            //we are gonna put inside this token
            var claims = new List<Claim>
            {
                 // store username in nameID
                 new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            //We will create Credential where we store algorithmvalue into _key (config[tokenKey])
            var credential = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);

            // we create tokenDescriptor where we will have 3 things
            //1.Header of the token {"alg": "HS512"}, {"type: "JWT"}
            //2.some payload like name, role, expiry data
            //3.signature

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credential
            };

            //now we will create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
