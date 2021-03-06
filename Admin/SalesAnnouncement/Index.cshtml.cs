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
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<SalesAnnouncements> SalesAnnouncements { get;set; }
        public IList<ParentTag> ParentTag { get; set; }
        public IList<ProductGroupTag> ProductGroupTag { get; set; }
        public IList<ProductGroupingTag> ProductGroupingTag { get; set; }


        public async Task OnGetAsync()
        {
            SalesAnnouncements = await _context.SalesAnnouncements.OrderByDescending(x => x.SalesAnnouncementId).ToListAsync();
            ParentTag = await _context.ParentTag.ToListAsync();
            ProductGroupTag = await _context.ProductGroupTag.ToListAsync(); 
            ProductGroupingTag = await _context.ProductGroupingTag.ToListAsync();
        }
    }
}
