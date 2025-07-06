using System.Diagnostics;
using KayseriTenisKortRehberi.Context;
using KayseriTenisKortRehberi.Models;
using KayseriTenisKortRehberi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KayseriTenisKortRehberi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;  // Doğru atama burada
        }

        public async Task<IActionResult> Index()
        {
            // Veritabanından tüm tesisleri ve kortlarını al
            var facilities = await dbContext.Facilities
                .Include(f => f.Courts)
                .Include(f => f.Address)  // Address tablosunu dahil et
                .ToListAsync();

            // Null kontrolü: facilities null olabilir, boş listeye çevir
            facilities ??= new List<Facility>();

            var locations = new List<MapLocationModel>();

            foreach (var facility in facilities)
            {
                // Courts null ise boş liste ata
                var courts = facility.Courts ?? new List<Court>();

                var courtInfos = courts.Select(c => new CourtInfo
                {
                    Id = c.Id,
                    CourtName = c.CourtName,
                    SurfaceType = c.SurfaceType
                }).ToList();

                var mainSurfaceType = courts
                    .GroupBy(c => c.SurfaceType)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key ?? "Bilinmiyor";

                var mapLocation = new MapLocationModel
                {
                    Id = $"facility_{facility.Id}",
                    FacilityId = facility.Id,
                    Latitude = facility.Latitude,
                    Longitude = facility.Longitude,
                    ZoomLevel = 16,
                    Title = facility.Name,
                    Description = facility.Description,
                    IconEmoji = "🎾",
                    AdditionalInfoLabel = "Kort Sayısı",
                    AdditionalInfo = $"{courts.Count} Kort",
                    PhotoUrl = facility.PhotoUrl,
                    Courts = courtInfos,
                    TotalCourts = courts.Count,
                    MainSurfaceType = mainSurfaceType,
                    Address = new AddressModel
                    {
                        TamAdres = facility.Address?.TamAdres ?? "Adres bulunamadı",
                        Il = facility.Address?.Il ?? "Kayseri",
                        Ilce = facility.Address?.Ilce ?? "Kayseri",
                        Mahalle = facility.Address?.Mahalle ?? "Kayseri",
                        Cadde = facility.Address?.Cadde ?? "Kayseri"
                    }
                };

                locations.Add(mapLocation);
            }

            var totalFacilities = facilities.Count;
            var totalCourts = facilities.Sum(f => (f.Courts?.Count) ?? 0);

            var availableSurfaceTypes = facilities
                .SelectMany(f => f.Courts ?? new List<Court>())
                .Select(c => c.SurfaceType)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .ToList();

            var viewModel = new MapIndexViewModel
            {
                Locations = locations,
                PageTitle = "🎾 Kayseri Tenis Kort Rehberi",
                PageDescription = "Kayseri'deki tüm tenis kortlarını ve tesislerini keşfedin",
                TotalFacilities = totalFacilities,
                TotalCourts = totalCourts,
                AvailableSurfaceTypes = availableSurfaceTypes
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
