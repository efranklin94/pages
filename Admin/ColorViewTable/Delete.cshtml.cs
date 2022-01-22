using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ColorViewTable
{
    public class DeleteModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DeleteModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ColorsCATRef colorsCATRef { get; set; }
        public string color { get; set; }
        public string color_category { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            colorsCATRef = await _context.ColorsCATRef.FirstOrDefaultAsync(m => m.color_Id == id);

            color = _context.Colors.Where(x => x.Id == colorsCATRef.colorsIdREF).Select(x => x.color_name).FirstOrDefault();
            color_category = _context.ColorCAT.Where(x => x.ID == colorsCATRef.colorCATIdREF).Select(x => x.color_category).FirstOrDefault();

            if (colorsCATRef == null)
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

            colorsCATRef = await _context.ColorsCATRef.FindAsync(id);

            if (colorsCATRef != null)
            {
                _context.ColorsCATRef.Remove(colorsCATRef);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
