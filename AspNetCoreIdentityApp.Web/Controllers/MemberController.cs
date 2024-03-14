using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;


        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;

        }


        public IActionResult Index()
        {

            var userClaims = User.Claims.ToList();

            var mail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            var currentUser = _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserViewModel() {
                Email = currentUser.Result.Email,
                userName= currentUser.Result.UserName,
                phoneNumber= currentUser.Result.PhoneNumber,
                pictureUrl =currentUser.Result.Picture
            };

            return View(userViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var checkOldPassword =await  _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);

            if(!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış.");
                return View();
            }


            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.Password);

            if(!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }


            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, request.Password,true,false);
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir.";
            return View();
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IActionResult> UserEdit()
        {

            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                BirthDate=currentUser.BirthDate,
                City= currentUser.City,
                Gender=currentUser.Gender


            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            currentUser.UserName=request.UserName;
            currentUser.Email=request.Email;
            currentUser.BirthDate=request.BirthDate;
            currentUser.City=request.City;
            currentUser.Gender=request.Gender;
            currentUser.PhoneNumber = request.Phone;


            if(request.Picture!=null && request.Picture.Length>0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "userImages").PhysicalPath, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToUser = await _userManager.UpdateAsync(currentUser);

            if(!updateToUser.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUser.Errors);
                return View();
            }
            
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();


            if(request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthday", currentUser.BirthDate.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser,true);

            }


            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir.";


            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };


            return View(userEditViewModel);
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            string message = string.Empty;


            message = "Bu sayfayı görmeye yetkiniz yoktur. Yetki almak için yöneticiniz ile görüşebilirsiniz.";
            ViewBag.message = message;

            return View();
        }


        [HttpGet]
        public  IActionResult Claims()
        {
            var userClaims = User.Claims.Select(x => new ClaimViewModel()
            {
                Issue = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();


            return View(userClaims);
        }

        [Authorize(Policy ="AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult ExchangePage()
        {
            return View();
        }

        [Authorize(Policy = "ViolancePolicy")]
        public IActionResult ViolancePage()
        {
            return View();
        }
    }
}
