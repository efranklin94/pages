using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;

namespace kamjaService.Pages.SalesInfo.sms
{
    public class smsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public smsModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        public IActionResult OnGetSend(string receptor)
        {
            try
            {
                //var sender = "10009003003300";
                //var sender = "982141725";
                //var receptor = "09379886607";
                
                var api = new Kavenegar.KavenegarApi("4654376F6D39673535334962524B49624C5850304333474552654E79644643683449556C564F665A416C4D3D");
                Random rnd = new Random();
                var result = api.VerifyLookup(receptor, rnd.Next().ToString(), "CatalogLink");
                return new JsonResult("پیامک ارسال گردید");
            }
            catch (Kavenegar.Exceptions.ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                Console.Write("Message : " + ex.Message);
                return new JsonResult(ex.Message);
            }
            catch (Kavenegar.Exceptions.HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                Console.Write("Message : " + ex.Message);
                return new JsonResult(ex.Message);

            }


        }
    }
}
