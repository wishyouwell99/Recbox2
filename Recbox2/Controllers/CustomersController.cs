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
    public class CustomersController : ApiController
    {
        private RedboxEntities db = new RedboxEntities();

        
        // GET: api/Customers/5
        /// <summary>
        /// Finds a customer in the database and validates email and password
        /// </summary>
        /// <param name="email">Email of the customer</param>
        /// <param name="password">Password of the customer</param>
        /// <returns>Returns message for Not Found "Email not found", Invalid Password "Invalid Password" or the customer object
        /// </returns>
        [HttpGet]
        //[ActionName("ValidateCustomer")]
        [Route("api/Customers/ValidateCustomer")]
        [ResponseType(typeof(Customer))]
        public HttpResponseMessage ValidateCustomer(string email, string password)
        {
            Customer customer = db.Customers.Where(r=>r.Email == email).FirstOrDefault();
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Email not found");
            }

            if (customer.Password!=password)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid password");
            }

            return Request.CreateResponse(HttpStatusCode.OK, customer);
        }

        /// <summary>
        /// Get a full list of all movies this customer has rented.
        /// Includes movies that are currently out and already returned.
        /// </summary>
        /// <param name="id">CustomerId</param>
        /// <returns>CustomerRentals - List of movies for this customer</returns>
        [HttpGet]
        //[ActionName("CustomerRentals")]
        [Route("api/Customers/CustomerRentals")]
        [ResponseType(typeof(spCustomerRentals_Result))]
        public HttpResponseMessage CustomerRentals(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Customer:{0} does not exist", id));
            }

            var customerRentals = db.spCustomerRentals(id);

            if (customerRentals==null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, string.Format("No rentals for Customer: {0}", id));
            }           

            return Request.CreateResponse(HttpStatusCode.OK, customerRentals);
        }


        // GET: api/Customers1
        [HttpGet]
        public IQueryable<Customer> GetCustomers()
        {
            return db.Customers;
        }

        [HttpGet]
        [ResponseType(typeof(Customer))]
        public HttpResponseMessage Get(int id)
        {
            Customer customer = db.Customers.AsNoTracking().Where(r => r.CustomerId == id).FirstOrDefault();
            if (customer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Customer not found");
            }

            db.Entry(customer).State = EntityState.Detached;
            return Request.CreateResponse(HttpStatusCode.OK, customer);
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            // First check if this customer exists and is active.
            var exist = db.Customers.Where(r => r.Email == customer.Email && r.IsActive == true && r.CustomerId!=id).FirstOrDefault();
            // if this email address is in use, return and warn 
            if (exist != null)
            {
                return BadRequest("This email address is already in use.");
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        [ResponseType(typeof(Customer))]
        public IHttpActionResult PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // First check if this customer exists and is active.
            var exist = db.Customers.Where(r => r.Email == customer.Email && r.IsActive==true).FirstOrDefault();
            // if this email address is in use, return and warn 
            if (exist!=null)
            {
                return BadRequest("This email address is already in use.");
            }
            
            // set the customer to active
            customer.IsActive = true;
            
            // get the current date time
            customer.CreatedOn = DateTime.Now;

            // if no issues, add the customer.
            db.Customers.Add(customer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [ResponseType(typeof(Customer))]
        public IHttpActionResult DeleteCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            // only do soft deletes. If a customer is deleting, set it to not active
            customer.IsActive = false;            
            db.SaveChanges();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}