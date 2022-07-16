using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using Microsoft.AspNetCore.Authorization;
using kamjaService.authorize;

namespace kamjaService.Pages.Admin
{
    //[Authorize(Policy = "bahrami")]
    //[Authorize(Policy = "hale")]
    //[Authorize(Policy = "abbasi")]
    //[Authorize(Policy = "edris")]
    //[Authorize(Policy = "farhangi")]
    [MultiplePolicysAuthorize("edris;hale;bahrami;abbasi;farhangi;shahabian;")]
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

       

        public async Task OnGetAsync()
        {
         
        }
    }
}
