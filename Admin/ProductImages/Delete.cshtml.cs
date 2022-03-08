using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ProductImages
{
    public class DeleteModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DeleteModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TBL_ProductsPicturesService TBL_ProductsPicturesService { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TBL_ProductsPicturesService = await _context.TBL_ProductsPicturesService.FirstOrDefaultAsync(m => m.Code == id);

            if (TBL_ProductsPicturesService == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TBL_ProductsPicturesService = await _context.TBL_ProductsPicturesService.FindAsync(id);

            if (TBL_ProductsPicturesService != null)
            {
                _context.TBL_ProductsPicturesService.Remove(TBL_ProductsPicturesService);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
