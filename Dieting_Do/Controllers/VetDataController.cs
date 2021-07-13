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
    public class VetDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Retrieve Vet data for all the vets available in the database
        /// </summary>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT : All Vets in the database
        /// </returns>
        /// <example>
        /// GET: api/VetData/ListVets
        /// </example>
        public IHttpActionResult ListVets()
        {
            List<Vet> Vets = db.Vets.ToList();
            List<VetDto> VetDto = new List<VetDto>();
            Vets.ForEach(v => VetDto.Add(new VetDto()
            {
                VetID = v.VetID,
                FirstName = v.FirstName,
                LastName = v.LastName,
                ClinicName = v.ClinicName,
                Location = v.Location,
                Phone = v.Phone
            }));
            return Ok(VetDto);
        }

        /// <summary>
        /// Find vet data for a specific vet id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT: Vet Data for requested vet id
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// GET: api/VetData/FindVet/5
        /// </example>
        [ResponseType(typeof(Vet))]
        public IHttpActionResult FindVet(int id)
        {
            Vet vet = db.Vets.Find(id);
            VetDto vetDto = new VetDto()
            {
                VetID = vet.VetID,
                FirstName = vet.FirstName,
                LastName = vet.LastName,
                Location = vet.Location,
                Phone = vet.Phone,
                ClinicName = vet.ClinicName
            };
            if (vet == null)
            {
                return NotFound();
            }

            return Ok(vetDto);
        }

        /// <summary>
        /// Update Vet data for a specific Vet id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vet"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// // POST: api/VetData/UpdateVet/5
        /// FORM DATA : Vet JSON object
        /// </example>
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateVet(int id, Vet vet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vet.VetID)
            {
                return BadRequest();
            }

            db.Entry(vet).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VetExists(id))
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
        /// Add Vet data to the database
        /// </summary>
        /// <param name="vet"></param>
        /// <returns>
        /// HEADER : StatusCode: 201(Created)
        /// CONTENT: VetID, Vet Data
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/VetData/AddVet
        /// FORM DATA : Vet JSON object
        /// </example
        [ResponseType(typeof(Vet))]
        public IHttpActionResult AddVet(Vet vet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Vets.Add(vet);
            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = vet.VetID }, vet);
        }
        /// <summary>
        /// Deletes vet data for a specific vet id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// POST: api/VetData/DeleteVet/5
        /// FORM DATA : empty
        /// </example>
        [ResponseType(typeof(Vet))]
        public IHttpActionResult DeleteVet(int id)
        {
            Vet vet = db.Vets.Find(id);
            if (vet == null)
            {
                return NotFound();
            }

            db.Vets.Remove(vet);
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

        private bool VetExists(int id)
        {
            return db.Vets.Count(e => e.VetID == id) > 0;
        }
    }
}