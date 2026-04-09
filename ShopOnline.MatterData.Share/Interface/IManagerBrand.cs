using ShopOnline.Auth.Share.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.MatterData.Share.Interface;

public interface IManagerBrand
{
    [Feature("Quản lý doanh mục chung", "Quản lý thương hiệu")]
    public enum PERMISSION
    {
        [Permistion("Xem danh sách")]
        ALLOW_VIEW,

        [Permistion("Thêm")]
        ALLOW_ADD,

        [Permistion("Sửa")]
        ALLOW_UPDATE,

        [Permistion("Xóa")]
        ALLOW_DELETE,
    }
}
