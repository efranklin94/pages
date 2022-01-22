using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ColorCategory
{
    public class EditModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public EditModel(kamjaService.Models.KamjaServiceDbContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ColorCAT).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColorCATExists(ColorCAT.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ColorCATExists(int id)
        {
            return _context.ColorCAT.Any(e => e.ID == id);
        }
    }
}
