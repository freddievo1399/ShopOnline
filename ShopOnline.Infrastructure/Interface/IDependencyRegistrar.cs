using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ShopOnline.Infrastructure.Interface;

public interface IDependencyRegistrar
{
    public void RegisterDI(IServiceCollection services)
    {
    }
}
