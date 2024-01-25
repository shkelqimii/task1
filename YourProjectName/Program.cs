using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YourProjectName.Data; // Replace with your actual namespace
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow specific origins
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigin",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:5173") // Vue.js frontend URL
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      });
});

// Ensure that the JWT secret key is not null
var jwtSecretKey = builder.Configuration["JwtConfig:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecretKey))
{
    throw new InvalidOperationException("JWT secret key is not configured properly.");
}

var keyBytes = Encoding.UTF8.GetBytes(jwtSecretKey);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Comment out or remove app.UseHttpsRedirection(); if not using HTTPS in development
// app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin"); // Apply the CORS policy

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
