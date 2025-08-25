using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathApp.Middleware;
using MathApp.Models.DbModels;
using MathApp.Services;
using MathApp.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MathApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<math_appContext>(options =>
            {
                if (!options.IsConfigured)
                {
                    options.UseMySql("server=localhost;user=root;database=math_app;port=3307;password=%s1WnX6*", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql"));
                }
            });
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMathProblemService, MathProblemService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddControllersWithViews();
            services.AddSession(options => { options.IdleTimeout = TimeSpan.FromHours(6); } );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();
            app.UseMiddleware<UserSessionMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
