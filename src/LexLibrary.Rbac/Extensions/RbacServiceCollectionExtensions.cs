using LexLibrary.Rbac.Abstractions;
using LexLibrary.Rbac.Models;
using LexLibrary.Rbac.Repositories;
using LexLibrary.Rbac.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace LexLibrary.Rbac.Extensions
{
    public static class RbacServiceCollectionExtensions
    {
        public static IServiceCollection AddLexLibraryRbac(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction,
            LexLibraryRbacSetting setting = null)
        {
            setting = setting ?? new LexLibraryRbacSetting();

            services.AddHttpContextAccessor();
            services.AddDbContextPool<LexLibraryRbacDbContext>(optionsAction);

            services.AddScoped(sp => setting);
            services.AddScoped(typeof(IRepository<>), typeof(LexLibraryRbacRepository<>));
            services.AddScoped(typeof(IUserManager), typeof(UserManager));

            return services;
        }


        public static IApplicationBuilder UseLexLibraryRbacRoute(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "LexLibraryRbac",
                    areaName: "LexLibraryRbac",
                    template: "LexLibraryRbac/{controller=Account}/{action=Login}/{id?}",
                    defaults: new string[] { "LexLibrary.Rbac.Areas.LexLibraryRbac.Controllers" });
            });

            return app;
        }
    }
}
