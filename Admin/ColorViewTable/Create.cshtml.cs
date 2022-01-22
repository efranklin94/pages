using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using kamjaService.Models;
using Microsoft.EntityFrameworkCore;

namespace kamjaService.Pages.Admin.ColorViewTable
{
    public class CreateModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public CreateModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }
        
        public ColorsCATRef colorsCATRef { get; set; }
        [BindProperty]
        public string color { get; set; }
        [BindProperty]
        public string color_category { get; set; }
        [BindProperty]
        public string flag { get; set; }
        public List<String> color_categories { get; set; }
        public List<String> colors { get; set; }

        public async Task OnGetAsync()
        {
            color_categories = await _context.ColorsView.Select(x => x.color_category).Distinct().ToListAsync();
            colors = await _context.ColorsView.Select(x => x.color).Distinct().ToListAsync();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var colorId_query = from dbcolor in _context.Colors
                                where dbcolor.color_name == color
                                select dbcolor.Id;
            var colorCATId_query = from dbcolorCAT in _context.ColorCAT
                                where dbcolorCAT.color_category == color_category
                                select dbcolorCAT.ID;

            colorsCATRef = new ColorsCATRef()
            {
                colorCATIdREF = colorCATId_query.FirstOrDefault(),
                flag = int.Parse(flag),
                colorsIdREF = colorId_query.FirstOrDefault()
            };

            _context.ColorsCATRef.Add(colorsCATRef);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
