using System;
using System.Collections;

namespace NHibernate.Transform
{
	[Serializable]
	public class AliasToEntityMapResultTransformer : AliasedTupleSubsetResultTransformer
	{
		private static readonly object Hasher = new object();

		public override object TransformTuple(object[] tuple, string[] aliases)
		{
			IDictionary result = new Hashtable();
			for (int i = 0; i < tuple.Length; i++)
			{
				string alias = aliases[i];
				if (alias != null)
				{
					// TODO: Incredibly dodgy!! what if the user defines an alias ending in "_"
					result[alias] = tuple[i];
				}
			}

			return result;
		}

		public override IList TransformList(IList collection)
		{
			return collection;
		}


		public override bool IsTransformedValueATupleElement(string[] aliases, int tupleLength)
		{
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetHashCode() != Hasher.GetHashCode())
			{
				return false;
			}
			// NH-3957: do not rely on hashcode alone.
			// Must be the exact same type
			return obj.GetType() == typeof(AliasToEntityMapResultTransformer);
		}

		public override int GetHashCode()
		{
			return Hasher.GetHashCode();
		}
	}
}