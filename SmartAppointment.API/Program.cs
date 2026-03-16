using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using SmartAppointment.API.Areas.Identity.Data;
using SmartAppointment.API.Authorization;
using SmartAppointment.API.Middleware;
using SmartAppointment.Application.Interfaces;
using SmartAppointment.Application.Services;
using SmartAppointment.Infrastructure.Data;
using SmartAppointment.Infrastructure.Identity;
using SmartAppointment.Infrastructure.Repositories;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //DbContext
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Add a space " " to the allowed characters list
                options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication();

        // Register dynamic permission policy provider and handler
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        builder.Services.AddScoped<IAppointmentService, AppointmentService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(Option =>
        {
            Option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "SmartAppointment API",
                Version = "v1",
                Description = "Appointment Management System API"
            });
        });
        // Register Health Check services
        builder.Services.AddHealthChecks()
            .AddCheck("JWT_Config_Check", () =>
                !string.IsNullOrEmpty(builder.Configuration["Jwt:Key"])
                    ? HealthCheckResult.Healthy("JWT Key is configured.")
                    : HealthCheckResult.Unhealthy("JWT Key is missing!"))
            .AddDbContextCheck<AppDbContext>(); // Automatically checks if SQL Server is reachable

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("BlazorPolicy", policy =>
            {
                policy
                    .WithOrigins(builder.Configuration["ApiSettings:BlazorUrl"] ?? "https://localhost:7161")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("api", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 60; // Max 60 requests per minute per connection
                opt.QueueLimit = 0;
            });
        });


        // 1. Fetch the key
        var jwtKey = builder.Configuration["Jwt:Key"];

        // 2. Validate
        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FATAL ERROR: JWT Key is missing or too short in User Secrets/Environment Variables.");
            Console.ResetColor();
            throw new InvalidOperationException("Security configuration is invalid.");
        }

        // configure authentication so JWT is used by default for API calls
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });



        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");
            await next();
        });

        using (var scope = app.Services.CreateScope())
        {
            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
        }
        app.UseHttpsRedirection();
        app.UseCors("BlazorPolicy");
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        //Global Exception Middleware
        app.UseMiddleware<ExceptionMiddleware>();
        app.MapControllers();

        app.Run();
    }
}
