using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Dto.Res;

public class FeaturesRes
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string Group { get; set; }
    public int MatterPermistion { get; set; } = 0;
    public List<PermistionRes> Permistions { get; set; } = new List<PermistionRes>();
}
