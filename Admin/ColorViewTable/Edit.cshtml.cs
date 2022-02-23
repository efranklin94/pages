using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
namespace kamjaService.Pages.Admin.ColorViewTable
{
    public class EditModel : PageModel
    {
        private IHostingEnvironment hostingEnv;
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public EditModel(kamjaService.Models.KamjaServiceDbContext context, IHostingEnvironment env)
        {
            _context = context;
            hostingEnv = env;
        }

        [BindProperty]
        public ColorsView ColorsViewTable { get; set; }
        [BindProperty, Display(Name = "File")]
        public IFormFile UploadedFile { get; set; }
        public ColorsCATRef colorsCATRef { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ColorsViewTable = await _context.ColorsView.FirstOrDefaultAsync(m => m.color_Id == id);

            if (ColorsViewTable == null)
            {
                return NotFound();
            }
            return Page();
        }

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
                var prevJpgFileName = await _context.ColorsView.Where(c => c.color_Id == ColorsViewTable.color_Id).Select(c => c.color).FirstOrDefaultAsync();

                string fullPath = hostingEnv.WebRootPath + "\\image\\colors\\new\\" + prevJpgFileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                var filename = UploadedFile.FileName;



                var fileExtension = Path.GetExtension(filename);

                var fileNameWithItsExtension = string.Concat(filename, fileExtension);

                using (FileStream output = System.IO.File.Create(GetPathAndFilename(fileNameWithItsExtension)))
                    UploadedFile.CopyTo(output);

                //TODO ColorsViewTable.color = filename;
                colorsCATRef.color_Id = ColorsViewTable.color_Id;
                colorsCATRef.colorCATIdREF = _context.ColorCAT.Where(c => c.color_category == ColorsViewTable.color_category).Select(c => c.ID).FirstOrDefault();
                colorsCATRef.colorsIdREF = _context.Colors.Where(c => c.color_name == ColorsViewTable.color).Select(c => c.Id).FirstOrDefault();
                colorsCATRef.file_path = GetPathAndFilename(fileNameWithItsExtension);
            }

            //TODO _context.Attach(ColorsViewTable).State = EntityState.Modified;
            _context.Attach(colorsCATRef).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColorsViewTableExists(ColorsViewTable.color_Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ColorsViewTableExists(int id)
        {
            return _context.ColorsView.Any(e => e.color_Id == id);
        }

        private string GetPathAndFilename(string filename)
        {
            string path = hostingEnv.WebRootPath + "\\image\\colors\\new\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }
    }
}
