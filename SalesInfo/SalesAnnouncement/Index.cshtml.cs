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
            var pg_tags = from m in _context.ProductGroupTag
                         select m;
            var pging_tags = from m in _context.ProductGroupingTag
                         select m;
            var pr2 = sales.Distinct();

            IQueryable<SalesAnnouncements> prs = pr2;
            if (!String.IsNullOrEmpty(searchString))
            {
                String[] searchstrZ = searchString.Split(' ');

                pr2 = pr2.Search(s => s.title).ContainingAll(searchstrZ).AsQueryable();
                pr2 = pr2.Where(s => s.title.Contains(s.title));
                
                var SalesAnnounceIds_parent = _context.ParentTag.Search(s=>s.tag_name).ContainingAll(searchstrZ).Select(s=>s).AsQueryable();
                var SalesAnnounceIds_productGroup = _context.ProductGroupTag.Search(s => s.tag_name).ContainingAll(searchstrZ).Select(s => s).AsQueryable(); 
                var SalesAnnounceIds_productGrouping = _context.ProductGroupingTag.Search(s => s.tag_name).ContainingAll(searchstrZ).Select(s => s).AsQueryable(); 

                var parent_query = from s in _context.SalesAnnouncements
                            join t in SalesAnnounceIds_parent
                            on s.SalesAnnouncementId equals t.SalesAnnouncementREF
                            select s;
                var productgroup_query = from s in _context.SalesAnnouncements
                                   join t in SalesAnnounceIds_productGroup
                                   on s.SalesAnnouncementId equals t.SalesAnnouncementREF
                                   select s;
                var productgrouping_query = from s in _context.SalesAnnouncements
                                   join t in SalesAnnounceIds_productGrouping
                                   on s.SalesAnnouncementId equals t.SalesAnnouncementREF
                                   select s;
                //for salesannouncement_id
                //IQueryable<SalesAnnouncements> salesNumberId_query;
                //for (int i = 0 ; i < searchstrZ.Length ; i++) {
                //    var id = 0;
                //    Int32.TryParse(searchstrZ[i], out id);
                //    salesNumberId_query = salesNumberId_query.Concat(from s in _context.SalesAnnouncements
                //                                   where s.SalesAnnouncementId == id
                //                                   select s);
                //}

                pr2 = pr2.Concat(parent_query);
                pr2 = pr2.Concat(productgroup_query);
                pr2 = pr2.Concat(productgrouping_query);



            }

            SalesAnnouncements = await pr2.OrderByDescending(x=>x.SalesAnnouncementId).Distinct().ToListAsync();
        }


    }
}





//prs = pr2.Where(s => s.AjzaKalaName.Contains(s.AjzaKalaName));
//////////////////////
// var records = p_tags.Where(x => x.tag_name.Contains("," + searchstrZ[0] + ",") ||
//  x.tag_name.StartsWith(searchstrZ[0] + ",") ||
//  x.tag_name.EndsWith("," + searchstrZ[0]) ||
//  x.tag_name.Contains(searchstrZ[0])).ToList();

//foreach (var s in records)
//{
//    Console.WriteLine(s.tag_name);
//}
//////////////////////
///


//public async Task<FileResult> OnPostDownloadPdf(int id)
//{
//    var file = await _context.SalesAnnouncements.Where(x => x.Id == id).FirstOrDefault();
//    if (file == null)
//    {
//        //TODO
//    }
//    return File(file.ReportContent, "application/pdf", "test.pdf");
//}