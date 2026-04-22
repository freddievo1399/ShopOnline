using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Abstractions.Requests;
using ShopOnline.Abstractions.Wrappers;
using ShopOnline.Auth.Share.Utils;
using ShopOnline.MatterData.Share.DTO.Req;
using ShopOnline.MatterData.Share.DTO.Respone;
using ShopOnline.MatterData.Share.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShopOnline.MatterData.Share.Interface.IBrandManager;

namespace ShopOnline.MatterData.Server.Controller
{
    [ApiController]
    [Route("/api/matterdata/[controller]")]
    [ApiExplorerSettings(GroupName = "MatterData")]
    public class BrandManagerController(IHttpContextAccessor httpContextAccessor) : ControllerBase, IBrandManager
    {
        [HttpPost(nameof(Create))]
        public Task<Result> Create([FromBody] BrandCreateReq brandCreateReq)
        {
            throw new NotImplementedException();
        }

        [HttpPost(nameof(GetData))]
        public Task<PagedResultsOf<BrandSummaryRes>> GetData([FromBody] DataQueryRequest dataQueryRequest)
        {
            throw new NotImplementedException();
        }
        [HttpGet(nameof(GetDetailBrand))]
        public Task<ResultOf<BrandDetailRes>> GetDetailBrand(Guid Id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete(nameof(Remove))]
        public async Task<Result> Remove(Guid Id)
        {
            Result checkPermission = httpContextAccessor?.HttpContext?.User.CheckPermission(PERMISSION.ALLOW_DELETE)??throw(new ());
            if (!checkPermission.Success)
            {
                return checkPermission;
            }
            var abc = httpContextAccessor;
            return Result.Ok();
        }

        [HttpPost(nameof(Update))]
        public Task<ResultOf<BrandSummaryRes>> Update([FromBody] BrandUpdateReq brandUpdateReq)
        {
            throw new NotImplementedException();
        }
    }
}
