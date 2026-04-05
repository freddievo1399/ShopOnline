using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Infrastructure.Identity;


public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreateUserId { get; set; } = "";
    public DateTime? RemovedAt { get; set; }
    public string? RemovedUserId { get; set; }
}
