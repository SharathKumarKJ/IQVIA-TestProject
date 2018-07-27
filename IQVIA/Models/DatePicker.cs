using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IQVIA.Models
{
    public class DatePicker
    {
         public long Id { get; set; }
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }
    }
}