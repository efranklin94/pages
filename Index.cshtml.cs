using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace kamjaService.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, kamjaService.Models.KamjaServiceDbContext context)
        {
            _logger = logger;
            _context = context;

        }

        public void OnGet()
        {

        }

    }
}
