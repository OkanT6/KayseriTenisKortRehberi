using System.ComponentModel.DataAnnotations;

namespace KayseriTenisKortRehberi.Models.ViewModels
{
    public class CourtViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "API Key gereklidir.")]
        [Display(Name = "API Key")]
        public string ApiKey { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Kort adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Kort adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Kort Adı")]
        public string CourtName { get; set; }

        [Required(ErrorMessage = "Zemin tipi gereklidir.")]
        [Display(Name = "Zemin Tipi")]
        public string SurfaceType { get; set; }

        [Required(ErrorMessage = "Tesis seçimi gereklidir.")]
        [Display(Name = "Tesis")]
        public int FacilityId { get; set; }
    }
}

