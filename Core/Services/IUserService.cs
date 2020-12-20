using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services {
    public interface IUserService {
        public abstract User GetAllowedUser(string userName);
        public abstract User CreateUser(string userName, string password, string role);
        }
}
