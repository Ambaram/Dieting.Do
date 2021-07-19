    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;

    namespace Dieting_Do.Models
    {
        public class Shelter
        {
            [Key]
            public int ShelterID { get; set; }
            public string ShelterName { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string OwnerName { get; set; }
        }
        public class ShelterDto
        {
            public int ShelterID { get; set; }
            public string ShelterName { get; set; }
            public string Location { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string OwnerName { get; set; }
        }
    }
