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
    public class AnnualEventDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Retrieve event data for all the events available in the database
        /// </summary>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT : All Events in the database
        /// </returns>
        /// <example>
        /// GET: api/AnnualEventData/ListEvents
        /// </example>
        public IHttpActionResult ListEvents()
        {
            List<AnnualEvent> annualEvents = db.AnnualEvents.ToList();
            List<AnnualEventDto> annualEventDto = new List<AnnualEventDto>();
            annualEvents.ForEach(e => annualEventDto.Add(new AnnualEventDto()
            {
                AnnualEventID = e.AnnualEventID,
                AnnulaEventName = e.AnnulaEventName,
                Organizer = e.Organizer,
                Category = e.Category
            }));
            return Ok();
        }

        /// <summary>
        /// Find event data for a specific event id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// CONTENT: Event Data for requested event id
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// GET: api/AnnualEventData/FindEvent/5
        /// </example>
        [ResponseType(typeof(AnnualEvent))]
        public IHttpActionResult FindEvent(int id)
        {
            AnnualEvent annualEvent = db.AnnualEvents.Find(id);
            AnnualEventDto annualEventDto = new AnnualEventDto()
            {
                AnnualEventID = annualEvent.AnnualEventID,
                AnnulaEventName = annualEvent.AnnulaEventName,
                Category = annualEvent.Category,
                Organizer = annualEvent.Organizer
            };
            if (annualEvent == null)
            {
                return NotFound();
            }

            return Ok(annualEvent);
        }

        /// <summary>
        /// Update species data for a specific event id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="annualEvent"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// // POST: api/AnnualEventData/UpdateEvent/5
        /// FORM DATA : Event JSON object
        /// </example>
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateEvent(int id, AnnualEvent annualEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != annualEvent.AnnualEventID)
            {
                return BadRequest();
            }

            db.Entry(annualEvent).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnualEventExists(id))
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
        /// Add event data to the database
        /// </summary>
        /// <param name="annualEvent"></param>
        /// <returns>
        /// HEADER : StatusCode: 201(Created)
        /// CONTENT: AnnualEventID, Event Data
        /// or
        /// HEADER : StatusCode: 400(Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AnnualEventData/AddEvent
        /// FORM DATA : Event JSON object
        /// </example>
        [ResponseType(typeof(AnnualEvent))]
        public IHttpActionResult AddEvent(AnnualEvent annualEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AnnualEvents.Add(annualEvent);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = annualEvent.AnnualEventID }, annualEvent);
        }

        /// <summary>
        /// Deletes event data for a specific event id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HEADER : StatusCode: 200(OK)
        /// or
        /// HEADER : StatusCode: 404(NOT Found)
        /// </returns>
        /// <example>
        /// POST: api/EventData/DeleteEvent/5
        /// FORM DATA : empty
        /// </example>
        [ResponseType(typeof(AnnualEvent))]
        public IHttpActionResult DeleteEvent(int id)
        {
            AnnualEvent annualEvent = db.AnnualEvents.Find(id);
            if (annualEvent == null)
            {
                return NotFound();
            }

            db.AnnualEvents.Remove(annualEvent);
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

        private bool AnnualEventExists(int id)
        {
            return db.AnnualEvents.Count(e => e.AnnualEventID == id) > 0;
        }
    }
}