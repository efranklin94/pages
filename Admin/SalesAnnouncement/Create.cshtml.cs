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
        public ParentTag ParentTag { get; set; }
        public ProductGroupTag ProductGroupTag { get; set; }
        public ProductGroupingTag ProductGroupingTag { get; set; }
        public List<string> parentTagNames { get; set; }
        public List<string> productGroupTagNames { get; set; }
        public List<string> productGroupingTagNames { get; set; }
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

        //[BindProperty]
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

            //foreach(var tag in parentTagNames)
            //{
            //    ParentTag.tag_name = tag;
            //    ParentTag.SalesAnnouncementREF = SalesAnnouncements.SalesAnnouncementId;
            //}

            //foreach (var tag in productGroupTagNames)
            //{
            //    ProductGroupTag.tag_name = tag;
            //    ProductGroupTag.SalesAnnouncementREF = SalesAnnouncements.SalesAnnouncementId;
            //}

            //foreach (var tag in productGroupingTagNames)
            //{
            //    ProductGroupingTag.tag_name = tag;
            //    ProductGroupingTag.SalesAnnouncementREF = SalesAnnouncements.SalesAnnouncementId;
            //}

            //_context.SalesAnnouncements.Add(SalesAnnouncements);
            //await _context.SaveChangesAsync();


            Console.WriteLine(ViewData["selectedParentsView"]);

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

        public async Task<IActionResult> OnGetParent(List<string> selectedParents)
        {
            List<long> selectedParentsId = new List<long>();
            foreach (string selectedParent in selectedParents) {
                selectedParentsId.Add(await _context.Parent.Where(p => p.Name == selectedParent).Select(p => p.ParentId).FirstOrDefaultAsync());
            }

            List<string> selectedProductGroups = new List<string>();
            foreach (var selectedParentId in selectedParentsId) {
               selectedProductGroups.AddRange(await _context.ProductGroup.Where(pg => pg.ParentRef == selectedParentId).Select(pg => pg.Expr1).ToListAsync());
            }

            return new JsonResult(selectedProductGroups);
        }
        public async Task<IActionResult> OnGetProductGroup(List<string> selectedProductGroups)
        {
            List<long> selectedProductGroupsId = new List<long>();
            foreach (string selectedProductGroup in selectedProductGroups)
            {
                selectedProductGroupsId.Add(await _context.ProductGroup.Where(pg => pg.Expr1 == selectedProductGroup).Select(pg => pg.ProductGroupId).FirstOrDefaultAsync());
            }

            List<string> selectedProductGroupings = new List<string>();
            foreach (var selectedProductGroupId in selectedProductGroupsId)
            {
                selectedProductGroupings.AddRange(await _context.ProductGrouping.Where(pging => pging.GroupRef == selectedProductGroupId).Select(pging => pging.Name).ToListAsync());
            }

            return new JsonResult(selectedProductGroupings);
        }
        public async Task<IActionResult> OnPostForm(string title, string date, List<string> selectedParents, List<string> selectedProductGroups)
        {
            SalesAnnouncements salesAnnouncement = new SalesAnnouncements();
            salesAnnouncement.title = title;
            salesAnnouncement.date = date;

            _context.SalesAnnouncements.Add(salesAnnouncement);
            Console.WriteLine(await _context.SaveChangesAsync());

            var SalesAnnouceId = _context.SalesAnnouncements.Max(s => s.SalesAnnouncementId);

            foreach (var selectedParent in selectedParents)
            {
                ParentTag parentTag = new ParentTag();
                parentTag.tag_name = selectedParent;

                parentTag.SalesAnnouncementREF = SalesAnnouceId;

                _context.ParentTag.Add(parentTag);
                await _context.SaveChangesAsync();
                
            }

            foreach (var selectedProductGroup in selectedProductGroups)
            {
                ProductGroupTag productGroupTag = new ProductGroupTag();
                productGroupTag.tag_name = selectedProductGroup;
                
                productGroupTag.SalesAnnouncementREF = SalesAnnouceId;

                _context.ProductGroupTag.Add(productGroupTag);
                await _context.SaveChangesAsync();
            }

            return new JsonResult("***success***");
        }

        //
    }
}
