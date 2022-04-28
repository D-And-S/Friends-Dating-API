using Friends_Date_API.Data;
using Friends_Date_API.Helpers;
using Friends_Date_API.Interfaces;
using Friends_Date_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Friends_Date_API.Extension
{
    /*
        --> exetention method for service
        --> extention class will be always static
     */
    public static class ApplicationServiceExtension
    {
        // must reference by this keyword and the class which specify which class this method belongs to
        // return type must be the class which i want to create extention

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            //Used addscoped because we want to alive it until single request is finished
            services.AddScoped<ITokenService, TokenService>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoServices>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            // this will find and initialize the auto mapper profile
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContextPool<DataContext>(options =>
                     options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
