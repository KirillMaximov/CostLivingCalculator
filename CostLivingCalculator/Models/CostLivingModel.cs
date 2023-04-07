using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostLivingCalculator.Models
{
    internal class CostLivingModel
    {
        public Int32 Id { get; set; }
        public Int32 RegionId { get; set; }
        public String Code { get; set; }
        public Int32 Person { get; set; }
        public Int32 Worker { get; set; }
        public Int32 Child { get; set; }
        public Int32 Pensioner { get; set; }
    }
}
