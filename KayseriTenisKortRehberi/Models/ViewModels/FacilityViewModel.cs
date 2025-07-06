namespace KayseriTenisKortRehberi.Models.ViewModels
{
    public class FacilityViewModel
    {
        public int Id { get; set; }
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FacilityName { get; set; }
        public string FacilityDescription { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? PhotoUrl { get; set; }
        public IFormFile? PhotoFile { get; set; }
    }

}
