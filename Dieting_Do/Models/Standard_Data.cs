    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;

    namespace Dieting_Do.Models
    {
        public class Standard_Data
        {
            [Key]
            public int SpeciesId { get; set; }
            public string SpeciesName { get; set; }
            public int St_SpeciesBMI { get; set; }
            public int St_Protein { get; set; }
            public int St_Carbs { get; set; }
            public int St_Fibre { get; set; }
            public int St_Vitamin { get; set; }
            public int St_Fat { get; set; }

            // Vet using our data to prescribe animal nutrition
            public ICollection<Vet> Vets { get; set; }
        }
        public class St_Dto
        {
            public int SpeciesId { get; set; }
            public string SpeciesName { get; set; }
            public int St_SpeciesBMI { get; set; }
            public int St_Protein { get; set; }
            public int St_Carbs { get; set; }
            public int St_Fibre { get; set; }
            public int St_Vitamin { get; set; }
            public int St_Fat { get; set; }

        }
    }
