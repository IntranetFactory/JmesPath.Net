using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathIndexExpression : JmesPathCompoundExpression
    {
        public JmesPathIndexExpression(JmesPathExpression expression, JmesPathExpression specifier)
            : base(expression, specifier)
        {
            System.Diagnostics.Debug.Assert(
                specifier is JmesPathIndex ||
                specifier is JmesPathFilterProjection ||
                specifier is JmesPathFlattenProjection ||
                specifier is JmesPathListWildcardProjection ||
                specifier is JmesPathSliceProjection ||
                false
                );
        }

        protected override JmesPathExpression CreateWith(JmesPathExpression left, JmesPathExpression right)
            => new JmesPathIndexExpression(left, right);
    }
}