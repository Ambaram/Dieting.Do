    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;

    namespace Dieting_Do.Models
    {
        public class Species
        {
            [Key]
            public int SpeciesId { get; set; }
            public string AnimalSpecies { get; set; }
            public ICollection<Animal> Animals { get; set; }
            public ICollection<Shelter> Shelters { get; set; }
            public ICollection<Vet> Vets { get; set; }

        }

        public class SpeciesDto
        {
            public int SpeciesId { get; set; }
            public string AnimalSpecies { get; set; }
        }
    }
