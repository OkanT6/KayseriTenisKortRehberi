using KayseriTenisKortRehberi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KayseriTenisKortRehberi.Controllers
{
    public class DeleteFacilityController : Controller
    {
        // Tesis silme onay sayfası
        public IActionResult Index(int id)
        {
            try
            {
                // Tesis bilgilerini getir
                // var facility = _facilityService.GetFacilityById(id);

                // Simüle edilmiş veri
                var facility = new FacilityViewModel
                {
                    Id = id,
                    FacilityName = "Örnek Tesis",
                    FacilityDescription = "Örnek açıklama",
                    Latitude = 38.7225,
                    Longitude = 35.4847
                };

                if (facility == null)
                {
                    TempData["ErrorMessage"] = "Tesis bulunamadı!";
                    return RedirectToAction("Index", "Home");
                }

                return View(facility);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Tesis bilgileri getirilirken hata oluştu: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // Tesis silme işlemi
        [HttpPost]
        public IActionResult Delete(int id, string apiKey, string userName, string password)
        {
            try
            {
                // Kimlik doğrulama kontrolü
                if (!ValidateAdmin(apiKey, userName, password))
                {
                    TempData["ErrorMessage"] = "Geçersiz kimlik bilgileri!";
                    return RedirectToAction("DeletePage", new { id = id });
                }

                // Tesisi sil
                // _facilityService.DeleteFacility(id);

                TempData["SuccessMessage"] = "Tesis başarıyla silindi!";
                return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Tesis silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("DeletePage", new { id = id });
            }
        }

        // Yardımcı method'lar
        private bool ValidateAdmin(string apiKey, string userName, string password)
        {
            // Gerçek uygulamada burada database kontrolü yapılacak
            // Şimdilik basit bir kontrol
            return !string.IsNullOrEmpty(apiKey) &&
                   !string.IsNullOrEmpty(userName) &&
                   !string.IsNullOrEmpty(password);
        }

       
    }
}
