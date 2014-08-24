using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Transform
{
	/// <summary> 
	/// Tranforms each result row from a tuple into a <see cref="IList"/>, such that what
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