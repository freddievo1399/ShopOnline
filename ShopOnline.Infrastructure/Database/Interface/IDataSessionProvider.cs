using ShopOnline.Infrastructure.Database.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Database.Interface;

public interface IDataSessionProvider
{
    Task<QuerySession> OpenQueryAsync();
    Task<WorkSession> OpenWorkAsync(params string[] lockKeys);
}
