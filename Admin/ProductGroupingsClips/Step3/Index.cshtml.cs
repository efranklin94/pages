using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService;
using kamjaService.Models;
using System.IO;

namespace kamjaService.Pages.Admin.ProductGroupingsClips.Step3
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<ProductGrouping> ProductGrouping { get; set; }
        public IList<ProductGroupingClips> ProductGroupingClips { get; set; }
        public IEnumerable<string> fanniSrcList { get; set; }
        public IEnumerable<string> salesInfoSrcList { get; set; }
        public long gid { get; set; }

        public async Task OnGetAsync(long? id)
        {
            ProductGroupingClips = await _context.ProductGroupingClips.Where(p => p.productGroupingId == id).Select(p=>p).Distinct().ToListAsync();

            if (Directory.Exists("wwwroot\\video\\slider\\fanni\\" + id))
            {
                fanniSrcList = Directory.EnumerateFiles("wwwroot\\video\\slider\\fanni\\" + id, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            }
            if (Directory.Exists("wwwroot\\video\\slider\\SalesInfo\\" + id))
            {
                salesInfoSrcList = Directory.EnumerateFiles("wwwroot\\video\\slider\\SalesInfo\\" + id, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            }
            gid = (long)id;
        }

        public async Task<IActionResult> OnGetName(long gid, string name, string title, string FanniOrSalesInfo)
        {
            ProductGroupingClips productGroupingClips = new ProductGroupingClips();
            productGroupingClips.productGroupingId = gid;
            productGroupingClips.name = name;
            productGroupingClips.title = title;
            productGroupingClips.FanniOrSalesInfo = FanniOrSalesInfo;

            var queryResult = _context.ProductGroupingClips.Where(
                    c => c.productGroupingId == gid &&
                    c.name == name &&
                    c.FanniOrSalesInfo == FanniOrSalesInfo
                );

            if (queryResult.Any())
            {
                productGroupingClips.Id = queryResult.Select(c => c.Id).SingleOrDefault();
                _context.Attach(productGroupingClips).State = EntityState.Modified;
            }
            else 
            { 
                _context.ProductGroupingClips.Add(productGroupingClips);
            }
            
            try
            {
                await _context.SaveChangesAsync();
                return new JsonResult("success");
            }
            catch (Exception ex) {
                return new JsonResult(ex+": failed");
            }
        }
    }
}
