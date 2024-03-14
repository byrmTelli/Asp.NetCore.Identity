using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Localization
{
    public class LocalizationIdentityErrorDescriber: IdentityErrorDescriber
    {

        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DuplicateUserName", Description = $"{userName} başka bir kullanıcı adı zaten mevcut." };
        }


        public override IdentityError DuplicateEmail(string email)
        {
            return new() { Code = "DuplicateEmail", Description = $"{email} adresi başka bir kullanıcı tarafından kullanılıyor." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordToShort", Description = "Parola en az 6 karakterden oluşmalıdır." };
        }

    }
}
