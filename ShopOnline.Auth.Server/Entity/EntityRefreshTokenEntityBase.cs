using ShopOnline.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Auth.Server.Entity
{
    [Table("Auth_RefreshToken")]
    public class EntityRefreshToken:EntityBase
    {
        [Required]
        public string AccessToken { get; set; } = default!;
        [Required]
        public string RefreshToken { get; set; } = default!;
        public DateTime RefreshTokenExpiration { get; set; }
        
        [Required]
        public string RefreshFrom { get; set; } = default!;

        [Required]
        public string UserName { get; set; } = default!;
    }
}
