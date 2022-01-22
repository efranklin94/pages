using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using System.Globalization;
using NinjaNye.SearchExtensions;

namespace kamjaService.Pages.services
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<Khadamat> Khadamat { get;set; }
        public string CurrentFilter { get; set; }
       
        public async Task OnGetAsync(string currentFilter, string searchString )
        {
          
           
            if (searchString != null)
            {
                //pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            var ProductG = from m in _context.Khadamat

                           select m;
          
           
               var  pr2 = ProductG.Select(x => new Khadamat { 
                                   Name= x.Name,Number=x.Number,Fee=x.Fee }).Distinct();
            IQueryable<Khadamat> prs = pr2;
            if (!String.IsNullOrEmpty(searchString))
            {
                String[] searchstrZ = searchString.Split(' ');
                pr2 = pr2.Search(s => s.Name, s => s.Number).ContainingAll(searchstrZ).AsQueryable();
                prs = pr2.Where(s => s.Name.Contains(s.Name));
                //pr2 = pr2.Where(s => s.Number.Contains(searchString)
                //              ||   s.Name.Contains(searchString));

            }
            Khadamat = await prs.ToListAsync();

            
        }
    }
}
