using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Abstractions.Constants
{
    public static class ResultCode
    {
        // 1. Không tìm thấy (API bị sai, hoặc bản ghi không tồn tại)
        public const string NotFound = "NOT_FOUND";

        // 2. Chưa đăng nhập hoặc Refresh Token thất bại (Client cần đá về Login)
        public const string Unauthenticated = "UNAUTHENTICATED";

        // 3. Đã đăng nhập nhưng PermissionHub chặn (Không đủ quyền)
        public const string AccessDenied = "ACCESS_DENIED";

        // 4. Lỗi nghiệp vụ (Dữ liệu gửi lên sai, kho hết hàng, v.v.)
        public const string BadRequest = "BAD_REQUEST";

        // 5. Lỗi server không xác định
        public const string SystemError = "SYSTEM_ERROR";
    }
}
