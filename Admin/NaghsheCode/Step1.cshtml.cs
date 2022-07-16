using kamjaService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kamjaService.Pages.Admin.NaghsheCode
{
    public class Step1Model : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public Step1Model(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }
        public IList<Parent> Parent { get; set; }

        public async Task OnGetAsync()
        {
            Parent = await _context.Parent.ToListAsync();
        }
    }
}
