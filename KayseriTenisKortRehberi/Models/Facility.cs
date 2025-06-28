namespace KayseriTenisKortRehberi.Models
{
    public class Facility
    {
        public Facility()
        {
            Courts = new HashSet<Court>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PhotoUrl  { get; set; }
        public ICollection<Court> Courts { get; set; } 
    }
}
