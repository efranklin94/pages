using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using NinjaNye.SearchExtensions;

namespace kamjaService.Pages.SalesInfo.SalesAnnouncement
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;
        public string CurrentFilter { get; set; }

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<SalesAnnouncements> SalesAnnouncements { get;set; }
        public IList<ParentTag> parentTags { get; set; }
        public IList<ProductGroupingTag> productGroupingTags { get; set; }
        public IList<ProductGroupTag> productGroupTags { get; set; }

        public async Task OnGetAsync(string searchString, string currentFilter)
        {

            if (searchString == null)
            {
                searchString = currentFilter;

            }

            CurrentFilter = searchString;

            var sales = from m in _context.SalesAnnouncements
                        select m;
            var p_tags = from m in _context.ParentTag
                         select m;
            var pr2 = sales.Distinct();

            IQueryable<SalesAnnouncements> prs = pr2;
            if (!String.IsNullOrEmpty(searchString))
            {
                String[] searchstrZ = searchString.Split(' ');

                pr2 = pr2.Search(s => s.title).ContainingAll(searchstrZ).AsQueryable();
                //prs = pr2.Where(s => s.AjzaKalaName.Contains(s.AjzaKalaName));
                //////////////////////
                 var records = p_tags.Where(x => x.tag_name.Contains("," + searchstrZ[0] + ",") ||
                  x.tag_name.StartsWith(searchstrZ[0] + ",") ||
                  x.tag_name.EndsWith("," + searchstrZ[0]) ||
                  x.tag_name.Contains(searchstrZ[0])).ToList();

                foreach (var s in records)
                {
                    Console.WriteLine(s.tag_name);
                }
                //////////////////////
                var SalesAnnounceIds = _context.ParentTag.Search(s=>s.tag_name).ContainingAll(searchstrZ).Select(s=>s.SalesAnnouncementREF).ToListAsync();
                
               foreach (var s in await SalesAnnounceIds)
                {
                    Console.WriteLine(s);
                }
            
            }

            SalesAnnouncements = await pr2.OrderByDescending(x=>x.SalesAnnouncementId).ToListAsync();
        }

        //public async Task<FileResult> OnPostDownloadPdf(int id)
        //{
        //    var file = await _context.SalesAnnouncements.Where(x => x.Id == id).FirstOrDefault();
        //    if (file == null)
        //    {
        //        //TODO
        //    }
        //    return File(file.ReportContent, "application/pdf", "test.pdf");
        //}
    }
}
