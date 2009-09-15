using System;
using System.Collections;
using System.Linq.Expressions;
using NHibernate.Transform;

namespace NHibernate.Linq
{
    [Serializable]
    public class ResultTransformer : IResultTransformer
    {
        private readonly Delegate _listTransformation;
        private readonly Delegate _itemTransformation;

        public ResultTransformer(LambdaExpression itemTransformation, LambdaExpression listTransformation)
        {
            if (itemTransformation != null)
            {
                _itemTransformation = itemTransformation.Compile();
            }
            if (listTransformation != null)
            {
                _listTransformation = listTransformation.Compile();
            }
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return _itemTransformation == null ? tuple : _itemTransformation.DynamicInvoke(new object[] { tuple } );
        }

        public IList TransformList(IList collection)
        {
            return _listTransformation == null ? collection : (IList) _listTransformation.DynamicInvoke(collection);
        }
    }
}