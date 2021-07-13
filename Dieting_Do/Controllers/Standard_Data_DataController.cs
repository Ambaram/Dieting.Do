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
    public class Standard_Data_DataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Get the list of available standard food requirement data
        /// </summary>
        /// <returns>
        /// Header: 200(OK)
        /// Content : The standard food requirement data for every species present in the database
        /// </returns>
        // GET: api/Standard_Data_Data
        [HttpGet]
        public IEnumerable<Standard_Data> StandardList()
        {
            List<Standard_Data> St_Data = db.Standard_Data.ToList();
            List<Standard_Data> St_Dto = new List<Standard_Data>();
            St_Data.ForEach(s => St_Dto.Add(new Standard_Data()
            {
                SpeciesId = s.SpeciesId,
                SpeciesName = s.SpeciesName,
                St_Protein = s.St_Protein,
                St_Carbs = s.St_Carbs,
                St_Fat = s.St_Fat,
                St_Vitamin = s.St_Vitamin,
                St_Fibre = s.St_Fibre
            }));
            return St_Dto;
        }

        /// <summary>
        /// Find the standard food requirement of specific species.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Header : ResponseCode : 200(OK)
        /// Header : ResponseCode : 404 (Not Found)
        /// Content :Standard food requiremnt of the selected species. 
        /// </returns>
        // GET: api/Standard_Data_Data/FindStandard/5
        [ResponseType(typeof(Standard_Data))]
        [HttpGet]
        public IHttpActionResult FindStandard(int id)
        {
            Standard_Data standard_Data = db.Standard_Data.Find(id);

            if (standard_Data == null)
            {
                return NotFound();
            }

            return Ok(standard_Data);
        }

        /// <summary>
        /// Update the standard food requirement of a species.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="standard_Data"></param>
        /// <returns>Updated standard food requirement of selected species</returns>
        // Post: api/Standard_Data_Data/UpdataStandard/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateStandard(int id, Standard_Data standard_Data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != standard_Data.SpeciesId)
            {
                return BadRequest();
            }

            db.Entry(standard_Data).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Standard_DataExists(id))
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
        /// Add standard food requirement for a new species
        /// </summary>
        /// <param name="standard_Data"></param>
        /// <returns>New species food requirement data</returns>
        // POST: api/Standard_Data_Data/AddStandard
        [ResponseType(typeof(Standard_Data))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddStandard(Standard_Data standard_Data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Standard_Data.Add(standard_Data);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = standard_Data.SpeciesId }, standard_Data);
        }

        /// <summary>
        /// Deletes a species standard foor requirement data
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Header: ResponseCode: 200(OK)
        /// Header: ResponseCode: 404(Not Found)
        /// </returns>
        [ResponseType(typeof(Standard_Data))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteStandard_Data(int id)
        {
            Standard_Data standard_Data = db.Standard_Data.Find(id);
            if (standard_Data == null)
            {
                return NotFound();
            }

            db.Standard_Data.Remove(standard_Data);
            db.SaveChanges();

            return Ok(standard_Data);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Standard_DataExists(int id)
        {
            return db.Standard_Data.Count(e => e.SpeciesId == id) > 0;
        }
    }
}