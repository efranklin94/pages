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
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<TBL_ProductsPicturesService> TBL_ProductsPicturesService { get;set; }

        public async Task OnGetAsync()
        {
            TBL_ProductsPicturesService = await _context.TBL_ProductsPicturesService.ToListAsync();
        }
    }
}
