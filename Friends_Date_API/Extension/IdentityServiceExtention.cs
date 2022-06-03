using Friends_Date_API.Data;
using Friends_Date_API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace Friends_Date_API.Extension
{
    //extention method for IserviceCollection class 
    public static class IdentityServiceExtention
    {
        public static IServiceCollection AddIdentitySerives(this IServiceCollection services, IConfiguration config)
        {
            // since we are using microsoft identity we need little configuration for identity
            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = false;
            })
                .AddRoles<Role>()

                //contains all required for role to get list of role, add role etc
                .AddRoleManager<RoleManager<Role>>()

                //contains all related method for user authentication 
                .AddSignInManager<SignInManager<User>>()

                // for validate the role
                .AddRoleValidator<RoleValidator<Role>>()
                .AddEntityFrameworkStores<DataContext>();

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

                        // this is for signal R access token for authentication we will pass the token from
                        // query string because websocket it does not support header

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = (context) =>
                            {
                                // client wil hold informaion through access_token
                                var accessToken = context.Request.Query["access_token"];

                                // where the request coming through
                                var path = context.HttpContext.Request.Path;

                                if (!string.IsNullOrEmpty(accessToken) && 
                                     path.StartsWithSegments("/hubs"))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

            /*
                In Policy Based Authorization where we add policy for role
                and we specify that policy to require method
             */

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy=> policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratorPhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

            });

            return services;
        }
    }
}
