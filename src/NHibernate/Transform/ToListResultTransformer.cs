using System;
using System.Collections;

namespace NHibernate.Transform
{
	/// <summary> 
	/// Tranforms each result row from a tuple into a <see cref="IList"/>, such that what
	/// you end up with is a <see cref="IList"/> of <see cref="IList"/>.
	/// </summary>
	[Serializable]
	public class ToListResultTransformer : IResultTransformer
	{
		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return new ArrayList(tuple);
		}

		public IList TransformList(IList list)
		{
			return list;
		}
	}
}