namespace KayseriTenisKortRehberi.Models.ViewModels
{
    public class MapIndexViewModel
    {
        public List<MapLocationModel> Locations { get; set; } = new List<MapLocationModel>();
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public int TotalFacilities { get; set; }
        public int TotalCourts { get; set; }
        public List<string> AvailableSurfaceTypes { get; set; } = new List<string>();
    }

    public class MapLocationModel
    {
        public string Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ZoomLevel { get; set; }
        public string? Title { get; set; }
        public string Description { get; set; }
        public string IconEmoji { get; set; }
        public string AdditionalInfo { get; set; }
        public string AdditionalInfoLabel { get; set; }

        // Facility bilgileri
        public int FacilityId { get; set; }
        public string PhotoUrl { get; set; }

        // Court bilgileri
        public List<CourtInfo> Courts { get; set; } = new List<CourtInfo>();
        public int TotalCourts { get; set; }
        public string MainSurfaceType { get; set; }

        public AddressModel Address { get; set; }
    }

    public class CourtInfo
    {
        public int Id { get; set; }
        public string CourtName { get; set; }
        public string SurfaceType { get; set; }
    }
}