using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Transform;
using Remotion.Collections;

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
            if (_listTransformation == null)
            {
                return collection;
            }

            object transformResult = collection;

            if (collection.Count > 0)
            {
                if (collection[0] is object[])
                {
                    if ( ((object[])collection[0]).Length != 1)
                    {
                        // We only expect single items
                        throw new NotSupportedException();
                    }

                    transformResult = _listTransformation.DynamicInvoke(collection.Cast<object[]>().Select(o => o[0]));
                }
                else
                {
                    transformResult = _listTransformation.DynamicInvoke(collection);
                }
            }

            if (transformResult is IList)
            {
                return (IList) transformResult;
            }

            var list = new ArrayList {transformResult};
            return list;
        }
    }
}