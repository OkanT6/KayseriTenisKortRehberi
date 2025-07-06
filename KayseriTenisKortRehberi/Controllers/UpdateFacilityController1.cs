using KayseriTenisKortRehberi.Models.ViewModels;
using KayseriTenisKortRehberi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KayseriTenisKortRehberi.Controllers
{
    public class UpdateFacilityController1 : Controller
    {
        // Tesis güncelleme sayfası
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

        // Tesis güncelleme işlemi
        [HttpPost]
        public IActionResult Update(FacilityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdatePage", model);
            }

            try
            {
                // Kimlik doğrulama kontrolü
                if (!ValidateAdmin(model.ApiKey, model.UserName, model.Password))
                {
                    ModelState.AddModelError("", "Geçersiz kimlik bilgileri!");
                    return View("UpdatePage", model);
                }

                // Koordinat validasyonu
                if (model.Latitude < -90 || model.Latitude > 90 ||
                    model.Longitude < -180 || model.Longitude > 180)
                {
                    ModelState.AddModelError("", "Geçersiz koordinat değerleri!");
                    return View("UpdatePage", model);
                }

                // Dosya yükleme işlemi
                string photoPath = model.PhotoUrl; // Mevcut fotoğrafı koru
                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    photoPath = SaveUploadedFile(model.PhotoFile);
                }

                // Veritabanında güncelle
                var facility = new Facility
                {
                    Id = model.Id,
                    Name = model.FacilityName,
                    Description = model.FacilityDescription,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    PhotoUrl = photoPath,
                    UpdatedDate = DateTime.Now
                };

                // _facilityService.UpdateFacility(facility);

                TempData["SuccessMessage"] = "Tesis başarıyla güncellendi!";
                return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Tesis güncellenirken bir hata oluştu: " + ex.Message);
                return View("UpdatePage", model);
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

        private string SaveUploadedFile(IFormFile file)
        {
            try
            {
                // Dosya adını oluştur
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, fileName);

                // Dosyayı kaydet
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                return "/uploads/" + fileName;
            }
            catch (Exception ex)
            {
                throw new Exception("Dosya kaydedilirken hata oluştu: " + ex.Message);
            }
        }
    }
}
