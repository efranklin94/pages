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
    public class DeleteModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DeleteModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Colors = await _context.Colors.FindAsync(id);

            if (Colors != null)
            {
                _context.Colors.Remove(Colors);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
