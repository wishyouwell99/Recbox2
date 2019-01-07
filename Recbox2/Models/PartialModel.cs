using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Recbox2.Models
{
    public class PartialModel
    {
    }

  

        [MetadataType(typeof(MetaData))]
    public partial class Customer
    {
        public class MetaData
        {
            [Required, StringLength(100), DataType(DataType.EmailAddress)]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email;

            [Required, StringLength(50), DataType(DataType.Password)]
            public string Password;

            [Required, StringLength(50)]
            public string FirstName;

            [Required, StringLength(50)]
            public string LastName;

            [Required, StringLength(50)]
            public string Address1;

            [StringLength(50)]
            public string Address2;

            [Required, StringLength(50)]
            public string City;

            [Required, StringLength(2)]
            public string State;

            [Required, StringLength(7)]
            public string Zip;

            [Required, StringLength(50), DataType(DataType.PhoneNumber)]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid Phone Number")]
            public string Phone;

            [Required, Range(18, 200)]
            public Nullable<int> Age;
            


        }
    }
}