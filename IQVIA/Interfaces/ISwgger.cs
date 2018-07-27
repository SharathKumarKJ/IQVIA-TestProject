using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQVIA.Interfaces
{
   public interface ISwagger
    {
         string Id { get; set; }
         string Stamp { get; set; }
         string Text { get; set; }
    }
}
