using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Entity;

public class EntityBase
{
    [Key]
    [Required]
    public required Guid Id { get; set; }=Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "NA";

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string UpdatedBy { get; set; } = "NA";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    [MaxLength(100)]
    public string? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
}
