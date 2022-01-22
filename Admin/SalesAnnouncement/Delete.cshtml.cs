using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.SalesAnnouncement
{
    public class DeleteModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DeleteModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SalesAnnouncements SalesAnnouncements { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SalesAnnouncements = await _context.SalesAnnouncements.FirstOrDefaultAsync(m => m.SalesAnnouncementId == id);

            if (SalesAnnouncements == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SalesAnnouncements = await _context.SalesAnnouncements.FindAsync(id);

            if (SalesAnnouncements != null)
            {
                _context.SalesAnnouncements.Remove(SalesAnnouncements);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
