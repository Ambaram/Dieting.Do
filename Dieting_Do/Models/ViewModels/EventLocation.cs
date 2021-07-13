using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dieting_Do.Models.ViewModels
{
    public class EventLocation
    {
        public AnnualEventDto selectedevent { get; set; }
        public IEnumerable<ShelterDto> shelterlist { get; set; }
    }
}