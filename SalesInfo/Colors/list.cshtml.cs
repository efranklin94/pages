using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.SalesInfo.Colors
{
    public class listModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public listModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<ColorsView> ColorsView { get;set; }
        public IList<ColorCAT> ColorCAT { get; set; }
        public string[,] colors { get; set; }
        public Dictionary<string, string> colorSrc { get; set; }
        public async Task OnGetAsync()
        {
            ColorsView = await _context.ColorsView.ToListAsync();
            ColorCAT = await _context.ColorCAT.ToListAsync();

            foreach (var colorName in ColorsView.Select(m=>m.color)) {
                //if (System.IO.Directory.Exists("wwwroot\\image\\Colors\\" + colorName))
                //{
                //    colorSrc[colorName] = System.IO.Directory.EnumerateFiles("wwwroot\\image\\Colors\\" + colorName, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".jpg")).FirstOrDefault();
                //}
                //else
                //{
                //    colorSrc[colorName] = System.IO.Directory.EnumerateFiles("wwwroot\\image", "*.*", System.IO.SearchOption.AllDirectories).Where(f => f.EndsWith("no_image.png")).FirstOrDefault();

                //    Console.WriteLine(colorSrc[colorName]);
                //}


                //if (System.IO.File.Exists("wwwroot\\image\\colors\\" + colorName + ".jpg"))
                //{
                //    Console.WriteLine(colorName + " exists.");
                //}


            }


        }
    }
}
