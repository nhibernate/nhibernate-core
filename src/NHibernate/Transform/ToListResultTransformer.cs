using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Transform
{
	/// <summary> 
	/// Transforms each result row from a tuple into a <see cref="IList"/>, such that what
	/// you end up with is a <see cref="IList"/> of <see cref="IList"/>.
	/// </summary>
	[Serializable]
	public class ToListResultTransformer : IResultTransformer
	{
		private static readonly object Hasher = new object();

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return new List<object>(tuple);
		}

		public IList TransformList(IList list)
		{
			return list;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetHashCode() != Hasher.GetHashCode())
			{
				return false;
			}
			// NH-3957: do not rely on hashcode alone.
			// Must be the exact same type
			return obj.GetType() == typeof(ToListResultTransformer);
		}

		public override int GetHashCode()
		{
			return Hasher.GetHashCode();
		}
	}
}