using RestEase;
using ShopOnline.Abstractions.Requests;
using ShopOnline.Abstractions.Wrappers;
using ShopOnline.Auth.Share.Attributes;
using ShopOnline.MatterData.Share.DTO.Req;
using ShopOnline.MatterData.Share.DTO.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.MatterData.Share.Interface;

[BasePath("api/matterdata/BrandManager")]
public interface IBrandManager
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

    [Post(nameof(GetData))]
    Task<PagedResultsOf<BrandSummaryRes>> GetData([Body] DataQueryRequest dataQueryRequest);

    [Get(nameof(GetDetailBrand))]
    Task<ResultOf<BrandDetailRes>> GetDetailBrand(Guid Id);


    [Post(nameof(Create))]
    Task<Result> Create([Body] BrandCreateReq brandCreateReq);

    [Post(nameof(Update))]
    Task<ResultOf<BrandSummaryRes>> Update([Body] BrandUpdateReq brandUpdateReq);

    [Delete(nameof(Remove))]
    Task<Result> Remove(Guid Id);

}
