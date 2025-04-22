using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Client.Order
{
    public class SuccessModel : PageModel
    {
        [FromQuery]
        public int OrderId { get; set; }

        public void OnGet()
        {
        }
    }
} 