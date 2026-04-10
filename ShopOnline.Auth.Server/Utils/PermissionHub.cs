using ShopOnline.Auth.Share.Attributes;
using ShopOnline.Auth.Share.Dto.Res;
using ShopOnline.Auth.Share.Utils;
using ShopOnline.Infrastructure.Utils;
using System.Reflection;

namespace ShopOnline.Auth.Server.Utils;

public static class PermissionHub
{
    private static readonly List<FeaturesDto> _features = new();
    private static bool _isScanned = false;

    public static IReadOnlyList<FeaturesDto> AllFeatures => _features.AsReadOnly();

    public static void ScanAll()
    {
        if (_isScanned) return;

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

            var featuresDto = new FeaturesDto()
            {
                Key = interfaceKey,
                Name = featureAttr!.Name,
                Group = featureAttr.GroupName
            };
)
            foreach (Enum function in Enum.GetValues(typeEnum))
            {
                var functionName = function.ToString();
                var member = typeEnum.GetMember(functionName).FirstOrDefault();

                var permAttr = member?.GetCustomAttribute<PermistionAttribute>()
                    ?? throw new Exception($"Thiếu cấu hình PermissionAttribute tại: {typeEnum.Name}.{functionName}");

                featuresDto.Permistions.Add(new PermistionDto
                {
                    Name = functionName,
                    Value = ClaimsPrincipalUtils.GetValuePermistion(function)
                });
            }

            _features.Add(featuresDto);
        }

        _isScanned = true;
    }
}