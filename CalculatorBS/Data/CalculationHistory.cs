using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalculatorBS.Data
{
    [Table("CalculationHistory", Schema = "dbo")]
    public class CalculationHistory
    {
        public int Id { get; set; }
        public string Formula { get; set; }
        public decimal Result { get; set; }
        public int Num { get; set; }
        public DateTime CreatedTime { get; set; }

        public List<FormulaQue> FormulaQues { get; set; }
    }
}