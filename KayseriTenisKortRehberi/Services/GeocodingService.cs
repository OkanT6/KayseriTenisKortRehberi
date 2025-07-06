using KayseriTenisKortRehberi.Models;
using Newtonsoft.Json.Linq;

namespace KayseriTenisKortRehberi.Services
{
    public class GeocodingService
    {
        private readonly HttpClient _httpClient = new();

        public GeocodingService()
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "KayseriTenisKortRehberi");
        }

        /// <summary>
        /// Koordinatlardan AddressModel döndürür.
        /// </summary>
        public async Task<AddressModel?> GetAddressAsync(double latitude, double longitude)
        {
            string url =
                $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";

            try
            {
                string response = await _httpClient.GetStringAsync(url);
                JObject json = JObject.Parse(response);

                // JSON içindeki alanları modele taşı
                var addrToken = json["address"];
                if (addrToken is null) return null;

                return new AddressModel
                {
                    TamAdres = json["display_name"]?.ToString() ?? "Adres bulunamadı.",
                    Il = addrToken["province"]?.ToString() ?? "",
                    Ilce = addrToken["town"]?.ToString() ?? "",
                    Mahalle = addrToken["suburb"]?.ToString() ?? "",
                    Cadde = addrToken["road"]?.ToString() ?? ""
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
