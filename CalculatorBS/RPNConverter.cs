using System;
using System.Collections.Generic;

namespace CalculatorBS
{
    public class RPNConverter
    {
        private Queue<string> _expQue = new Queue<string>();
        private IEnumerator<string> _iterator;

        public string Current
            => _iterator.Current;

        public RPNConverter(Queue<string> expQue)
        {
            _expQue = expQue;
            _iterator = GetTokens().GetEnumerator();
        }

        public string NextToken()
            => _iterator.MoveNext() ? _iterator.Current : null;

        public void SkipToken(string s)
        {
            if (Current != s)
                throw new InvalidOperationException("");
            NextToken();
        }

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

        public static bool IsSymbol(string c)
            => c == "+" || c == "-" || c == "*" || c == "/" || c == "(" || c == ")" || c == "pow" || c == "sqrt" || c == "recp";
    }
}