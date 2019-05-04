using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Options;
using WebMVC.Infrastructure;

namespace WebMVC.Services
{
    public class PaymentService : IPaymentService
    {
        private HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptions<AppSettings> _settings;

        public PaymentService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;

            _remoteServiceBaseUrl = $"{settings.Value.PaymentUrl}/api/v1/payment/";
        }

        public async Task<decimal> GetCurrentAmount(ApplicationUser user)
        {
            var uri = API.Payment.GetUserAmmount(_remoteServiceBaseUrl, user.Id);

            var responseString = await _httpClient.GetStringAsync(uri);

            return decimal.Parse(responseString);
        }
    }
}