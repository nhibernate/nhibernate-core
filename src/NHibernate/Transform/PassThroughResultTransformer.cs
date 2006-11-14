using System;
using System.Collections;

namespace NHibernate.Transform
{
	[Serializable]
	public class PassThroughResultTransformer : IResultTransformer
	{
		#region IResultTransformer Members

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return tuple.Length == 1 ? tuple[0] : tuple;
		}

		public IList TransformList(IList collection)
		{
			return collection;
		}

		#endregion
	}
}
