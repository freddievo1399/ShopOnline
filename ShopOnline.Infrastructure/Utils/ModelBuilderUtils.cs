using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopOnline.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Utils
{
    public static class ModelBuilderUtils
    {
        public static EntityTypeBuilder<T> EntityRegister<T>(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<T>>? configure = null) where T : EntityBase
        {
            var entity = modelBuilder.Entity<T>();
            entity.HasIndex(e => e.Id).IsUnique();
            if (configure != null)
            {
                configure?.Invoke(entity);
            }
            return entity;
        }
    }
}
