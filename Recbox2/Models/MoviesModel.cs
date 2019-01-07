using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Recbox2.Models
{
    public class MoviesModel
    {
        public string Title { get; set; }
        public int Minutes { get; set; }
        public decimal Stars { get; set; }
        public string Description { get; set; }
        public string MovieImage { get; set; }
        public int Year { get; set; }
        public decimal Fee { get; set; }
        public string Rating { get; set; }
        public string Genre { get; set; }
        public int Quantity { get; set; }
        public int OnHand { get; set; }
        public int MovieId { get; set; }


    }
}