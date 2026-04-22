using ShopOnline.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.MatterData.Server.Entity;

[Table("MatterData_Brand")]
public class EntityBrand : EntityBase
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Code { get; set; }

    [MaxLength(500)]
    public string? UrlImage { get; set; }

    [MaxLength(500)]
    public string? UrlImageThumbnail { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
