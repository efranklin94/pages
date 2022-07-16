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

namespace kamjaService.Pages.yaraghs
{
    public class IndexModel2 : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel2(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<YaraghVM> Yaragh { get;set; }
        public string CurrentFilter { get; set; }
        public IList<String> ZeroFeeYaraghs { get; set; }
        public async Task OnGetAsync(string currentFilter, string searchString,string pn )
        {
            ViewData["prn"] = pn;

            if (searchString != null)
            {
                searchString = PersianToEnglish(searchString);
                //pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
              
            }

            CurrentFilter = searchString;
          
            var ProductG = from m in _context.Yaragh
                           
                           select m;
          
           
               var  pr2 = ProductG.Select(x => new YaraghVM { 
                                   AjzakalaCode= x.AjzakalaCode,AjzaKalaName=x.AjzaKalaName,Fee=x.Fee,واحد = x.واحد}).Distinct();
                                   //.Where(x=>x.Fee>0);

            IQueryable<YaraghVM> prs=pr2;
                if (!String.IsNullOrEmpty(searchString))
            {
                //String[] searchstrZ = new string[] { "", "", "" };
                //searchstrZ[0] = "";

                String[]  searchstrZ = searchString.Split(' ');
             



                pr2 = pr2.Search(s => s.AjzaKalaName, s => s.AjzakalaCode).ContainingAll(searchstrZ).AsQueryable();
                prs = pr2.Where(s => s.AjzaKalaName.Contains(s.AjzaKalaName));

                //&& s.AjzaKalaName.Contains(searchstrZ[1])
                //&& s.AjzaKalaName.Contains(searchstrZ[2])
                //|| s.AjzakalaCode.Contains(searchString));
            }
            Yaragh = await prs.ToListAsync();

            ZeroFeeYaraghs = await ProductG.Where(yaragh => yaragh.Fee == 0).Select(yaragh => yaragh.AjzakalaCode).ToListAsync();
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
