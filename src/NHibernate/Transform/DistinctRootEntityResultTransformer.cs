using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace NHibernate.Transform
{
	[Serializable]
	public class DistinctRootEntityResultTransformer : IResultTransformer, ITupleSubsetResultTransformer
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(DistinctRootEntityResultTransformer));
		private static readonly object Hasher = new object();

		internal sealed class IdentityComparer : IEqualityComparer<object>
		{
			public new bool Equals(object x, object y)
			{
				return ReferenceEquals(x, y);
			}

			public int GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return tuple[tuple.Length - 1];
		}

		public IList TransformList(IList list)
		{
			if (list.Count < 2)
				return list;

			IList result = (IList) Activator.CreateInstance(list.GetType());
			var distinct = new HashSet<object>(new IdentityComparer());

			for (int i = 0; i < list.Count; i++)
			{
				object entity = list[i];
				if (distinct.Add(entity))
				{
					result.Add(entity);
				}
			}

			if (log.IsDebugEnabled())
			{
				log.Debug("transformed: {0} rows to: {1} distinct results", list.Count, result.Count);
			}
			return result;
		}


		public bool[] IncludeInTransform(String[] aliases, int tupleLength)
		{
			//return RootEntityResultTransformer.INSTANCE.includeInTransform(aliases, tupleLength);
			var transformer = new RootEntityResultTransformer();
			return transformer.IncludeInTransform(aliases, tupleLength);
		}


		public bool IsTransformedValueATupleElement(String[] aliases, int tupleLength)
		{
			//return RootEntityResultTransformer.INSTANCE.isTransformedValueATupleElement(null, tupleLength);
			var transformer = new RootEntityResultTransformer();
			return transformer.IsTransformedValueATupleElement(null, tupleLength);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetHashCode() != Hasher.GetHashCode())
			{
				return false;
			}
			// NH-3957: do not rely on hashcode alone.
			// Must be the exact same type
			return obj.GetType() == typeof(DistinctRootEntityResultTransformer);
		}

		public override int GetHashCode()
		{
			return Hasher.GetHashCode();
		}
	}
}
