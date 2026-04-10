using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShopOnline.Infrastructure.Database;
using ShopOnline.Infrastructure.Utils;
using WebApp.Component;
using WebApp.Internal;

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
await AppBootstrap.RunAsync(app.Services);
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
    .AddAdditionalAssemblies(AssembliesUtil.GetAssembliesBlazor().ToArray());

app.UseStatusCodePagesWithRedirects("/NotFound");
app.Run();
