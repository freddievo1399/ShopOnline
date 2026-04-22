using Microsoft.EntityFrameworkCore.Internal;
using ShopOnline.BaseBlazor.Interface;
using ShopOnline.Infrastructure.Database.Interface;
using ShopOnline.Infrastructure.Database.Service;
using ShopOnline.Infrastructure.Interface;
using ShopOnline.Infrastructure.Utils;
using WebApp.Services;

namespace WebApp.Internal;

public static class RegisterServices
{
    public static void AddInternalServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDataSessionProvider, DataSessionProvider>();
        var serviceRegisters = AssembliesUtil.GetAssemblies().GetInstances<IDependencyRegistrar>();
        foreach (var i in serviceRegisters)
        {
            i.RegisterDI(builder.Services);
        }
        builder.Services.AddScoped(typeof(IAppBlazorService<>), typeof(AutoDI<>));
    }
}
