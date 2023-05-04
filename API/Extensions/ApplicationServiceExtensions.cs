using System;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Application.Profiles;
using FluentValidation;
using Infrastructure.Blob;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddMediatR(typeof(ProfileGet).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            AssemblyScanner.FindValidatorsInAssembly(typeof(ProfileGet).Assembly)
                .ForEach(result =>
                    {
                        services.AddTransient(result.InterfaceType, result.ValidatorType);
                    }
                );

            services.AddDbContext<DataContext>(options =>
            {
                var connectionString = config.GetConnectionString("DefaultConnection");;
                options.UseSqlServer(
                    connectionString, 
                    x=> x.UseNetTopologySuite());
            });

            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IBlobManagerService, BlobManagerService>();

            return services;
        }
    }
}