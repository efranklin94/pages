using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using kamjaService.Models;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NinjaNye.SearchExtensions;

namespace kamjaService.Pages.Admin.ProductImages.Step3
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;
        private IHostingEnvironment hostingEnv;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context, IHostingEnvironment env)
        {
            _context = context; this.hostingEnv = env;
        }
        [BindProperty]
        public TBL_ProductsPicturesService TBL_ProductsPicturesService { get; set; }
        public IList<VW_RemainingProductsPictures> productGroupings { get; set; }
        [BindProperty]
        public TBL_dimensionImgsService TBL_dimensionImgsService { get; set; }
        public IList<VW_RemainingProductDimensions> productGroupingsDims { get; set; }

        [BindProperty, Display(Name = "File")]
        public IFormFile UploadedFile { get; set; }
        [BindProperty, Display(Name = "DimFile")]
        public IFormFile UploadedDim { get; set; }
        public string fileExtensionViewBag { get; set; }
        public string dimensionFileName { get; set; }
        public string CurrentFilter { get; set; }
        public long? gID { get; set; }
        public void OnGet(long? id, string currentFilter, string searchString)
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
            var ProductG = from m in _context.VW_RemainingProductsPictures
                           select m;
            var pr = ProductG.Where(x => x.GroupRef == id).Distinct();
            IQueryable<VW_RemainingProductsPictures> prs = pr;
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


            productGroupings = prs.OrderBy(p => p.Number).ToList();
            productGroupingsDims = _context.VW_RemainingProductDimensions.Where(p => p.GroupRef == id).OrderBy(p => p.Number).ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string PicOrDim = Request.Form["PicOrDim"];
            if (PicOrDim == "Pic")
            {

                if (UploadedFile != null)
                {
                    string code = Request.Form["code"];
                    string name = Request.Form["name"];

                    TBL_ProductsPicturesService picturesService = _context.TBL_ProductsPicturesService.Where(p => p.Code == code).FirstOrDefault();

                    if (picturesService == null)
                    {
                        var filename = UploadedFile.FileName;
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                        TBL_ProductsPicturesService.Code = code;
                        TBL_ProductsPicturesService.Name = name;
                        var fileExtension = Path.GetExtension(filename);
                        fileExtensionViewBag = fileExtension;
                        var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);
                        TBL_ProductsPicturesService.FileName = fileNameWithItsExtension;

                        using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension,"Pic")))
                            UploadedFile.CopyTo(output);

                        _context.TBL_ProductsPicturesService.Add(TBL_ProductsPicturesService);
                        await _context.SaveChangesAsync();
                    }

                    else
                    {
                        var filename = UploadedFile.FileName;
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                        TBL_ProductsPicturesService.Code = picturesService.Code;
                        TBL_ProductsPicturesService.Name = picturesService.Name;
                        var fileExtension = Path.GetExtension(filename);
                        fileExtensionViewBag = fileExtension;
                        var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);
                        TBL_ProductsPicturesService.FileName = fileNameWithItsExtension;

                        using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension,"Pic")))
                            UploadedFile.CopyTo(output);

                        //TBL_ProductsPictures prev_productsPictures = new TBL_ProductsPictures();
                        //prev_productsPictures.Code = picturesService.Code;
                        //prev_productsPictures.Name = picturesService.Name;
                        //prev_productsPictures.FileName = picturesService.FileName;
                        
                        //prev_productsPictures.IsDefault = true;
                        //_context.Entry(prev_productsPictures).State = EntityState.Deleted;

                        _context.Attach(picturesService).State = EntityState.Detached;
                        _context.Attach(TBL_ProductsPicturesService).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    return new JsonResult("success");
                }
            }

            if (PicOrDim == "Dim")
            {
                if (UploadedDim != null)
                {
                    string code = Request.Form["code"];
                    string name = Request.Form["name"];

                    TBL_dimensionImgsService DimService = _context.TBL_dimensionImgsService.Where(p => p.Code == code).FirstOrDefault();

                    if (DimService == null)
                    {
                        var filename = UploadedDim.FileName;
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                        TBL_dimensionImgsService.Code = code;
                        TBL_dimensionImgsService.Name = name;
                        var fileExtension = Path.GetExtension(filename);
                        fileExtensionViewBag = fileExtension;
                        var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);
                        TBL_dimensionImgsService.FileName = fileNameWithItsExtension;

                        using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension,"Dim")))
                            UploadedDim.CopyTo(output);

                        _context.TBL_dimensionImgsService.Add(TBL_dimensionImgsService);
                        await _context.SaveChangesAsync();
                    }

                    else
                    {
                        var filename = UploadedDim.FileName;
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                        TBL_dimensionImgsService.Code = DimService.Code;
                        TBL_dimensionImgsService.Name = DimService.Name;
                        var fileExtension = Path.GetExtension(filename);
                        fileExtensionViewBag = fileExtension;
                        var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);
                        TBL_dimensionImgsService.FileName = fileNameWithItsExtension;

                        using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension,"Dim")))
                            UploadedDim.CopyTo(output);

                        _context.Attach(DimService).State = EntityState.Detached;
                        _context.Attach(TBL_dimensionImgsService).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    return new JsonResult("success");
                }
            }
            return new JsonResult("failed");
        }



        private string GetPathAndFilename(string filename, string PicOrDim)
        {
            string path = string.Empty;
            if (PicOrDim == "Pic") { 
                path = hostingEnv.WebRootPath + "\\image\\Product\\pic\\";
            }
            if (PicOrDim == "Dim") { 
                path = hostingEnv.WebRootPath + "\\image\\dimensions\\";
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }
    }

}



























//public async Task<IActionResult> OnPostDimAsync()
//{
//    if (!ModelState.IsValid)
//    {
//        return Page();
//    }

//    if (UploadedDim != null)
//    {
//        string code = Request.Form["code"];
//        string name = Request.Form["name"];

//        TBL_dimensionImgsService DimService = _context.TBL_dimensionImgsService.Where(p => p.Code == code).FirstOrDefault();

//        if (DimService == null)
//        {
//            var filename = UploadedDim.FileName;
//            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
//            TBL_dimensionImgsService.Code = code;
//            TBL_dimensionImgsService.Name = name;
//            var fileExtension = Path.GetExtension(filename);
//            fileExtensionViewBag = fileExtension;
//            var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);
//            TBL_dimensionImgsService.FileName = fileNameWithItsExtension;

//            using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension, "Dim")))
//                UploadedDim.CopyTo(output);

//            _context.TBL_dimensionImgsService.Add(TBL_dimensionImgsService);
//            await _context.SaveChangesAsync();
//        }

//        else
//        {
//            var filename = UploadedDim.FileName;
//            var myUniqueFileName = Convert.ToString(Guid.NewGuid());
//            TBL_dimensionImgsService.Code = DimService.Code;
//            TBL_dimensionImgsService.Name = DimService.Name;
//            var fileExtension = Path.GetExtension(filename);
//            fileExtensionViewBag = fileExtension;
//            var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);
//            TBL_dimensionImgsService.FileName = fileNameWithItsExtension;

//            using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension, "Dim")))
//                UploadedDim.CopyTo(output);

//            _context.Attach(DimService).State = EntityState.Detached;
//            _context.Attach(TBL_dimensionImgsService).State = EntityState.Modified;
//            await _context.SaveChangesAsync();
//        }
//        return new JsonResult("success");
//    }

//    return new JsonResult("failed");
//}