using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ColorCategory
{
    public class DetailsModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DetailsModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public ColorCAT ColorCAT { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ColorCAT = await _context.ColorCAT.FirstOrDefaultAsync(m => m.ID == id);

            if (ColorCAT == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
