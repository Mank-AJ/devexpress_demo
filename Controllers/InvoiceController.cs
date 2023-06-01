using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using static devexpress_demo.Models.InvoiceModel;
//using Document = System.Reflection.Metadata.Document;
//using iTextSharp.text;
//using iTextSharp.text.html.simpleparser;
//using iTextSharp.text.pdf;
//using System.IO;


namespace QBOApiCallDemo.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: InvoiceController
        public static int minorversion=65;
        public static long CompanyId= 4620816365303325910;
        public async Task<ActionResult> Index()
        {
          
            return View();
        }
        public async Task<JsonResult> dxDataGrid_List()
        {
            string AccessToken = Request.Cookies["AccessTokenResponse"];
            if (AccessToken != null && AccessToken != "")
            {
                var options = new RestClientOptions("https://sandbox-quickbooks.api.intuit.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/v3/company/" + CompanyId + "/query?minorversion=" + minorversion, Method.Post);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/text");
                request.AddHeader("Authorization", "Bearer " + AccessToken);
                var body = @"select * from invoice";
                request.AddParameter("application/text", body, ParameterType.RequestBody);
                RestResponse response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var responseData = response.Content;
                    Root InvoiceResponse = JsonConvert.DeserializeObject<Root>(responseData);

                    var invoiceData = InvoiceResponse.QueryResponse.Invoice;
                    return Json(invoiceData);
                 
                }
                else
                {
                    Response.Cookies.Append("AccessTokenResponse", "");
                    return Json(new { errorMessage = response.ErrorMessage });
                }
            }
            return Json(null);
        }

        public async Task<ActionResult> Details(int? rowcount)
        {
            string AccessToken = Request.Cookies["AccessTokenResponse"];
            if (AccessToken != null && AccessToken != "")
            {
                var options = new RestClientOptions("https://sandbox-quickbooks.api.intuit.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/v3/company/" + CompanyId + "/query?minorversion=" + minorversion, Method.Post);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/text");
                request.AddHeader("Authorization", "Bearer " + AccessToken);
                var body = @"select * from invoice startposition 1 maxresults 5";
                request.AddParameter("application/text", body, ParameterType.RequestBody);
                RestResponse response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var responseData = response.Content;
                    Root InvoiceResponse = JsonConvert.DeserializeObject<Root>(responseData);
                    InvoiceResponse.rowcount = rowcount;
                    return View(InvoiceResponse);
                }
                else
                {
                    Response.Cookies.Append("AccessTokenResponse", "");
                    return RedirectToAction("InitiateAuth", "Home", new { submitButton = "Connect to QuickBooks" });
                    string errorMessage = response.ErrorMessage;
                }
            }
            return View(null);
        }

    
        public ActionResult LogOut()
        {
            Response.Cookies.Append("AccessTokenResponse", "");
            return RedirectToAction("Index", "Home");
        }
}
}
