using LT.DigitalOffice.GUI.Services;
using LT.DigitalOffice.GUI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LT.DigitalOffice.GUI.Helpers.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            if (services == null)
            {
                return services;
            }

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IMessageService, MessageService>();

            return services;
        }
    }
}
