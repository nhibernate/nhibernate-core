using System;
using System.Collections;
using System.Linq.Expressions;
using NHibernate.Transform;

namespace NHibernate.Linq
{
    public class ResultTransformer : IResultTransformer
    {
        private readonly LambdaExpression _expression;
        private readonly Delegate _projector;

        public ResultTransformer(LambdaExpression expression)
        {
            _expression = expression;
            _projector = _expression.Compile();
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return _projector.DynamicInvoke(new object[] { tuple });
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }
    }
}