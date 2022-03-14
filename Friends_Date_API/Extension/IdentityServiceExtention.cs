using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Friends_Date_API.Extension
{
    //extention method for IserviceCollection class 
    public static class IdentityServiceExtention
    {
        public static IServiceCollection AddIdentitySerives(this IServiceCollection services, IConfiguration config)
        {
            //Add and configure service for our authentcation by jwt bearer Authentication
            /*
                --> basically we validate the token here 
                --> which contain some property
             */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true, // server will sign the token and we need to tell the server that it is valid token.
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])), // get or set the security key
                            ValidateIssuer = false, // api server will issue the key
                            ValidateAudience = false, // who uses the token as audience? Angular
                        };
                    });
            return services;
        }
    }
}
