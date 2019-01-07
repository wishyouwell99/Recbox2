using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Recbox2.Models
{
    public class RentalListModel
    {
        // list of movies to rent.  
        [Required]
        public List<int> MovieList;

        [Required]
        public int CustomerId;

        [Required]
        public int KioskId;
            

    }
}