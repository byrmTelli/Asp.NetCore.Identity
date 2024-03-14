using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Services;
using System.Security.Claims;


namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //userla ilgili iþlemlerde kullanýlýr.
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly IEmailService _emailService;


        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,
                PhoneNumber = request.Phone,
                Email = request.Email
            },
                request.ConfirmPassword);


            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            }



            var exchangeExpireClaims = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
            var user = await _userManager.FindByNameAsync(request.UserName);

            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaims);


            if (!claimResult.Succeeded) 
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(_ => _.Description).ToList());
            }

            TempData["SuccessMessage"] = "Üyelik Kayýt Ýþlemi Baþarýyla Gerçekleþmiþtir.";
            return RedirectToAction(nameof(HomeController.SignUp));

        }


        public IActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email ya da þifre yanlýþ");
                return View();

            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriþ yapamazsýnýz." });
                return View();
            }


            if(!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { $"Email ya da þifre yanlýþ. Baþarýsýz giriþ sayýsý {await _userManager.GetAccessFailedCountAsync(hasUser)}" });
                return View();
            }

            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, new[] { new Claim("birthday", hasUser.BirthDate.Value.ToString()) });
            }
            return Redirect(returnUrl!);
        }



        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel user)
        {
            var hasUser = await _userManager.FindByEmailAsync(user.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Bu email adresine sahip kullanýcý bulunamamýþtýr.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken },
                HttpContext.Request.Scheme);


            //Email Service ihtiyaç var.

            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

            TempData["success"] = "Þifre sýfýrlama linki, e posta adresinize gönderilmiþtir.";
            return RedirectToAction(nameof(ForgetPassword));
        }




        public IActionResult ResetPassword(string userID, string token)
        {
            TempData["UserId"] = userID;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userID = TempData["UserId"];
            var token = TempData["token"];

            if (userID == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi.");
            }



            var hasUser = await _userManager.FindByIdAsync(userID.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Kullanýcý bulunamadý.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);


            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Þifreniz baþarýyla yenilenmiþtir.";

            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(_ => _.Description).ToList());
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
