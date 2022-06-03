using Friends_Date_API.Data;
using Friends_Date_API.Extension;
using Friends_Date_API.Interfaces;
using Friends_Date_API.Middleware;
using Friends_Date_API.Services;
using Friends_Date_API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friends_Date_API
{
    public class Startup
    {
        private readonly IConfiguration _config;
       

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Created the Extension method of IServiceCollection for db connecton and token
            services.AddApplicationServices(_config);
            services.AddControllers();

            services.AddHttpsRedirection(options =>
            {
                //set port
                options.HttpsPort = 5001;
            });
            //Extention Method
            services.AddSwaggerServices();

            //Extention Method
            services.AddIdentitySerives(_config);

            // To resolve cors policy
            services.AddCors();

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<ExcepitonMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Friends_Date_API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => policy.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("https://localhost:4200")
                .AllowCredentials()); // since we use signal R we need allowCredentials;

            app.UseAuthentication(); // JWT Authentication
            app.UseAuthorization();

            // if there is index.html then it will use that
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // whenever it will not find the route it will hit index method of fallback controller
                endpoints.MapFallbackToController("Index", "Fallback");

                //// since we register signal R we need to configure our connection
                ///*
                //    1.  we configure route for incoming request
                // */
                endpoints.MapHub<MessageHub>("hubs/message");
                endpoints.MapHub<PresenceHub>("hubs/presence");


            });
        }
    }
}
