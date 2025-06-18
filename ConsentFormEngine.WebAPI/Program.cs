using System.ComponentModel.DataAnnotations;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ConsentFormEngine.Business.Extensions;
using ConsentFormEngine.Business.Interceptors;
using ConsentFormEngine.Business.Security.JWT;
using ConsentFormEngine.Core.Helpers;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.DataAccess.Context;
using ConsentFormEngine.DataAccess.UnitOfWork;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();


builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterType<TransactionInterceptor>();
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.FullName!.StartsWith("ConsentFormEngine"))
        .ToArray();

    containerBuilder.RegisterAssemblyTypes(assemblies)
        .Where(t => t.Name.EndsWith("Manager"))
        .AsImplementedInterfaces()
        .EnableClassInterceptors()
        .InterceptedBy(typeof(TransactionInterceptor));
});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

builder.Services.AddDbContext<BaseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TestDb")));
builder.Services.AddBusinessServices();
builder.Services.AddHttpClient<ApiClient>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

// Builder aşamasında
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", p =>
        {
            p
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

var tokenOptionsSection = builder.Configuration.GetSection("TokenOptions");
builder.Services.Configure<TokenOptions>(tokenOptionsSection);
var tokenOptions = tokenOptionsSection.Get<TokenOptions>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenOptions!.Issuer,
            ValidAudience = tokenOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
        };
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConsentFormEngine API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by space and JWT token"
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

app.UseCors("AllowAll");
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs")),
    RequestPath = "/docs"
});

// Configure the HTTP request pipeline.
app.MapHealthChecks("/health");
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var code = ex is ValidationException ? 400 : 500;
        context.Response.StatusCode = code;
        await context.Response.WriteAsJsonAsync(new {
            success = false,
            message = ex?.Message
        });
    });
});

app.UseHttpsRedirection();

app.UseAuthentication(); 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
    db.Database.Migrate();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
