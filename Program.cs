global using udemy_net_webapi.Models;
global using udemy_net_webapi.Services.Character;
global using udemy_net_webapi.Services.Fight;
global using udemy_net_webapi.Services.Weapon;
global using udemy_net_webapi.Services.ContextAccess;
global using udemy_net_webapi.DTOs.Character;
global using udemy_net_webapi.DTOs.Skill;
global using udemy_net_webapi.DTOs.User;
global using udemy_net_webapi.DTOs.Weapon;
global using udemy_net_webapi.DTOs.Fight;
global using udemy_net_webapi.Data;
global using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adding the database context and specifying the connection
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configuring that swagger UI to use authorization in header, by using the "bearer" token
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        Description = """Standard Authorization, bearer in header scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Adding AutoMapper and enabling injection of this into our services
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register the dependency injections for our services
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IWeaponService, WeaponService>();
builder.Services.AddScoped<IFightService, FightService>();
builder.Services.AddScoped<IContextAccessor, ContextAccessor>();

// Configure the JWT authentication scheme
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// This enables us to inject the http context into our services. Where we can access information about the executing user etc.
builder.Services.AddHttpContextAccessor();

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
