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
using Recbox2.Models;

namespace Recbox2.Controllers
{
    public class KiosksController : ApiController
    {
        private RedboxEntities db = new RedboxEntities();

        // GET: api/Kiosks
        public IQueryable<Kiosk> GetKiosks()
        {
            return db.Kiosks;
        }

        // GET: api/Kiosks/5
        [ResponseType(typeof(Kiosk))]
        public IHttpActionResult GetKiosk(int id)
        {
            Kiosk kiosk = db.Kiosks.Find(id);
            if (kiosk == null)
            {
                return NotFound();
            }

            return Ok(kiosk);
        }

        // PUT: api/Kiosks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutKiosk(int id, Kiosk kiosk)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != kiosk.KioskId)
            {
                return BadRequest();
            }

            db.Entry(kiosk).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KioskExists(id))
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

        // POST: api/Kiosks
        [ResponseType(typeof(Kiosk))]
        public IHttpActionResult PostKiosk(Kiosk kiosk)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Kiosks.Add(kiosk);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = kiosk.KioskId }, kiosk);
        }

        // DELETE: api/Kiosks/5
        [ResponseType(typeof(Kiosk))]
        public IHttpActionResult DeleteKiosk(int id)
        {
            Kiosk kiosk = db.Kiosks.Find(id);
            if (kiosk == null)
            {
                return NotFound();
            }

            db.Kiosks.Remove(kiosk);
            db.SaveChanges();

            return Ok(kiosk);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KioskExists(int id)
        {
            return db.Kiosks.Count(e => e.KioskId == id) > 0;
        }
    }
}