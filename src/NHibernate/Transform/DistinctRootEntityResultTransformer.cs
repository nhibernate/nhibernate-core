using System;
using System.Collections;
using System.Runtime.CompilerServices;

using Iesi.Collections.Generic;

namespace NHibernate.Transform
{
	[Serializable]
	public class DistinctRootEntityResultTransformer : IResultTransformer
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(DistinctRootEntityResultTransformer));

		internal sealed class Identity
		{
			internal readonly object entity;

			internal Identity(object entity)
			{
				this.entity = entity;
			}

			public override bool Equals(object other)
			{
				Identity that = (Identity) other;
				return ReferenceEquals(entity, that.entity);
			}

			public override int GetHashCode()
			{
				return RuntimeHelpers.GetHashCode(entity);
			}
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return tuple[tuple.Length - 1];
		}

		public IList TransformList(IList list)
		{
			IList result = (IList)Activator.CreateInstance(list.GetType());
			ISet<Identity> distinct = new HashedSet<Identity>();

			for (int i = 0; i < list.Count; i++)
			{
				object entity = list[i];
				if (distinct.Add(new Identity(entity)))
				{
					result.Add(entity);
				}
			}

			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("transformed: {0} rows to: {1} distinct results",
				                        list.Count, result.Count));
			}
			return result;
		}
	}
}