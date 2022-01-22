using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ColorsList
{
    public class DetailsModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DetailsModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public Colors Colors { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Colors = await _context.Colors.FirstOrDefaultAsync(m => m.Id == id);

            if (Colors == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
