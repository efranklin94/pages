using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;

namespace kamjaService.Pages.Admin.SalesAnnouncement
{
    public class EditModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public EditModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SalesAnnouncements SalesAnnouncements { get; set; }
        public List<string> parents { get; set; }
        public List<string> productGroups { get; set; }
        public List<string> productGroupings { get; set; }
        public string checkedParents { get; set; }
        public string checkedProductGroups { get; set; }
        public string checkedProductGroupings { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SalesAnnouncements = await _context.SalesAnnouncements.FirstOrDefaultAsync(m => m.SalesAnnouncementId == id);
            productGroups = await _context.ProductGroup.OrderBy(x => x.ProductGroupId).Select(x => x.Expr1).Distinct().ToListAsync();
            productGroupings = await _context.ProductGrouping.Select(x => x.Name).Distinct().ToListAsync();
            parents = await _context.Parent.Select(x => x.Name).Distinct().ToListAsync();

            checkedParents = await _context.ParentTag.Where(t => t.SalesAnnouncementREF.Equals(id)).Select(t => t.tag_name).FirstOrDefaultAsync();
            checkedProductGroups = await _context.ProductGroupTag.Where(t => t.SalesAnnouncementREF.Equals(id)).Select(t => t.tag_name).FirstOrDefaultAsync();
            checkedProductGroupings = await _context.ProductGroupingTag.Where(t => t.SalesAnnouncementREF.Equals(id)).Select(t => t.tag_name).FirstOrDefaultAsync();

            //String[] checkedParentsArray = checkedParents.Split(',');

            if (SalesAnnouncements == null)
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

            //Updating Tags databases
            var selectedParents = Request.Form["selectedParents"];
            ParentTag parentTag = new ParentTag();
            var parentTag_id = await _context.ParentTag.Where(x => x.SalesAnnouncementREF == SalesAnnouncements.SalesAnnouncementId).Select(x => x.ParentTagId).FirstOrDefaultAsync();

            parentTag.SalesAnnouncementREF = SalesAnnouncements.SalesAnnouncementId;
            parentTag.ParentTagId = parentTag_id;
            parentTag.tag_name = selectedParents;


            var selectedProductGroups = Request.Form["selectedProductGroups"];
            ProductGroupTag productGroupTag = new ProductGroupTag();
            var productGroupTag_id = await _context.ProductGroupTag.Where(x => x.SalesAnnouncementREF == SalesAnnouncements.SalesAnnouncementId).Select(x => x.ProductGroupTagId).FirstOrDefaultAsync();

            productGroupTag.SalesAnnouncementREF = SalesAnnouncements.SalesAnnouncementId;
            productGroupTag.ProductGroupTagId = productGroupTag_id;
            productGroupTag.tag_name = selectedProductGroups;


            var selectedProductGroupings = Request.Form["selectedProductGroupings"];
            ProductGroupingTag productGroupingTag = new ProductGroupingTag();
            var productGroupingTag_id = await _context.ProductGroupingTag.Where(x => x.SalesAnnouncementREF == SalesAnnouncements.SalesAnnouncementId).Select(x => x.ProductGroupingTagId).FirstOrDefaultAsync();

            productGroupingTag.SalesAnnouncementREF = SalesAnnouncements.SalesAnnouncementId;
            productGroupingTag.ProductGroupingTagId = productGroupingTag_id;
            productGroupingTag.tag_name = selectedProductGroupings;


            _context.Attach(parentTag).State = EntityState.Modified;
            _context.Attach(productGroupTag).State = EntityState.Modified;
            _context.Attach(productGroupingTag).State = EntityState.Modified;
            _context.Attach(SalesAnnouncements).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesAnnouncementsExists(SalesAnnouncements.SalesAnnouncementId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return new JsonResult("success");
        }

        private bool SalesAnnouncementsExists(long id)
        {
            return _context.SalesAnnouncements.Any(e => e.SalesAnnouncementId == id);
        }

        public async Task<IActionResult> OnGetParent(List<string> selectedParents)
        {
            List<long> selectedParentsId = new List<long>();
            foreach (string selectedParent in selectedParents)
            {
                selectedParentsId.Add(await _context.Parent.Where(p => p.Name == selectedParent).Select(p => p.ParentId).FirstOrDefaultAsync());
            }

            List<string> selectedProductGroups = new List<string>();
            foreach (var selectedParentId in selectedParentsId)
            {
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
    }
}
