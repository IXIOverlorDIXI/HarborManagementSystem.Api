using System.Linq;
using API.Extensions;
using API.Middleware;
using FluentValidation.AspNetCore;
using Infrastructure.Mqtt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config);
            services.AddIdentityServices(_config);
            
            services.AddControllers(opt => 
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddFluentValidation();
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    var origins = _config.GetSection("CorsOrigin:Links").Get<string[]>();
                    builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
                    //builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            
            var serviceProvider = services.BuildServiceProvider();
            var mqttClient = serviceProvider.GetService<MqttClient>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API"));

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");
            //app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}