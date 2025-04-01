using Application.Extensions;
using Application.Model.Orders;
using Application.Services;
using Application.ViewModel.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Application.Models;
using static Application.Model.Orders.Order;

namespace Application.Areas.Manager
{
    [Authorize(Roles = "Manager")]
    public class IndexModel : PageModel
    {
        public IndexModel(DataManager dataManager)
        {
            this.dataManager = dataManager;
        }

        private readonly DataManager dataManager;


        public async Task<List<OrderViewModel>> GetOrders()
        {
            return await dataManager.Orders.All
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new OrderViewModel(x))
                .ToListAsync();
        }

        [BindProperty]
        public List<SelectListItem> Filters { get; set; }

        public IActionResult OnGet()
        {
            Filters = Enum.GetValues(typeof(Order.States)).Cast<Order.States>().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = EnumHelper.GetDisplayName(s),
                Selected = true,
            }).ToList();


            return Page();
        }

        public async Task<IActionResult> OnGetOrders()
        {
            return Partial("_OrdersPartial", await GetOrders());
        }
        public async Task<IActionResult> OnPostSort(string sortBy, string sortOrder)
        {
            IEnumerable<OrderViewModel> orders = await GetOrders();
            HttpContext.Session.SetString("sortBy", sortBy);
            HttpContext.Session.SetString("sortOrder", sortOrder);

            orders = GetFiltered(orders);
            orders = GetSorted(orders);
            orders = GetSearched(orders);

            return Partial("_OrdersPartial", orders);
        }
        public async Task<IActionResult> OnPostFilter(List<string> selectedStates)
        {
            IEnumerable<OrderViewModel> orders = await GetOrders();

            string filterString = string.Join(",", selectedStates);
            HttpContext.Session.SetString("filters", filterString);

            orders = GetFiltered(orders);
            orders = GetSorted(orders);
            orders = GetSearched(orders);

            return Partial("_OrdersPartial", orders);
        }
        public async Task<IActionResult> OnPostSearch(string searchString)
        {
            IEnumerable<OrderViewModel> orders = await GetOrders();

            HttpContext.Session.SetString("search", searchString ?? "");

            orders = GetFiltered(orders);
            orders = GetSorted(orders);
            orders = GetSearched(orders);


            return Partial("_OrdersPartial", orders);
        }

        private IEnumerable<OrderViewModel> GetSorted(IEnumerable<OrderViewModel> orders)
        {
            string sortBy = HttpContext.Session.GetString("sortBy") ?? "";
            string sortOrder = HttpContext.Session.GetString("sortOrder") ?? "";

            switch (sortBy)
            {
                case "state":
                    if (sortOrder == "asc")
                    {
                        orders = orders.OrderBy(p => p.State);
                    }
                    else
                    {
                        orders = orders.OrderByDescending(p => p.State);
                    }
                    break;
                case "date":
                    if (sortOrder == "asc")
                    {
                        orders = orders.OrderBy(p => p.OrderDate);
                    }
                    else
                    {
                        orders = orders.OrderByDescending(p => p.OrderDate);
                    }
                    break;
                case "price":
                    if (sortOrder == "asc")
                    {
                        orders = orders.OrderBy(p => p.Amount);
                    }
                    else
                    {
                        orders = orders.OrderByDescending(p => p.Amount);
                    }
                    break;
                case "quantity":
                    if (sortOrder == "asc")
                    {
                        orders = orders.OrderBy(p => p.Quantity);
                    }
                    else
                    {
                        orders = orders.OrderByDescending(p => p.Quantity);
                    }
                    break;
                case "type":
                    if (sortOrder == "asc")
                    {
                        orders = orders.OrderBy(p => p.Type);
                    }
                    else
                    {
                        orders = orders.OrderByDescending(p => p.Type);
                    }
                    break;
                default:
                    orders = orders.OrderBy(p => p.OrderDate);
                    break;
            }

            return orders;
        }
        private IEnumerable<OrderViewModel> GetFiltered(IEnumerable<OrderViewModel> orders)
        {
            string filtersString = HttpContext.Session.GetString("filters") ?? "";

            if (string.IsNullOrEmpty(filtersString))
                return orders;

            List<string> filters = new(filtersString.Split(","));
            List<Order.States> states = new List<Order.States>();
            foreach (string filter in filters)
            {
                states.Add((Order.States)Enum.Parse(typeof(Order.States), filter));
            }

            return orders.Where(x => states.Contains(x.State));
        }
        private IEnumerable<OrderViewModel> GetSearched(IEnumerable<OrderViewModel> orders)
        {
            string searchString = HttpContext.Session.GetString("search") ?? "";

            if (string.IsNullOrEmpty(searchString))
                return orders;

            
            return orders.Where(x => x.ClientName.Contains(searchString));
        }
    }
}
