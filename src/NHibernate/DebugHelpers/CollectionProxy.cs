using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NHibernate.DebugHelpers
{
	public class CollectionProxy
	{
		private readonly ICollection set;

		public CollectionProxy(ICollection dic)
		{
			this.set = dic;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public object[] Items
		{
			get
			{
				object[] entries = new object[set.Count];
				set.CopyTo(entries, 0);
				return entries;
			}
		}
	}

	public class CollectionProxy<T>
	{
		private readonly ICollection<T> set;

		public CollectionProxy(ICollection<T> dic)
		{
			this.set = dic;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items
		{
			get
			{
				T[] entries = new T[set.Count];
				set.CopyTo(entries, 0);
				return entries;
			}
		}
	}
}
