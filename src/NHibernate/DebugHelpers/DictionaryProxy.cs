using System.Collections.Generic;
using System.Diagnostics;

namespace NHibernate.DebugHelpers
{
	public class DictionaryProxy<K, V>
	{
		private readonly IDictionary<K, V> dic;

		public DictionaryProxy(IDictionary<K, V> dic)
		{
			this.dic = dic;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<K, V>[] Items
		{
			get
			{
				KeyValuePair<K, V>[] entries = new KeyValuePair<K, V>[dic.Count];
				dic.CopyTo(entries, 0);
				return entries;
			}
		}
	}
}
