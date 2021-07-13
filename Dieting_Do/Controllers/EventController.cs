using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Dieting_Do.Models;
using System.Net.Http;

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
            client.BaseAddress = new Uri("https://localhost:44324/api/");
        }
        /// <summary>
        /// Communicates with the api to fetch event list from the database
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// GET: Event/EventList
        /// </example>
        public ActionResult ListEvents()
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
        public ActionResult NewEvent()
        {
            string url = "ShelterData/ListShelter";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ShelterDto> shelterist = response.Content.ReadAsAsync<IEnumerable<ShelterDto>>().Result;
            return View();
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
        /// 
        /// </example>
        [HttpPost]
        public ActionResult AddEvent(AnnualEvent annualEvent)
        {
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
        public ActionResult EditEvent(int id)
        {
            string url = "AnnualEventData/FindEvent" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnnualEventDto selectedEvent = response.Content.ReadAsAsync<AnnualEventDto>().Result;
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
        public ActionResult UpdateEvent(int id, AnnualEvent annualEvent)
        {
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
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="annualEvent"></param>
        /// <returns>
        /// Success: ACTION: Animal/EventList
        /// Failure: ACTION: Event/Error
        /// </returns>
        /// <example>
        /// GET: Event/DeleteEvent/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult DeleteEvent(int id, AnnualEvent annualEvent)
        {
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