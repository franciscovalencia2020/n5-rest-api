using Microsoft.Extensions.DependencyInjection.Extensions;
using Services;
using Services.ElasticSearch.Interfaces;
using Services.ElasticSearch;
using Services.Interfaces;

namespace Api
{
    public static class InjectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterScopedClients(services);
        }

        static void RegisterScopedClients(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionTypeService, PermissionTypeService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IPermissionElasticService, PermissionElasticService>();
        }
    }
}
