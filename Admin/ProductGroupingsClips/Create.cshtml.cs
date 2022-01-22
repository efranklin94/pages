using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using kamjaService.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;

namespace kamjaService.Pages.Admin.ProductGroupingsClips
{
    public class CreateModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;
        private IHostingEnvironment hostingEnv;
        [BindProperty, Display(Name = "File")]
        public IFormFile UploadedFile { get; set; }

        public string fileNameViewBag { get; set; }
        public string fileExtensionViewBag { get; set; }

        public CreateModel(kamjaService.Models.KamjaServiceDbContext context, IHostingEnvironment env)
        {
            _context = context;
            hostingEnv = env;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ProductGroupingClips ProductGroupingClips { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (UploadedFile != null)
            {
                var filename = UploadedFile.FileName;
                // Unique filename "Guid"  
                var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                //store the filename into the database
                ProductGroupingClips.name = myUniqueFileName;
                fileNameViewBag = myUniqueFileName;
                // Getting Extension  
                var fileExtension = Path.GetExtension(filename);
                fileExtensionViewBag = fileExtension;
                // Concating filename + fileExtension (unique filename)  
                var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);

                using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension)))
                    await UploadedFile.CopyToAsync(output);
            }

            _context.ProductGroupingClips.Add(ProductGroupingClips);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string GetPathAndFilename(string filename)
        {
            string path = hostingEnv.WebRootPath + "\\video\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }




    }
}
