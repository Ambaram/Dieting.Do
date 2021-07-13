using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieting_Do.Models.ViewModels
{
    public class ShelterAnimalSpecies
    {
        public ShelterDto selectedshelter { get; set; }
        public IEnumerable<AnimalDto> animallist { get; set; }
        public IEnumerable<St_Dto> standardlist { get; set; }
    }
}