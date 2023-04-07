using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostLivingCalculator.Models
{
    internal class AnswerModel
    {
        public Int32 Id { get; set; }
        public Int64 ChatId { get; set; }
        public Int32 RegionId { get; set; }
        public Int32 NumberPeople { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
