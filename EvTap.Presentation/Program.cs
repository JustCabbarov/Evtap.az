using System.Reflection;
using EvTap.Application.Exceptions;
using EvTap.Application.Profiles;
using EvTap.Application.Services;
using EvTap.Contracts.Options;
using EvTap.Contracts.Services;
using EvTap.Domain.Entities;
using EvTap.Domain.Repositories;
using EvTap.Infrastructure.Configurations;
using EvTap.Infrastructure.Services;
using EvTap.Persistence.Data;
using EvTap.Persistence.Repositories;
using EvTap.Presentation.ExceptionHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// DbContexts
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ScrapingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ScrapingDb")));

// Quartz jobs

builder.Services.AddHttpClient();

builder.Services.AddQuartzJobs();


// 1. Serilog konfiqurasiya et
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext() 
    .WriteTo.Console()      
    .WriteTo.MongoDB("mongodb://127.0.0.1:27017/LoggingDB", collectionName: "Logs") // MongoDB-yə yaz
    .CreateLogger();

// 2. Host-u Serilog ilə əvəz et
builder.Host.UseSerilog();

// 3. DI container-ə servislərini əlavə et
builder.Services.AddScoped<IScrapingRepository, ScrapingRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<NullExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<ITokenHnadler,TokenHandler>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddAutoMapper(m =>
{
    m.AddProfile(new CustomProfiles());
});

builder.Services.AddProblemDetails();




// DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(OptionsBuilderConfigurationExtensions =>
{
  
    OptionsBuilderConfigurationExtensions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("Jwt"));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
    await roleService.SeedRolesAsync();
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
   app.UseExceptionHandler();

}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

Log.CloseAndFlush();
