using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace webrazorapp.models {
    public class AppUser : IdentityUser {
        [Column(TypeName = "nvarchar")]
        [StringLength(255)]
        public string? HomeAddress {get; set;}
    }

}