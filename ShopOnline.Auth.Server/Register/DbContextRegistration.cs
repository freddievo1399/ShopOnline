using Microsoft.EntityFrameworkCore;
using ShopOnline.Infrastructure.Interface;
using ShopOnline.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Server.Register
{
    public class DbContextRegistration : IDbContextRegistration
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.EntityRegister<Entity.EntityRefreshToken>(e => e.HasIndex(e => e.RefreshToken).IsUnique());
        }
    }
}
