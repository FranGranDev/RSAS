using Application.Model.Stocks;
using Application.Services;
using Application.ViewModel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly DataManager dataManager;

        public EditModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }


        [BindProperty] public ProductViewModel Product { get; set; }

        [BindNever] public string ReturnUrl { get; set; }

        public void OnGet(int id, string returnUrl)
        {
            ReturnUrl = returnUrl;

            Product = new ProductViewModel(dataManager.Products.Get(id));
        }

        public IActionResult OnPostApply()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage(new { id = Product.Id, returnUrl = ReturnUrl });
            }

            var product = new Product
            {
                Id = Product.Id,
                Name = Product.Name,
                Description = Product.Description,
                WholesalePrice = Product.WholesalePrice,
                RetailPrice = Product.RetailPrice
            };

            dataManager.Products.Save(product);

            TempData["success"] = "Товар успешно изменен";

            return OnPostBack();
        }

        public IActionResult OnPostBack()
        {
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                return RedirectToPage("Index");
            }

            return Redirect(ReturnUrl);
        }
    }
}