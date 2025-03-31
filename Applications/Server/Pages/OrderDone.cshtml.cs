using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Pages
{
    public class OrderDoneModel : PageModel
    {
        public OrderDoneModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        private readonly DataManager dataManager;



        public void OnGet(int orderId)
        {

        }
    }
}
