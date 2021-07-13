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
    public class SpeciesDataController : ApiController
    {
        /// <summary>
        /// Retrieve species data for all the species available in the database
        /// </summary>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT : All Species in the database
        /// </returns>
        /// <example>
        /// GET: api/SpeciesData/ListSpecies
        /// </example>
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpGet]
        public IHttpActionResult ListSpecies()
        {
            List<Species> Species = db.Species.ToList();
            List<SpeciesDto> SpeciesDto = new List<SpeciesDto>();
            Species.ForEach(s => SpeciesDto.Add(new SpeciesDto()
            {
                SpeciesId = s.SpeciesId,
                AnimalSpecies = s.AnimalSpecies
            }));
            return Ok(SpeciesDto);
        }

        /// <summary>
        /// Find species data for a specific species id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT: Species Data for requested species id
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// GET: api/SpeciesData/FindSpecies/5
        /// </example>
        [ResponseType(typeof(Species))]
        public IHttpActionResult FindSpecies(int id)
        {
            Species species = db.Species.Find(id);
            SpeciesDto speciesDto = new SpeciesDto()
            {
                SpeciesId = species.SpeciesId,
                AnimalSpecies = species.AnimalSpecies
            };

            if (species == null)
            {
                return NotFound();
            }

            return Ok(speciesDto);
        }

        /// <summary>
        /// Update species data for a specific species id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="species"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// // POST: api/SpeciesData/UpdateSpecies/5
        /// FORM DATA : Species JSON object
        /// </example>
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateSpecies(int id, Species species)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != species.SpeciesId)
            {
                return BadRequest();
            }

            db.Entry(species).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesExists(id))
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
        /// Add species data to the database
        /// </summary>
        /// <param name="species"></param>
        /// <returns>
        /// HEADER : StatusCode: 201(Created)
        /// CONTENT: SpeciesID, Species Data
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ShelterData/AddSpecies
        /// FORM DATA : Species JSON object
        /// </example>
        [ResponseType(typeof(Species))]
        public IHttpActionResult AddSpecies(Species species)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Species.Add(species);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = species.SpeciesId }, species);
        }

        /// <summary>
        /// Deletes species data for a specific species id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// POST: api/SpeciesData/DeleteSpecies/5
        /// FORM DATA : empty
        /// </example>
        [ResponseType(typeof(Species))]
        public IHttpActionResult DeleteSpecies(int id)
        {
            Species species = db.Species.Find(id);
            if (species == null)
            {
                return NotFound();
            }

            db.Species.Remove(species);
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

        private bool SpeciesExists(int id)
        {
            return db.Species.Count(e => e.SpeciesId == id) > 0;
        }
    }
}