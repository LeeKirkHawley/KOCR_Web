using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Services;
using KOCR_Web.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Core;

namespace KOCR_Web.Controllers {
    public class AccountController : Controller {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;

        public AccountController(IUserService userService, IAccountService accountService) {
            _userService = userService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password) {


            ClaimsPrincipal principal = _accountService.Login(userName, password);
            if(principal == null) {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "CWDocs");
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied() {
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string userName, string password) {
            ClaimsPrincipal principal = _accountService.CreateUser(userName, password, "user");
            if (principal == null) {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("CWDocs", "Home");

        }
    }
}
