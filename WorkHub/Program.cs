using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WorkHub.Business.Mapping;
using WorkHub.Business.Service;
using WorkHub.Business.Service.IService;
using WorkHub.DataAccess.Data;
using WorkHub.DataAccess.Repository;
using WorkHub.DataAccess.Repository.IRepository;
using PayOS;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.SetIsOriginAllowed(origin =>
              {
                  var uri = new Uri(origin);
                  return uri.Host == "localhost"
                        || uri.Host == "workhub.com.vn"     // ← add this
                        || uri.Host == "www.workhub.com.vn" // ← and this
                        || uri.Host.EndsWith(".vercel.app");
              })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// DbContext (SQL Server)
builder.Services.AddDbContext<WorkHubDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );

// Configure payOS for order controller
builder.Services.AddKeyedSingleton("OrderClient", (sp, key) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new PayOSClient(new PayOSOptions
    {
        ClientId = config["PayOS:ClientId"] ?? Environment.GetEnvironmentVariable("PAYOS_CLIENT_ID"),
        ApiKey = config["PayOS:ApiKey"] ?? Environment.GetEnvironmentVariable("PAYOS_API_KEY"),
        ChecksumKey = config["PayOS:ChecksumKey"] ?? Environment.GetEnvironmentVariable("PAYOS_CHECKSUM_KEY"),
        LogLevel = LogLevel.Debug,
    });
});



// Health Check
builder.Services.AddHealthChecks()
        .AddDbContextCheck<WorkHubDbContext>("Database");

    // ================= Add service =================

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<JwtService>();
    builder.Services.AddScoped<IAuthService,AuthService>();
    builder.Services.AddScoped<IGoogleAuthService,GoogleAuthService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IMediaService, MediaService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("Cloudinary"));




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
    app.UseStaticFiles();

    // CORS must be BEFORE MapControllers
    app.UseCors("AllowAll");

    app.UseAuthentication(); // 👈 MUST be before authorization
    app.UseAuthorization();

    // Health endpoint (ALB)
    app.MapHealthChecks("/health");
    app.MapControllers();

    app.Run();



//https://workhub-backend-m47s.onrender.com/health
//If it returns Healthy → DB is connected ✅
//If it returns Unhealthy → connection string still wrong ❌


//However, there are 3 big risks if you don't have /health:

//"False Positives": Your app might show as "Live" on Render because the web server started, even if it's crashing internally because it can't connect to the database.
//Deployment Downtime: Render uses health checks to decide when to switch traffic from the "Old" version to the "New" version. Without a check, it might switch too early and show users a 500 error page.
//Harder Debugging: When you see a 500 error (like the one you just had with the connection string), you have to check logs to find out why. With /health, you can instantly see Database: Unhealthy and know exactly where the problem is.