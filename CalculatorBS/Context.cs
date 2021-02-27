using System.Collections.Generic;

namespace CalculatorBS
{
    public class Context
    {
        public RPNConverter Converter { get; set; }
        public ReversePolishNotation Notation { get; set; }

        public string CurrentToken
            => Converter.Current;

        public Context(Queue<string> expQue)
        {
            Converter = new RPNConverter(expQue);
            Converter.NextToken();
            Notation = new ReversePolishNotation();
        }

        public string NextToken()
            => Converter.NextToken() == null ? null : CurrentToken;

        public void SkipToken(string token)
            => Converter.SkipToken(token);
    }
}