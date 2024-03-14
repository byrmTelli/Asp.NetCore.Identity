using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class UserEditViewModel
    {
        [Display(Name = "Kullanıcı Adı: ")]
        [Required(ErrorMessage = "Kullanıcı Adı alanı boş bırakılamaz.")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Email: ")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Telefon: ")]
        [Required(ErrorMessage = "Telefon kısmı boş bırakılamaz.")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Doğum Tarihi: ")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Şehir: ")]
        public string? City { get; set; }

        [Display(Name = "Profil Resmi: ")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Cinsiyet: ")]
        public Gender? Gender { get; set; }
    }
}
