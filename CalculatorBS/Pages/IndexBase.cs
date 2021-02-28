using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BlazorContextMenu;
using CalculatorBS.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CalculatorBS.Pages
{
    /// <summary>
    /// トップページ
    /// </summary>
    public class IndexBase : ComponentBase
    {
        // 演算子リスト
        private readonly List<string> _operators = new List<string> { "+", "-", "*", "/", /*"pow", "sqrt", "recp"*/ };
        // 計算回数
        private int _num;
        // 計算フラグ
        private bool _isCalc = false;
        private bool _isClear = false;

        /// <summary>
        /// 計算式キュー
        /// </summary>
        protected Queue<string> FormulaQueue { get; set; }
        /// <summary>
        /// 前回の計算結果
        /// </summary>
        protected string PreResult { get; set; }
        /// <summary>
        /// 計算式(表示)
        /// </summary>
        protected string Formula { get; set; }
        /// <summary>
        /// 計算結果
        /// </summary>
        protected string Result { get; set; }
        /// <summary>
        /// メッセージ
        /// </summary>
        protected string Message { get; set; }
        /// <summary>
        /// 履歴リスト
        /// </summary>
        protected List<CalculationHistory> CalculationHistories { get; set; }

        /// <summary>
        /// 履歴データサービス
        /// </summary>
        [Inject]
        protected CalculationDataService CalculationDataService { get; set; }
        /// <summary>
        /// js
        /// </summary>
        [Inject]
        protected IJSRuntime JSInterop { get; set; }
        /// <summary>
        /// ログ
        /// </summary>
        [Inject]
        private ILogger<IndexBase> Logger { get; set; }

        /// <summary>
        /// 初期化
        /// </summary>
        protected override void OnInitialized()
        {
            CalculationHistories = new List<CalculationHistory>();
            FormulaQueue = new Queue<string>();
            _num = 0;
            AllClear();
        }

        /// <summary>
        /// クリア
        /// </summary>
        protected void AllClear()
        {
            Formula = string.Empty;
            Result = "0";
            FormulaQueue.Clear();
            _isClear = true;
            _isCalc = false;
        }

        /// <summary>
        /// クリアエントリー
        /// </summary>
        protected void ClearEntry()
        {
            Result = "0";

            if (_isCalc)
            {
                Formula = string.Empty;
                FormulaQueue.Clear();
                _isCalc = false;
            }
            _isClear = true;
        }

        /// <summary>
        /// 計算
        /// </summary>
        protected async void Calc()
        {
            Message = string.Empty;

            try
            {
                _isCalc = true;

                // キューの最後が演算子の場合、入力中の数値を入れる
                if (_operators.Contains(FormulaQueue.Last()))
                {
                    Formula += Result;
                    FormulaQueue.Enqueue(Result);
                }


                var tmpQue = FormulaQueue.ToArray();
                // 計算式の解析
                var context = new Context(FormulaQueue);
                var node = new ExpressionNode();
                node.Parse(context);
                // 計算
                Result = RpnCalculator.Exec(context.Notation).ToString();

                var history = await CalculationDataService.GetNewNumHistory();
                if (history != null)
                {
                    _num = _num != 0 ? CalculationHistories.First().Num : history.Num + 1;
                }

                // 履歴をDBに保存
                var insertHistory = new CalculationHistory { Result = decimal.Parse(Result), Formula = Formula, Num = _num, FormulaQues = new List<FormulaQue>() };
                for (var i = 0; i < tmpQue.Length; i++)
                {
                    var f = tmpQue[i];
                    var formulaQueue = new FormulaQue()
                    {
                        Order = i,
                        Formula = f
                    };
                    insertHistory.FormulaQues.Add(formulaQueue);
                }
                await CalculationDataService.InsertCalculationHistory(insertHistory);
                await CalculationDataService.SaveChangesAsync();

                CalculationHistories.Add(insertHistory);
            }
            catch(DbUpdateException e)
            {
                Message = "履歴保存エラー";
                Logger.LogError(e.Message);
                if(e.InnerException != null)
                {
                    Logger.LogError(e.InnerException.Message);
                }
            }
            catch (Exception e)
            {
                Message = "計算エラー";
                Logger.LogError(e.Message);
            }
            StateHasChanged();
        }

        /// <summary>
        /// 数字ボタンクリック
        /// </summary>
        /// <param name="num"></param>
        protected void OnNumberButtonClick(string num)
        {
            if (FormulaQueue.Count > 0)
            {
                if (_isCalc)
                    Formula = string.Empty;

                if (Result == PreResult || _isClear)
                    Result = num;
                else
                    Result += num;
            }
            else
            {
                if (Result == "0" || _isCalc)
                {
                    Result = num;
                    Formula = string.Empty;
                }
                else
                    Result += num;
            }

            _isCalc = false;
            _isClear = false;
        }

        /// <summary>
        /// パーセントボタンクリック
        /// </summary>
        protected void Per()
        {
            if(decimal.TryParse(Result, out var result))
            {
                result /= 100;
                Result = result.ToString();
            }
        }

        /// <summary>
        /// ±ボタンクリック
        /// </summary>
        protected void Negate()
        {
            if (decimal.TryParse(Result, out var result))
            {
                result = decimal.Negate(result);
                Result = result.ToString();
            }
        }

        /// <summary>
        /// 逆数、二乗、平方根ボタンクリック
        /// </summary>
        /// <param name="exp"></param>
        protected void OnSpecialOperatorsButtonClick(string exp)
        {
            PreResult = Result;

            if (_isCalc || Formula.All(_ => char.IsDigit(Formula.LastOrDefault())))
                Formula = string.Empty;

            if (exp == "pow")
            {
                Formula += $"sqr({Result})";
            }
            else if (exp == "sqrt")
            {
                Formula += $"√({Result})";
            }
            else if (exp == "recp")
            {
                Formula += $"1/({Result})";
            }

            FormulaQueue.Enqueue(Result);
            FormulaQueue.Enqueue(exp);

            Calc();
        }

        /// <summary>
        /// 演算子ボタンクリック
        /// </summary>
        /// <param name="exp"></param>
        protected void OnOperatorsButtonClick(string exp)
        {
            PreResult = Result;

            FormulaQueue.Enqueue(Result);
            FormulaQueue.Enqueue(exp);

            if (_isCalc)
            {
                Formula = Result + exp;
            }
            else
            {
                Formula += Result + exp;
            }

            _isCalc = false;
            _isClear = false;
        }

        /// <summary>
        /// 計算履歴クリック
        /// </summary>
        /// <param name="num"></param>
        protected void OnHistoryClick(int num)
        {
            // クリックされた履歴を取得
            var history = CalculationHistories[num];
            Formula = history.Formula;
            Result = history.Result.ToString();
            FormulaQueue.Clear();
            foreach (var f in history.FormulaQues.OrderBy(x => x.Order))
            {
                FormulaQueue.Enqueue(f.Formula);
            }
            StateHasChanged();
        }

        /// <summary>
        /// バックスペースボタンクリック
        /// </summary>
        protected void OnBackSapce()
        {
            if (!string.IsNullOrEmpty(Formula))
                Formula = string.Empty;
            else if (Result.Length >= 2)
                Result.Remove(Result.Length);
        }

        /// <summary>
        /// 履歴削除ボタン
        /// </summary>
        /// <param name="e"></param>
        protected void OnHistoryMenuDeleteClick(ItemClickEventArgs e)
        {
            if (e.Data is CalculationHistory his)
                CalculationHistories.Remove(his);
            // js 呼び出し
            JSInterop.InvokeVoidAsync("deleteHistory", e.ContextMenuTrigger.Id);
            StateHasChanged();
        }

        protected void OnDismiss()
        {
            Message = string.Empty;
            StateHasChanged();
        }
    }
}
