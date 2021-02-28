using System;
using System.Collections.Generic;
using System.Numerics;

namespace CalculatorBS
{
    /// <summary>
    /// 逆ポーランド記法計算
    /// </summary>
    public class RpnCalculator
    {
        /// <summary>
        /// 計算実行
        /// </summary>
        /// <param name="rpn"></param>
        /// <returns></returns>
        public static decimal Exec(ReversePolishNotation rpn)
        {
            var stack = new Stack<object>();
            foreach (var token in rpn.Tokens)
            {
                if (IsFunc(token))
                {
                    var a = (decimal)stack.Pop();
                    var c = Operate(a, token);
                    stack.Push(c);
                }
                else if (IsOperator(token))
                {
                    var b = (decimal)stack.Pop();
                    var a = (decimal)stack.Pop();
                    var c = Operate(a, b, token);
                    stack.Push(c);
                }
                else
                    stack.Push(decimal.Parse(token));
            }
            return (decimal)stack.Pop();
        }

        /// <summary>
        /// 逆数、二乗、平方根判定
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsFunc(string s)
        {
            if (s.Length >= 1)
            {
                if (s == "pow" || s == "sqrt" || s == "recp")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 四則演算判定
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsOperator(string s)
        {
            if (s.Length == 1)
            {
                if (s[0] == '+' || s[0] == '-' || s[0] == '*' || s[0] == '/')
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 四則演算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="ope"></param>
        /// <returns></returns>
        private static decimal Operate(decimal a, decimal b, string ope)
        {
            return ope switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                _ => a / b,
            };
        }

        /// <summary>
        /// 二乗、平方根
        /// </summary>
        /// <param name="a"></param>
        /// <param name="ope"></param>
        /// <returns></returns>
        private static decimal Operate(decimal a, string ope)
        {
            return ope switch
            {
                "pow" => Convert.ToDecimal(Math.Pow(decimal.ToDouble(a), 2)),
                "sqrt" => Convert.ToDecimal(Math.Sqrt(decimal.ToDouble(a))),
                _ => Reciprocal(a),
            };
        }

        /// <summary>
        /// 逆数
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static decimal Reciprocal(decimal a)
        {
            var b = (Complex)a;
            var r1 = Complex.Reciprocal(b);

            return Convert.ToDecimal(r1.Real);
        }
    }
}