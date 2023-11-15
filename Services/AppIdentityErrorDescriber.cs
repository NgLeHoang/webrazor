using Microsoft.AspNetCore.Identity;

namespace MyApp.Services 
{
    public class AppIdentityErroDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateRoleName(string role)
        {
            var error = base.DuplicateRoleName(role);
            return new IdentityError() 
            {
                Code = error.Code,
                Description = $"Role có tên {role} bị trùng"
            };
        }
    }
}