using Microsoft.AspNetCore.Mvc;
using Intuit.Ipp.OAuth2PlatformClient;
using System.Security.Claims;
using System.Net;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Authentication;

namespace devexpress_demo.Controllers
{
    public class HomeController : Controller
    {
        public static OAuth2Client auth2Client;
        public static string clientid;
        public static string clientsecret;
        public static string redirectUrl;
        public static string environment;

        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            //var keysObj = this.configuration.GetSection("QboApiCredentials");
            clientid = configuration.GetSection("ClientId").Value!;
            clientsecret = configuration.GetSection("ClientSecret").Value!;
            redirectUrl = configuration.GetSection("RedirectUrl").Value!;
            environment = configuration.GetSection("AppEnvironment").Value!;

            auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);

        }
        // GET: HomeController
        public async Task<IActionResult> Index()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("Cookies");
            string AccessToken = Request.Cookies["AccessTokenResponse"] ?? "";
            if (AccessToken.Length > 0)
            {

                return RedirectToAction("Index", "Invoice");
            }
            else
            {
                return RedirectToAction("InitiateAuth", "Home", new { submitButton = "Connect to QuickBooks" });
            }
            return View();
        }
        public IActionResult InitiateAuth(string submitButton)
        {
            string AccessToken = Request.Cookies["AccessTokenResponse"] ?? "";
            if (AccessToken.Length > 0)
            {

                return RedirectToAction("Index", "Invoice");
            }
            switch (submitButton)
            {
                case "Connect to QuickBooks":
                    List<OidcScopes> scopes = new List<OidcScopes>();
                    scopes.Add(OidcScopes.Accounting);
                    string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
                    return Redirect(authorizeUrl);
                    break;
                default:
                    return (View());
            }
        }
        public ActionResult ApiCallService()
        {
            var session = HttpContext.Session.GetString("realmId");
            if (session != null && session != "")
            {
                string realmId = session.ToString();
                try
                {
                    var principal = User as ClaimsPrincipal;
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(principal.FindFirst("access_token")!.Value);

                    // Create a ServiceContext with Auth tokens and realmId
                    ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                    serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
                    // string baseUrl = "https://sandbox-quickbooks.api.intuit.com/"; // Sandbox base URL

                    // Create a QuickBooks QueryService using ServiceContext
                    //string companyInfoEndpoint = $"{baseUrl}/v3/company/{realmId}/companyinfo";

                    QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);
                    CompanyInfo companyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                    //CompanyInfo companyInfo = querySvc.ExecuteIdsQuery(companyInfoEndpoint).FirstOrDefault();

                    string output = "Company Name: " + companyInfo.CompanyName + " Company Address: " + companyInfo.CompanyAddr.Line1 + ", " + companyInfo.CompanyAddr.City + ", " + companyInfo.CompanyAddr.Country + " " + companyInfo.CompanyAddr.PostalCode;
                    return View("ApiCallService", (object)("QBO API call Successful!! Response: " + output));
                }
                catch (Exception ex)
                {
                    return View("ApiCallService", (object)("QBO API call Failed!" + " Error message: " + ex.Message));
                }
            }
            else
            {
                return RedirectToAction("InitiateAuth", "Home", new { submitButton = "Connect to QuickBooks" });
            }
            //return View("ApiCallService", (object)"QBO API call Failed!");
        }

        /// <summary>
        /// Use the Index page of App controller to get all endpoints from discovery url
        /// </summary>
        public ActionResult Error()
        {
            return View("Error");
        }

        /// <summary>
        /// Action that takes redirection from Callback URL
        /// </summary>
        public ActionResult Tokens()
        {
            string AccessToken = Request.Cookies["AccessTokenResponse"] ?? "";
            if (AccessToken.Length > 0)
            {

                return RedirectToAction("Index", "Invoice");
            }

            return View("Tokens");
        }

        // GET: HomeController/Details/5
        public ActionResult Details()
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
