using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.NaghsheCode.Step2
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<ProductGroup> ProductGroup { get; set; }

        public async Task OnGetAsync(long? id)
        {
            var ProductG = from m in _context.ProductGroup
                           select m;
            var pr = ProductG.Where(x => x.ParentRef == id).Distinct();
            ProductGroup = await pr.ToListAsync();

        }
    }
}
