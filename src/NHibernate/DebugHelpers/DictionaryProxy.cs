using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NHibernate.DebugHelpers
{
	/// <summary>
	/// Used to show a better debug display for dictionaries
	/// </summary>
	public class DictionaryProxy
	{
		private readonly IDictionary set;

		public DictionaryProxy(IDictionary dic)
		{
			this.set = dic;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public DictionaryEntry[] Items
		{
			get
			{
				DictionaryEntry[] entries = new DictionaryEntry[set.Count];
				set.CopyTo(entries, 0);
				return entries;
			}
		}
	}

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
