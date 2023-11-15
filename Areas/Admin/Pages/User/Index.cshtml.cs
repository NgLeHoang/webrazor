using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webrazorapp.models;

namespace MyApp.Admin.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public class UserAndRole : AppUser
        {
            public string RoleNames { get ; set; }
        }

        public const int ITEMS_PER_PAGE = 15;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }
        public int CountPage { get; set; }

        public int TotalUsers { get; set; }
        public List<UserAndRole> Users { get; set; }
        public async Task OnGet()
        {
            // Users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var query = _userManager.Users.OrderBy(u => u.UserName);

            TotalUsers = await query.CountAsync();

            CountPage = (int)Math.Ceiling((double)TotalUsers / ITEMS_PER_PAGE);

            if (CurrentPage < 1)
                CurrentPage = 1;
            if (CurrentPage > CountPage)
                CurrentPage = CountPage;

            var query_1 = query.Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                        .Take(ITEMS_PER_PAGE)
                        .Select(u => new UserAndRole(){
                            Id = u.Id,
                            UserName = u.UserName,
                        });

            Users = await query_1.ToListAsync();

            foreach (var user in Users) 
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
