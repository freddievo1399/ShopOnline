using ShopOnline.Abstractions.Constants;
using ShopOnline.Abstractions.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Share.Utils
{
    public static class ClaimsPrincipalUtils
    {
        public static string? GetUserName(this System.Security.Claims.ClaimsPrincipal principal)
        {
            return principal.Identity?.Name;
        }
        public static Result CheckPermission(this ClaimsPrincipal user, params Enum[] permissions)
        {
            // Check input
            if (user == null || permissions == null || permissions.Length == 0)
                return ResultCode.Unauthenticated;

            if (user.Identity?.Name == null)
            {
                throw new Exception("User identity is not available.");
            }
            var firstPermission = permissions.First();
            if (firstPermission == null)
                throw new ArgumentNullException(nameof(firstPermission), "Permission cannot be null");

            var enumType = firstPermission.GetType();
            var declaringClass = enumType.DeclaringType;
            if (declaringClass == null)
                throw new ArgumentNullException(nameof(declaringClass), "Declaring class cannot be null");

            // Lấy claim chứa quyền của user
            var claims = user.Claims
                .Where(c => c.Type == declaringClass.Name)
                .ToList();

            if (claims.Count == 0)
            {
                // lkiem63 tra phân quyền có được khai báo 
                return false;
            }
            if (claims.Count > 1)
                throw new Exception("Lỗi cấu hình dư claim");

            var totalPermission = Convert.ToInt32(claims.First().Value);

            // Gộp tất cả quyền yêu cầu thành một mask
            int requiredPermissionMask = 0;
            foreach (var permission in permissions)
            {
                if (permission == null)
                    throw new ArgumentNullException(nameof(permission), "Permission cannot be null");

                var CurrentDeclaringClass = permission.GetType().DeclaringType;
                if (CurrentDeclaringClass == null)
                    throw new ArgumentNullException(nameof(CurrentDeclaringClass), "Declaring class cannot be null");

                if (declaringClass.Name != CurrentDeclaringClass.Name)
                    throw new Exception($"Lỗi xác nhận interface{declaringClass.Name}!={CurrentDeclaringClass.Name}");

                requiredPermissionMask |= GetValuePermistion(permission);
            }

            // Kiểm tra xem user có đủ tất cả quyền không
            var result = HasPermisson(totalPermission, requiredPermissionMask);
            return result ? true : ResultCode.Unauthenticated;
        }
        public static int GetValuePermistion(Enum permissionEnum)
        {
            return 1 << Convert.ToInt32(permissionEnum);
        }
        public static bool HasPermisson(int totalPermission, int permistionRequired)
        {
            return (totalPermission & permistionRequired) == permistionRequired;
        }
        public static int MergerPermisson(params Enum[] permissionEnums)
        {
            return permissionEnums.Aggregate(0, (current, next) => current | GetValuePermistion(next));
        }
        public static int MergerPermisson(params int[] permissionEnums)
        {
            return permissionEnums.Aggregate(0, (current, next) => current | next);
        }
        public static int MergerPermisson(params string[] permissionEnums)
        {
            return MergerPermisson(permissionEnums.Select(int.Parse).ToArray());
        }
    }
}
