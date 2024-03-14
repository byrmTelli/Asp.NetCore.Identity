using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
            
        }

        public SignInViewModel(string email,string password)
        {
            Email = email;
            Password = password;
        }

        [EmailAddress(ErrorMessage ="Email formatı yanlıştır.")]
        [Required(ErrorMessage="Email alanı boş bırakılamaz.")]
        [Display(Name = "Email: ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password alanı boş bırakılamaz.")]
        [Display(Name = "Şifre: ")]
        [DataType(DataType.Password)]
        public string  Password { get; set; } = null!;

        [Display(Name = "Beni Hatırla ")]
        public bool RememberMe { get; set; }
    }
}
