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
    public class DeleteModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DeleteModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ColorCAT = await _context.ColorCAT.FindAsync(id);

            if (ColorCAT != null)
            {
                _context.ColorCAT.Remove(ColorCAT);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
