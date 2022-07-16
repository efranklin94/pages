using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using DT = System.Data;
using QC = System.Data.SqlClient;
using System.Data.SqlClient;
using System.Data;
using NinjaNye.SearchExtensions;

namespace kamjaService.Pages.Admin.NaghsheCode.Step5
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public YaraghCVM Yaragh { get; set; }

        public string CurrentFilter { get; set; }
        public long? ID { get; set; }
        public string Yt { get; set; }
        public string productGroupCode { get; set; }
        public async Task OnGetAsync(long? id, string yt, string currentFilter, string searchString, string pn, string pcode)
        {
            //ViewData["prin"] = name;
            //ViewData["pri"] = id;
            //ViewData["pcode"] = code;
            //ViewData["code"] = "/image/Product/pic/" + fileName;
            //PartGrouping = await _context.PartGrouping.ToListAsync();
            productGroupCode = pcode;
            Yaragh = new YaraghCVM();
            ViewData["prn"] = pn;
            if (id != null && yt != null)
            {
                ID = id;
                Yt = yt;
            }
            if (searchString != null)
            {

            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;


            var pr2 = from m in _context.YaraghComponent
                      where m.ProductId == ID && m.گروه.Contains(Yt)
                      select m;


            Yaragh.Components = await (from m in _context.Component
                                       where m.ProductId == ID
                                       select m).ToListAsync();
            IQueryable<YaraghComponent> prs = pr2;

            if (!String.IsNullOrEmpty(searchString))
            {
                String[] searchstrZ = searchString.Split(' ');



                pr2 = pr2.Search(s => s.AjzaKalaName, s => s.AjzakalaCode).ContainingAll(searchstrZ).AsQueryable();
                prs = pr2.Where(s => s.AjzaKalaName.Contains(s.AjzaKalaName));
            }
            ViewData["pcode"] = "/image/Product/solid/" + productGroupCode + ".png";
            Yaragh.YaraghComponent = await prs.ToListAsync();
        }

        public async Task<IActionResult> OnPostNaghsheCode(long code, decimal naghsheCode, string productCode)
        {
            string commandText = $"" +
                            $"update component " +
                            $"set کد_نقشه = @naghsheCode " +
                            $"where ProductCode = @ProductCode " +
                            $"and ajzakalaCode = @ajzakalaCode;";
            try
            {
                using (SqlConnection connection = new SqlConnection("Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"))
                {
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.Add("@ProductCode", SqlDbType.NVarChar);
                    command.Parameters["@ProductCode"].Value = productCode;

                    command.Parameters.Add("@ajzakalaCode", SqlDbType.BigInt);
                    command.Parameters["@ajzakalaCode"].Value = code;

                    command.Parameters.Add("@naghsheCode", SqlDbType.Decimal);
                    command.Parameters["@naghsheCode"].Value = naghsheCode;

                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine("RowsAffected: {0}", rowsAffected);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return new JsonResult("failure");
            }

            return new JsonResult("success");

        }
        private bool ComponentExists(string id)
        {
            return _context.Component.Any(e => e.ajzakalaCode.Equals(id));
        }
    }
}
