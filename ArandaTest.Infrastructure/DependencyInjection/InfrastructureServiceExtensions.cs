using System.Text;
using ArandaTest.Application.Implementations;
using ArandaTest.Application.Interfaces;
using ArandaTest.Domain.Entities;
using ArandaTest.Domain.Interfaces;
using ArandaTest.Domain.Utils;
using ArandaTest.Infrastructure.Data;
using ArandaTest.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ArandaTest.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("ConnectionAranda")));

            services.AddScoped(typeof(IAppRepository<>), typeof(AppRepository<>));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();


            services.AddCors(o => o.AddPolicy("ArandaPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(3);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ArandaTest API", Version = "v1" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Portal Para Registro de Productos y categorias"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            Array.Empty<string>()
                    }
                });
                c.CustomSchemaIds(type => type.ToString());
            });

            return services;
        }

        public static void AddFirstData(AppDbContext context, Security security)
        {
            if (!context.Category.Any(a => a.Name.ToLower() == "tecnología"))
            {
                context.Category.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Tecnología",
                    Description = "Categoría dedicada a productos tecnológicos como computadoras, teléfonos, tablets, etc.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                });
                context.SaveChanges();
            }

            if (!context.Users.Any(a => a.Document == "1023970895"))
            {
                context.Users.Add(new Users
                {
                    Id = Guid.NewGuid(),
                    Name = "ROINER GOMEZ",
                    Document = "1023970895",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Email = "rstiven_98@hotmail.com",
                    Password = security.EncryptP("1023970895")
                });

                context.SaveChanges();
            }
        }
    }
}
