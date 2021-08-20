using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dieting_Do.Models;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace Dieting_Do.Controllers
{
    public class VetController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static VetController()
        {
            HttpClientHandler handler = new HttpClientHandler() 
            { 
                AllowAutoRedirect = false, 
                UseCookies = false 
            };
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44398/api/");
        }
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
            //use it to pass along to the WebAPI
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        /// <summary>
        /// Communicate with api to retrieve the list of vets present in the database
        /// </summary>
        /// <returns>Returns a View assosiated with the action to show vet list</returns>
        ///<example>
        ///GET: Vet/ListVet
        ///</example>
        [HttpGet]
        public ActionResult VetList()
        {
            string url = "VetData/ListVets";
            HttpResponseMessage response = client.GetAsync(url).Result;
            VetDto vets = response.Content.ReadAsAsync<VetDto>().Result;
            return View(vets);
        }
        /// <summary>
        /// Displays an error if anything goes wrong while adding/updating/deleting vet data.
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// GET: Vet/VetError
        /// </example>
        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        // GET : Vet/NewVet
        public ActionResult NewVet()
        {
            return View();
        }

        /// <summary>
        /// Adds vet to the database
        /// </summary>
        /// <param name="vet"></param>
        /// <returns>
        /// Success : VIEW : Vet/VetList
        /// Failure : VIEW : Vet/Error
        /// </returns>
        /// <example>
        /// POST: Vet/AddVet
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult AddVet(Vet vet)
        {
            GetApplicationCookie(); //If authorized, Get token credentials
            // curl -H "Content-Type:application/json" -d @vet.json https://localhost:44398/api/VetData/AddVet
            string url = "VetData/AddVet";
            string jsonpayload = jss.Serialize(vet);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage respone = client.PostAsync(url, content).Result;
            if (respone.IsSuccessStatusCode)
            {
                return RedirectToAction("ListVet");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        ///<example>
        /// GET : Vet/EditVet/5
        ///</example>
        [HttpGet]
        [Authorize]
        public ActionResult EditVet(int id)
        {
            string url = "VetData/FindVet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            VetDto selectedvet = response.Content.ReadAsAsync<VetDto>().Result;
            return View(selectedvet);
        }

        /// <summary>
        /// Update selected Vet
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vet"></param>
        /// <returns>
        /// Success: VIEW: Vet/VetList
        /// Failure: VIEW: Vet/Error
        /// </returns>
        /// <example>
        /// POST: Vet/UpdateVet/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult UpdateVet(int id, Vet vet)
        {
            GetApplicationCookie();//If authorized, Get token credentials
            // curl -H "Content-Type:application/json" -d @vet.json https://localhost:44398/api/VetData/UpdateVet/{id}
            string url = "VetData/UpdateVet/" + id;
            string jsonpayload = jss.Serialize(vet);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("VetList");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET : Vet/DeleteConfirm/5
        [HttpGet]
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "VetData/FindVet/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            VetDto selectedvet = response.Content.ReadAsAsync<VetDto>().Result;
            return View(selectedvet);
        }

        /// <summary>
        /// Deletes vet from the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vet"></param>
        /// <returns>
        /// Success : VIEW: Vet/VetList
        /// Failure : VIEW : Vet/Error
        /// </returns>
        [HttpPost]
        [Authorize]
        public ActionResult DeleteVet(int id, Vet vet)
        {
            GetApplicationCookie(); ////If authorized, Get token credentials
            string url = "VetData/DeleteVet/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("VetList");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
