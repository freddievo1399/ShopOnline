using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Interface;

public interface IDbContextRegistration
{
    void RegisterEntities(ModelBuilder modelBuilder);
}
