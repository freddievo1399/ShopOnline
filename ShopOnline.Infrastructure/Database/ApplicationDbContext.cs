using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Infrastructure.Identity;
using ShopOnline.Infrastructure.Interface;
using ShopOnline.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entityRegisters = AssembliesUtil.GetAssemblies().GetInstances<IDbContextRegistration>();
        foreach (var i in entityRegisters)
        {
            i.RegisterEntities(modelBuilder);
        }
        base.OnModelCreating(modelBuilder);
    }
}

