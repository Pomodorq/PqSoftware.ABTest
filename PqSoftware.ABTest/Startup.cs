using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PqSoftware.ABTest.Data;
using PqSoftware.ABTest.Services;
using System;

namespace PqSoftware.ABTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
                  builder
                    .WithOrigins(Configuration["AllowedCORS"])
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("Profiler-Info")
                  ));

            string connection = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION_3");
            
            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

            services.AddScoped<IDataRepository, DataRepository>();
            services.AddTransient<IUsersLifetimeService, UsersLifetimeService>();
            services.AddTransient<IRollingRetentionService, RollingRetentionAllService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}