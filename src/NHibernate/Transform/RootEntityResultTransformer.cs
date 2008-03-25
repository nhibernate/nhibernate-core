using System;
using System.Collections;

namespace NHibernate.Transform
{
	[Serializable]
	public class RootEntityResultTransformer : IResultTransformer
	{
		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return tuple[tuple.Length - 1];
		}

		public IList TransformList(IList collection)
		{
			return collection;
		}
	}
}