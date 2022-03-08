using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ProductImages
{
    public class CreateModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public CreateModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public TBL_ProductsPicturesService TBL_ProductsPicturesService { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TBL_ProductsPicturesService.Add(TBL_ProductsPicturesService);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
