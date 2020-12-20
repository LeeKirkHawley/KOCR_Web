using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Core.Services {
    public interface IAccountService {

        public abstract ClaimsPrincipal Login(string userName, string password);
        public abstract ClaimsPrincipal CreateUser(string userName, string password, string role);
    }
}
