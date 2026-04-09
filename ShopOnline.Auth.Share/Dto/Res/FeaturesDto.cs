using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Dto.Res;

public class FeaturesDto
{
    public string Name { get; set; }
    public string Group { get; set; }
    public List<PermistionDto> Permistions { get; set; } = new List<PermistionDto>();
}
