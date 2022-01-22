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
namespace kamjaService.Pages.Shared
{
    public class _ModakViewModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public _ModakViewModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }
        public IList<ComponentDetails> compdt { get; set; }
        public async Task OnGetAsync(string id)

        {
            compdt = await (from m in _context.ComponentDetails
                            where m.AjzakalaCode == id
                                            select m).ToListAsync();

           
        }
    }
}
