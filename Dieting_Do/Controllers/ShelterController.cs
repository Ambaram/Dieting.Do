using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Script.Serialization;
using Dieting_Do.Models;
using Dieting_Do.Models.ViewModels;

namespace Dieting_Do.Controllers
{
    public class ShelterController : Controller
    {
        private static readonly HttpClient client;
        private static JavaScriptSerializer Jss = new JavaScriptSerializer();

        static ShelterController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://44324/api/ShelterData/");
        }
        /// <summary>
        /// Retrieve a list of shelters present in the database by communicating through ShelterData API
        /// </summary>
        /// <returns>List of shelters present in the database</returns>
        ///<example>GET : Shelter/ListShelter</example>
        public ActionResult ShelterList()
        {
            string url = "ListShelter";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ShelterDto> Shelters = response.Content.ReadAsAsync<IEnumerable<ShelterDto>>().Result;
            return View(Shelters);
        }

        // GET: Shelter/AddShelter
        [HttpGet]
        [Authorize]
        public ActionResult AddShelter()
        {
            return View();
        }

        // POST: Shelter/CreateShelter
        [HttpPost]
        [Authorize]
        public ActionResult CreateShelter(Shelter shelter)
        {
            string url = "AddShelter";
            string jsonpayload = Jss.Serialize(shelter);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListShelter");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        [Authorize]
        // GET: Shelter/UpdateShelter/5
        public ActionResult UpdateShelter(int id)
        {
            return View();
        }

        // POST: Shelter/UpdateShelter/5
        [HttpPost]
        [Authorize]
        public ActionResult EditShelter(int id, Shelter shelter)
        {
            string url = "ShelterData/UpdateShelter";
            string jsonpayload = Jss.Serialize(shelter);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListShelter");
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
        // GET: Shelter/DeleteShelter/5
        [HttpGet]
        [Authorize]
        public ActionResult DeleteSelter(int id)
        {
            string url = "findshelter/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ShelterDto selectedshelter = response.Content.ReadAsAsync<ShelterDto>().Result;
            return View(selectedshelter);
        }

        // POST: Shelter/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            string url = "deleteshelter" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListShelter");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}