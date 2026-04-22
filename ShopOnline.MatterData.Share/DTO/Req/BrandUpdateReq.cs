using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.MatterData.Share.DTO.Req
{
    public class BrandUpdateReq
    {
        [Required]
        public required Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Code { get; set; }
        public string? Description { get; set; }
    }
}
