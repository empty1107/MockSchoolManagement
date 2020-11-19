using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;
using NLog.Web.LayoutRenderers;

namespace MockSchoolManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdminController> logger;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<AdminController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        #region 角色管理
        /// <summary>
        /// 打开创建角色页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //我们只需要指定一个不重复的角色名称来创建新角色
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                //将角色保存在AspNetCoreRoles表
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        /// <summary>
        /// 根据角色ID查询角色信息，用于修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            //查找角色
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={id}的信息不存在，请重试。";
                View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            var users = userManager.Users.ToList();

            //查所有用户
            foreach (var user in users)
            {
                //如果拥有此角色，添加到模型集合中
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        /// <summary>
        /// 修改角色信息，保存数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={model.Id}的信息不存在，请重试。";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                //更新角色
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                //更新失败，显示错误信息
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的角色信息";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                //如果触发的异常是DbUpdateException，则知道我们无法删除角色
                //因为该角色中已存在用户信息
                catch (DbUpdateException ex)
                {
                    //将异常记录到日志文件中
                    logger.LogError($"发送异常：{ex}");
                    //我们使用ViewBag.ErrorTitle 和 ViewBag.ErrorMessage来传递错误标题和详细信息
                    ViewBag.ErrorTitle = $"角色：{role.Name} 正在被使用中...";
                    ViewBag.ErrorMessage = $" 无法删除{role.Name}角色，因为此角色中已经存在用户。如果读者想删除此角色，需要先从该角色中删除用户，然后尝试删除该角色本身。";
                    return View("Error");
                }
            }
        }
        #endregion

        #region 角色中的用户
        /// <summary>
        /// 打开修改用户设置角色页面
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            //查询角色
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={roleId}的信息不存在，请重试。";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                //判断当前用户是否已经存在于改角色中
                var isInRole = await userManager.IsInRoleAsync(user, role.Name);
                userRoleViewModel.IsSelected = isInRole;

                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        /// <summary>
        /// 保存用户角色设置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            //查询角色
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id={roleId}的信息不存在，请重试。";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                //选中，并且没有在角色里
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))//没有选中。但是用户已有改角色，则删除
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    //判断当前用户是否为最后一个用户，如果是，则跳转回EditRole视图
                    //不是，则进入下一次循环
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { id = roleId });
                    }
                }
            }
            return RedirectToAction("EditRole", new { id = roleId });
        }
        #endregion

        #region 用户管理
        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }

        /// <summary>
        /// 打开编辑用户界面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的用户";
                return View("NotFound");
            }
            // GetClaimsAsync()返回用户声明列表
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync() 返回用户角色列表
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims,
                Roles = userRoles
            };
            return View(model);
        }

        /// <summary>
        /// 修改用户，保存数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{model.Id}的用户";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的用户";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListUsers");
            }
        }

        /// <summary>
        /// 打开用户角色分配界面
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{userId}的用户";
                return View("NotFound");
            }
            var model = new List<RolesInUserViewModel>();
            foreach (var role in roleManager.Roles)
            {
                var rolesInUserViewModel = new RolesInUserViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                //判断当前用户是否已经拥有该角色
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    //设置选中
                    rolesInUserViewModel.IsSelected = true;
                }
                else
                {
                    rolesInUserViewModel.IsSelected = false;
                }
                model.Add(rolesInUserViewModel);
            }
            return View(model);
        }

        /// <summary>
        /// 保存用户角色分配
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<RolesInUserViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{userId}的用户";
                return View("NotFound");
            }
            //获取当前用户已有角色
            var roles = await userManager.GetRolesAsync(user);
            //移除当前用户的所有角色信息
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法删除用户中的现有角色");
                return View(model);
            }
            //查询模型列表中被选中的RoleName并添加到用户中
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(x => x.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法向用户添加选定的角色");
                return View(model);
            }
            return RedirectToAction("EditUser", new { id = userId });
        }

        /// <summary>
        /// 打开用户声明设置界面
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{userId}的用户";
                return View("NotFound");
            }
            //获取当前用户所有声明
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = userId
            };
            //遍历每个声明
            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                //如果选中声明，设置勾选
                if (existingUserClaims.Any(x => x.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Cliams.Add(userClaim);
            }
            return View(model);
        }

        /// <summary>
        /// 保存用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{model.UserId}的用户";
                return View("NotFound");
            }
            //获取所有的用户声明，并删除它
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法删除当前用户的声明");
                return View(model);
            }
            //添加Claim列表到数据库中，然后对UI界面上被选中的值进行bool判断
            result = await userManager.AddClaimsAsync(user, model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法向用户添加选定的声明");
                return View(model);
            }
            return RedirectToAction("EditUser", new { id = model.UserId });
        }
        #endregion

        /// <summary>
        /// 用户拒绝访问时的处理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
