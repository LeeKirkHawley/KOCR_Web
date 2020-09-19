using KOCR_Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KOCR_Web.Services {
    public class AccountService {

        private readonly SQLiteDBContext _context;
        private readonly UserService _userService;

        public AccountService(SQLiteDBContext context, UserService userService) {
            _context = context;
            _userService = userService;
        }

        public ClaimsPrincipal Login(string userName, string password) {

            User user = _userService.GetAllowedUser(userName);
            if (user == null) {
                return null;
            }

            if(user.pwd != password) {
                return null;
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.userName));

            //foreach (var role in user.Roles) {
            //    identity.AddClaim(new Claim(ClaimTypes.Role, role.Role));
            //}
            identity.AddClaim(new Claim(ClaimTypes.Role, user.role));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            return principal;
        }

        public ClaimsPrincipal CreateUser(string userName, string password, string role) {

            User user = _userService.CreateUser(userName, password, role);
            if (user == null) {
                return null;
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.userName));
            //identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            //identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

            //foreach (var role in user.Roles) {
            //    identity.AddClaim(new Claim(ClaimTypes.Role, role.Role));
            //}
            identity.AddClaim(new Claim(ClaimTypes.Role, user.role));

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            return principal;
        }

    }
}
