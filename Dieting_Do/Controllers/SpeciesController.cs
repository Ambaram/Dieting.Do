using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Script.Serialization;
using Dieting_Do.Models;

namespace Dieting_Do.Controllers
{
    public class SpeciesController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static SpeciesController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44398/api/SpeciesData");
        }
        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        /// <summary>
        /// Communicate with species data controller web api to retrieve list of species present in the datatbase
        /// </summary>
        /// <returns>List of species</returns>
        /// <example>
        /// GET : Species/SpeciesList
        /// </example>
        public ActionResult ListSpecies()
        {
            string url = "ListSpecies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<Species> species = response.Content.ReadAsAsync<IEnumerable<Species>>().Result;

            return View(species);
        }
        public ActionResult Error()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// GET: Species/NewSpecies
        /// </example>
        public ActionResult NewSpecies()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        /// <example>
        /// POST: Species/Create
        /// </example>
        [HttpPost]
        public ActionResult AddSpecies(Species species)
        {
            string url = "AddSpecies";
            string jsonpayload = jss.Serialize(species);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListStandard");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <example>
        /// GET: Species/EditSpecies/5
        /// </example> 
        public ActionResult EditSpecies(int id)
        {
            string url = "FindSpecies" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SpeciesDto selectedspecies = response.Content.ReadAsAsync<SpeciesDto>().Result;
            return View(selectedspecies);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="species"></param>
        /// <returns></returns>
        /// <example>
        /// POST: Species/UpdateSpecies/5
        /// </example> 
        [HttpPost]
        public ActionResult UpdateSpecies(int id, Species species)
        {
            string url = "UpdateSpecies" + id;
            string jsonpayload = jss.Serialize(species);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListSpecies");
            }

            else
            {
                return RedirectToAction("Error");
            }
        }

        ///<summary>
        /// 
        ///</summary>
        ///<param name="id"></param>
        ///<returns>
        ///
        ///</returns>
        ///<example>
        ///GET: Species/DeleteConfirm/5
        ///</example> 
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindSpecies" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SpeciesDto selectedspecies = response.Content.ReadAsAsync<SpeciesDto>().Result;

            return View(selectedspecies);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="species"></param>
        /// <returns></returns>
        /// <example>
        /// POST: Species/DeleteSpecies/5
        /// </example> 
        [HttpPost]
        public ActionResult DeleteSpecies(int id, Species species)
        {
            string url = "DeleteSpecies" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return View("ListSpecies");
            }
            else
            {
                return View("Error");

            }
        }
    }
}