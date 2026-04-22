using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Abstractions.Wrappers;

public class Result
{
    public bool Success { get; set; } = false;

    public string Message { get; set; } = "";

    public Result()
    {
    }
    public static Result Ok(string mess)
    {
        return new Result { Success = true, Message = mess };
    }
    public static Result Ok()
    {
        return new Result { Success = true };
    }

    public static Result Error(string error)
    {
        return new Result { Message = error };
    }

    public static implicit operator Result(string error)
    {
        return Result.Error(error);
    }

    public static implicit operator Result(bool success)
    {
        return new Result { Success = success };
    }

    public static implicit operator Result((bool, string) result)
    {
        return new Result { Success = result.Item1, Message = result.Item2 };
    }
}
