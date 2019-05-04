using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IPaymentService
    {
        Task<decimal> GetCurrentAmount(ApplicationUser user);
    }
}