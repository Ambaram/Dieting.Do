    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace Dieting_Do.Models
    {
        public class Animal
        {
            [Key]
            public int AnimalId { get; set; }
            public string AnimalName { get; set; }
            public int AnimalWeight { get; set; }
            public int AnimalHeight { get; set; }
            public bool animalwithpic { get; set; }
            public string picformat { get; set; }


            // ForeignKey SpeciedId
            [ForeignKey("Standard_Data")]
            public int SpeciesId { get; set; }
            public virtual Standard_Data Standard_Data { get; set; }

        }
        public class AnimalDto
        {
            public int AnimalId { get; set; }
            public string AnimalName { get; set; }
            public int AnimalWeight { get; set; }
            public int AnimalHeight { get; set; }
            public string SpeciesName { get; set; }
            public string SpeciesId { get; set; }
            public bool Animalwithpic { get; set; }
            public string Picformat { get; set; }
        }
    }
