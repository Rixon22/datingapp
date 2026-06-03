using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Middlewares;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;

namespace API;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHealthChecks();
        AddServiceDefaults(builder);

        builder.Services.AddControllers()
            .AddMvcOptions(options =>
            {
                // Add the filter we could have
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddMemoryCache();

        AddDbContext(builder);
        AddScopedServices(builder);
        AddOpenApiDocument(builder);
        AddIdentity(builder);



        WebApplication app = builder.Build();

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            context.Database.Migrate();
            await context.Connections.ExecuteDeleteAsync();
            await Seed.SeedUsers(userManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
            logger.LogError(ex, "Migration process failed!");
        }

        // Configure the HTTP request pipeline.
        app.UseMiddleware<ExceptionMiddleware>();
        if (app.Environment.IsDevelopment())
        {
            app.UseCors(x => x.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200"
            ));

            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi();

            app.UseReDoc(options =>
            {
                options.Path = "/redoc";
            });
        }
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<PresenceHub>("hubs/presence");
        app.MapHub<MessageHub>("hubs/messages");
        app.Run();
    }

    private static void AddServiceDefaults(WebApplicationBuilder builder)
    {
        builder.Services.AddCors();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var tokenKey = builder.Configuration["TokenKey"]
                    ?? throw new ArgumentNullException("Cannot get the token key - Program.cs");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
    }

    private static void AddDbContext(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
        });
    }

    private static void AddScopedServices(WebApplicationBuilder builder)
    {
        // Repositories and services
        builder.Services.AddScoped<IMembersRepository, MembersRepository>();
        builder.Services.AddScoped<IMessagesRepository, MessagesRepository>();        
        builder.Services.AddScoped<ILikesRepository, LikesRepository>();
        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
        builder.Services.AddScoped<UserActivityLogger>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<PresenceTracker>();
    }

    private static void AddOpenApiDocument(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApiDocument(options =>
        {
            options.PostProcess = document =>
            {
                document.Info = new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Dating API",
                    Description = "An ASP.NET Core Web API for managing Dating items",
                    TermsOfService = "https://example.com/terms",
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = "https://example.com/contact"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = "https://example.com/license"
                    }
                };
            };
        });
    }

    private static void AddIdentity(WebApplicationBuilder builder)
    {
        builder.Services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>();
    }
}
