using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleApp.Services;

namespace SampleApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger _logger;

        public string resultToPrint;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var x2 = Program._host;
            var x1 = Program._host.Services;
            var x3 = Program._host.Services.GetService(typeof(ServiceA));
            var a = Program._sa;
            var _hostedServices = Program._hostedServicesA;

           


            var receivedReqId=Request.Cookies["ReqId"];
            if (receivedReqId == null)
            {
                ServiceA sa = (ServiceA)Program._hostedServicesA;
                String reqID = Guid.NewGuid().ToString();

                var result= sa.Do(reqID);
                if (result.Key == false)
                {
                    Response.Cookies.Append("ReqId", reqID);
                    resultToPrint = "please wait to praper data  ... ";
                }
                else
                {
                    resultToPrint = result.Value.ToString();
                }
            }
            else
            {
                ServiceA sa = (ServiceA)Program._hostedServicesA;

                var result = sa.Do(receivedReqId);
                if (result.Key != false) 
                {
                    Response.Cookies.Delete("ReqId");
                    resultToPrint = result.Value.ToString();
                }
                else
                {
                    resultToPrint = "please wait to praper data  ... ";
                }
                //esle continue
            }
            _logger.LogInformation("Logged from the IndexModel.OnGet method at {Time}", DateTimeOffset.Now);
        }
    }
}
