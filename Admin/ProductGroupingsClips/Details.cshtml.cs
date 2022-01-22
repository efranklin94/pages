using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ProductGroupingsClips
{
    public class DetailsModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DetailsModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public ProductGroupingClips ProductGroupingClips { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductGroupingClips = await _context.ProductGroupingClips.FirstOrDefaultAsync(m => m.Id == id);

            if (ProductGroupingClips == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
