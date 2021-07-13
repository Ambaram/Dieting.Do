using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Dieting_Do.Models;

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
            return Ok();
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
        [ResponseType(typeof(Animal))]
        public IHttpActionResult ListAnimals(int id)
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
        public IHttpActionResult DeleteAnimal(int id)
        {
            Animal animal = db.Animal.Find(id);
            if (animal == null)
            {
                return NotFound();
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