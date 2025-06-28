namespace KayseriTenisKortRehberi.Models
{
    public class Court
    {
        public int Id { get; set; }

        public string CourtName { get; set; }
        public string SurfaceType { get; set; }

        public int FacilityId { get; set; }
        public Facility Facility { get; set; }
    }
}
