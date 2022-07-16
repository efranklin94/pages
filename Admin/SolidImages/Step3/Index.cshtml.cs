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

namespace kamjaService.Pages.Admin.SolidImages.Step3
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }
        public IEnumerable<string> SolidImgSrcList { get; set; }
        public void OnGet(long? id)
        {
            if (id != null)
            {
                var productGroupingCodes = _context.ProductGrouping.Where(x => x.GroupRef == id).Select(x => x.Number).ToList();

                if (Directory.Exists("wwwroot\\image\\Product\\solid"))
                {
                    IEnumerable<string> SolidImgSrcListTemp = SolidImgSrcList;
                    var number = "1003";
                    SolidImgSrcList = SolidImgSrcListTemp ?? Enumerable.Empty<string>().Concat(Directory.EnumerateFiles("wwwroot\\image\\Product\\solid", number + ".png") ?? Enumerable.Empty<string>());

                    IEnumerable<string> SolidImgSrcListTemp2 = SolidImgSrcList;
                    var number2 = "1002";
                    SolidImgSrcList = SolidImgSrcListTemp2 ?? Enumerable.Empty<string>().Concat(Directory.EnumerateFiles("wwwroot\\image\\Product\\solid", number2 + ".png") ?? Enumerable.Empty<string>());
                    
                    Console.WriteLine(SolidImgSrcList.Count());
                }
            }
        }
        
    }

}















//SolidImgSrcList.Concat(Directory.EnumerateFiles("wwwroot\\image\\Product\\solid", "*.png"));
//var together = (first ?? Enumerable.Empty<string>()).Concat(second ?? Enumerable.Empty<string>()); //amending `<string>` to the appropriate type



//////////////////////////////////////////////////////////////


//SolidImgSrcList = Directory.EnumerateFiles("wwwroot\\image\\Product\\solid", productGroupings.FirstOrDefault().Number + ".*", SearchOption.AllDirectories);
//Console.WriteLine(SolidImgSrcList.FirstOrDefault());
//Console.WriteLine(productGroupings.FirstOrDefault().Number);
///////////////////////////////////////////////////////////////




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