using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Dto.Res;

public class PermistionDto
{
    public required string Name { get; set; } = "";
    public required int Value { get; set; }
    public bool IsEnabled { get; set; } = false;
}
