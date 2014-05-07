using System;
using System.Collections;

namespace NHibernate.Transform
{
	/// <summary>
	/// An implementation of TupleSubsetResultTransformer that ignores a
	/// tuple element if its corresponding alias is null.
	/// </summary>
	/// @author Gail Badner
	[Serializable]
	public abstract class AliasedTupleSubsetResultTransformer : ITupleSubsetResultTransformer
	{

		public abstract bool IsTransformedValueATupleElement(string[] aliases, int tupleLength);

		public bool[] IncludeInTransform(string[] aliases, int tupleLength)
		{
			if (aliases == null)
				throw new ArgumentNullException("aliases");

			if (aliases.Length != tupleLength)
			{
				throw new ArgumentException(
					"aliases and tupleLength must have the same length; " +
					"aliases.length=" + aliases.Length + "tupleLength=" + tupleLength
					);
			}
			bool[] includeInTransform = new bool[tupleLength];
			for (int i = 0; i < aliases.Length; i++)
			{
				if (aliases[i] != null)
				{
					includeInTransform[i] = true;
				}
			}
			return includeInTransform;
		}

		public abstract object TransformTuple(object[] tuple, string[] aliases);
		public abstract IList TransformList(IList collection);
	}
}
