using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Dto.Req;

public class LoginReq
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
