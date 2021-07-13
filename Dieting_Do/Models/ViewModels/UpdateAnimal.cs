using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieting_Do.Models.ViewModels
{
    public class UpdateAnimal
    {
        public AnimalDto selectedanimal { get; set; }
        public IEnumerable<SpeciesDto> SpeciesList { get; set; }
    }
}