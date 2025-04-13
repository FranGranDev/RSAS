using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Application.MyTagHelpers
{
    [HtmlTargetElement(Attributes = "if-role")]
    public class ShowIfRoleTagHelper : TagHelper
    {
        private readonly IActionContextAccessor actionContext;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public ShowIfRoleTagHelper(IActionContextAccessor actionContext, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.actionContext = actionContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HtmlAttributeName("if-role")] public string Role { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = actionContext.ActionContext.HttpContext.User;
            var currentUser = await userManager.GetUserAsync(user);

            if (currentUser != null && user.Identity.IsAuthenticated)
            {
                var roleExists = await roleManager.RoleExistsAsync(Role);

                if (roleExists && await userManager.IsInRoleAsync(currentUser, Role))
                {
                    // Пользователь авторизован и принадлежит роли, показываем содержимое тега.
                    return;
                }
            }

            // Пользователь не авторизован или не принадлежит указанной роли, удаляем содержимое тега.
            output.SuppressOutput();
        }
    }
}