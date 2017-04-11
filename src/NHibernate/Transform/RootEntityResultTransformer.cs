using System;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Transform
{
	[Serializable]
	public class RootEntityResultTransformer : IResultTransformer, ITupleSubsetResultTransformer
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


		public bool IsTransformedValueATupleElement(String[] aliases, int tupleLength)
		{
			return true;
		}


		public bool[] IncludeInTransform(String[] aliases, int tupleLength)
		{
			bool[] includeInTransform;
			if (tupleLength == 1)
			{
				includeInTransform = ArrayHelper.True;
			}
			else
			{
				includeInTransform = new bool[tupleLength];
				includeInTransform[tupleLength - 1] = true;
			}
			return includeInTransform;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetHashCode() != Hasher.GetHashCode())
			{
				return false;
			}
			// NH-3957: do not rely on hashcode alone.
			// Must be the exact same type
			return obj.GetType() == typeof(RootEntityResultTransformer);
		}

		public override int GetHashCode()
		{
			return Hasher.GetHashCode();
		}
	}
}