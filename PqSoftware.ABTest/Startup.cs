using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PqSoftware.ABTest.Data;
using PqSoftware.ABTest.Data.Dto;
using PqSoftware.ABTest.Services;
using System;

namespace PqSoftware.ABTest
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment HostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostEnvironment = env;
        }

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

            string connection = HostEnvironment.IsDevelopment() ? 
                Configuration.GetConnectionString("DefaultConnection") : 
                Environment.GetEnvironmentVariable("DEFAULT_CONNECTION_3");

            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

            services.AddScoped<IDataRepository, DataRepository>();
            services.AddTransient<IUsersLifetimeService, UsersLifetimeService>();
            services.AddTransient<IRollingRetentionService, RollingRetentionService>();

            services.AddProblemDetails(setup =>
            {
                setup.IncludeExceptionDetails = (ctx, ex) => HostEnvironment.IsDevelopment() || HostEnvironment.IsStaging();

                setup.Map<LogicException>(exception => new ProblemDetails
                {
                    Title = exception.Title,
                    Detail = exception.Detail,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = exception.Type,
                });
            });

            services.AddAutoMapper(typeof(ProjectProfile));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseProblemDetails();

            if (!env.IsDevelopment())
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