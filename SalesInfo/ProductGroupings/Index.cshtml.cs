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

        public IList<ProductGroupEXP> productGroupDescription { get; set; }
        public IList<ProColView> proColViews { get; set; }
        public IList<ProductGroupingexp> productGroupingexps { get; set; }
        public IEnumerable<string> imageSrcList { get; set; }
        public IEnumerable<string> videoSrcList { get; set; }
        public IList<ProductGroupingClips> productGroupingClips { get; set; }
        public bool isExistingProductGroup { get; set; }
        public List<int> ColorsViews { get; set; }
        public string productGroupName { get; set; }
        public string productGroupingFileName { get; set; }
        public List<VW_AllProductsDimensions> dimensionFileNames { get; set; }
        public List<VW_AllProductsPictures> productsPicturesFileNames { get; set; }
        public long groupRef { get; set; }
        public string generalProductGroupEXP { get; set; }
        public async Task OnGetAsync(long? id, string currentFilter, string searchString)
        {
            if (searchString != null)
            {
                searchString = PersianToEnglish(searchString);
                //pageIndex = 1;
            }
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

                String[] searchstrZ = searchString.Split(' ');

                //remove the spaces to prevent exception
                List<string> searchstrZWithoutNull = new List<string>();
                foreach (String s in searchstrZ)
                {
                    if(s == "")
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

            var prgd = from pg in _context.ProductGroup
                       where pg.ProductGroupId == id
                       select pg;

            productGroup = await prgd.SingleOrDefaultAsync();

            var pg_desc_query = from pd in _context.ProductGroupEXP
                                where pd.ProductGroupIdref == id
                                select pd;
            proColViews = await prs.OrderBy(p => p.Number).ToListAsync();


            productGroupingexps = await _context.ProductGroupingexp.ToListAsync();
            productGroupDescription = await pg_desc_query.ToListAsync();

            if (Directory.Exists("wwwroot\\image\\slider\\SalesInfo\\" + gID)) {
                imageSrcList = Directory.EnumerateFiles("wwwroot\\image\\slider\\SalesInfo\\" + gID, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg"));
            }
            else
            {
                imageSrcList = Directory.EnumerateFiles("wwwroot\\image", "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            }

            if (Directory.Exists("wwwroot\\video\\slider\\SalesInfo\\" + gID))
            {
                videoSrcList = Directory.EnumerateFiles("wwwroot\\video\\slider\\SalesInfo\\" + gID, ".", SearchOption.AllDirectories).Where(s => s.EndsWith(".mp4"));
            }
            else
            {
                videoSrcList = Directory.EnumerateFiles("wwwroot\\image", ".", SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png"));
            }


            productGroupName = _context.ProductGroup.Where(pg => pg.ProductGroupId == gID).Select(pg => pg.Expr1).FirstOrDefault();

            dimensionFileNames = _context.VW_AllProductsDimensions.ToList();
            productsPicturesFileNames = _context.VW_AllProductsPictures.ToList();

            productGroupingClips = await _context.ProductGroupingClips.Where(c => c.productGroupingId == gID).ToListAsync();
            groupRef = (long)id;


            generalProductGroupEXP = await pg_desc_query.Where(p => p.catergory == "1").Select(p => p.pgexp).FirstOrDefaultAsync();
        }

        public async Task<IActionResult> OnGetColors(string colorCatName)
        {
            var Colors = await _context.ColorsView.Where(c => c.color_category == colorCatName).Where(c => c.flag == 1).Select(c => c.color).ToListAsync();

            var colorEntites = (from c in _context.Colors
                                where Colors.Contains(c.color_name)
                                select c)
                            .ToList();


            return new JsonResult(colorEntites);
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
