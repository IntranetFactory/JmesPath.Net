using System.Collections.Generic;
using DevLab.JmesPath.Interop;
using Newtonsoft.Json.Linq;

namespace DevLab.JmesPath.Expressions
{
    public sealed class JmesPathMultiSelectHash : JmesPathExpression
    {
        private readonly IDictionary<string, JmesPathExpression> dictionary_
            = new Dictionary<string, JmesPathExpression>()
            ;

        public JmesPathMultiSelectHash(IDictionary<string, JmesPathExpression> dictionary)
        {
            foreach (var key in dictionary.Keys)
                dictionary_.Add(key, dictionary[key]);
        }

        protected override JmesPathArgument Transform(JToken json)
        {
            var properties = new List<JProperty>();

            foreach (var key in dictionary_.Keys)
            {
                var expression = dictionary_[key];
                var result = expression.Transform(json).AsJToken();
                properties.Add(new JProperty(key, result));
            }

            return new JObject(properties);
        }

        public override void Accept(IVisitor visitor)
        {
            base.Accept(visitor);
            foreach (var key in dictionary_.Keys)
                dictionary_[key].Accept(visitor);
        }

        public override JmesPathExpression Accept(ITransformVisitor visitor)
        {
            var anyChanged = false;
            var visited = new Dictionary<string, JmesPathExpression>();

            foreach (var item in dictionary_)
            {
                var visitedItem = item.Value.Accept(visitor);
                visited[item.Key] = visitedItem;
                anyChanged |= visitedItem != item.Value;
            }

            return visitor.Visit(anyChanged
                ? new JmesPathMultiSelectHash(visited)
                : this);
        }
    }
}