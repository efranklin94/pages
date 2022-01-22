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

namespace kamjaService.Pages.Admin.SalesAnnouncement
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
            this.hostingEnv = env;
        }
        public List<String> productGroups { get; set; }
        public List<String> productGroupings { get; set; }
        public List<String> parents { get; set; }

        [BindProperty]
        public string productGroup { get; set; }
        [BindProperty]
        public string productGrouping { get; set; }
        [BindProperty]
        public string parent { get; set; }

        public async Task OnGetAsync()
        {
            productGroups = await _context.ProductGroup.OrderBy(x => x.ProductGroupId).Select(x=>x.Expr1).Distinct().ToListAsync();
            productGroupings = await _context.ProductGrouping.Select(x=>x.Name).Distinct().ToListAsync();
            parents = await _context.Parent.Select(x => x.Name).Distinct().ToListAsync();
        }

        [BindProperty]
        public SalesAnnouncements SalesAnnouncements { get; set; }

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
                SalesAnnouncements.file_name = myUniqueFileName;
                fileNameViewBag = myUniqueFileName;
                // Getting Extension  
                var fileExtension = Path.GetExtension(filename);
                fileExtensionViewBag = fileExtension;
                // Concating filename + fileExtension (unique filename)  
                var fileNameWithItsExtension = string.Concat(myUniqueFileName, fileExtension);

                using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension)))
                    await UploadedFile.CopyToAsync(output);
            }

            _context.SalesAnnouncements.Add(SalesAnnouncements);
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
            string path = hostingEnv.WebRootPath + "\\SalesAnnouncePdf\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }

        public JsonResult OnGetParent(string selectedParent)
        {
            var selectedParentId = _context.Parent.Where(p => p.Name == selectedParent).Select(p => p.ParentId).FirstOrDefault();
            var selectedProductGroups = _context.ProductGroup.Where(pg => pg.ParentRef == selectedParentId).Select(pg => pg.Expr1).ToList();

            return new JsonResult(selectedProductGroups);
        }
        public JsonResult OnGetProductGroup(string selectedProductGroup)
        {
            var selectedProductGroupId = _context.ProductGroup.Where(p => p.Expr1 == selectedProductGroup).Select(p => p.ProductGroupId).FirstOrDefault();
            var selectedProductGroupings = _context.ProductGrouping.Where(pg => pg.GroupRef == selectedProductGroupId).Select(pg => pg.Name).ToList();

            return new JsonResult(selectedProductGroupings);
        }
    }
}
