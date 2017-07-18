using DevLab.JmesPath.Expressions;

namespace DevLab.JmesPath.Interop
{
    public interface ITransformVisitor
    {
        JmesPathExpression Visit(JmesPathExpression expression);
    }
}