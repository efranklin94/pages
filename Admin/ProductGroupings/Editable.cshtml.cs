using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using DT = System.Data;
using QC = Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using System.Data;
using NinjaNye.SearchExtensions;

namespace kamjaService.Pages.Admin.ProductGroupings
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context)
        {
            _context = context;
        }

        public IList<ProductGrouping> productGroupings { get; set; }
        public ProductGroup productGroup { get; set; }
        public IList<ProColView> proColViews { get; set; }
        public IList<ColorCAT> colors { get; set; }
        public IList<int> productBadaneColCat { get; set; }
        public IList<int> productNamaColCat { get; set; }
        public IList<int> productPayeChoobiColCat { get; set; }
        public string productGroupDescription { get; set; }
        [BindProperty]
        public IList<ProductGroupingexp> productGroupingexps { get; set; }
        public String ProductGroupName { get; set; }
        public string productGroupName  { get; set; }
        public string CurrentFilter { get; set; }
        public long? gID { get; set; }

        public async Task OnGetAsync(long? id, string currentFilter, string searchString)
        {
            if (id != null)
            {
                gID = id;
            }
            if (searchString != null)
            {
                //pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;
            var ProductG = from m in _context.ProColView
                           select m;
            var pr = ProductG.Where(x => x.GroupRef == id).Distinct();
            IQueryable<ProColView> prs = pr;
            if (!String.IsNullOrEmpty(searchString))

            {
                String[] searchstrZ = searchString.Split(' ');

                //remove the spaces to prevent exception
                List<string> searchstrZWithoutNull = new List<string>();
                foreach (String s in searchstrZ)
                {
                    if (s == "")
                    {
                        continue;
                    }
                    else
                    {
                        searchstrZWithoutNull.Add(s);
                    }
                }
                searchstrZ = searchstrZWithoutNull.ToArray();

                var prName = pr.Search(s => s.Name).ContainingAll(searchstrZ).AsQueryable();
                var prsName = prName.Where(s => s.Name.Contains(s.Name));

                var prNumber = pr.Search(s => s.Number).ContainingAll(searchstrZ).AsQueryable();
                var prsNumber = prNumber.Where(s => s.Number.Contains(s.Number));

                prs = prsName.Concat(prsNumber);
            }


            var prgd = from pg in _context.ProductGroup
                       where pg.ProductGroupId == id
                       select pg;
            var pg_desc_query = from pd in _context.ProductGroupEXP
                                where pd.ProductGroupIdref == id
                                select pd.pgexp;

            var pgingdesc_query = from pgindesc in _context.ProductGroupingexp
                                  select pgindesc;

            //var product_group_name = from pgn in _context.ProductGroup
            //                         where 

            //productGroup = await prgd.SingleOrDefaultAsync();
            proColViews = await prs.ToListAsync();
            colors = await _context.ColorCAT.ToListAsync();
            productGroupDescription = await pg_desc_query.FirstOrDefaultAsync();
            productGroupingexps = await pgingdesc_query.ToListAsync();

            productGroupName = _context.ProductGroup.Where(pg => pg.ProductGroupId == gID).Select(pg => pg.Expr1).FirstOrDefault();
        }

        public async Task<IActionResult> OnGetBadaneColor(int tikId)
        {
            var badane_query = from b in _context.ProductBadaneColCat
                               where b.ProductGroupIdREF == tikId
                               select b.colCATidREF;

            productBadaneColCat = await badane_query.ToListAsync();
            return new JsonResult(productBadaneColCat);
        }
        public async Task<IActionResult> OnGetNamaColor(int tikId)
        {
            var nama_query = from b in _context.ProductNamaColCat
                             where b.ProductGroupIdREF == tikId
                             select b.colCATidREF;

            productNamaColCat = await nama_query.ToListAsync();
            return new JsonResult(productNamaColCat);

        }
        public async Task<IActionResult> OnGetPayeColor(int tikId)
        {
            var paye_query = from b in _context.ProductPayeChoobiColCat
                             where b.ProductGroupIdREF == tikId
                             select b.colCATidREF;

            productPayeChoobiColCat = await paye_query.ToListAsync();
            return new JsonResult(productPayeChoobiColCat);
        }

        public async Task<IActionResult> OnPostSetPayeColor(bool isChecked, int colorId, long productId)
        {
            if (isChecked)
            {
                using (var connection = new QC.SqlConnection(
                "Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"
                ))
                {
                    connection.Open();
                    InsertRows(connection, productId, colorId, "dbo.ProductPayeChoobiColCat");
                    connection.Close();
                }
                return new JsonResult("adding to db...");

            }

            else //IsChecked is false
            {
                using (var connection = new QC.SqlConnection(
                "Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"
                ))
                {
                    connection.Open();
                    DeleteRows(connection, productId, colorId, "dbo.ProductPayeChoobiColCat");
                    connection.Close();
                }

                return new JsonResult("removing from db...");
            }
        }

        public async Task<IActionResult> OnPostSetNamaColor(bool isChecked, int colorId, long productId)
        {
            if (isChecked)
            {
                using (var connection = new QC.SqlConnection(
                "Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"
                ))
                {
                    connection.Open();
                    InsertRows(connection, productId, colorId, "dbo.ProductNamaColCat");
                    connection.Close();
                }
                return new JsonResult("adding to db...");

            }

            else //IsChecked is false
            {
                using (var connection = new QC.SqlConnection(
                "Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"
                ))
                {
                    connection.Open();
                    DeleteRows(connection, productId, colorId, "dbo.ProductNamaColCat");
                    connection.Close();
                }

                return new JsonResult("removing from db...");
            }
        }

        public async Task<IActionResult> OnPostSetBadaneColor(bool isChecked, int colorId, long productId)
        {
            if (isChecked)
            {
                using (var connection = new QC.SqlConnection(
                "Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"
                ))
                {
                    connection.Open();
                    InsertRows(connection, productId, colorId, "dbo.ProductBadaneColCat");
                    connection.Close();
                }
                return new JsonResult("adding to db...");

            }

            else //IsChecked is false
            {
                using (var connection = new QC.SqlConnection(
                "Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"
                ))
                {
                    connection.Open();
                    DeleteRows(connection, productId, colorId, "dbo.ProductBadaneColCat");
                    connection.Close();
                }

                return new JsonResult("removing from db...");
            }
        }

        public async Task<IActionResult> OnPostDescription(long id, string description)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"))
                {

                    string spName = @"dbo.[AddProductDescription]";
                    SqlCommand cmd = new SqlCommand(spName, conn);

                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@product_group_id";
                    param1.SqlDbType = SqlDbType.BigInt;
                    param1.Value = id;
   
                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@desc";
                    param2.SqlDbType = SqlDbType.NVarChar;
                    if(description == null)
                        param2.Value = DBNull.Value;
                    else
                        param2.Value = description;

                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param1);
                    
                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    //Console.WriteLine(Environment.NewLine + "Executing stored procedure..." + Environment.NewLine);
                    //Console.WriteLine("id: " + id + " description: " + description);
                    cmd.ExecuteNonQuery();

                    //Console.WriteLine(Environment.NewLine + "The stored procedure has been successfully executed." + Environment.NewLine);

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return new JsonResult("failure");
            }

            return new JsonResult("success");
        }
        public async Task<IActionResult> OnPostCurrent_pging_description(long id, string description)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"))
                {

                    string spName = @"dbo.[AddProductGroupingDescription]";
                    SqlCommand cmd = new SqlCommand(spName, conn);

                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@product_grouping_id";
                    param1.SqlDbType = SqlDbType.BigInt;
                    param1.Value = id;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@desc";
                    param2.SqlDbType = SqlDbType.NVarChar;
                    if (description == null)
                        param2.Value = DBNull.Value;
                    else
                        param2.Value = description;

                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param1);

                    conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    //Console.WriteLine(Environment.NewLine + "Executing stored procedure..." + Environment.NewLine);
                    //Console.WriteLine("id: " + id + " description: " + description);
                    cmd.ExecuteNonQuery();

                    //Console.WriteLine(Environment.NewLine + "The stored procedure has been successfully executed." + Environment.NewLine);

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return new JsonResult("failure");
            }

            return new JsonResult("success");
        }

        

        static public void InsertRows(QC.SqlConnection connection, long productId, int colorId, string tableName)
        {
            QC.SqlParameter parameter;

            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = $"insert into {tableName} (ProductGroupIdREF, colCATidREF)values (@productId, @colorId);";

                parameter = new QC.SqlParameter("@productId", DT.SqlDbType.BigInt);
                parameter.Value = productId;
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@colorId", DT.SqlDbType.Int);
                parameter.Value = colorId;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
        static public void DeleteRows(QC.SqlConnection connection, long productId, int colorId, string tableName)
        {
            QC.SqlParameter parameter;

            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = $"delete from {tableName} where ProductGroupIdREF = @productId and colCATidREF = @colorId;";

                parameter = new QC.SqlParameter("@productId", DT.SqlDbType.BigInt);
                parameter.Value = productId;
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@colorId", DT.SqlDbType.Int);
                parameter.Value = colorId;
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }
}
