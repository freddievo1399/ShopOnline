using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Abstractions.Wrappers;

public class PagedResultsOf<T> : Result
{
    public IEnumerable<T> Items { get; set; } = [];

    public int TotalCount { get; set; }


    public static PagedResultsOf<T> Empty()
    {
        return new PagedResultsOf<T> { Success = true, Items = new List<T>(), TotalCount = 0 };
    }

    public static PagedResultsOf<T> Ok(IEnumerable<T> data, int totalCount)
    {
        return new PagedResultsOf<T> { Success = true, Items = data, TotalCount = totalCount };
    }

    public static new PagedResultsOf<T> Error(string msg = "")
    {
        return new PagedResultsOf<T> { Message = msg };
    }

    public static implicit operator PagedResultsOf<T>(string error)
    {
        return PagedResultsOf<T>.Error(error);
    }

    public static implicit operator PagedResultsOf<T>(List<T> data)
    {
        return new PagedResultsOf<T> { Success = true, Items = data, TotalCount = data.Count };
    }
}
