using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {

        public SignUpViewModel()
        {
            
        }

        public SignUpViewModel(string userName,string email,string phone, string password)
        {
            UserName = userName;
            Email = email;
            Phone = phone;
            Password = password;
        }
        [Display(Name ="Kullanıcı Adı: ")]
        [Required(ErrorMessage ="Kullanıcı Adı alanı boş bırakılamaz.")]
        public string UserName { get; set; } = null!;

        [Display(Name ="Email: ")]
        [Required(ErrorMessage ="Email alanı boş bırakılamaz.")]
        public string Email { get; set; } = null!;

        [Display(Name ="Telefon: ")]
        [Required(ErrorMessage ="Telefon kısmı boş bırakılamaz.")]
        public string Phone { get; set; } = null!;

        [Display(Name ="Şifre: ")]
        [Required(ErrorMessage ="Şifre alanı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakterden oluşmalıdır.")]
        public string Password { get; set; } = null!;

        [Display(Name = "Şifre Onay: ")]
        [Compare(nameof(Password),ErrorMessage ="Girilen şifreler uyuşmamaktadır.")]
        [Required(ErrorMessage ="Şifre Onay alanı boş bırakılamaz.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakterden oluşmalıdır.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
