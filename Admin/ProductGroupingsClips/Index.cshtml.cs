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
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        //public IList<ProductGroupingClips> ProductGroupingClips { get;set; }
        public IList<Parent> Parent { get; set; }

        public async Task OnGetAsync()
        {
            //ProductGroupingClips = await _context.ProductGroupingClips.ToListAsync();
            Parent = await _context.Parent.ToListAsync();

        }
    }
}
