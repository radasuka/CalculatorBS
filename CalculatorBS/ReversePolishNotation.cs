using System.Collections.Generic;

namespace CalculatorBS
{
    /// <summary>
    /// 逆ポーランド記法
    /// </summary>
    public class ReversePolishNotation
    {
        private List<string> _tokens;

        /// <summary>
        /// トークン
        /// </summary>
        public IEnumerable<string> Tokens
        {
            get => _tokens;
            private set => _tokens.AddRange(value);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ReversePolishNotation()
            => _tokens = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokens"></param>
        private ReversePolishNotation(IEnumerable<string> tokens) : this()
            => Tokens = tokens;

        /// <summary>
        /// トークン追加
        /// </summary>
        /// <param name="token"></param>
        public void Add(string token)
            => _tokens.Add(token);
    }
}