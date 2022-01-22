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
    public class DetailsModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public DetailsModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

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
    }
}
