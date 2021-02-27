using System;

namespace CalculatorBS
{
    public class InvalidException : Exception
    {
        public InvalidException() { }
        public InvalidException(string s) : base(s) { }
    }

    public abstract class Node
    {
        public abstract void Parse(Context context);

        public static bool IsOperator(char c)
            => (c == '+' || c == '-' || c == '*' || c == '/');

        public static bool IsParenthesis(char c)
            => (c == '(' || c == ')');

        public static bool IsSign(char c)
            => (c == '-' || c == '+');

        public static int GetSign(string s)
            => s[0] == '-' ? -1 : 1;
    }

    /// <summary>
    /// 数字ノード
    /// </summary>
    public class SignedNumberNode : Node
    {
        public override void Parse(Context context)
        {
            var s = context.CurrentToken;
            var sign = 1;
            var nstr = s;

            if (IsSign(s[0]))
            {
                sign = GetSign(s);
                s = context.NextToken();
            }

            if (decimal.TryParse(s, out _))
                context.Notation.Add(string.Format("{0}", decimal.Parse(s) * sign));
            else
                throw new InvalidException();

            context.NextToken();
        }
    }

    /// <summary>
    /// かっこ
    /// </summary>
    public class FactorNode : Node
    {
        public override void Parse(Context context)
        {
            var s = context.CurrentToken;
            if (s == "(")
            {
                context.SkipToken("(");
                var node = new ExpressionNode();
                node.Parse(context);
                context.SkipToken(")");
            }
            else
            {
                var node = new SignedNumberNode();
                node.Parse(context);
            }
        }
    }

    /// <summary>
    /// 平方根
    /// </summary>
    public class SqrtNode : Node
    {
        public override void Parse(Context context)
        {
            var node1 = new FactorNode();
            node1.Parse(context);

            var s = context.CurrentToken;
            while (s == "sqrt")
            {
                if (context.NextToken() != null)
                {
                    var node2 = new ExpressionNode();
                    node2.Parse(context);
                }
                context.Notation.Add(s);
                s = context.NextToken();
            }
        }
    }

    /// <summary>
    /// 二乗
    /// </summary>
    public class PowNode : Node
    {
        public override void Parse(Context context)
        {
            var node1 = new SqrtNode();
            node1.Parse(context);

            var s = context.CurrentToken;
            while (s == "pow")
            {
                if (context.NextToken() != null)
                {
                    var node2 = new SqrtNode();
                    node2.Parse(context);
                }
                context.Notation.Add(s);
                s = context.NextToken();
            }
        }
    }

    /// <summary>
    /// 割り算
    /// </summary>
    public class DivTermNode : Node
    {
        public override void Parse(Context context)
        {
            var node1 = new PowNode();
            node1.Parse(context);

            var s = context.CurrentToken;
            while (s == "/")
            {
                context.NextToken();
                var node2 = new PowNode();
                node2.Parse(context);
                context.Notation.Add(s);
                s = context.CurrentToken;
            }
        }
    }

    /// <summary>
    /// 掛け算
    /// </summary>
    public class TermNode : Node
    {
        public override void Parse(Context context)
        {
            var node1 = new DivTermNode();
            node1.Parse(context);

            var s = context.CurrentToken;
            while (s == "*")
            {
                context.NextToken();
                var node2 = new DivTermNode();
                node2.Parse(context);
                context.Notation.Add(s);
                s = context.CurrentToken;
            }
        }
    }

    /// <summary>
    /// 足し算、引き算
    /// </summary>
    public class ExpressionNode : Node
    {
        public override void Parse(Context context)
        {
            var s = context.CurrentToken;
            var node1 = new TermNode();
            node1.Parse(context);
            s = context.CurrentToken;

            while (s == "+" || s == "-")
            {
                context.NextToken();
                var node2 = new TermNode();
                node2.Parse(context);
                context.Notation.Add(s);
                s = context.CurrentToken;
            }
        }
    }
}
