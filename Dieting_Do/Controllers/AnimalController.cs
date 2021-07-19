using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Dieting_Do.Models;
using Dieting_Do.Models.ViewModels;
using System.Web.Script.Serialization;

namespace Dieting_Do.Controllers
{
    public class AnimalController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AnimalController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44398/api/");
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
        /// Communicate with animal data controller web api to retrieve list of animals present in the datatbase
        /// </summary>
        /// <returns>List of animals</returns>
        /// <example>
        /// GET : Animal/AnimalList
        /// </example>
        public ActionResult AnimalList()
        {

            string url = "AnimalData/ListAnimal";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<Animal> animals = response.Content.ReadAsAsync<IEnumerable<Animal>>().Result;

            return View(animals);
        }

        /// <summary>
        /// Retrieve specific animal's food requirement by communicating through web api  
        /// </summary>
        /// <param name="id"></param>
        /// <returns>VIEW: AnimalRequirement </returns>
        /// <example>
        /// Get : Animal/AnimalRequirement/5
        /// </example>
        [HttpGet]
        public ActionResult AnimalRequirement(int id)
        {
            AnimalRequirement ViewModel = new AnimalRequirement();

            // Find specific animal
            string url = "AnimalData/FindAnimal/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto SelectedAnimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            ViewModel.selectedanimal = SelectedAnimal;
            return View(ViewModel);

        }

        /// <summary>
        /// Communicate through web api to to add new animal to the database
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// Get : Animal/NewAnimal
        /// </example>
        [HttpGet]
        [Authorize]
        public ActionResult NewAnimal()
        {
            string url = "SpeciesData/ListSpecies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<SpeciesDto> speciesList = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;

            return View(speciesList);
        }

        // POST: Animal/AddAnimal
        [HttpPost]
        [Authorize]
        public ActionResult AddAnimal(Animal animal)
        {
            GetApplicationCookie();
            string url = "AnimalData/AddAnimal";

            string jsonpayload = jss.Serialize(animal);
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

        /// <summary>
        /// Displays the view to update an animal from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>VIEW: AnimalData/EditAnimal/{animal id}</returns>
        /// <example>
        /// GET : Animal/EditAnimal/5
        /// </example>
        [HttpGet]
        [Authorize]
        public ActionResult EditAnimal(int id)
        {
            UpdateAnimal ViewModel = new UpdateAnimal();
            // Find the animal to be updated.
            string url = "AnimalData/FindAnimal" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto Selectedanimal = response.Content.ReadAsAsync<AnimalDto>().Result;

            //List species by communicating through species Data API.
            url = "SpeciesData/ListSpecies";
            response = client.GetAsync(url).Result;
            IEnumerable<SpeciesDto> SpeciesList = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;
            ViewModel.selectedanimal = Selectedanimal;
            ViewModel.SpeciesList = SpeciesList;

            return View(ViewModel);
        }

        /// <example>
        /// POST : Animal/UpdateAnimal/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult UpdateAnimal(int id, Animal animal,HttpPostedFileBase AnimalPic)
        {
            GetApplicationCookie();
            string url = "AnimalData/UpdateAnimal" + id;
            string jsonpayload = jss.Serialize(animal);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode && AnimalPic!= null)
            {
                url = "AnimalData/UploadAnimalPic/" + id;
                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(AnimalPic.InputStream);
                requestcontent.Add(imagecontent, "AnimalPic", AnimalPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                return RedirectToAction("ListAnimal");
            }
            else if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAnimal");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Displays the view to delete an animal from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>VIEW : Animal/DeleteConfrim</returns>
        /// <example>
        /// GET : Animal/DeleteConfirm/5
        /// </example>
        [HttpGet]
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "AnimalData/FindAnimal" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto selectedanimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            return View(selectedanimal);
        }

        /// <summary>
        /// Deletes the requsted animal id as received from Animal/DeleteConfirm 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="animal"></param>
        /// <returns>
        /// POST : Empty request
        /// </returns>
        /// <example>
        /// POST : Animal/DeleteAnimal/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult DeleteAnimal(int id, Animal animal)
        {
            GetApplicationCookie();
            string url = "AnimalData/DeleteAnimal" + id;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAnimal");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}