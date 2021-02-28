using System;
using System.Collections.Generic;

namespace CalculatorBS
{
    /// <summary>
    /// 逆ポーランド記法コンバーター
    /// </summary>
    public class RPNConverter
    {
        // 計算式
        private Queue<string> _expQue = new Queue<string>();

        private IEnumerator<string> _iterator;

        /// <summary>
        /// 現在のトークン
        /// </summary>
        public string Current
            => _iterator.Current;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="expQue"></param>
        public RPNConverter(Queue<string> expQue)
        {
            _expQue = expQue;
            _iterator = GetTokens().GetEnumerator();
        }

        /// <summary>
        /// 次のトークンへ進める
        /// </summary>
        /// <returns></returns>
        public string NextToken()
            => _iterator.MoveNext() ? _iterator.Current : null;

        /// <summary>
        /// 指定したトークンをスキップ
        /// </summary>
        /// <param name="s"></param>
        public void SkipToken(string s)
        {
            if (Current != s)
                throw new InvalidOperationException("");
            NextToken();
        }

        /// <summary>
        /// トークンを取得
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTokens()
        {
            var token = string.Empty;

            while (_expQue.Count != 0)
            {
                var c = _expQue.Dequeue();
                if (decimal.TryParse(c, out _) || (!string.IsNullOrEmpty(token) && c == "."))
                    token += c;
                else
                {
                    if (token != "")
                        yield return token;
                    token = "";

                    if (IsSymbol(c))
                        yield return c.ToString();
                    else if (c != " ")
                        throw new InvalidOperationException("正しい式ではありません");
                }
            }

            if (token != string.Empty)
                yield return token;
        }

        /// <summary>
        /// 演算子判定
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsSymbol(string c)
            => c == "+" || c == "-" || c == "*" || c == "/" || c == "(" || c == ")" || c == "pow" || c == "sqrt" || c == "recp";
    }
}