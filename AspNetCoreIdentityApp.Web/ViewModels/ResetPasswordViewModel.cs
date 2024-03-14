using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Yeni Şifre: ")]
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }



        [Display(Name = "Yeni Şifre Onay: ")]
        [Compare(nameof(Password), ErrorMessage = "Girilen şifreler uyuşmamaktadır.")]
        [Required(ErrorMessage = "Şifre Onay alanı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
