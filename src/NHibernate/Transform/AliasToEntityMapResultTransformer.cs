using System;
using System.Collections;

namespace NHibernate.Transform
{
	[Serializable]
	public class AliasToEntityMapResultTransformer : IResultTransformer
	{
		private static readonly object Hasher = new object();

		public object TransformTuple(object[] tuple, string[] aliases)
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

		public IList TransformList(IList collection)
		{
			return collection;
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