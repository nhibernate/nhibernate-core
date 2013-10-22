using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Iesi.Collections.Generic;

namespace NHibernate.Transform
{
	[Serializable]
	public class DistinctEntitiesResultTransformer : IResultTransformer
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DistinctEntitiesResultTransformer));
		private static readonly object Hasher = new object();
		private readonly bool[] returns;

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

		public DistinctEntitiesResultTransformer()
		{
			returns = null;
		}
	
		public DistinctEntitiesResultTransformer(bool[] returns)
		{
			this.returns = returns;
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			return tuple;
		}

		public IList TransformList(IList list)
		{
			IList result = new List<object>();
			if (list.Count <= 0) {
				return result;
			}

			object[] first = (object[])list[0];
	    
			if (returns != null && returns.Length != first.Length)
			{
				throw new ArgumentException("Return indicators constructor parameter does not match tuple length");
			}
	    
			List<object>[] lists = new List<object>[first.Length];
			ISet<Identity>[] distincts = new ISet<Identity>[first.Length];
			for (int j = 0; j < lists.Length; j++)
			{
				System.Collections.Generic.List<object> l = null;
	    
				if (returns == null || returns[j])
				{
					l = new List<Object>();
					lists[j] = l;
					distincts[j] = new HashedSet<Identity>();
				}
				else
				{
					lists[j] = null;
					distincts[j] = null;
				}
	    
				result.Add(l);
			}

			for (int i = 0; i < list.Count; i++)
			{
				object[] entities = (object[])list[i];
				for (int j = 0; j < entities.Length; j++)
				{
					if (lists[j] != null)
					{
						object entity = entities[j];
						if (distincts[j].Add(new Identity(entity)))
						{
							lists[j].Add(entity);
						}
					}
				}
			}

			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("transformed: {0} rows to: {1} lists of distinct results",
										list.Count, lists.Length));
			}
			
			return result;
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