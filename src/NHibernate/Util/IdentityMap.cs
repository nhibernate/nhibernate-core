using System;
using System.Collections;
using System.Collections.Specialized;

namespace NHibernate.Util 
{

	/// <summary>
	/// An <c>IDictionary</c> where keys are compared by object identity, rather than <c>equals</c>.
	/// 
	/// All external users of this class need to have no knowledge of the IdentityKey - it is all
	/// hidden by this class.
	/// </summary>
	/// <remarks>
	/// Do NOT use a struct/System.Value type as the key for this Hashtable - only classes.  See
	/// the google thread 
	/// http://groups.google.com/groups?hl=en&lr=&ie=UTF-8&oe=UTF-8&threadm=bds2rm%24ruc%241%40charly.heeg.de&rnum=1&prev=/groups%3Fhl%3Den%26lr%3D%26ie%3DUTF-8%26oe%3DUTF-8%26q%3DSystem.Runtime.CompilerServices.RuntimeHelpers.GetHashCode%26sa%3DN%26tab%3Dwg
	/// about why using Structs is a bad thing.
	/// <p>
	/// If I understand it correctly, the first call to get an object defined by a DateTime("2003-01-01")
	/// would box the DateTime and return the identity key for the box.  If you were to get that Key and
	/// unbox it into a DateTime struct, then the next time you passed it in as the Key the IdentityMap
	/// would box it again (into a different box) and it would have a different IdentityKey - so you would
	/// not get the same value for the same DateTime value. 
	/// </p>
	/// </remarks>
	[Serializable]
	public sealed class IdentityMap : IDictionary 
	{
		
		// key = IdentityKey of the passed in Key
		// value = object passed in
		IDictionary map;

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(IdentityMap));
		
		
		/// <summary>
		/// Create a new instance of the IdentityMap that has no 
		/// iteration order.
		/// </summary>
		/// <returns>A new IdentityMap based on a Hashtable.</returns>
		public static IDictionary Instantiate() 
		{
			return new IdentityMap(new Hashtable());
		}

		/// <summary>
		/// Create a new instance of the IdentityMap that has an 
		/// iteration order of the order the objects were added
		/// to the Map.
		/// </summary>
		/// <returns>A new IdentityMap based on ListDictionary.</returns>
		public static IDictionary InstantiateSequenced() 
		{
			return new IdentityMap(new ListDictionary());
		}
		
		/// <summary>
		/// Return the Dictionary Entries (as instances of <c>DictionaryEntry</c> in a collection
		/// that is safe from concurrent modification).  Ie - we may safely add new instances
		/// to the underlying <c>IDictionary</c> during enumeration of the <c>Values</c>.
		/// </summary>
		/// <param name="map">The IDictionary to get the enumeration safe list.</param>
		/// <returns>A Collection of DictionaryEntries</returns>
		public static ICollection ConcurrentEntries(IDictionary map) 
		{
			IList list = new ArrayList(map.Count);
			foreach(DictionaryEntry de in map) 
			{
				DictionaryEntry newEntry = new DictionaryEntry(((IdentityKey)de.Key).Key, de.Value);
				list.Add(newEntry);
			}

			return list;
		}

		/// <summary>
		/// Create the IdentityMap class with the correct class for the IDictionary.
		/// Unsorted = Hashtable
		/// Sorted = ListDictionary
		/// </summary>
		/// <param name="underlyingMap">A class that implements the IDictionary for storing the objects.</param>
		private IdentityMap(IDictionary underlyingMap) 
		{
			this.map = underlyingMap;
		}

		/// <summary>
		/// <see cref="ICollection.Count"/>
		/// </summary>
		public int Count 
		{
			get { return map.Count; }
		}

		/// <summary>
		/// <see cref="ICollection.IsSynchronized"/>
		/// </summary>
		public bool IsSynchronized 
		{
			get { return map.IsSynchronized; }
		}

		/// <summary>
		/// <see cref="ICollection.SyncRoot"/>
		/// </summary>
		public object SyncRoot 
		{
			get { return map.SyncRoot; }
		}

		/// <summary>
		/// <see cref="IDictionary.Add"/>
		/// </summary>
		public void Add(object key, object val) 
		{
			IdentityKey identityKey = new IdentityMap.IdentityKey(key);
			map.Add(identityKey, val);
		}

		/// <summary>
		/// <see cref="IDictionary.Clear"/>
		/// </summary>
		public void Clear() 
		{
			map.Clear();
		}

		/// <summary>
		/// <see cref="IDictionary.Contains"/>
		/// </summary>
		public bool Contains(object key) 
		{
			IdentityKey identityKey = new IdentityMap.IdentityKey(key);
			return map.Contains(identityKey);
		}

		/// <summary>
		/// <see cref="ICollection.GetEnumerator"/>
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() 
		{
			return map.GetEnumerator();
		}

		/// <summary>
		/// <see cref="IDictionary.GetEnumerator"/>
		/// </summary>
		public IDictionaryEnumerator GetEnumerator() 
		{
			return map.GetEnumerator();
		}

		/// <summary>
		/// <see cref="IDictionary.IsFixedSize"/>
		/// </summary>
		public bool IsFixedSize 
		{
			get { return map.IsFixedSize; }
		}

		/// <summary>
		/// <see cref="IDictionary.IsReadOnly"/>
		/// </summary>
		public bool IsReadOnly 
		{
			get { return map.IsReadOnly; }
		}

		/// <summary>
		/// Returns the Keys used in this IdentityMap
		/// <see cref="IDictionary.IsReadOnly"/>
		/// </summary>
		public ICollection Keys 
		{
			get 
			{
				ArrayList keyObjects = new ArrayList(map.Keys.Count);
				foreach(IdentityKey key in map.Keys) 
				{
					keyObjects.Add(key.Key);
				}

				return keyObjects;
			}
		}

		/// <summary>
		/// <see cref="IDictionary.Remove"/>
		/// </summary>
		public void Remove(object key) 
		{
			IdentityKey identityKey = new IdentityMap.IdentityKey(key);
			map.Remove(identityKey);
		}

		/// <summary>
		/// <see cref="IDictionary.Item"/>
		/// </summary>
		public object this [object key] 
		{
			get 
			{ 
				IdentityKey identityKey = new IdentityMap.IdentityKey(key);
				return map[identityKey];
			}
			set 
			{
				IdentityKey identityKey = new IdentityMap.IdentityKey(key);
				map[identityKey] = value;
			}
		}

		/// <summary>
		/// <see cref="IDictionary.Values"/>
		/// </summary>
		public ICollection Values 
		{
			get { return map.Values; }
		}

		

		/// <summary>
		/// <see cref="ICollection.CopyTo"/>
		/// </summary>
		public void CopyTo(Array array, int i) 
		{
			map.CopyTo(array, i);
		}

		/// <summary>
		/// Provides a snapshot VIEW in the form of a Dictionary of the contents of the IdentityMap.
		/// You can safely iterate over this VIEW and modify the actual IdentityMap because the
		/// VIEW is a copy of the contents, not a reference to the existing Map.
		/// 
		/// The view of the IdentityMap where the key-value pair is
		/// key - the underlying object, NOT the IdentityKey
		/// value - the same value object as in the Dictionary
		/// </summary>
		/// <remarks>
		/// In Java there is a Set class so this returns an Set that contains the Map.Entry 
		/// (DictionaryEntry struct) interface.  Since there is no equivalent I am just going to 
		/// return a Dictionary that contains the Key & Value instead of a Set of the Entries
		/// </remarks>
		public IDictionary EntryDictionary 
		{
			get 
			{
				ICollection coll = IdentityMap.ConcurrentEntries(this);
				
				Hashtable entryHashtable = new Hashtable(coll.Count);
				foreach(DictionaryEntry de in coll) 
				{
					entryHashtable.Add(de.Key, de.Value);
				}
				
				return entryHashtable;
			}
		}
		

		/// <summary>
		/// Provides a snapshot VIEW in the form of a List of the contents of the IdentityMap.
		/// You can safely iterate over this VIEW and modify the actual IdentityMap because the
		/// VIEW is a copy of the contents, not a reference to the existing Map.
		/// 
		/// Contains a copy (not that actual instance stored) of the DictionaryEntries in a List.
		/// </summary>
		public IList EntryList 
		{
			get 
			{

				ICollection coll = IdentityMap.ConcurrentEntries(this);
				IList list = new ArrayList(coll.Count);
				foreach(DictionaryEntry de in map) 
				{
					DictionaryEntry newEntry = new DictionaryEntry(de.Key, de.Value);
					list.Add(newEntry);
				}

				return list;
			}
		}


		[Serializable]
		public sealed class IdentityKey  
		{
			private object key;

			internal IdentityKey(Object key) 
			{
				if(key is System.ValueType) {
					throw new ArgumentException("A ValueType can not be used with IdentityKey.  " + 
						"The thread at google has a good description about what happens with boxing " + 
						"and unboxing ValueTypes and why they can not be used as an IdentityKey: " + 
						"http://groups.google.com/groups?hl=en&lr=&ie=UTF-8&oe=UTF-8&threadm=bds2rm%24ruc%241%40charly.heeg.de&rnum=1&prev=/groups%3Fhl%3Den%26lr%3D%26ie%3DUTF-8%26oe%3DUTF-8%26q%3DSystem.Runtime.CompilerServices.RuntimeHelpers.GetHashCode%26sa%3DN%26tab%3Dwg"
						,"key");

				}
				this.key=key;
			}

			public override bool Equals(Object other) 
			{
				return key == ((IdentityKey) other).Key;
			}

			public override int GetHashCode() 
			{
				return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(key);
			}

			public object Key 
			{
				get {return key;}
			}

		}
		
	
		
	}
}
