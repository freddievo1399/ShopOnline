using ShopOnline.Auth.Server.Model;
using ShopOnline.Auth.Share.Attributes;
using ShopOnline.Auth.Share.Dto.Res;
using ShopOnline.Auth.Share.Utils;
using ShopOnline.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Server.Utils;

public static class PermissionHub
{
    internal static List<FeaturesDto> features = new List<FeaturesDto>();
    public static void ScanAll()
    {
        var assemblyShares = AssembliesUtil.GetAssembliesShare();
        var typeEnums = assemblyShares.SelectMany(a => a.DefinedTypes.Where(x => x.IsEnum && x.GetCustomAttribute<FeatureAttribute>() != null));
        foreach (var typeEnum in typeEnums)
        {
            var FeatureAttribute = typeEnum.GetCustomAttribute<FeatureAttribute>();
            FeaturesDto featuresDto = new FeaturesDto() { Group = FeatureAttribute!.GroupName, Name = FeatureAttribute.Name };


            foreach (Enum function in Enum.GetValues(typeEnum))
            {
                var functionName = function.ToString();
                var member = typeEnum.GetMember(functionName).FirstOrDefault();
                var functionAttr = member!.GetCustomAttribute<PermistionAttribute>() ?? throw new Exception($"Chưa cấu hình Permistion {typeEnum.Name}.{functionName}");
                featuresDto.Permistions.Add(new() { Name = functionName!, Value = (ClaimsPrincipalUtils.GetValuePermistion(function)) });
            }
            features.Add(featuresDto);
        }
    }
}
