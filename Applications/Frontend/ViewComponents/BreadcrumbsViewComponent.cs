using Microsoft.AspNetCore.Mvc;
using Frontend.Services;

namespace Frontend.ViewComponents
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        private readonly NavigationService _navigationService;

        public BreadcrumbsViewComponent(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public IViewComponentResult Invoke()
        {
            var currentPath = HttpContext.Request.Path.Value;
            
            if (!_navigationService.ShouldShowBreadcrumbs(currentPath))
            {
                return Content(string.Empty);
            }

            var breadcrumbs = _navigationService.GetBreadcrumbs(currentPath);
            return View(breadcrumbs);
        }
    }
} 