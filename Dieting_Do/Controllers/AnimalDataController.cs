using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Dieting_Do.Models;
using System.Diagnostics;
using System.IO;

namespace Dieting_Do.Controllers
{
    public class AnimalDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Retrieve event data for all the animals available in the database
        /// </summary>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT : All Animals in the database
        /// </returns>
        /// <example>
        /// GET: api/AnimalData/ListAnimal
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AnimalDto))]
        public IHttpActionResult ListAnimal()
        {
            List<Animal> animals = db.Animal.ToList();
            List<AnimalDto> animalDtos = new List<AnimalDto>();
            animals.ForEach(a => animalDtos.Add(new AnimalDto()
            {
                AnimalId = a.AnimalId,
                AnimalHeight = a.AnimalHeight,
                AnimalWeight = a.AnimalWeight,
                AnimalName = a.AnimalName

            }));
            return Ok(animalDtos);
        }

        /// <summary>
        /// Find animal data for a specific animal id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT: Event Data for requested animal id
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// GET: api/AnimalData/FindAnimal/5
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AnimalDto))]
        public IHttpActionResult FindAnimal(int id)
        {
            Animal animal = db.Animal.Find(id);
            AnimalDto animalDto = new AnimalDto()
            {
                AnimalId = animal.AnimalId,
                AnimalName = animal.AnimalName,
                AnimalHeight = animal.AnimalHeight,
                AnimalWeight = animal.AnimalWeight
            };

            if (animal == null)
            {
                return NotFound();
            }

            return Ok(animalDto);
        }

        /// <summary>
        /// Update animal data for a specific animal id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="animal"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// // POST: api/AnimalData/UpdateAnimal/5
        /// FORM DATA : Animal JSON object
        /// </example>
        [HttpPost]
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateAnimal(int id, Animal animal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != animal.AnimalId)
            {
                return BadRequest();
            }

            db.Entry(animal).State = EntityState.Modified;
            db.Entry(animal).Property(a => a.animalwithpic).IsModified = false;
            db.Entry(animal).Property(a => a.picformat).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public IHttpActionResult UploadAnimalPic(int id)
        {
            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Multipart data received");
                int files = HttpContext.Current.Request.Files.Count;
                if (files == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var animalpic = HttpContext.Current.Request.Files[0];
                    if(animalpic.ContentLength > 0)
                    {
                        var validtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var format = Path.GetExtension(animalpic.FileName).Substring(1);
                        if (validtypes.Contains(format))
                        {
                            try
                            {
                                string fn = id + "." + format;
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/"), fn);
                                animalpic.SaveAs(path);
                                haspic = true;
                                picextension = format;

                                Animal SelectedAnimal = db.Animal.Find(id);
                                SelectedAnimal.animalwithpic = haspic;
                                SelectedAnimal.picformat = format;
                                db.Entry(SelectedAnimal).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Image not saved successfully!!");
                                Debug.WriteLine("Exception: " + ex);
                                return BadRequest();
                            }
                        }
                    }
                }
                return Ok(); 
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Add Animal data to the database
        /// </summary>
        /// <param name="animal"></param>
        /// <returns>
        /// HEADER : StatusCode: 201(Created)
        /// CONTENT: AnnualEventID, Event Data
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AnimalData/AddAnimal
        /// FORM DATA : Animal JSON object
        /// </example>
        [ResponseType(typeof(Animal))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddAnimal(Animal animal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Animal.Add(animal);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = animal.AnimalId }, animal);
        }

        /// <summary>
        /// Deletes animal data for a specific animal id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// POST: api/AnimalData/DeleteAnimal/5
        /// FORM DATA : empty
        /// </example>
        [ResponseType(typeof(Animal))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteAnimal(int id)
        {
            Animal animal = db.Animal.Find(id);
            if (animal == null)
            {
                return NotFound();
            }
            // Delete AnimalPic
            if(animal.animalwithpic && animal.picformat != null)
            {
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/" + id + "." + animal.picformat);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            db.Animal.Remove(animal);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AnimalExists(int id)
        {
            return db.Animal.Count(e => e.AnimalId == id) > 0;
        }
    }
}