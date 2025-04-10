using Application.Areas.Identity.Data;
using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Application.Pages
{
    [Authorize(Roles = "Client, Company")]
    public class OrdersModel : PageModel
    {
        public OrdersModel(DataManager dataManager, UserManager<AppUser> userManager)
        {
            this.dataManager = dataManager;
            this.userManager = userManager;
        }

        private readonly UserManager<AppUser> userManager;
        private readonly DataManager dataManager;


        public IEnumerable<OrderViewModel> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                Orders = new List<OrderViewModel>();
                return Page();
            }


            Orders = await dataManager.Orders.All
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new OrderViewModel(x))
                .ToListAsync();

            return Page();
        }
    }
}
