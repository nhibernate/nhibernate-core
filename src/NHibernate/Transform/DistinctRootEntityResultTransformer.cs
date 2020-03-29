using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NHibernate.Transform
{
	[Serializable]
	public class DistinctRootEntityResultTransformer : IResultTransformer, ITupleSubsetResultTransformer
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(DistinctRootEntityResultTransformer));
		internal static readonly DistinctRootEntityResultTransformer Instance = new DistinctRootEntityResultTransformer();

		sealed class IdentityComparer<T> : IEqualityComparer<T>
		{
			public bool Equals(T x, T y)
			{
				return ReferenceEquals(x, y);
			}

			public int GetHashCode(T obj)
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
			var distinct = new HashSet<object>(new IdentityComparer<object>());

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

		internal static List<T> TransformList<T>(IEnumerable<T> list)
		{
			var result = list.Distinct(new IdentityComparer<T>()).ToList();
			if (log.IsDebugEnabled())
			{
				log.Debug("transformed: {0} rows to: {1} distinct results", list.Count(), result.Count);
			}
			return result;
		}

		public bool[] IncludeInTransform(String[] aliases, int tupleLength)
		{
			return RootEntityResultTransformer.Instance.IncludeInTransform(aliases, tupleLength);
		}

		public bool IsTransformedValueATupleElement(String[] aliases, int tupleLength)
		{
			return RootEntityResultTransformer.Instance.IsTransformedValueATupleElement(null, tupleLength);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, this))
				return true;
			if (obj == null)
				return false;
			return obj.GetType() == GetType();
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(Instance);
		}
	}
}
