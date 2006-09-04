using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Transform
{
    public class PassThroughResultTransformer : IResultTransformer
    {
        #region IResultTransformer Members

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            return tuple.Length == 1 ? tuple[0] : tuple;
        }

        public System.Collections.IList TransformList(System.Collections.IList collection)
        {
            return collection;
        }

        #endregion
    }
    /*
public class PassThroughResultTransformer implements ResultTransformer {

	public Object transformTuple(Object[] tuple, String[] aliases) {
		return tuple.length==1 ? tuple[0] : tuple;
	}

	public List transformList(List collection) {
		return collection;
	}


     * }*/
}
