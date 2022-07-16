using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using NinjaNye.SearchExtensions;

namespace kamjaService.Pages.technical.search
{
    public class searchModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public searchModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }
        public string CurrentFilter { get; set; }
        public int resultCount { get; set; }
        public IList<ProductGrouping> productGroupings { get;set; }

        public async Task OnGetAsync(string CurrentFilter, string searchString)
        {
            if (searchString != null)
            {
                searchString = PersianToEnglish(searchString);
                //pageIndex = 1;
            }
            CurrentFilter = searchString;
            var ProductGrouping = from m in _context.ProductGrouping
                                  select m;
            var pr = ProductGrouping.Distinct();

            IQueryable<ProductGrouping> prs = pr;

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
            productGroupings = await prs.OrderBy(p => p.Number).ToListAsync();
            resultCount = productGroupings.Count();
            //ProductGrouping = await _context.ProductGrouping.ToListAsync();
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


