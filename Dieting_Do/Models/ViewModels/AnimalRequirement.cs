using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieting_Do.Models.ViewModels
{
    public class AnimalRequirement
    {
        public AnimalDto selectedanimal { get; set; }
        public SpeciesDto selectedspecies { get; set; }
        public St_Dto selectedstandard { get; set; }
    }
}