using KayseriTenisKortRehberi.Context;
using KayseriTenisKortRehberi.Models;
using KayseriTenisKortRehberi.Models.ViewModels;
using KayseriTenisKortRehberi.Services;
using Microsoft.AspNetCore.Mvc;

namespace KayseriTenisKortRehberi.Controllers
{
    public class CreateFacilityController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly GeocodingService geocodingService;
        private readonly ApplicationDbContext dbContext;

        public CreateFacilityController(IConfiguration configuration, GeocodingService geocodingService, ApplicationDbContext dbContext)
        {
            this.configuration = configuration;
            this.geocodingService = geocodingService;
            this.dbContext = dbContext;
        }
        // Admin paneli ana sayfası (isteğe bağlı)
        public IActionResult Index()
        {
            // Admin paneli dashboard'u
            return View();
        }

        //// Yeni tesis ekleme sayfası
        //public IActionResult Add()
        //{
        //    return View();
        //}

        // Yeni tesis ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> AddAsync(FacilityViewModel model)
        {
            //Valid Değilse Sayfanın kendisini yeniden gönder
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Kimlik doğrulama kontrolü
                if (!ValidateAdmin(model.ApiKey, model.UserName, model.Password))
                {
                    ModelState.AddModelError("", "Geçersiz kimlik bilgileri!");
                    return View("Index", model);
                }

                // Koordinat validasyonu
                if (model.Latitude < -90 || model.Latitude > 90 ||
                    model.Longitude < -180 || model.Longitude > 180)
                {
                    ModelState.AddModelError("", "Geçersiz koordinat değerleri!");
                    return View("Index", model);
                }

                // Dosya yükleme işlemi
                string photoPath = model.PhotoUrl;
                if (model.PhotoFile != null && model.PhotoFile.Length > 0)
                {
                    photoPath = SaveUploadedFile(model.PhotoFile);
                }

                // Veritabanına kaydet
                var facility = new Facility
                {
                    Name = model.FacilityName,
                    Description = model.FacilityDescription,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    PhotoUrl = photoPath ?? model.PhotoUrl,
                    CreatedDate = DateTime.Now
                };


                AddressModel? addressModel = await geocodingService.GetAddressAsync(model.Latitude, model.Longitude);

                AddressModel address = new AddressModel
                {
                    TamAdres = addressModel?.TamAdres ?? "",
                    Il = addressModel?.Il ?? "",
                    Ilce = addressModel?.Ilce ?? "",
                    Mahalle = addressModel?.Mahalle ?? "",
                    Cadde = addressModel?.Cadde ?? ""
                };

                facility.Address = address;

                dbContext.Facilities.Add(facility);
                dbContext.SaveChanges();

                // Başarılı işlem sonrası aynı view'da kalıp mesaj göster
                ViewBag.SuccessMessage = "Tesis başarıyla eklendi!";

                // Formu temizlemek için boş model gönder
                return View("Index", new FacilityViewModel());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Tesis eklenirken bir hata oluştu: " + ex.Message);
                return View("Index", model);
            }
        }

        // Yardımcı method'lar
        private bool ValidateAdmin(string apiKey, string userName, string password)
        {
            // API anahtarı kontrolü
            if (apiKey != configuration["ApiKeyAdmin"])
            {
                // loglama yapılabilir
                return false;
            }

            // Kullanıcı adı ve şifre kontrolü
            var admin = dbContext.Admins.FirstOrDefault(a => a.Username == userName && a.Password == password);
            if (admin == null)
            {
                // loglama yapılabilir
                return false;
            }

            return true;
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