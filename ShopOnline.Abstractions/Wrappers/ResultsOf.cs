using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Abstractions.Wrappers;

public class ResultsOf<T> : Result
{
    public IEnumerable<T> Items { get; set; } = [];

    public ResultsOf(bool success, string message, IEnumerable<T> items)
    {
        Success = success;
        Message = message;
        Items = items;
    }

    public ResultsOf()
    {
    }

    public static ResultsOf<T> Ok(IEnumerable<T> items)
    {
        return new ResultsOf<T> { Success = true, Items = items };
    }

    public static new ResultsOf<T> Error(string msg = "")
    {
        return new ResultsOf<T> { Message = msg };
    }

    public static implicit operator ResultsOf<T>(string error)
    {
        return ResultsOf<T>.Error(error);
    }

    public static implicit operator ResultsOf<T>(T[] items)
    {
        return ResultsOf<T>.Ok(items);
    }

    public static implicit operator ResultsOf<T>(List<T> items)
    {
        return ResultsOf<T>.Ok(items);
    }

    public static implicit operator T[](ResultsOf<T> result)
    {
        return result.Success ? result.Items.ToArray() : [];
    }
    public static implicit operator ResultsOf<T>((bool, string, IEnumerable<T> T) result)
    {
        return new ResultsOf<T>(result.Item1, result.Item2, result.Item3);
    }

    public static implicit operator ResultsOf<T>((IEnumerable<T>, string) result)
    {
        return new ResultsOf<T>(true, result.Item2, result.Item1);
    }
}
