using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Abstractions.Wrappers;

public class ResultOf<T> : Result
{
    public ResultOf()
    {
    }

    public ResultOf(bool s, string m, T i)
    {
        Success = s;
        Message = m;
        Item = i;
    }


    public T Item { get; set; } = default!;


    public static ResultOf<T> Ok(T item)
    {
        return new ResultOf<T> { Success = true, Item = item };
    }

    public static ResultOf<T> Ok(T data, string msg)
    {
        return new ResultOf<T> { Success = true, Item = data, Message = msg };
    }

    public static new ResultOf<T> Error(string msg = "")
    {
        return new ResultOf<T> { Message = msg };
    }


    public static implicit operator ResultOf<T>(string error)
    {
        return Error(error);
    }

    public static implicit operator ResultOf<T>(T item)
    {
        if (item == null)
        {
            return "Not found: Dev not check null";
        }
        return ResultOf<T>.Ok(item);
    }

    public static implicit operator ResultOf<T>((bool, string, T) result)
    {
        return new ResultOf<T>(result.Item1, result.Item2, result.Item3);
    }

    public static implicit operator ResultOf<T>((T, string) result)
    {
        return new ResultOf<T>(true, result.Item2, result.Item1);
    }
}
