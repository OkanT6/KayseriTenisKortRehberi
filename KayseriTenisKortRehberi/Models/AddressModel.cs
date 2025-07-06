namespace KayseriTenisKortRehberi.Models
{
    public class AddressModel
    {
        public int Id { get; set; }
        public string TamAdres { get; set; } = string.Empty;
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string Mahalle { get; set; } = string.Empty;
        public string Cadde { get; set; } = string.Empty;
    }
}
