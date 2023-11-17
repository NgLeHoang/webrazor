using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webrazorapp.models;

namespace MyApp.Admin.Role 
{
    [Authorize(Roles = "Admin")]
    public class EditRoleClaim : RolePageModel
    {
        public EditRoleClaim(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }

        public class InputModel 
        {
            [Display(Name = "Kiểu (tên) claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimType { get; set; }

            [Display(Name = "Giá trị")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimValue { get; set; }

        }

        [BindProperty]
        public InputModel Input { get ; set; }
        public IdentityRole? Role { get; set; }
        IdentityRoleClaim<string>? Claim { get; set; }
        public async Task<IActionResult> OnGet(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");

            Claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (Claim == null) return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null) return NotFound("Không tìm thấy role");

            Input = new InputModel()
            {
                ClaimType = Claim.ClaimType,
                ClaimValue = Claim.ClaimValue
            };
            
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");

            Claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (Claim == null) return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null) return NotFound("Không tìm thấy role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            if (_context.RoleClaims.Any(c => c.RoleId == Role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != Claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            Claim.ClaimType = Input.ClaimType;
            Claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = "Vừa cập nhật claim.";

            return RedirectToPage("./Edit", new {roleid = Role.Id});
        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");

            Claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (Claim == null) return NotFound("Không tìm thấy role");

            Role = await _roleManager.FindByIdAsync(Claim.RoleId);
            if (Role == null) return NotFound("Không tìm thấy role");
     
            await _roleManager.RemoveClaimAsync(Role, new Claim(Claim.ClaimType, Claim.ClaimValue));

            StatusMessage = "Vừa xóa claim";
            return RedirectToPage("./Edit", new {roleid = Role.Id});
        }
    }
}