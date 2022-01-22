using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using Newtonsoft.Json;
using System.IO;

namespace kamjaService.Pages.yaraghhavale
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<YaraghHavale> YaraghHavale { get;set; }
        public string havaleCode { get; set; }

        public async Task OnGetAsync(string searchString)
        {
            havaleCode = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                IQueryable<YaraghHavale> custQuery =
                    from havale in _context.YaraghHavale
                    where havale.OrderNumber == searchString
                    select havale;

                var havales = custQuery.Select(x => new YaraghHavale
                {
                    OrderNumber = x.OrderNumber,
                    ID = x.ID,
                    نام_محصول = x.نام_محصول,
                    Number = x.Number,
                    فرمول_اجزاي_کالا = x.فرمول_اجزاي_کالا,
                    کد_کالا = x.کد_کالا,
                    نام_کالا = x.نام_کالا,
                    کد_اجزاي_کالا = x.کد_اجزاي_کالا,
                    نام_اجزاي_کالا = x.نام_اجزاي_کالا,
                    PartID = x.PartID,
                    ProductID1 = x.ProductID1,
                    partid1 = x.partid1,
                    تعداد = x.تعداد,
                    توضیحات_قلم_حواله = x.توضیحات_قلم_حواله,
                    توضیحات_هدر_حواله = x.توضیحات_هدر_حواله,
                    OrderRef = x.OrderRef
                }).Distinct().OrderBy(x => x.OrderNumber);

                YaraghHavale = await havales.ToListAsync();
            }
            else
            {
                //Do Nothing and return nothing to the table  
            }
        }


        public IActionResult OnPostCheckbox(string checkboxValue, int id)
        {
            //Console.WriteLine("value: " +checkboxValue + " id: " + id);
            return new JsonResult(checkboxValue);
        }

        public IActionResult OnGetTest()
        {
            return new JsonResult("Hello");
        }

    }
    
}
