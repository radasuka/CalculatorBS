using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CalculatorBS.Data
{
    public class CalculationDbContext : DbContext
    {
        public CalculationDbContext(DbContextOptions<CalculationDbContext> options)
            : base(options)
        {
            ChangeTracker.StateChanged += UpdateTimestamps;
            ChangeTracker.Tracked += UpdateTimestamps;
        }

        private static void UpdateTimestamps(object sender, EntityEntryEventArgs e)
        {
            if (e.Entry.Entity is CalculationHistory entityWithTimestamps)
            {
                entityWithTimestamps.CreatedTime = DateTime.Now;
            }
        }

        public virtual DbSet<CalculationHistory> CalculationHistoryEntities { get; set; }
        public virtual DbSet<FormulaQue> FormulaQues { get; set; }
    }
}