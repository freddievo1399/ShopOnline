using ShopOnline.Abstractions.Wrappers;
using ShopOnline.Auth.Share.Dto.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Server.Model;

public class FeaturesModel : FeaturesRes
{
    public new List<PermistionModel> Permistions { get; set; }

    public FeaturesModel(FeaturesRes featuresDto)
    {
        this.Name = featuresDto.Name;
        this.Group = featuresDto.Group;
        this.Permistions = featuresDto.Permistions.Select(x => (PermistionModel)x).ToList();
    }
}