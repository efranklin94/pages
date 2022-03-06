﻿using System;
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
        public List<int> ColorsViews { get; set; }
        public string productGroupName { get; set; }
        public string productGroupingFileName { get; set; }
        public List<TBL_dimensionImgs> dimensionFileNames { get; set; }
        public List<TBL_ProductsPictures> productsPicturesFileNames { get; set; }

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
            proColViews = await prs.OrderBy(p => p.Number).ToListAsync();


            productGroupingexps = await _context.ProductGroupingexp.ToListAsync();
            productGroupDescription = await pg_desc_query.FirstOrDefaultAsync();

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

            dimensionFileNames = _context.TBL_dimensionImgs.ToList();
            productsPicturesFileNames = _context.TBL_ProductsPictures.ToList();

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

    }
}
