using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;


namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;


        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            return View(roles);
        }

        [Authorize(Roles ="admin,role-action")]
        public  IActionResult RoleCreate()
        {
            return View();
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            TempData["SuccessMessage"] = "Role başarıyla oluşturulmuştur.";
            return RedirectToAction(nameof(RolesController.Index));
        }


        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var _roletoUpdate = await _roleManager.FindByIdAsync(id);

            if (_roletoUpdate == null)
            {
                throw new Exception("Böyle bir rol bulunamamıştır.");
            }

            return View(new RoleUpdateViewModel()
            {
                Id=_roletoUpdate.Id,
                Name= _roletoUpdate!.Name!,
            });
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamadı.");
            }
            roleToUpdate.Name = request.Name;
            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Role Bilgisi Güncellenmiştir.";

            return View();
        }


        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete == null)
            {
                throw new Exception("ilgili rol bulunamadı.");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if(!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x=> x.Description).First());
            }
            TempData["SuccessMessage"] = "Role başarıyla silinmiştir.";
            return RedirectToAction(nameof(RolesController.Index));

        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            ViewBag.userId = id;
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var roleViewModelList = new List<AssignRoleToUserViewModel>();



            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                };

                if (userRoles.Contains(role.Name))
                {
                    assignRoleToUserViewModel.RoleExist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var assignedRoleUser = await _userManager.FindByIdAsync(userId);

            foreach(var role in requestList)
            {
                if(role.RoleExist)
                {
                    await _userManager.AddToRoleAsync(assignedRoleUser,role.Name);

                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(assignedRoleUser, role.Name);
                }
            }

            return RedirectToAction(nameof(HomeController.UserList),"Home");
        }
    }
}
