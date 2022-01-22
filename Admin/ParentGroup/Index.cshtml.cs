using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using Microsoft.AspNetCore.Authorization;

namespace kamjaService.Pages.Admin.ParentGroup
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
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
