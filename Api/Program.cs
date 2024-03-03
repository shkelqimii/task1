using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Pomelo.EntityFrameworkCore.MySql;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigin",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:5173") 
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      });
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); 

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages")),
    RequestPath = "/UploadedImages"
});



app.UseCors("AllowSpecificOrigin"); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
