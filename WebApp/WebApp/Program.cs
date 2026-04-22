using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopOnline.Abstractions.License;
using ShopOnline.Auth.Share.Interface;
using ShopOnline.BaseBlazor.Interface;
using ShopOnline.Infrastructure.Database;
using ShopOnline.Infrastructure.Identity;
using ShopOnline.Infrastructure.Utils;
using Syncfusion.Blazor;
using System.Text;
using WebApp.Client.Services;
using WebApp.Component;
using WebApp.Internal;
using WebApp.Middleware;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);
var ConnectionStrings__SqlServerConnection = builder.Configuration.GetConnectionString("SqlServerConnection");
SyncfusionLicense.RegisterLicense();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var controllerBuilder = builder.Services.AddControllers();

var assemblies = AssembliesUtil.GetAssembliesServer();

foreach (var assembly in assemblies)
{
    var isAlreadyAdded = controllerBuilder.PartManager.ApplicationParts
        .OfType<AssemblyPart>()
        .Any(p => p.Assembly.FullName == assembly.FullName);

    if (!isAlreadyAdded)
    {
        controllerBuilder.AddApplicationPart(assembly);
    }
}
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization", // Key mặc định của Header
        Type = SecuritySchemeType.Http, // Dùng Http thay vì ApiKey
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header, // Để là Header
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
       {

            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }    ,[]
       }
    });
    option.SwaggerDoc("MatterData", new OpenApiInfo { Title = "MatterData api", Version = "v1" });
    option.SwaggerDoc("customer", new OpenApiInfo { Title = "customer api", Version = "v1" });
    option.SwaggerDoc("Auth", new OpenApiInfo { Title = "Auth api", Version = "v1" });
    option.DocInclusionPredicate((groupname, apidesc) => apidesc.GroupName == groupname);
});
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(ConnectionStrings__SqlServerConnection);
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});
builder.Services.AddScoped(p =>
    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext()
);
// Đảm bảo dùng đúng class ApplicationUser và IdentityRole (nếu có)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth:Key"]!));
var jwtissuer = builder.Configuration["Auth:issuer"];
var jwtaudience = builder.Configuration["Auth:audience"];
var AuthorizationName = builder.Configuration["Auth:AuthorizationName"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = jwtissuer,
        ValidateAudience = true,
        ValidAudience = jwtaudience,
        IssuerSigningKey = securityKey
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies[AuthorizationName];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };

});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSyncfusionBlazor();
builder.AddInternalServices();
builder.Services.AddMemoryCache();
var app = builder.Build();

// 1. Chạy Bootstrap (Cái này Huy làm chuẩn rồi)
await AppBootstrap.RunAsync(app.Services);

// 2. Swagger & Development Tools
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/MatterData/swagger.json", "MatterData API v1");
        options.SwaggerEndpoint("/swagger/Auth/swagger.json", "Auth API v1");
        options.SwaggerEndpoint("/swagger/Customer/swagger.json", "Customer API v1");
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.UseMiddleware<ApiErrorHandlingMiddleware>();
});

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.UseStatusCodePagesWithRedirects("/NotFound");
});

app.UseMiddleware<AuthSyncMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(AssembliesUtil.GetAssembliesBlazor().ToArray());

app.Run();
