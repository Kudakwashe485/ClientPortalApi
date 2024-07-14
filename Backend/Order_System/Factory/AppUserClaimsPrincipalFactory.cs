using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Order_System.models.Login;

namespace Order_System.Factory
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, IdentityRole>
	{
		public AppUserClaimsPrincipalFactory(UserManager<AppUser> userManager,
		RoleManager<IdentityRole> roleManager,
		IOptions<IdentityOptions> optionsAccessor)
		: base(userManager, roleManager, optionsAccessor)
		{
		}

	}
}
