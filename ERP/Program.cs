using Erp.Module.Core.Data;
using Erp.Module.Core.Entities;
using Erp.Shared.Interfaces;
using ERP.Interfaces;
using ERP.Middleware;
using ERP.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Register our Current User Service so any module can ask "Who is running this code?"
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// db connections
builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.MigrationsAssembly("Erp.Module.Core")));

builder.Services.AddControllers();

// Configure CORS to allow the frontend at http://localhost:5173
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowLocalhost5173",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Register the exception handler in the DI container
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JWT Secret Key is missing from configuration.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = false, // Set to true in production (e.g., "https://aegiserp.com")
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

// 3. Enable Role-Based Authorization
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Enable CORS - must run before authentication/authorization and before routing to controllers
app.UseCors("AllowLocalhost5173");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================================
// RUN THE DATABASE SEEDER ON STARTUP
// ==========================================
using (var scope = app.Services.CreateScope())
{
    // We try-catch this so if the database is offline, the app still launches and shows the error
    try
    {
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
        Console.WriteLine("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

app.Run();
