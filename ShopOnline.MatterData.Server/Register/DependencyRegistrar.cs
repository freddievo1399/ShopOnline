using Microsoft.Extensions.DependencyInjection;
using ShopOnline.Infrastructure.Interface;
using ShopOnline.MatterData.Server.Controller;
using ShopOnline.MatterData.Share.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.MatterData.Server.Register
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void RegisterDI(IServiceCollection services)
        {
            services.AddScoped<IBrandManager,BrandManagerController>();
        }
    }
}
