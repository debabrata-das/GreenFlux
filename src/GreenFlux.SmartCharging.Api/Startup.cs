using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Data;
using System.Net;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace GreenFlux.SmartCharging.Api
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "GreenFlux SmartCharging API",
                    Description = "GreenFlux SmartCharging ASP.NET Core Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "Debabrata Das",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/debabrata-das"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://choosealicense.com/licenses/mit/"),
                    }
                });
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.CorsConfiguration();
            services.AddControllers();
            services.PersistenceConfiguration();
            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenFlux.SmartCharging.Api v1"));
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Custom Metrics to count requests for each endpoint and the method
            var counter = Metrics.CreateCounter("greenfluxapi_path_counter", "Counts requests to the GreenFlux API endpoints", new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });
            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });
            
            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
