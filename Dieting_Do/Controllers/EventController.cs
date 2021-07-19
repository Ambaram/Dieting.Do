using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Dieting_Do.Models;
using System.Net.Http;
using Dieting_Do.Models.ViewModels;

namespace Dieting_Do.Controllers
{
    public class AnnualEventController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AnnualEventController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
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
        /// Communicates with the api to fetch event list from the database
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// GET: Event/EventList
        /// </example>
        [HttpGet]
        public ActionResult EventList()
        {
            string url = "AnnualEventData/ListEvents";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<AnnualEvent> annualEvents = response.Content.ReadAsAsync<IEnumerable<AnnualEvent>>().Result;
            return View(annualEvents);
        }

        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Communicates with the api to add new event to the database
        /// </summary>
        /// <example>
        /// GET : Event/NewEvent
        /// </example>
        [HttpGet]
        [Authorize]
        public ActionResult NewEvent()
        {
            string url = "ShelterData/ListShelter";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ShelterDto> shelterist = response.Content.ReadAsAsync<IEnumerable<ShelterDto>>().Result;
            return View(shelterist);
        }

        /// <summary>
        /// Adds event to the database
        /// </summary>
        ///<param name="annualEvent"></param>
        /// <returns>
        /// SUCCESS: VIEW: Event/EventList
        /// FAILURE: VIEW: Event/Error 
        /// </returns>
        /// <example>
        /// POST : Event/AddEvent
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult AddEvent(AnnualEvent annualEvent)
        {
            GetApplicationCookie();
            string url = "AnnualEventData/AddEvent";
            string jsonpayload = jss.Serialize(annualEvent);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("EventList");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Communicates with  the api to update the selected event
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// VIEW: Event/UpdateEvent/5
        /// </returns>
        /// <example>
        /// GET : AnnualEvent/EditEvent/5
        /// </example>
        [HttpGet]
        [Authorize]
        public ActionResult EditEvent(int id)
        {
            EventLocation ViewModel = new EventLocation();
            string url = "AnnualEventData/FindEvent" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnnualEventDto selectedEvent = response.Content.ReadAsAsync<AnnualEventDto>().Result;
            ViewModel.selectedevent = selectedEvent;

            url = "ShelterData/ListShelter";
            response = client.GetAsync(url).Result;
            IEnumerable<ShelterDto> shelterlist = response.Content.ReadAsAsync<IEnumerable<ShelterDto>>().Result;
            ViewModel.shelterlist = shelterlist;
            return View(selectedEvent);
        }

        /// <summary>
        /// Updates requested event data.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="annualEvent"></param>
        /// <returns>
        /// SUCCESS: VIEW: Event/EventList
        /// FAILURE: VIEW: Shared/Error
        /// </returns>
        /// <example>
        /// POST : Event/UpdateEvent/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult UpdateEvent(int id, AnnualEvent annualEvent)
        {
            GetApplicationCookie();
            string url = "AnnualEventData/UpdateEvent" + id;
            string jsonpayload = jss.Serialize(annualEvent);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("EventList");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        /// Communicates with the api to delete selected animal from the database
        /// </summary>
        /// <param name="id"></param>
        /// <example>
        /// GET : Event/DeleteConfirm/5
        /// </example>
        [HttpGet]
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "AnnualEventData/FindEvent" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnnualEventDto selectedevent = response.Content.ReadAsAsync<AnnualEventDto>().Result;
            return View(selectedevent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Success: ACTION: Animal/EventList
        /// Failure: ACTION: Event/Error
        /// </returns>
        /// <example>
        /// GET: Event/DeleteEvent/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult DeleteEvent(int id)
        {
            GetApplicationCookie();
            string url = "AnnualEventData/DeleteEvent" + id;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListEvents");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}