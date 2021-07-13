    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace Dieting_Do.Models
    {
        public class AnnualEvent
        {
            [Key]
            public int AnnualEventID { get; set; }
            public string AnnulaEventName { get; set; }
            public string Organizer { get; set; }
            public string Category { get; set; }

            [ForeignKey("Shelter")]
            public int ShelterID { get; set; }
            public virtual Shelter Shelter { get; set; }
        }
        public class AnnualEventDto
        {
            public int AnnualEventID { get; set; }
            public string AnnulaEventName { get; set; }
            public string Organizer { get; set; }
            public string Category { get; set; }
        }
    }
