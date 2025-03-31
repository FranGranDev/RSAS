using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.ViewModel.Data;
using Application.Model.Stocks;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Application.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        public CreateModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        private readonly DataManager dataManager;


        [BindProperty]
        public ProductViewModel Product { get; set; }

        public void OnGet()
        {
            Product = new ProductViewModel();
        }
        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            Product product = new Product
            {
                Name = Product.Name,
                Description = Product.Description,
                WholesalePrice = Product.WholesalePrice,
                RetailPrice = Product.RetailPrice,
            };

            dataManager.Products.Save(product);

            TempData["success"] = "Товар успешно создан";

            return RedirectToPage("Index");
        }
    }
}
