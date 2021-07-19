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
            //use it to pass along to the WebAPI.
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        /// <summary>
        /// Retrieve a list of shelters present in the database by communicating through ShelterData API
        /// </summary>
        /// <returns>List of shelters present in the database</returns>
        ///<example>GET : Shelter/ListShelter</example>
        [HttpGet]
        public ActionResult ShelterList()
        {
            ShelterAnimalSpecies ViewModel = new ShelterAnimalSpecies();
            // Fetch ShelterList
            string url = "ShelterData/ListShelter";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ShelterDto> shelterlist = response.Content.ReadAsAsync<IEnumerable<ShelterDto>>().Result;
            ViewModel.shelters = shelterlist;

            //Fetch AnimalList
            url = "AnimalData/ListAnimal";
            response = client.GetAsync(url).Result;
            IEnumerable<AnimalDto> animalslist = response.Content.ReadAsAsync<IEnumerable<AnimalDto>>().Result;
            ViewModel.animallist = animalslist;

            url = "Standard_Data_Data/StandardList";
            response = client.GetAsync(url).Result;
            IEnumerable<St_Dto> standardlist = response.Content.ReadAsAsync<IEnumerable<St_Dto>>().Result;
            ViewModel.standardlist = standardlist;

            return View(ViewModel);
        }

        // GET: Shelter/AddShelter
        [HttpGet]
        [Authorize]
        public ActionResult AddShelter()
        {
            string url = "AnimalData/ListAnimal";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<AnimalDto> animallist = response.Content.ReadAsAsync<IEnumerable<AnimalDto>>().Result;
            return View(animallist);
        }
        /// <summary>
        /// Add new shelter data to the database
        /// </summary>
        /// <param name="shelter"></param>
        /// <returns>
        /// Success : VIEW : Shelter/ShelterList
        /// Failure : VIEW : Shelter/Error
        /// </returns>
        // POST: Shelter/CreateShelter
        [HttpPost]
        [Authorize]
        public ActionResult CreateShelter(Shelter shelter)
        {
            GetApplicationCookie();
            string url = "ShelterData/AddShelter";
            string jsonpayload = Jss.Serialize(shelter);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ShelterList");
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
            string url = "ShelterData/FindShelter/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ShelterDto selectedshelter = response.Content.ReadAsAsync<ShelterDto>().Result;
            return View(selectedshelter);
        }
        /// <summary>
        /// Updates selected shelter data retrieved from Shelter/UpdateShelter/{shelterid}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shelter"></param>
        /// <returns>
        /// Success : VIEW : Shelter/ShelterList
        /// Failure : VIEW : Shelter/Error
        /// </returns>
        // POST: Shelter/EditShelter/5
        [HttpPost]
        [Authorize]
        public ActionResult EditShelter(int id, Shelter shelter)
        {
            GetApplicationCookie();
            string url = "ShelterData/UpdateShelter/"+id;
            string jsonpayload = Jss.Serialize(shelter);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ShelterList");
            }
            else
            {
                return RedirectToAction("Error");
            }


        }
        // GET: Shelter/DeleteShelter/5
        [HttpGet]
        [Authorize]
        public ActionResult DeleteSelter(int id)
        {
            string url = "ShelterData/FindShelter/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ShelterDto selectedshelter = response.Content.ReadAsAsync<ShelterDto>().Result;
            return View(selectedshelter);
        }
        /// <summary>
        /// Deletes selected shelter from the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shelter"></param>
        /// <returns>
        /// Success : VIEW : Shelter/ShelterList
        /// Failure : VIEW : Shelter/Error
        /// </returns
        // POST: Shelter/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, Shelter shelter)
        {
            GetApplicationCookie();
            string url = "ShelterData/DeleteShelter/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ShelterList");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}