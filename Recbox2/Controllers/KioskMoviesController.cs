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
    public class KioskMoviesController : ApiController
    {
        private RedboxEntities db = new RedboxEntities();

        // GET: api/KioskMovies
        public IQueryable<KioskMovy> GetKioskMovies()
        {
            return db.KioskMovies;
        }

        // GET: api/KioskMovies/5
        [ResponseType(typeof(KioskMovy))]
        public IHttpActionResult GetKioskMovy(int id)
        {
            KioskMovy kioskMovy = db.KioskMovies.Find(id);
            if (kioskMovy == null)
            {
                return NotFound();
            }

            return Ok(kioskMovy);
        }

        // PUT: api/KioskMovies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutKioskMovy(int id, KioskMovy kioskMovy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != kioskMovy.KioskMovieId)
            {
                return BadRequest();
            }

            db.Entry(kioskMovy).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KioskMovyExists(id))
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

        // POST: api/KioskMovies
        [ResponseType(typeof(KioskMovy))]
        public IHttpActionResult PostKioskMovy(KioskMovy kioskMovy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.KioskMovies.Add(kioskMovy);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = kioskMovy.KioskMovieId }, kioskMovy);
        }

        // DELETE: api/KioskMovies/5
        [ResponseType(typeof(KioskMovy))]
        public IHttpActionResult DeleteKioskMovy(int id)
        {
            KioskMovy kioskMovy = db.KioskMovies.Find(id);
            if (kioskMovy == null)
            {
                return NotFound();
            }

            db.KioskMovies.Remove(kioskMovy);
            db.SaveChanges();

            return Ok(kioskMovy);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KioskMovyExists(int id)
        {
            return db.KioskMovies.Count(e => e.KioskMovieId == id) > 0;
        }
    }
}