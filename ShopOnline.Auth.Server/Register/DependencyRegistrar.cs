using Microsoft.Extensions.DependencyInjection;
using ShopOnline.Auth.Server.Service;
using ShopOnline.Auth.Share.Interface;
using ShopOnline.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Server.Register
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void RegisterDI(IServiceCollection services)
        {
            services.AddScoped<IAuth, AuthService>();
        }
    }
}
