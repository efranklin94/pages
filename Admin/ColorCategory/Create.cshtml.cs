using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.ColorCategory
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
        public ColorCAT ColorCAT { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ColorCAT.Add(ColorCAT);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
