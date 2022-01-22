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
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        //public IList<YaraghComponent> Yaragh { get;set; }
        public YaraghCVM Yaragh { get; set; }

        public string CurrentFilter { get; set; }
        public long? ID { get; set; }
        public string Yt { get; set; }
        public async Task OnGetAsync(long? id, string yt, string currentFilter, string searchString, string pn,string pcode)
        {
            Yaragh = new YaraghCVM();
            ViewData["prn"] = pn;
            if (id != null && yt != null)
            {
                ID = id;
                Yt = yt;
            }
            if (searchString != null)
            {
                //pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            //Yaragh.YaraghComponent = await (from m in _context.YaraghComponent
            //                        where m.ProductId == ID && m.گروه.Contains(Yt)
            //                        select m).ToListAsync();

          var pr2 = from m in _context.YaraghComponent
                                            where m.ProductId == ID && m.گروه.Contains(Yt)
                                            select m;


            Yaragh.Components = await (from m in _context.Component
                                       where m.ProductId == ID
                                       select m).ToListAsync();
            IQueryable<YaraghComponent> prs = pr2;
           
            if (!String.IsNullOrEmpty(searchString))
            {
                String[] searchstrZ = searchString.Split(' ');



               pr2= pr2.Search(s => s.AjzaKalaName, s => s.AjzakalaCode).ContainingAll(searchstrZ).AsQueryable();
                prs = pr2.Where(s => s.AjzaKalaName.Contains(s.AjzaKalaName));

                //Yaragh.YaraghComponent= Yaragh.YaraghComponent.Where(s => s.AjzaKalaName.Contains(searchString)
                //              || s.AjzakalaCode.Contains(searchString)).ToList();
            }
            ViewData["pcode"] = "/image/Product/solid/" + pcode + ".png";
            Yaragh.YaraghComponent = await prs.ToListAsync();



        }
    }
}
