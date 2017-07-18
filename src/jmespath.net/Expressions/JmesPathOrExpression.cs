using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesParenExpression : JmesPathExpression
    {
        private readonly JmesPathExpression expression_;

        public JmesParenExpression(JmesPathExpression expression)
        {
            expression_ = expression;
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            return expression_.Transform(json);
        }

        public override JmesPathExpression Accept(Interop.ITransformVisitor visitor)
        {
            var visitedExpression = expression_.Accept(visitor);
            return visitor.Visit(visitedExpression == expression_
                ? this
                : new JmesParenExpression(visitedExpression));
        }
    }

    public class JmesPathOrExpression : JmesPathCompoundExpression
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="JmesPathOrExpression"/> class
        /// with two <see cref="JmesPathExpression"/> objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public JmesPathOrExpression(JmesPathExpression left, JmesPathExpression right)
            : base(left, right)
        {
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            var token = Left.Transform(json);
            return !JmesPathArgument.IsFalse(token) ? token : Right.Transform(json);
        }

        protected override JmesPathExpression CreateWith(JmesPathExpression left, JmesPathExpression right)
            => new JmesPathOrExpression(left, right);
    }
}