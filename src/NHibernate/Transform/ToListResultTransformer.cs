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
		internal static readonly ToListResultTransformer Instance = new ToListResultTransformer();

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
			if (ReferenceEquals(obj, this))
				return true;
			if (obj == null)
				return false;
			return obj.GetType() == GetType();
		}

		public override int GetHashCode()
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(Instance);
		}
	}
}
