using ShopOnline.Auth.Server.Model;
using ShopOnline.Auth.Share.Attributes;
using ShopOnline.Auth.Share.Dto.Res;
using ShopOnline.Auth.Share.Utils;
using ShopOnline.Infrastructure.Utils;
using System.Reflection;
using System.Security.Claims;

namespace ShopOnline.Auth.Server.Utils;

public static class PermissionHub
{
    private static List<FeaturesRes> _features= new();

    public static void ScanAll()
    {
        var assemblyShares = AssembliesUtil.GetAssembliesShare();

        var typeEnums = assemblyShares.SelectMany(a => a.DefinedTypes
            .Where(x => x.IsEnum && x.GetCustomAttribute<FeatureAttribute>() != null));

        foreach (var typeEnum in typeEnums)
        {
            var featureAttr = typeEnum.GetCustomAttribute<FeatureAttribute>();

            var interfaceKey = typeEnum.DeclaringType?.Name ?? throw new Exception("Enum phân quyền k khai báo tại interface thực thi");

            var duplicateFeature = _features.FirstOrDefault(x => x.Name == featureAttr!.Name);
            if (duplicateFeature != null)
            {
                throw new Exception($"Trùng tên Feature: '{featureAttr!.Name}'. " +
                                    $"Interface mới: '{interfaceKey}' đang trùng với '{duplicateFeature.Key}'.");
            }

            var featuresDto = new FeaturesRes()
            {
                Key = interfaceKey,
                Name = featureAttr!.Name,
                Group = featureAttr.GroupName
            };

            foreach (Enum function in Enum.GetValues(typeEnum))
            {
                var functionName = function.ToString();
                var member = typeEnum.GetMember(functionName).FirstOrDefault();

                var permAttr = member?.GetCustomAttribute<PermistionAttribute>()
                    ?? throw new Exception($"Thiếu cấu hình PermissionAttribute tại: {typeEnum.Name}.{functionName}");

                featuresDto.Permistions.Add(new PermistionRes
                {
                    Name = functionName,
                    Value = ClaimsPrincipalUtils.GetValuePermistion(function)
                });
            }

            featuresDto.MatterPermistion = featuresDto.Permistions.Aggregate(0, (current, next) => current | next.Value);

            _features.Add(featuresDto);
        }
    }
    public static List<Claim> GetClaims()
    {
        var claims = new List<Claim>();
        if (_features.Count==0)
        {
            ScanAll();
        }
        foreach (var feature in _features)
        {
            claims.Add(new Claim(feature.Key, feature.MatterPermistion.ToString()));
        }
        return claims;
    }
}