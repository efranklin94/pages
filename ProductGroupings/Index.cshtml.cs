using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using NinjaNye.SearchExtensions;
using System.Data;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
namespace kamjaService.Pages.ProductGroupings
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
        public IEnumerable<string> imageSrcList { get; set; }
        public IEnumerable<string> videoSrc { get; set; }
        public IList<ProductGroupingClips> productGroupingClips { get; set; }
        public IEnumerable<string> videoSrcList { get; set; }
        public string productGroupName { get; set; }
        public List<TBL_dimensionImgs> dimensionFileNames { get; set; }
        public List<VW_AllProductsPictures> productsPicturesFileNames { get; set; }
        public long groupRef { get; set; }
        public async Task OnGetAsync(long? id, string currentFilter, string searchString)
        {
            
            if (id != null)
            {
                gID = id;
            }
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

            var ProductG = from m in _context.ProductGrouping
                           select m;
            var pr = ProductG.Where(x => x.GroupRef == id).Distinct();

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

            ProductGrouping = await prs.OrderBy(p => p.Number).ToListAsync();
            //ProductGrouping = await _context.ProductGrouping.ToListAsync();


            if (Directory.Exists("wwwroot\\image\\slider\\SalesInfo\\" + gID)) {
                imageSrcList = Directory.EnumerateFiles("wwwroot\\image\\slider\\SalesInfo\\" + gID, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg"));
            }
            else
            {
                imageSrcList = Directory.EnumerateFiles("wwwroot\\image", "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            }

            //if (Directory.Exists("wwwroot\\video\\fanni\\" + gID))
            //{
            //    videoSrc = Directory.EnumerateFiles("wwwroot\\video\\fanni\\" + gID, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            //}
            //else
            //{
            //    videoSrc = Directory.EnumerateFiles("wwwroot\\image", "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            //}
            if (Directory.Exists("wwwroot\\video\\slider\\fanni\\" + gID))
            {
                videoSrcList = Directory.EnumerateFiles("wwwroot\\video\\slider\\fanni\\" + gID, ".", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            }
            else
            {
                videoSrcList = Directory.EnumerateFiles("wwwroot\\image", ".", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            }
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

            productGroupName = _context.ProductGroup.Where(pg => pg.ProductGroupId == gID).Select(pg => pg.Expr1).FirstOrDefault();

            dimensionFileNames = _context.TBL_dimensionImgs.ToList();
            productsPicturesFileNames = _context.VW_AllProductsPictures.ToList();

            productGroupingClips = _context.ProductGroupingClips.Where(c => c.productGroupingId == gID).Where(c => c.FanniOrSalesInfo == "Fanni").ToList();
            groupRef = (long)id;
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
