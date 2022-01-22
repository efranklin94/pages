using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using System.Globalization;

namespace kamjaService.Pages.yaraghs
{
    public class componentdetailModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public componentdetailModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }
        public IList<ComponentDetails> compdt { get; set; }
        public async Task OnGetAsync()

        {
            compdt = await _context.ComponentDetails.ToListAsync();
        }
    }
}
