using System;
using System.Collections;

namespace NHibernate.Transform
{
	[Serializable]
	public class RootEntityResultTransformer : IResultTransformer
	{
		private static readonly object Hasher = new object();

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return tuple[tuple.Length - 1];
		}

		public IList TransformList(IList collection)
		{
			return collection;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return obj.GetHashCode() == Hasher.GetHashCode();
		}

		public override int GetHashCode()
		{
			return Hasher.GetHashCode();
		}
	}
}