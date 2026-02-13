    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using WorkHub.Business.Mapping;
    using WorkHub.Business.Service;
    using WorkHub.Business.Service.IService;
    using WorkHub.DataAccess.Data;
    using WorkHub.DataAccess.Repository;
    using WorkHub.DataAccess.Repository.IRepository;

using WorkHub.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

    // Controllers
    builder.Services.AddControllers();


    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

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
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),

            ClockSkew = TimeSpan.Zero
        };
    });


    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WorkHub API",
            Version = "v1"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter: Bearer {your JWT token}"
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

    // AutoMapper ✅ PLACE IT HERE
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    });

    // DbContext (SQL Server)
    builder.Services.AddDbContext<WorkHubDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );

    // Health Check
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<WorkHubDbContext>("Database");

    // ================= Add service =================

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<JwtService>();
    builder.Services.AddScoped<IAuthService,AuthService>();
    builder.Services.AddScoped<IGoogleAuthService,GoogleAuthService>();
    builder.Services.AddScoped<IEmailService, EmailService>();




// ================= Add service =================

var app = builder.Build();

    // ================= PIPELINE =================

    // Enable Swagger in all environments for testing Render deployment
    app.UseSwagger();
    app.UseSwaggerUI();


    // ================= SEEDING DATA =================
    using (var scope = app.Services.CreateScope())
    {
        //“Pretend this is one HTTP request lifetime”
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<WorkHubDbContext>();
            DbSeeder.Seed(db);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seed failed: {ex.Message}");
        }
    }
    // ================================================

    app.UseHttpsRedirection();

    // CORS must be BEFORE MapControllers
    app.UseCors("AllowAll");

    app.UseAuthentication(); // 👈 MUST be before authorization
    app.UseAuthorization();

    // Health endpoint (ALB)
    app.MapHealthChecks("/health");

    app.MapControllers();

    app.Run();