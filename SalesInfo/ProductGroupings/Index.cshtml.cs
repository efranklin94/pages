using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using NinjaNye.SearchExtensions;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace kamjaService.Pages.SalesInfo.ProductGroupings
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<ProductGrouping> ProductGrouping { get; set; }
        public string CurrentFilter { get; set; }
        public long? gID { get; set; }
        public ProductGroup productGroup { get; set; }

        public string productGroupDescription { get; set; }
        public IList<ProColView> proColViews { get; set; }
        public IList<ProductGroupingexp> productGroupingexps { get; set; }
        public IEnumerable<string> imageSrcList { get; set; }
        public IEnumerable<string> videoSrcList { get; set; }
        public IList<ProductGroupingClips> productGroupingClips { get; set; }
        public bool isExistingProductGroup { get; set; }
        public List<string> ColorsViews { get; set; }
        public async Task OnGetAsync(long? id, string currentFilter, string searchString)
        {
            if (id != null)
            {
                gID = id;
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

            var Procolview_query = from m in _context.ProColView
                                   select m;
            var pr = Procolview_query.Where(x => x.GroupRef == id).Distinct();

            IQueryable<ProColView> prs = pr;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (Regex.IsMatch(searchString, @"^\d+$")) {
                    String[] searchstrZ = searchString.Split(' ');
                    pr = pr.Search(s => s.Number).ContainingAll(searchstrZ).AsQueryable();
                    prs = pr.Where(s => s.Number.Contains(s.Number));
                }
                else {
                    String[] searchstrZ = searchString.Split(' ');
                    pr = pr.Search(s => s.Name).ContainingAll(searchstrZ).AsQueryable();
                    prs = pr.Where(s => s.Name.Contains(s.Name));
                }
            }

            var prgd = from pg in _context.ProductGroup
                       where pg.ProductGroupId == id
                       select pg;

            productGroup = await prgd.SingleOrDefaultAsync();

            var pg_desc_query = from pd in _context.ProductGroupEXP
                                where pd.ProductGroupIdref == id
                                select pd.pgexp;
            proColViews = await prs.ToListAsync();


            productGroupingexps = await _context.ProductGroupingexp.ToListAsync();
            productGroupDescription = await pg_desc_query.FirstOrDefaultAsync();

            if (Directory.Exists("wwwroot\\image\\slider\\SalesInfo\\" + gID)) {
                imageSrcList = Directory.EnumerateFiles("wwwroot\\image\\slider\\SalesInfo\\" + gID, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg"));
            }
            else
            {
                imageSrcList = Directory.EnumerateFiles("wwwroot\\image", "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            }

            //if (Directory.Exists("wwwroot\\video\\slider\\SalesInfo\\" + gID))
            //{
            //    videoSrcList = Directory.EnumerateFiles("wwwroot\\video\\slider\\SalesInfo\\" + gID, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            //}
            //else
            //{
            //    videoSrcList = Directory.EnumerateFiles("wwwroot\\image", "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            //}

            //productGroupingClips = await _context.ProductGroupingClips.Where(x => x.FanniOrSalesInfo == "SalesInfo").Where(x => x.productGroupingId == gID).ToListAsync();
            //var videoSrcList = "\\video\\SalesInfo\\" + productGroupingClips.Select(x => x.name) + ".mp4";
            //var clipNames = productGroupingClips.Select(x => x.name);

            //for (int i=0; i< productGroupingClips.Select(x => x.name).Count(); i++)
            //{
            //    //@if (System.IO.File.Exists("wwwroot\\image\\colors\\" + color.color + ".jpg"))
            //    if (System.IO.File.Exists("wwwroot\\video\\SalesInfo\\" + clipName + ".mp4"))
            //    {
            //        productGroupingClips. = Directory.EnumerateFiles("wwwroot\\video\\SalesInfo\\" + gID, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            //    }
            //    else
            //    {
            //        videoSrcList = Directory.EnumerateFiles("wwwroot\\image", "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            //    }
            //}

            if (Directory.Exists("wwwroot\\video\\slider\\SalesInfo\\" + gID))
            {
                videoSrcList = Directory.EnumerateFiles("wwwroot\\video\\slider\\SalesInfo\\" + gID, ".", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            }
            else
            {
                videoSrcList = Directory.EnumerateFiles("wwwroot\\image", ".", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            }
        }

        public async Task<IActionResult> OnGetColors(string colorCatName)
        {
            ColorsViews = await _context.ColorsView.Where(c => c.color_category == colorCatName).Select(c => c.color).ToListAsync();

            return new JsonResult(ColorsViews);
        }

    }
}
