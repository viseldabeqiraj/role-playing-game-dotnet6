global using dotnetcore6_rpg.Models;
using dotnetcore6_rpg.Data;
using dotnetcore6_rpg.Service.AuthService;
using dotnetcore6_rpg.Service.CharacterService;
using dotnetcore6_rpg.Service.WeaponService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//modified to use authorization in swagger ui, after installing swashbuckle filters as a nuget
builder.Services.AddSwaggerGen(c=>
{
    c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Standard authorization header using the Bearer scheme, e.g. \"bearer {token} \"",
        In=Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name="Authorization",
        Type= Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program).Assembly); //Added for Automapper config
builder.Services.AddScoped<ICharacterService, CharacterService>(); //added for DI 
builder.Services.AddScoped<IAuthService, AuthService>(); //added for DI 
builder.Services.AddScoped<IWeaponService, WeaponService>(); //added for DI 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer (options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes
            (builder.Configuration.GetSection("AppSettings:Token").Value)),
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
