using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webrazorapp.models;

namespace MyApp.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, AppDbContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }
        
        public class RoleModel : IdentityRole
        {
            public string[] Claims { get; set; }
        }
        public List<RoleModel> Roles { get; set; }
        public async Task OnGet()
        {
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

            Roles = new List<RoleModel>();
            foreach (var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);
                var claimsString = claims.Select(c => c.Type + "=" + c.Value);
                var rm = new RoleModel()
                {
                    Name = _r.Name,
                    Id = _r.Id,
                    Claims = claimsString.ToArray()
                };
                Roles.Add(rm);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
