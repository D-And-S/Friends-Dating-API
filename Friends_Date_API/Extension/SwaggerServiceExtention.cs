using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Friends_Date_API.Extension
{
    public static class SwaggerServiceExtention
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Friends_Date_API", Version = "v1" });
            });
            return services;
        }
    }
}
