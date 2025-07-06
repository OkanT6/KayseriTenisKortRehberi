using KayseriTenisKortRehberi.Models;
using KayseriTenisKortRehberi.Models.ViewModels;
using KayseriTenisKortRehberi.Services;
using Microsoft.AspNetCore.Mvc;

namespace KayseriTenisKortRehberi.Controllers
{
    public class FacilityDetailsController : Controller
    {
        // Basit örnek olduğu için direkt instance; gerçek projede DI kullan
        private readonly GeocodingService _geoService = new();

        // ÖRNEK koordinatlar sabit; ileride DB'den veya query‑string’ten alabilirsin
        private const double SampleLat = 38.731221;
        private const double SampleLon = 35.478702;

        public async Task<IActionResult> FacilityDetails(string id)
        {
            var address = await _geoService.GetAddressAsync(SampleLat, SampleLon);

            var vm = new FacilityDetailsViewModel
            {
                Id = id,
                Address = address
            };

            return View(vm);
        }
    }
}
