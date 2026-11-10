using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrepHub.Core.Entities;
using PrepHub.Core.Interfaces;
using PrepHub.Infrastructure.Data;
using PrepHub.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=prephub;Username=postgres;Password=postgres";

builder.Services.AddDbContext<PrepHubDbContext>(options =>
{
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("PrepHub.Infrastructure"));
});

// 2. Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PrepHubDbContext>()
.AddDefaultTokenProviders();

// 3. JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "PrepHub";
var audience = builder.Configuration["Jwt:Audience"] ?? "PrepHub";

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
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IMcqService, McqService>();
builder.Services.AddScoped<IInterviewQuestionService, InterviewQuestionService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IContributionService, ContributionService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IHiringStageService, HiringStageService>();
builder.Services.AddScoped<ICodeRunnerService, StubCodeRunnerService>();
builder.Services.AddScoped<IAiService, StubAiService>();

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 6. Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PrepHub (Crack30) API",
        Version = "v1",
        Description = "Placement Training Revision Platform API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Configure Middleware Pipeline
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrepHub API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () => Results.Ok(new
{
    Status = "Healthy",
    Version = "1.0.0",
    Timestamp = DateTime.UtcNow
}));

// Auto-seed database if enabled
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PrepHubDbContext>();
    // If running in development with accessible database, seed
    if (context.Database.IsRelational())
    {
        await SeedData.InitializeAsync(app.Services);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization note: {ex.Message}");
}

app.Run();
