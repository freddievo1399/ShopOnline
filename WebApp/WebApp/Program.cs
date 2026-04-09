using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShopOnline.Auth.Server.Utils;
using ShopOnline.Infrastructure.Database;
using ShopOnline.Infrastructure.Utils;
using WebApp.Client.Components;
using WebApp.Component;

var builder = WebApplication.CreateBuilder(args);
var ConnectionStrings__SqlServerConnection = builder.Configuration.GetConnectionString("SqlServerConnection");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("admin", new OpenApiInfo { Title = "admin api", Version = "v1" });
    option.SwaggerDoc("customer", new OpenApiInfo { Title = "customer api", Version = "v1" });
    option.SwaggerDoc("auth", new OpenApiInfo { Title = "auth api", Version = "v1" });

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


var app = builder.Build();
PermissionHub.ScanAll();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebApp.Client._Imports).Assembly);

app.UseStatusCodePagesWithRedirects("/NotFound");
app.Run();
