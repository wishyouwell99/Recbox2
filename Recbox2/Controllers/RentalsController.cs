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
    public class RentalsController : ApiController
    {
        private RedboxEntities db = new RedboxEntities();

        // GET: api/Rentals
        public IQueryable<Rental> GetRentals()
        {
            return db.Rentals;
        }


        // POST: api/RentMovie
        /// <summary>
        /// Rent a list of movies for a specific customer
        /// </summary>
        /// <param name="rentalList">containts list of movies, kiosk, customer</param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(RentalListModel))]
        [Route("api/Rentals/RentMovie")]
        public HttpResponseMessage RentMovie(RentalListModel rentalList)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            // check that customer exists
            var customer = db.Customers.Find(rentalList.CustomerId);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Email not found");
            }

            // check if kiosk exists
            var kiosk = db.Kiosks.Find(rentalList.KioskId);
            if (kiosk == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Kiosk");
            }

            // check if there are movies to rent
            if (rentalList.MovieList.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No movies selected to rent.");
            }

            decimal feeTotal = 0;
            var rentalDays = kiosk.RentalLength ?? 0;

            // check if movies exists and is available at this kiosk
            foreach (var movieId in rentalList.MovieList)
            {
                // find this movie in the movie database
                var movie = db.Movies.Find(movieId);
                if (movie == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, string.Format("Invalid Movie: {0}", movieId));

                // get this movie at this kiosk
                var kioskMovie = db.KioskMovies.Where(r => r.KioskId == rentalList.KioskId && r.MovieId == movieId).FirstOrDefault();
                if (kioskMovie == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Movie: {0}  not available at Kiosk: {1}", movie.Title, kiosk.KioskId));
                }

                // check if still available
                if (kioskMovie.OnHand <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Movie: {0}  is out of stock at Kiosk: {1}", movie.Title, kiosk.KioskId));
                }

                // total up costs of rentals
                feeTotal += movie.Fee;

                // rent this movie
                var rental = new Rental();
                rental.CustomerId = rentalList.CustomerId;
                rental.KioskMovieId = kioskMovie.KioskMovieId;
                rental.Fee = movie.Fee;
                rental.RentalDate = DateTime.Now;
                rental.CreatedOn = DateTime.Now;
                rental.RentalDueDate = DateTime.Now.AddDays(rentalDays);
                db.Rentals.Add(rental);
                kioskMovie.OnHand--;
                db.Entry(kioskMovie).State = EntityState.Modified;
            }

            // charge movie fees.
            int transactionId = Helpers.Charge.ChargeCustomer(customer.CustomerId, feeTotal);

            if (transactionId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Transaction Cancelled.  Error charging customer: {0}", customer.CustomerId));
            }

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, rentalList);
        }



        // GET: api/Rentals/5
        /// <summary>
        /// Returns a movie that a customer has rented.
        /// The customer does not have to return the movie at the same kiosk it was rented.
        /// If returning to a different kiosk, the movie will be added to inventory if not already there.
        /// It will increment the count of onhand copies of this movie.f
        /// </summary>
        /// <param name="customerId">customer that rented the movie</param>
        /// <param name="movieId">id of the movie being returned</param>
        /// <param name="kioskId">the id kiosk the movie is being returned at</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Rentals/ReturnMovie")]
        public HttpResponseMessage ReturnMovie(int customerId, int movieId, int kioskId)
        {

            var customer = db.Customers.Find(customerId);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Customer:{0} does not exist", customerId));
            }

            // find this movie in the movie database
            var movie = db.Movies.Find(movieId);
            if (movie == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, string.Format("Invalid Movie: {0}", movieId));

            // check if kiosk exists
            var kiosk = db.Kiosks.Find(kioskId);
            if (kiosk == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid Kiosk");
            }

            // check if this kiosk has this movie.
            var kioskMovie = db.KioskMovies.Where(r => r.KioskId == kioskId && r.MovieId == movieId).FirstOrDefault();
            // if htis kiosk doesn't yet have the movie, then create it there.
            if (kioskMovie == null)
            {
                kioskMovie = new KioskMovy();
                kioskMovie.KioskId = kioskId;
                kioskMovie.MovieId = movieId;
                kioskMovie.OnHand = 1;
                kioskMovie.CreatedOn = DateTime.Now;
                db.KioskMovies.Add(kioskMovie);
            }
            else
            {
                // add this movie into inventory in this kiosk
                kioskMovie.OnHand++;
                db.Entry(kioskMovie).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, string.Format("Movie: {0} for Customer: {1} Returned at Kiosk: {2}", movieId, customerId, kioskId));
        }




        // GET: api/Rentals/5
        [ResponseType(typeof(Rental))]
        public IHttpActionResult GetRental(int id)
        {
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return NotFound();
            }

            return Ok(rental);
        }

        // PUT: api/Rentals/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRental(int id, Rental rental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rental.RentalId)
            {
                return BadRequest();
            }

            db.Entry(rental).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
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


      
        // POST: api/Rentals
        [ResponseType(typeof(Rental))]
        public IHttpActionResult PostRental(Rental rental)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Rentals.Add(rental);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = rental.RentalId }, rental);
        }

        // DELETE: api/Rentals/5
        [ResponseType(typeof(Rental))]
        public IHttpActionResult DeleteRental(int id)
        {
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return NotFound();
            }

            db.Rentals.Remove(rental);
            db.SaveChanges();

            return Ok(rental);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RentalExists(int id)
        {
            return db.Rentals.Count(e => e.RentalId == id) > 0;
        }
    }
}