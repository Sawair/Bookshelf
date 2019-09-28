using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookshelf.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Bookshelf.Service
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public IdentityUser GetCurrentUser()
        {
            var uId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _context.Users.First(u => u.Id == uId);
        }

        public IEnumerable<string> GetUsersNames()
        {
            return _context.Users.Select(u => u.UserName).ToList();
        }
    }
}
