using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using DevLab.JmesPath.Functions;
using DevLab.JmesPath.Interop;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathFunctionExpression : JmesPathExpression
    {
        private readonly string name_;
        private readonly JmesPathExpression[] expressions_;
        private readonly JmesPathFunction function_;

        public IReadOnlyList<JmesPathExpression> Arguments => expressions_;
        public JmesPathFunction Function => function_;

        private JmesPathFunctionExpression(string name, JmesPathExpression[] expressions, JmesPathFunction function)
        {
            name_ = name;
            expressions_ = expressions;
            function_ = function;
        }

        public JmesPathFunctionExpression(string name, params JmesPathExpression[] expressions)
            : this(JmesPathFunctionFactory.Default, name, expressions)
        {

        }

        public JmesPathFunctionExpression(IFunctionRepository repository, string name, IList<JmesPathExpression> expressions)
            : this(repository, name, expressions.ToArray())
        {

        }

        public JmesPathFunctionExpression(IFunctionRepository repository, string name, params JmesPathExpression[] expressions)
        {
            if (!repository.Contains(name))
                throw new Exception($"Error: unknown-function, no function named {name} has been registered.");

            function_ = repository[name];

            var variadic = function_.Variadic;
            var expected = function_.MinArgumentCount;
            var actual = expressions?.Length;

            if (actual < expected || (!variadic && actual > expected))
            {
                var more = variadic ? "or more " : "";
                var only = variadic ? "only " : "";
                var report = actual == 0 ? "none" : $"{only}{actual}";
                var plural = expected > 1 ? "s" : "";

                throw new Exception($"Error: invalid-arity, the function {name} expects {expected} argument{plural} {more}but {report} were supplied.");
            }

            name_ = name;
            expressions_ = expressions;
        }

        public JToken Name => name_;

        protected override JmesPathArgument Transform(JToken json)
        {
            var arguments = expressions_.Select(
                expression =>
                {
                    if (expression.IsExpressionType)
                        return new JmesPathFunctionArgument(expression);
                    else
                        return new JmesPathFunctionArgument(expression.Transform(json).AsJToken());
                })
                .ToArray()
                ;

            function_.Validate(arguments);

            return function_.Execute(arguments);
        }

        public override JmesPathExpression Accept(ITransformVisitor visitor)
        {
            JmesPathExpression[] visitedExpressionsStorage = expressions_;
            JmesPathExpression[] visitedExpressions = null;

            for (int i = 0, n = expressions_.Length; i < n; i++)
            {
                var expression = expressions_[i];
                var visitedExpression = expression.Accept(visitor);

                // the first time we visited a transformed argument, allocate new
                // argument storage and copy any preceding untransformed arguments
                if (expression != visitedExpression && visitedExpressions == null)
                {
                    visitedExpressions = new JmesPathExpression[n];
                    visitedExpressionsStorage = visitedExpressions;
                    if (i > 0)
                        Array.Copy(expressions_, visitedExpressions, i - 1);
                }

                visitedExpressionsStorage[i] = visitedExpression;
            }

            // if our visited argument storage array and the one backing this
            // instance are the same, then no arguments were transformed
            if (visitedExpressionsStorage == expressions_)
                return visitor.Visit(this);

            return visitor.Visit(new JmesPathFunctionExpression(name_, visitedExpressionsStorage, function_));
        }
    }
}