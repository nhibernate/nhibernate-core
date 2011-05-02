using System;
using System.Collections;
using System.Linq;
using NHibernate.Transform;

namespace NHibernate.Linq
{
    [Serializable]
    public class ResultTransformer : IResultTransformer
    {
        private readonly Delegate _listTransformation;
        private readonly Delegate _itemTransformation;

        public ResultTransformer(Delegate itemTransformation, Delegate listTransformation)
        {
            _itemTransformation = itemTransformation;
            _listTransformation = listTransformation;
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

            //if (collection.Count > 0)
            {
                if (collection.Count > 0 && collection[0] is object[])
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

    	public bool Equals(ResultTransformer other)
    	{
    		if (ReferenceEquals(null, other))
    		{
    			return false;
    		}
    		if (ReferenceEquals(this, other))
    		{
    			return true;
    		}
    		return Equals(other._listTransformation, _listTransformation) && Equals(other._itemTransformation, _itemTransformation);
    	}

    	public override bool Equals(object obj)
    	{
				return Equals(obj as ResultTransformer);
    	}

    	public override int GetHashCode()
    	{
    		unchecked
    		{
    			var lt = (_listTransformation != null ? _listTransformation.GetHashCode() : 0);
    			var it = (_itemTransformation != null ? _itemTransformation.GetHashCode() : 0);
    			return (lt * 397) ^ (it * 17);
    		}
    	}
    }
}