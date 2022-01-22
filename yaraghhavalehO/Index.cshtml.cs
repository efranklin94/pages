using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using kamjaService.Models;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;

namespace kamjaService.Pages.yaraghhavalehO
{
    public class IndexModel : PageModel
    {
        private readonly kamjaService.Models.KamjaServiceDbContext _context;

        public IndexModel(kamjaService.Models.KamjaServiceDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _environment = hostingEnvironment;
        }

        public IList<YaraghHavaleOrder> YaraghHavaleOrders { get; set; }
        public string havaleCode { get; set; }
        [BindProperty]
        public YaraghHavaleOrder yaraghHavaleOrder { get; set; }
        public string yaraghHavaleDesc { get; set; }
        private readonly IHostingEnvironment _environment;
        public IList<string> imagesNames { get; set; }
        public IList<HavaleFullView> havaleFulls { get; set; }
        public YaraghHavaleImages YaraghHavaleImage { get; set; }

        public async Task OnGetAsync(string searchString)
        {

            havaleCode = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {

                if (isPersianNumber(searchString))
                {
                    searchString = changePersianNumbersToEnglish(searchString);
                    havaleCode = searchString;

                }

                IQueryable<YaraghHavaleOrder> custQuery =
                    from havale in _context.YaraghHavaleorder
                    where havale.OrderNumber == searchString
                    select havale;

                var havales = custQuery.Select(x => new YaraghHavaleOrder
                {
                    OrderNumber = x.OrderNumber,
                    ID = x.ID,
                    product_name = x.product_name,
                    Number = x.Number,
                    formula_azja = x.formula_azja,
                    kala_code = x.kala_code,
                    kala_name = x.kala_name,
                    ajza_code = x.ajza_code,
                    ajza_name = x.ajza_name,
                    PartID = x.PartID,
                    ProductID1 = x.ProductID1,
                    partid1 = x.partid1,
                    tedad = x.tedad,
                    exp_order = x.exp_order,
                    exp_havale = x.exp_havale,
                    OrderRef = x.OrderRef,
                    OrderSalesOfficeName = x.OrderSalesOfficeName,
                    OrderCustomerName = x.OrderCustomerName,
                    OrderOneTimeCustomerName = x.OrderOneTimeCustomerName,
                    tik = x.tik

                }).Distinct().OrderBy(x => x.OrderNumber);

                YaraghHavaleOrders = await havales.ToListAsync();

                var yhd_query = from yhe in _context.YaraghHavaleOrderEXP
                                where yhe.OrderNumberRef == long.Parse(havaleCode)
                                select yhe.Pexp;
                yaraghHavaleDesc = await yhd_query.FirstOrDefaultAsync();

                var yaragh_havale_images_query = from yhi in _context.YaraghHavaleImages
                                                 where yhi.OrderNumber == havaleCode
                                                 select yhi.ImageName;
                imagesNames = await yaragh_havale_images_query.ToListAsync();

                //var havalefull_query = _context.HavaleFullView.Where(yh => yh.OrderNumber == havaleCode);
                var havalefull_query = _context.HavaleFullView.Select(
                    x => new HavaleFullView
                    {
                        deliverer = x.deliverer,
                        description = x.description,
                        from_hour = x.from_hour,
                        installation_or_delivery_date = x.installation_or_delivery_date,
                        installer = x.installer,
                        OrderCustomerName = x.OrderCustomerName,
                        OrderCustomerNumber = x.OrderCustomerNumber,
                        OrderDate = x.OrderDate,
                        OrderItemDescription = x.OrderItemDescription,
                        OrderItemProductName = x.OrderItemProductName,
                        OrderItemProductNumber = x.OrderItemProductNumber,
                        OrderItemQuantity = x.OrderItemQuantity,
                        OrderItemRowNumber = x.OrderItemRowNumber,
                        OrderItemSourceNumber = x.OrderItemSourceNumber,
                        OrderItemSourceType = x.OrderItemSourceType,
                        OrderNumber = x.OrderNumber,
                        OrderOneTimeCustomerName = x.OrderOneTimeCustomerName,
                        OrderOneTimeCustomerNumber = x.OrderOneTimeCustomerNumber,
                        OrderSalesOfficeName = x.OrderSalesOfficeName,
                        to_hour = x.to_hour,
                        tf1 = x.tf1,
                        tf2 = x.tf2
                    }).Distinct().Where(x => x.OrderNumber == havaleCode).OrderBy(x => x.OrderItemRowNumber);

                //foreach (var yaraghhavalefull in havalefull_query)
                //{
                //    Console.WriteLine(Environment.NewLine + yaraghhavalefull.OrderItemProductNumber);
                //}

                havaleFulls = await havalefull_query.ToListAsync();
            
            }
            else
            {
                //Do Nothing and return nothing to the table  
                
            }

        }


        public async Task<IActionResult> OnPostCheckbox(bool checkboxValue, int id)
        {
            yaraghHavaleOrder = await _context.YaraghHavaleorder.FirstOrDefaultAsync(m => m.ID == id);

            _context.Attach(yaraghHavaleOrder).State = EntityState.Modified;

            yaraghHavaleOrder.tik = checkboxValue;
            await _context.SaveChangesAsync();

            return new JsonResult(checkboxValue);
        }

        public async Task<IActionResult> OnGetCheckbox(bool checkboxValue, int id)
        {
            yaraghHavaleOrder = await _context.YaraghHavaleorder.FirstOrDefaultAsync(m => m.ID == id);

            _context.Attach(yaraghHavaleOrder).State = EntityState.Modified;

            yaraghHavaleOrder.tik = checkboxValue;
            await _context.SaveChangesAsync();

            return new JsonResult(checkboxValue);
        }

        public async Task<IActionResult> OnPostUpdate(string havale_code)
        {
            if (havale_code != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection("Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"))
                    {

                        string spName = @"dbo.[updateYaraghHavaleOrder]";
                        SqlCommand cmd = new SqlCommand(spName, conn);

                        SqlParameter param1 = new SqlParameter();
                        param1.ParameterName = "@OrderNumber";
                        param1.SqlDbType = SqlDbType.NVarChar;
                        param1.Value = havale_code;
                        cmd.Parameters.Add(param1);

                        conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        //Console.WriteLine(Environment.NewLine + "Executing stored procedure..." + Environment.NewLine);

                        cmd.ExecuteNonQuery();

                        //Console.WriteLine(Environment.NewLine + "The stored procedure has been successfully executed." + Environment.NewLine);

                        conn.Close();
                    }

                    ///////
                    havaleCode = havale_code;
                    if (!String.IsNullOrEmpty(havale_code))
                    {
                        IQueryable<YaraghHavaleOrder> custQuery =
                            from havale in _context.YaraghHavaleorder
                            where havale.OrderNumber == havale_code
                            select havale;

                        var havales = custQuery.Select(x => new YaraghHavaleOrder
                        {
                            OrderNumber = x.OrderNumber,
                            ID = x.ID,
                            product_name = x.product_name,
                            Number = x.Number,
                            formula_azja = x.formula_azja,
                            kala_code = x.kala_code,
                            kala_name = x.kala_name,
                            ajza_code = x.ajza_code,
                            ajza_name = x.ajza_name,
                            PartID = x.PartID,
                            ProductID1 = x.ProductID1,
                            partid1 = x.partid1,
                            tedad = x.tedad,
                            exp_order = x.exp_order,
                            exp_havale = x.exp_havale,
                            OrderRef = x.OrderRef,
                            OrderSalesOfficeName = x.OrderSalesOfficeName,
                            OrderCustomerName = x.OrderCustomerName,
                            OrderOneTimeCustomerName = x.OrderOneTimeCustomerName

                        }).Distinct().OrderBy(x => x.OrderNumber);

                        YaraghHavaleOrders = await havales.ToListAsync();
                    }
                    else
                    {
                        //Do Nothing and return nothing to the table  
                    }
                    ///////

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                    return new JsonResult("failure");
                }

                return new JsonResult("success");
            }

            return new JsonResult("failure");
        }

        public async Task<IActionResult> OnPostDescription(long id, string description)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"))
                {

                    string spName = @"dbo.[AddYaraghHavaleDescription]";
                    SqlCommand cmd = new SqlCommand(spName, conn);

                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@havale_code";
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

        public IActionResult OnPostCapture(string name)
        {
            var files = HttpContext.Request.Form.Files;

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Getting Filename  
                        var fileName = file.FileName;
                        // Unique filename "Guid"  
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                        // Getting Extension  
                        var fileExtension = Path.GetExtension(fileName);
                        // Concating filename + fileExtension (unique filename)  
                        var newFileName = string.Concat(myUniqueFileName, fileExtension);

                        var havale_code = Request.Form["havaleCode"].ToString();
                        //Create folder if does not exist
                        var dir = Path.Combine(_environment.WebRootPath, "technical/YaraghHavaleImages");
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        //  Generating Path to store photo   
                        var filepath = dir + $@"\{newFileName}";
                        Console.WriteLine(Environment.NewLine + "file path captured : " + filepath );
                        
                        if (!string.IsNullOrEmpty(filepath))
                        {
                            // Storing Image in Folder  
                            StoreInFolder(file, filepath);
                        }

                        /*
                         * inserting images data into database
                         */
                        string commandText = "INSERT YaraghHavaleImages (ImageName, OrderNumber) VALUES (@fileName, @havaleCode) ";

                        using (SqlConnection connection = new SqlConnection("Server=172.16.231.14;Database=KamjaServiceDb;User Id=sa;Password=h0$t@2019;MultipleActiveResultSets=true"))
                        {
                            SqlCommand command = new SqlCommand(commandText, connection);
                            command.Parameters.Add("@fileName", SqlDbType.NVarChar);
                            command.Parameters["@fileName"].Value = myUniqueFileName;
                            command.Parameters.Add("@havaleCode", SqlDbType.NVarChar);
                            command.Parameters["@havaleCode"].Value = havale_code;
                            try
                            {
                                connection.Open();
                                Int32 rowsAffected = command.ExecuteNonQuery();
                                //Console.WriteLine("RowsAffected: {0}", rowsAffected);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        /*
                         */

                    }
                }
                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }

        }

        /// <summary>  
        /// Saving captured image into Folder.  
        /// </summary>  
        /// <param name="file"></param>  
        /// <param name="fileName"></param>  
        private void StoreInFolder(IFormFile file, string fileName)
        {
            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }

        public async Task<IActionResult> OnPostDeleteHavaleImage(string code, string imageName)
        {
                YaraghHavaleImage = await _context.YaraghHavaleImages.FirstOrDefaultAsync(m => m.ImageName == imageName);

                _context.Attach(YaraghHavaleImage).State = EntityState.Deleted;
                var res = await _context.SaveChangesAsync();

                return new JsonResult(res);
        }

        public bool isPersianNumber(string number) { 
            Regex regex = new Regex("[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]");
            return regex.IsMatch(number);
        }
        private string changePersianNumbersToEnglish(string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(persian[j], j.ToString());

            return input;
        }
    }
}
