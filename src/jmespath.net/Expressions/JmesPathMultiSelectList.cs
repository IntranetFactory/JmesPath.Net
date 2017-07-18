using System.Collections.Generic;
using DevLab.JmesPath.Interop;
using Newtonsoft.Json.Linq;
using DevLab.JmesPath.Utils;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathMultiSelectList : JmesPathExpression
    {
        private readonly IList<JmesPathExpression> expressions_;

        JmesPathMultiSelectList(List<JmesPathExpression> expressions)
        {
            expressions_ = expressions;
        }

        public JmesPathMultiSelectList(params JmesPathExpression[] expressions)
            : this(new List<JmesPathExpression>(expressions))
        {
        }

        public JmesPathMultiSelectList(IEnumerable<JmesPathExpression> expressions)
            : this(new List<JmesPathExpression>(expressions))
        {
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            var items = new List<JToken>();
            foreach (var expression in expressions_)
            {
                var result = expression.Transform(json).AsJToken();
                items.Add(result);
            }

            return new JArray().AddRange(items);
        }

        public override void Accept(IVisitor visitor)
        {
            base.Accept(visitor);            
            foreach (var expression in expressions_)
                expression.Accept(visitor);
        }

        public override JmesPathExpression Accept(ITransformVisitor visitor)
        {
            var anyChanged = false;
            var visited = new List<JmesPathExpression>();

            foreach (var expression in expressions_)
            {
                var visitedExpression = expression.Accept(visitor);
                visited.Add(visitedExpression);
                anyChanged |= visitedExpression != expression;
            }

            return visitor.Visit(anyChanged
                ? new JmesPathMultiSelectList(visited)
                : this);
        }
    }
}