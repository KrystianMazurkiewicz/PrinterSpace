using System.ComponentModel.DataAnnotations;

namespace BachelorsProject.Models
{
    public class User
    {
        public int Id { get; set; }
        //[RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}$")]  
        //[RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}$")]
        //public string FirstName { get; set; }
        
        //[RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}$")]
        //public string LastName { get; set; }
        //[RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,50}$")]

        //public string Address { get; set; }
        // [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{1,20}$")] 
        //public string PostName { get; set; } 
        // [RegularExpression(@"^[0-9]{4}$")]
        // public int PostalCode { get; set; }
        //[RegularExpression(@"^s\d{6}@oslomet.no$")]
        //[RegularExpression(@"\@oslomet.no\")]
        // email starts with s, followed by 6digits,@,oslomet.no
        //[RegularExpression(@"\b[a-zA-ZæøåÆØÅ.0-9]+@oslomet\.no\b")]
        [RegularExpression(@"^[a-zA-Z0-9æøåÆØÅ]{1,20}@oslomet\.no$")]
        public string Email { get; set; }

        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{2,50}$")]

        public string? Username { get; set; }
       // [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,8}$")]

        //public string Password { get; set; }

        //public  Role  Role { get;  set; } // added here! 


    }
}
