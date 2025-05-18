using System.Collections.Generic;
using System.Linq;

namespace Frontend.Services
{
    public class NavigationService
    {
        public class BreadcrumbItem
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public bool IsActive { get; set; }
            public bool IsClickable { get; set; } = true;
        }

        private readonly Dictionary<string, string> _segmentTitles = new()
        {
            { "Account", "Аккаунт" },
            { "Client", "Клиент" },
            { "Manager", "Менеджер" },
            { "Order", "Заказы" },
            { "Manage", "Управление" },
            { "Create", "Создание" },
            { "Edit", "Редактирование" },
            { "Details", "Детали" },
            { "Stocks", "Склады" },
            { "View", "Просмотр" },
            { "Products", "Товары" },
            { "Analytics", "Аналитика" },
            { "Profile", "Профиль" },
            { "Catalog", "Каталог" },
            { "Success", "Успешно" }
        };

        private readonly HashSet<string> _excludedPaths = new()
        {
            "/Account/Login",
            "/Account/Register",
            "/",
            "/Manager",
            "/Client",
            "/Account"
        };

        private readonly HashSet<string> _nonClickableSegments = new()
        {
            "Success",
            "View",
            "Manage",
            "Create",
            "Edit",
            "Details",
            "Account"
        };

        private readonly HashSet<string> _skipSegments = new()
        {
            "Client",
            "Manager"
        };

        public bool ShouldShowBreadcrumbs(string currentPath)
        {
            return !_excludedPaths.Contains(currentPath);
        }

        public List<BreadcrumbItem> GetBreadcrumbs(string currentPath)
        {
            if (!ShouldShowBreadcrumbs(currentPath))
            {
                return new List<BreadcrumbItem>();
            }

            var breadcrumbs = new List<BreadcrumbItem>();
            
            // Добавляем домашнюю страницу
            breadcrumbs.Add(new BreadcrumbItem 
            { 
                Title = "Главная", 
                Url = "/", 
                IsActive = currentPath == "/" 
            });

            // Разбиваем путь на сегменты
            var segments = currentPath.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
            var currentUrl = "";

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                currentUrl += $"/{segment}";
                
                // Пропускаем сегменты, которые ведут на главную
                if (_skipSegments.Contains(segment) && i == 0)
                {
                    continue;
                }
                
                // Проверяем, является ли сегмент числом (ID)
                bool isNumeric = int.TryParse(segment, out _);
                
                // Проверяем, является ли следующий сегмент числом
                bool nextIsNumeric = i + 1 < segments.Length && int.TryParse(segments[i + 1], out _);
                
                // Определяем, можно ли кликать по ссылке
                bool isClickable = !isNumeric && 
                                 !nextIsNumeric && 
                                 !_nonClickableSegments.Contains(segment);

                var title = GetTitleFromSegment(segment, segments, i);
                
                breadcrumbs.Add(new BreadcrumbItem
                {
                    Title = title,
                    Url = isClickable ? currentUrl : null,
                    IsActive = currentUrl == currentPath,
                    IsClickable = isClickable
                });
            }

            return breadcrumbs;
        }

        private string GetTitleFromSegment(string segment, string[] allSegments, int currentIndex)
        {
            if (_segmentTitles.TryGetValue(segment, out var title))
            {
                return title;
            }

            // Если сегмент является числом (ID)
            if (int.TryParse(segment, out _))
            {
                // Определяем тип сущности на основе предыдущих сегментов
                if (allSegments.Contains("Order"))
                {
                    return $"Заказ #{segment}";
                }
                else if (allSegments.Contains("Stocks"))
                {
                    return $"Склад #{segment}";
                }
                else
                {
                    return $"#{segment}";
                }
            }

            return segment;
        }
    }
} 