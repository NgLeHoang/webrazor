// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webrazorapp.models;

namespace MyApp.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _context;
        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            MyBlogContext myBlogContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = myBlogContext;
        }

        [TempData]
        public string StatusMessage { get; set; }


        public AppUser User { get; set; }

        [BindProperty]
        [DisplayName("Các role gán cho user")]
        public string[] RoleNames { get; set; }
        public SelectList AllRoles { get; set; }

        public List<IdentityRoleClaim<string>> ClaimsInRole { get; set; }
        public List<IdentityUserClaim<string>> ClaimsInUserClaim { get; set; } 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có User");
            }
            User = await _userManager.FindByIdAsync(id);

            if (User == null)
            {
                return NotFound($"Không thấy user, id = '{id}'.");
            }

            RoleNames = (await _userManager.GetRolesAsync(User)).ToArray<string>();

            List<string> roleName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(roleName);

            await GetClaims(id);

            return Page();
        }
        async Task GetClaims(string id)
        {
            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;
            
            var _claimsInRole = from c in _context.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;

            ClaimsInRole = await _claimsInRole.ToListAsync();

            ClaimsInUserClaim = await (from c in _context.UserClaims
            where c.UserId == id select c).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có User");
            }

            User = await _userManager.FindByIdAsync(id);

            if (User == null)
            {
                return NotFound($"Không thấy user, id = '{id}'.");
            }

            await GetClaims(id);

            var OldRoleNames = (await _userManager.GetRolesAsync(User)).ToArray();

            var deleteRoles = OldRoleNames.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !OldRoleNames.Contains(r));

            List<string> roleName = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            AllRoles = new SelectList(roleName);

            var resultDelete = await _userManager.RemoveFromRolesAsync(User, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(User, addRoles);
            if (!resultDelete.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Vừa cập nhật role cho user: {User.UserName}.";

            return RedirectToPage("./Index");
        }
    }
}
