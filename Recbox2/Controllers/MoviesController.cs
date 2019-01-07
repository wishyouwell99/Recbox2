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
    public class MoviesController : ApiController
    {
        private RedboxEntities db = new RedboxEntities();

        // GET: api/Movies
        /// <summary>
        /// Get a full list of all movies available for rent at this kiosk
        /// </summary>
        /// <param name="kioskId">Specify the kioskid number</param>
        /// <returns>A list of movies</returns>
        [HttpGet]
        [Route("api/Movies/GetMoviesForKiosk")]
        [ResponseType(typeof(IQueryable<MoviesModel>))]
        public IHttpActionResult GetMoviesForKiosk(int kioskId)
        {
            // check if this kiosk exists
            var kiosk = db.Kiosks.Find(kioskId);
            if (kiosk == null)
            {
                return NotFound();
            }

            // get list of movies at this kiosk from the database
            var movielist = db.spKioskMoviesForRentGet(kioskId);
            // list that will be return
            List<MoviesModel> returnVal = new List<MoviesModel>();

            // if got values from the db then put in list
            if (movielist != null)
            {
                foreach (var movie in movielist)
                {
                    MoviesModel m = new MoviesModel();
                    m.Title = movie.Title;
                    m.Stars = movie.Stars;
                    m.Rating = movie.RatingValue;
                    m.Genre = movie.GenreName;
                    m.Description = movie.Description;
                    m.Fee = movie.Fee;
                    m.Minutes = movie.Minutes;
                    m.OnHand = movie.OnHand ?? 0;
                    m.MovieImage = movie.MovieImage;
                    m.Year = movie.Year;
                    m.MovieId = movie.MovieId;
                    returnVal.Add(m);
                }
            }

            // if there are no results then not found
            if (returnVal.Count == 0)
            {
                return NotFound();
            }
            else
                return Ok(returnVal.AsQueryable());
        }


        // GET: api/Movies
        public IQueryable<Movy> GetMovies()
        {
            return db.Movies;
        }

        // GET: api/Movies/5
        [ResponseType(typeof(Movy))]
        public IHttpActionResult GetMovy(int id)
        {
            Movy movy = db.Movies.Find(id);
            if (movy == null)
            {
                return NotFound();
            }

            return Ok(movy);
        }

        // PUT: api/Movies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMovy(int id, Movy movy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movy.MovieId)
            {
                return BadRequest();
            }

            db.Entry(movy).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovyExists(id))
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

        // POST: api/Movies
        [ResponseType(typeof(Movy))]
        public IHttpActionResult PostMovy(Movy movy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Movies.Add(movy);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = movy.MovieId }, movy);
        }

        // DELETE: api/Movies/5
        [ResponseType(typeof(Movy))]
        public IHttpActionResult DeleteMovy(int id)
        {
            Movy movy = db.Movies.Find(id);
            if (movy == null)
            {
                return NotFound();
            }

            db.Movies.Remove(movy);
            db.SaveChanges();

            return Ok(movy);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MovyExists(int id)
        {
            return db.Movies.Count(e => e.MovieId == id) > 0;
        }
    }
}