using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using Microsoft.AspNetCore.Authorization;
using NinjaNye.SearchExtensions;

namespace kamjaService.Pages.technical
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<Parent> Parent { get;set; }
        public string CurrentFilter { get; set; }
        public ProductGroup productGroup { get; set; }
        public IList<ProColView> proColViews { get; set; }

        public async Task OnGetAsync(string currentFilter, string searchString)
        {
            Parent = await _context.Parent.ToListAsync();

            if (searchString != null)
            {
                searchString = PersianToEnglish(searchString);
                //pageIndex = 1;
            }
            CurrentFilter = searchString;

            var Procolview_query = from m in _context.ProColView
                                   select m;
            var pr = Procolview_query.Distinct();

            IQueryable<ProColView> prs = pr;

            if (!String.IsNullOrEmpty(searchString))
            {

                String[] searchstrZ = searchString.Split(' ');

                //remove the spaces to prevent exception
                List<string> searchstrZWithoutNull = new List<string>();
                foreach (String s in searchstrZ)
                {
                    if (s == "")
                    {
                        continue;
                    }
                    else
                    {
                        searchstrZWithoutNull.Add(s);
                    }
                }
                searchstrZ = searchstrZWithoutNull.ToArray();

                var prName = pr.Search(s => s.Name).ContainingAll(searchstrZ).AsQueryable();
                var prsName = prName.Where(s => s.Name.Contains(s.Name));

                var prNumber = pr.Search(s => s.Number).ContainingAll(searchstrZ).AsQueryable();
                var prsNumber = prNumber.Where(s => s.Number.Contains(s.Number));

                prs = prsName.Concat(prsNumber);
            }
            proColViews = await prs.OrderBy(p => p.Number).ToListAsync();
        }

        public IActionResult OnGetSearch(string customerName)
        {
            var products = from p in _context.ProductGrouping
                           where p.Name.Contains(customerName)
                           select p.Name;
            return new JsonResult(products.ToList());
        }

        public string PersianToEnglish(string input)
        {
            string EnglishNumbers = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    EnglishNumbers += char.GetNumericValue(input, i);
                }
                else
                {
                    EnglishNumbers += input[i].ToString();
                }
            }
            return EnglishNumbers;
        }
    }
}
