using System.Collections.Generic;
using System.Linq;

namespace CalculatorBS
{
    public class ReversePolishNotation
    {
        private List<string> _tokens;

        public IEnumerable<string> Tokens
        {
            get => _tokens;
            private set => _tokens.AddRange(value);
        }

        public ReversePolishNotation()
            => _tokens = new List<string>();

        private ReversePolishNotation(IEnumerable<string> tokens) : this()
            => Tokens = tokens;

        public void Add(string token)
            => _tokens.Add(token);

        public static ReversePolishNotation CreateFromExp(string exp)
        {
            var tokens = exp.Split(' ').Where(s => s != "").ToList();
            return new ReversePolishNotation(tokens);
        }

        public override string ToString()
            => string.Join(" ", Tokens.ToArray());

    }
}
