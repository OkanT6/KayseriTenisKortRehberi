using KayseriTenisKortRehberi.Context;
using KayseriTenisKortRehberi.Models;
using KayseriTenisKortRehberi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace KayseriTenisKortRehberi.Controllers
{
    public class CreateCourtController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext dbContext;

        public CreateCourtController(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        // GET: CreateCourt
        public async Task<IActionResult> Index()
        {
            try
            {
                // Async metodu await ile çağırıyoruz
                var facilities = await dbContext.Facilities
                    .Include(f => f.Address) // Address bilgilerini de dahil et
                    .ToListAsync();

                return View(facilities);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapabilirsiniz
                Console.WriteLine($"Hata: {ex.Message}");
                return View(new List<Facility>());
            }
        }

        [HttpPost]
        [Route("CreateCourt/AddCourt")]
        public async Task<IActionResult> AddCourt(CourtViewModel model)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Lütfen tüm gerekli alanları doldurun." });
                    }
                    TempData["ErrorMessage"] = "Lütfen tüm gerekli alanları doldurun.";
                    return RedirectToAction("Index");
                }

                // API Key ve admin bilgilerini kontrol et
                var configApiKey = configuration["ApiKeyAdmin"];

                // Veritabanından admin bilgilerini kontrol et
                var admin = await dbContext.Admins
                    .FirstOrDefaultAsync(a => a.Username == model.UserName && a.Password == model.Password);

                // Configuration değerlerinin varlığını kontrol et
                if (string.IsNullOrEmpty(configApiKey))
                {
                    var errorMessage = "Sistem yapılandırma hatası. Lütfen yönetici ile iletişime geçin.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToAction("Index");
                }

                // Admin bilgilerini doğrula (API Key config'den, kullanıcı bilgileri database'den)
                if (model.ApiKey != configApiKey || admin == null)
                {
                    if (model.ApiKey != configApiKey)
                    {
                        // apiKey hatasını logla
                    }
                    if (admin==null)
                    {
                        // Kullanıcı adı veya şifre hatasını logla
                    }

                    var errorMessage = "Yetkisiz erişim. API Key, kullanıcı adı veya şifre hatalı.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToAction("Index");
                }

                // Facility'nin var olup olmadığını kontrol et
                var facility = await dbContext.Facilities
                    .FirstOrDefaultAsync(f => f.Id == model.FacilityId);

                if (facility == null)
                {
                    var errorMessage = "Seçilen tesis bulunamadı.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToAction("Index");
                }

                // Aynı isimde kort olup olmadığını kontrol et
                var existingCourt = await dbContext.Courts
                    .FirstOrDefaultAsync(c => c.CourtName == model.CourtName && c.FacilityId == model.FacilityId);

                if (existingCourt != null)
                {
                    var errorMessage = $"Bu tesiste '{model.CourtName}' adında bir kort zaten mevcut.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMessage });
                    }
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToAction("Index");
                }

                // Yeni kort oluştur
                var newCourt = new Court
                {
                    CourtName = model.CourtName.Trim(),
                    SurfaceType = model.SurfaceType,
                    FacilityId = model.FacilityId
                };

                // Veritabanına ekle
                dbContext.Courts.Add(newCourt);
                await dbContext.SaveChangesAsync();

                // Başarı mesajı
                var successMessage = $"'{model.CourtName}' kortu başarıyla '{facility.Name}' tesisine eklendi.";

                // AJAX isteği ise JSON response dön
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = true,
                        message = successMessage,
                        courtName = model.CourtName,
                        facilityName = facility.Name
                    });
                }

                TempData["SuccessMessage"] = successMessage;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Console.WriteLine($"Kort ekleme hatası: {ex.Message}");
                var errorMessage = "Kort eklenirken bir hata oluştu. Lütfen tekrar deneyin.";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = errorMessage });
                }
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Index");
            }
        }

        // GET: CreateCourt/GetFacilityInfo/{id}
        [HttpGet]
        [Route("CreateCourt/GetFacilityInfo/{id}")]
        public async Task<IActionResult> GetFacilityInfo(int id)
        {
            try
            {
                var facility = await dbContext.Facilities
                    .Include(f => f.Address)
                    .Include(f => f.Courts)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (facility == null)
                {
                    return NotFound();
                }

                var facilityInfo = new
                {
                    Id = facility.Id,
                    Name = facility.Name,
                    Description = facility.Description,
                    Address = facility.Address?.TamAdres ?? "Adres bilgisi yok",
                    CourtCount = facility.Courts?.Count ?? 0,
                    Courts = facility.Courts?.Select(c => new
                    {
                        Id = c.Id,
                        Name = c.CourtName,
                        SurfaceType = c.SurfaceType
                    }).ToList()
                };

                return Json(facilityInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tesis bilgisi getirme hatası: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // GET: CreateCourt/CheckCourtName
        [HttpGet]
        [Route("CreateCourt/CheckCourtName")]
        public async Task<IActionResult> CheckCourtName(string courtName, int facilityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(courtName))
                {
                    return Json(new { exists = false });
                }

                var exists = await dbContext.Courts
                    .AnyAsync(c => c.CourtName.ToLower() == courtName.ToLower() && c.FacilityId == facilityId);

                return Json(new { exists = exists });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kort adı kontrol hatası: {ex.Message}");
                return Json(new { exists = false });
            }
        }
    }
}