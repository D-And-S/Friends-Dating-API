using Friends_Date_API.Data;
using Friends_Date_API.Helpers;
using Friends_Date_API.Interfaces;
using Friends_Date_API.Services;
using Friends_Date_API.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Friends_Date_API.Extension
{

    /*
        --> exetention method for service


        --> extention class will be always static
     */
    public static class ApplicationServiceExtension
    {
        public static IServiceProvider serviceProvider;
        // must reference by this keyword and the class which specify which class this method belongs to
        // return type must be the class which i want to create extention

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // for PresenceTracker we will addSingletone which will create instance only once 
            // we want share presenceTracker dictionary to every single connection (online users)
            //register the signalR class
            services.AddSignalR(e =>
            {
                e.EnableDetailedErrors = true;
            });

            services.AddSingleton<PresenceTracker>();

            //Used addscoped because we want to alive it(create it's instance) until single request is finished
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoServices>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // this will find and initialize the auto mapper profile
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddDbContext<DataContext>(options =>
            {
                // for sql server
                //options.UseSqlServer(config.GetConnectionString("DefaultConnection"))

                //for postgres
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);

            });

            serviceProvider = services.BuildServiceProvider();

            return services;
        }
    }


}
