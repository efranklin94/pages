using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.PartGroupings
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<PartGrouping> PartGrouping { get;set; }
        public string fileName { get; set; }
        public async Task OnGetAsync(long? id,string name,string code)
        {
           
            fileName = _context.VW_AllProductsPictures.Where(d => d.Number == code).Select(d => d.FileName).FirstOrDefault();

            ViewData["prin"] = name;
            ViewData["pri"] = id;
            ViewData["pcode"] = code;
            ViewData["code"] = "/image/Product/pic/" + fileName;
            PartGrouping = await _context.PartGrouping.ToListAsync();
            

        }
    }
}
