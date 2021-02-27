using System.ComponentModel.DataAnnotations.Schema;

namespace CalculatorBS.Data
{
    [Table("FormulaQue", Schema = "dbo")]
    public class FormulaQue
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Formula { get; set; }

        public int HistoryId { get; set; }

        [ForeignKey("HistoryId")]
        public CalculationHistory CalculationHistory { get; set; }
    }
}