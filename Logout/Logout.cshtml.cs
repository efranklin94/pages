using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace kamjaService.Pages.Logout
{
    public class Logout : PageModel
    {
        private readonly IAuthenticationService _authService;

        public Logout(IAuthenticationService authService)
        {
            _authService = authService;
        }
        public void OnGet()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //return RedirectToPage("Login");
        }
    }
}
