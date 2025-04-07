using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Application.Extensions
{
    public static class EnumHelper
    {
        public static string GetDisplayName(this Enum value)
        {
            try
            {
                return value.GetType()
                    .GetMember(value.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>()
                    .GetName();
            }
            catch
            {
                return value.ToString();
            }
        }
    }
}