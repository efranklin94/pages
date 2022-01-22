using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; }

        public async Task OnGetAsync(long? id)
        {

            var Products = from m in _context.Product
                           select m;
            var pr = Products.Where(x => x.PartRef == id).Distinct();
            //pt = ProductG.Select(x => new { x.Expr1,x.ParentRef}).Where(x => x.ParentRef == id);
            Product = await pr.ToListAsync();
            //Product = await _context.Product.ToListAsync();
        }
    }
}
