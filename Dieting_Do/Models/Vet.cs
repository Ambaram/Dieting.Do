    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;

    namespace Dieting_Do.Models
    {
        public class Vet
        {
            [Key]
            public int VetID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ClinicName { get; set; }
            public string Location { get; set; }
            public string Phone { get; set; }
            // Vet can treat many animals
            public ICollection<Animal> Animals { get; set; }
        }
        public class VetDto
        {
            public int VetID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ClinicName { get; set; }
            public string Location { get; set; }
            public string Phone { get; set; }

        }
    }
