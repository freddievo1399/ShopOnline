using Microsoft.EntityFrameworkCore;
using ShopOnline.Infrastructure.Interface;
using ShopOnline.Infrastructure.Utils;
using ShopOnline.MatterData.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.MatterData.Server.Register
{
    public class DbContextRegistration : IDbContextRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.EntityRegister<EntityBrand>();

        }
    }
}
