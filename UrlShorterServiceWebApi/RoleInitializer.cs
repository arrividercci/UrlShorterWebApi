using Microsoft.AspNetCore.Identity;
using UrlShorterServiceWebApi.Entities;

namespace UrlShorterServiceWebApi
{
    public static class RoleInitializer
    {
        public static async Task Initialize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "";
            string adminPassword = "";
            if (await roleManager.FindByNameAsync(RolesString.Admin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(RolesString.Admin));
            }
            if (await roleManager.FindByNameAsync(RolesString.User) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(RolesString.User));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User()
                {
                    Email = adminEmail,
                    UserName = adminEmail
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, RolesString.Admin);
                }
            }
        }
    }
}
