using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webrazorapp.models;

namespace MyApp.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;
        public EditUserRoleClaimModel(MyBlogContext myBlogContext, UserManager<AppUser> userManager)
        {
            _context = myBlogContext;
            _userManager = userManager;
        }
        
        [TempData]
        public string StatusMessage { get; set; }
        public NotFoundObjectResult OnGet() => NotFound("Không được truy cập");

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
        public InputModel Input { get; set; }
        public new AppUser? User { get; set; }
        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            User = await _userManager.FindByIdAsync(userid);
            if (User == null) return NotFound("Không tìm thấy user");
            return Page();
        }
        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            User = await _userManager.FindByIdAsync(userid);
            if (User == null) return NotFound("Không tìm thấy user");

            if (!ModelState.IsValid) return Page();

            var claims = _context.UserClaims.Where(c => c.UserId == User.Id);

            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có");
                return Page();
            }

            await _userManager.AddClaimAsync(User, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm đặc tính cho user";
            return RedirectToPage("./AddRole", new {Id = userid});
        }
        public IdentityUserClaim<string>? UserClaim { get; set; }
        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy user");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            User = await _userManager.FindByIdAsync(UserClaim.UserId);

            if (User == null) return NotFound("Không tìm thấy user");

            Input = new InputModel()
            {
                ClaimType = UserClaim.ClaimType,
                ClaimValue = UserClaim.ClaimValue
            };

            return Page();
        }
        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy user");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            User = await _userManager.FindByIdAsync(UserClaim.UserId);

            if (User == null) return NotFound("Không tìm thấy user");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.UserClaims.Any(c => c.UserId == User.Id 
                && c.ClaimType == Input.ClaimType 
                && c.ClaimValue == Input.ClaimValue 
                && c.Id != UserClaim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có");
                return Page();
            }

            UserClaim.ClaimType = Input.ClaimType;
            UserClaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa cập nhật claim";

            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy user");

            UserClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            User = await _userManager.FindByIdAsync(UserClaim.UserId);

            if (User == null) return NotFound("Không tìm thấy user"); 
            
            await _userManager.RemoveClaimAsync(User, new Claim(UserClaim.ClaimType, UserClaim.ClaimValue));
            StatusMessage = "Bạn đã xóa claim";

            return Page();
        }
    }
}