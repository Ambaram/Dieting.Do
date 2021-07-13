using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieting_Do.Models.ViewModels
{
    public class StandardVet
    {
        public IEnumerable<St_Dto> standardlist { get; set; }
        public IEnumerable<VetDto> vetlist { get; set; }
    }
}