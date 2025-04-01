using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly DataManager dataManager;

        public CreateModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }


        [BindProperty] public ProductViewModel Product { get; set; }

        public void OnGet()
        {
            Product = new ProductViewModel();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var product = new Product
            {
                Name = Product.Name,
                Description = Product.Description,
                WholesalePrice = Product.WholesalePrice,
                RetailPrice = Product.RetailPrice
            };

            dataManager.Products.Save(product);

            TempData["success"] = "Товар успешно создан";

            return RedirectToPage("Index");
        }
    }
}