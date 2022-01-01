using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using React_ASPNETCORE_JWT_Auth.Authentication;
using React_ASPNETCORE_JWT_Auth.Configurations;
using React_ASPNETCORE_JWT_Auth.Extensions;
using React_ASPNETCORE_JWT_Auth.Filters;
using React_ASPNETCORE_JWT_Auth.Middlewares;
using React_ASPNETCORE_JWT_Auth.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//serilog related config
builder.WebHost.UseSerilog();

Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(builder.Configuration)
                .CreateLogger();

// Add services to the DI container.
builder.Services.AddControllersWithViews(options => options.Filters.Add<ValidationFilter>())
                .AddFluentValidation(config => 
                { 
                    config.RegisterValidatorsFromAssemblyContaining<Program>(); 
                });


//[ApiController] attributes perform the Model validation while model binding and sends back a standard response as BadRequest.
//Following config changes disables that behvior
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddAutoMapper(typeof (AutoMapperConfigurations));

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddScoped<IJwtUtils, JwtUtils>();

builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddDBServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.ConfigureGlobalErrorHandlerMiddleware();

app.UseMiddleware<JwtMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();