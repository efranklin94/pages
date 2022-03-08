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
        public IList<ProductGrouping> productGroupings { get; set; }
        [BindProperty, Display(Name = "File")]
        public IFormFile UploadedFile { get; set; }
        public string fileExtensionViewBag { get; set; }

        public void OnGet(long? id)
        {
            productGroupings = _context.ProductGrouping.Where(p => p.GroupRef == id).OrderBy(p => p.Name).ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (UploadedFile != null)
            {
                var code = Request.Form["code"];
                var name = Request.Form["name"];

                var filename = UploadedFile.FileName;
                var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                TBL_ProductsPicturesService.FileName = myUniqueFileName;
                TBL_ProductsPicturesService.Code = code;
                TBL_ProductsPicturesService.Name = name;
                var fileExtension = Path.GetExtension(filename);
                fileExtensionViewBag = fileExtension;
                var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);

                using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension)))
                    UploadedFile.CopyTo(output);

                _context.TBL_ProductsPicturesService.Add(TBL_ProductsPicturesService);
                await _context.SaveChangesAsync();

                return new JsonResult("success");
            }

            return new JsonResult("failed");
        }

        private string GetPathAndFilename(string filename)
        {
            string path = hostingEnv.WebRootPath + "\\image\\Product\\pic\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }
    }

}
