using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CalculatorBS.Data
{
    public class CalculationDataService
    {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        public CalculationDbContext Context { get; }

        public CalculationDataService(CalculationDbContext context)
            => Context = context;

        /// <summary>
        /// 計算履歴を全て取得
        /// </summary>
        /// <returns></returns>
        public Task<List<CalculationHistory>> GetCalculationHistoriesAsync()
            => Context.CalculationHistoryEntities.Include(x => x.FormulaQues).OrderBy(x => x.Id).ToListAsync();

        /// <summary>
        /// 最新の履歴を取得
        /// </summary>
        /// <returns></returns>
        public Task<CalculationHistory> GetNewNumHistory()
            => Context.CalculationHistoryEntities.Include(x => x.FormulaQues).OrderByDescending(x => x.Num).FirstOrDefaultAsync();

        /// <summary>
        /// 履歴を追加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsertCalculationHistory(CalculationHistory entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await Context.CalculationHistoryEntities.AddAsync(entity);
        }

        /// <summary>
        /// データベースへ保存
        /// </summary>
        public void SaveChanges()
            => Context.SaveChanges();

        /// <summary>
        /// データベースへ保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
            => await Context.SaveChangesAsync();
    }
}
