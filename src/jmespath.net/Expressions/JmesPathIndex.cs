using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public class JmesPathIndex : JmesPathExpression
    {
        public int Value { get; }

        /// <summary>
        /// Initialize a new instance of the <see cref="JmesPathIndex"/>
        /// with the given index.
        /// </summary>
        /// <param name="index"></param>
        public JmesPathIndex(int index)
        {
            Value = index;
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            if (json.Type != JTokenType.Array)
                return null;

            var array = json as JArray;
            if (array == null)
                return null;

            var index = Value;

            if (index < 0)
                index = array.Count + index;
            if (index < 0 || index >= array.Count)
                return null;

            return array[index];
        }

        public override JmesPathExpression Accept(Interop.ITransformVisitor visitor)
            => visitor.Visit(this);
    }
}