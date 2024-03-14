using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Display(Name = "Eski Şifre: ")]
        [Required(ErrorMessage = "Şifre Alanı Boş Bırakılamaz.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakterden oluşmalıdır.")]
        public string PasswordOld { get; set; } = null!;
        [Display(Name = "Yeni Şifre: ")]
        [Required(ErrorMessage = "Şifre Alanı Boş Bırakılamaz.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakterden oluşmalıdır.")]
        public string Password { get; set; } = null!;

        [Display(Name = "Yeni Şifre Tekrar: ")]
        [Compare(nameof(Password), ErrorMessage = "Girilen şifreler uyuşmamaktadır.")]
        [Required(ErrorMessage = "Yeni Şifre Tekrar Alanı Boş Bırakılamaz.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakterden oluşmalıdır.")]
        public string ConfirmPassword { get; set; } = null!;

    }
}
