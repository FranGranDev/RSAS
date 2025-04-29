using System.Threading.Tasks;
using Application.DTOs.Test;

namespace Application.Services.Test
{
    public interface ITestDataService
    {
        Task GenerateTestSalesAsync(GenerateSalesDto dto, string userId);
    }
} 