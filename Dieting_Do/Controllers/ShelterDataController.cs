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
    public class ShelterDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Retrieve shelter data for all the shelters available in the database
        /// </summary>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT : All shelters in the database
        /// </returns>
        /// <example>
        /// GET: api/ShelterData/ListSheter
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ShelterDto))]
        public IHttpActionResult ListShelter()
        {
            List<Shelter> Shelters = db.Shelters.ToList();
            List<ShelterDto> sheltersDto = new List<ShelterDto>();
            Shelters.ForEach(s => sheltersDto.Add(new ShelterDto()
            {
                ShelterID = s.ShelterID,
                ShelterName = s.ShelterName,
                Address = s.Address,
                Phone = s.Phone,
                OwnerName = s.OwnerName,
            }));
            return Ok(sheltersDto);
        }

        /// <summary>
        /// Find shelter data for a specific shelter id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT: Shelter Data for requested shelter id
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// GET: api/ShelterData/FindSheter/5
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ShelterDto))]
        public IHttpActionResult FindShelter(int id)
        {
            Shelter shelter = db.Shelters.Find(id);
            ShelterDto sheltersDto = new ShelterDto()
            {
                ShelterID = shelter.ShelterID,
                ShelterName = shelter.ShelterName,
                OwnerName = shelter.OwnerName,
                Address = shelter.Address,
                Phone = shelter.Phone
            };
            if (shelter == null)
            {
                return NotFound();
            }

            return Ok(sheltersDto);
        }
        /// <summary>
        /// Update shelter data for a specific shelter id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shelter"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// // POST: api/ShelterData/UpdateSheter/5
        /// FORM DATA : Shelter JSON object
        /// </example>

        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateShelter(int id, Shelter shelter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shelter.ShelterID)
            {
                return BadRequest();
            }

            db.Entry(shelter).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShelterExists(id))
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
        /// Add shelter data to the database
        /// </summary>
        /// <param name="shelter"></param>
        /// <returns>
        /// HEADER : StatusCode: 201(Created)
        /// CONTENT: ShelterID, Shelter Data
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ShelterData/AddShelter
        /// FORM DATA : Shelter JSON object
        /// </example>
        [ResponseType(typeof(Shelter))]
        public IHttpActionResult AddShelter(Shelter shelter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Shelters.Add(shelter);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = shelter.ShelterID }, shelter);
        }

        /// <summary>
        /// Deletes shelter data for a specific shelter id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// POST: api/ShelterData/DeleteSheter/5
        /// FORM DATA : empty
        /// </example>
        [ResponseType(typeof(Shelter))]
        public IHttpActionResult DeleteShelter(int id)
        {
            Shelter shelter = db.Shelters.Find(id);
            if (shelter == null)
            {
                return NotFound();
            }

            db.Shelters.Remove(shelter);
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

        private bool ShelterExists(int id)
        {
            return db.Shelters.Count(e => e.ShelterID == id) > 0;
        }
    }
}