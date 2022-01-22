using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.SalesAnnouncement
{
    public class EditModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public EditModel(kamjaService.Models.KamjaServiceDbContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(SalesAnnouncements).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesAnnouncementsExists(SalesAnnouncements.SalesAnnouncementId))
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

        private bool SalesAnnouncementsExists(long id)
        {
            return _context.SalesAnnouncements.Any(e => e.SalesAnnouncementId == id);
        }
    }
}
