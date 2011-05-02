using System;
using System.Collections;

namespace NHibernate.Transform
{
	[Serializable]
	public class PassThroughResultTransformer : IResultTransformer
	{
		private static readonly object Hasher = new object();

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