using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Models;
using Core.Services;

namespace KOCR_Web.Services {

    public class UserService : IUserService{
        private readonly SQLiteDBContext _context;

        public UserService(SQLiteDBContext context) {
            _context = context;
        }

        public User GetAllowedUser(string userName) {
            User user = _context.Users.Where(u => u.userName == userName).FirstOrDefault();
            return user;
        }

        public User CreateUser(string userName, string password, string role) {

            var newuser = _context.Users.Add(new User {
                userName = userName,
                pwd = password,
                role = role
            });
            _context.SaveChanges();

            return newuser.Entity;
        }
    }
}
