using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Features;
using TaskMaster.Application.Interfaces;
using TaskMaster.Application.Mappings;
using TaskMaster.Domain.Entities;
using TaskMaster.Infrastructure.Data;
using TaskMaster.Infrastructure.Repositories;
using TaskMaster.Infrastructure.UnitOfWork;

using Microsoft.OpenApi.Models;
using TaskMaster.Application.Services;
using TaskMaster.Infrastructure.Services;

using TaskMaster.Presentation.Middleware;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/taskmaster-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);



    builder.Host.UseSerilog(); // Usar Serilog para el host

    // Add services to the container.

    builder.Services.AddControllers();

    // --- Configuración de AutoMapper ---
    builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

    // --- Configuración de API Versioning ---
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    });

    builder.Services.AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV"; // Formato para los grupos de Swagger (ej. v1, v2)
        options.SubstituteApiVersionInUrl = true;
    });

    // --- Configuración de Swagger/OpenAPI ---
    builder.Services.AddSwaggerGen(options =>
    {
        var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName,
                new OpenApiInfo()
                {
                    Title = $"TaskMaster API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = "API para la gestión de tareas empresariales."
                });
        }

        // Añadir soporte para JWT en Swagger UI
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
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
                new string[] { }
            }
        });
    });

    // --- Configuración de EF Core ---
    if (builder.Environment.IsEnvironment("IntegrationTests"))
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("InMemoryDbForTesting"));
    }
    else
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    }

    // --- Configuración de ASP.NET Core Identity ---
    builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // --- Configuración de Autenticación JWT ---
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var jwtKey = jwtSettings["Key"]!;
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
    logger.LogInformation("JWT Key used for validation: {JwtKey}", jwtKey);
    var key = Encoding.UTF8.GetBytes(jwtKey); // Usar ! para indicar que no será null

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
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

    // --- Registro de Repositorios y Unit of Work ---
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // --- Registro de Handlers de Casos de Uso ---
    builder.Services.AddScoped<CreateTaskCommandHandler>();
    builder.Services.AddScoped<GetTaskByIdQueryHandler>();
    builder.Services.AddScoped<UpdateTaskCommandHandler>();
    builder.Services.AddScoped<DeleteTaskCommandHandler>();
    builder.Services.AddScoped<GetAllTasksQueryHandler>();
    builder.Services.AddScoped<CreateProjectCommandHandler>(); // Add this line


    // Añadir registro de ILogger para los handlers que lo necesiten
    builder.Services.AddLogging(); // Esto registra los servicios de logging

    // --- Configuración de MemoryCache ---
    builder.Services.AddMemoryCache();

    // --- Registro de Servicios de Seguridad (para JWT) ---
    builder.Services.AddScoped<ITokenService, TokenService>();

    var app = builder.Build();

    // Apply database migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Database migrations applied.");
    }

    app.UseSerilogRequestLogging(); // Loggear las solicitudes HTTP

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<ExceptionHandlerMiddleware>(); // Añadir esta línea, preferiblemente al principio del pipeline

    app.UseAuthentication(); // Debe ir antes de UseAuthorization
    app.UseAuthorization();

    app.MapControllers();


    // --- Seed de Roles y Usuario Admin ---
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new User { UserName = "admin", Email = "admin@taskmaster.com" };
            var createAdmin = await userManager.CreateAsync(adminUser, "Admin123!");
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { } // Hacer la clase Program pública para pruebas