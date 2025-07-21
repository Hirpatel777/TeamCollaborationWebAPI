using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TeamCollaborationWebAPI.Auth;
using TeamCollaborationWebAPI.Hubs;
using TeamCollaborationWebAPI.Interfaces;
using TeamCollaborationWebAPI.Models;
using TeamCollaborationWebAPI.Services;
var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
var builder = WebApplication.CreateBuilder(args);

// Add sebuilder.Services.AddDbContext<AppDbContext>(options =>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SupplierChainManagement", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Bearer [space] and then your token in the text input below. \r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
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
            }, Array.Empty<string>()
        }
    });
});
var key = Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwtToken12345");

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "MyApi",
        ValidAudience = "MyApiUsers",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Needed for SignalR JWT auth
            var accessToken = context.Request.Query["access_token"]; //configuration["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/taskhub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});
//string jwtToken = configuration["access_token"]; ; // Replace with real token or load from config

//var hubService = new TaskHubService();
//await hubService.ConnectAsync(jwtToken);
//builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskHubService, TaskHubService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins("https://localhost:7250") // ? Your frontend origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // ? Required for SignalR
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
//app.MapHub<TaskHub>("/taskhub");
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<TaskHub>("/taskhub");
});
app.MapControllers();

app.Run();
