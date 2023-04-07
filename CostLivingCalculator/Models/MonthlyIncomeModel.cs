using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostLivingCalculator.Models
{
    internal class MonthlyIncomeModel
    {
        public Int32 Id { get; set; }
        public Int64 ChatId { get; set; }
        public Int32 Amount { get; set; }
        public String Month { get; set; }

    }
}
