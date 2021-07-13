using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dieting_Do.Models;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace Dieting_Do.Controllers
{
    public class VetController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static VetController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44324/api/VetData/");
        }
        /// <summary>
        /// Communicate with api to retrieve the list of vets present in the database
        /// </summary>
        /// <returns>Returns a View assosiated with the action to show vet list</returns>
        ///<example>
        ///GET: Vet/ListVet
        ///</example>
        public ActionResult VetList()
        {
            string url = "ListVet";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<VetDto> animals = response.Content.ReadAsAsync<IEnumerable<VetDto>>().Result;
            return View(animals);
        }
        /// <summary>
        /// Displays an error if anything goes wrong while adding/updating/deleting vet data.
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// GET: Vet/VetError
        /// </example>
        public ActionResult Error()
        {
            return View();
        }
        // GET: Vet/NewVet
        [HttpGet]
        [Authorize]
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
            string url = "AddVet";
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
            string url = "FindVet" + id;
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
            string url = "UpdateVet" + id;
            string jsonpayload = jss.Serialize(vet);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListVet");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET : Vet/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindVet" + id;
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
        public ActionResult DeleteVet(int id, Vet vet)
        {
            string url = "DeleteVet" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListVet");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
