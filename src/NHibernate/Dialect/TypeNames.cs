using System;
using System.Data;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Dialect {

	/// <summary>
	/// This class maps a type to names.
	/// </summary>
	/// <remarks>
	/// Associations may be marked with a capacity. Calling the <c>Get()</c>
	/// method with a type and actual size n will return the associated
	/// name with smallest capacity >= n, if available and an unmarked
	/// default type otherwise.
	/// Eg, setting
	/// <code>
	///		Names.Put(type,			"TEXT" );
	///		Names.Put(type,	255,	"VARCHAR($1)" );
	///		Names.Put(type,	65534,	"LONGVARCHAR($1)" );
	/// </code>
	/// will give you back the following:
	/// <code>
	///		Names.Get(type)			// --> "TEXT" (default)
	///		Names.Get(type,100)		// --> "VARCHAR(100)" (100 is in [0:255])
	///		Names.Get(type,1000)	// --> "LONGVARCHAR(1000)" (100 is in [256:65534])
	///		Names.Get(type,100000)	// --> "TEXT" (default)
	/// </code>
	/// On the other hand, simply putting
	/// <code>
	///		Names.Put(type, "VARCHAR($1)" );
	/// </code>
	/// would result in
	/// <code>
	///		Names.Get(type)			// --> "VARCHAR($1)" (will cause trouble)
	///		Names.Get(type,100)		// --> "VARCHAR(100)" 
	///		Names.Get(type,1000)	// --> "VARCHAR(1000)"
	///		Names.Get(type,10000)	// --> "VARCHAR(10000)"
	/// </code>
	/// </remarks>
	public class TypeNames {
		private string placeholder;
		private Hashtable weighted = new Hashtable();
		private Hashtable defaults = new Hashtable();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="placeholder">String to be replaced by actual size/length in type names</param>
		public TypeNames(string placeholder) {
			this.placeholder = placeholder;
		}

		/// <summary>
		/// Get default type name for specified type
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <returns>the default type name associated with the specified key</returns>
		public string Get(DbType typecode) {
			return (string) defaults[typecode];
		}

		/// <summary>
		/// Get the type name specified type and size
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <param name="size">the (maximum) type size/length</param>
		/// <returns>The associated name with smallest capacity >= size if available and the
		/// default type name otherwise</returns>
		public string Get(DbType typecode, int size) {
			IDictionary map = weighted[typecode] as IDictionary;
			if (map != null && map.Count > 0) {
				foreach(int entrySize in map.Keys) {
					if (size <= entrySize) {
						return StringHelper.ReplaceOnce(
							(string) map[entrySize],
							placeholder,
							size.ToString()
							);
					}
				}
			}
			return StringHelper.ReplaceOnce(Get(typecode), placeholder, size.ToString());
		}

		/// <summary>
		/// Set a type name for specified type key and capacity
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <param name="size">the (maximum) type size/length</param>
		/// <param name="value">The associated name</param>
		public void Put(DbType typecode, int capacity, string value) {
			SequencedHashMap map = weighted[ typecode ] as SequencedHashMap;
			if (map==null)
				weighted[typecode] = map = new SequencedHashMap();
			map[capacity] = value;
		}

		public void Put(DbType typecode, string value) {
			defaults[typecode] = value;
		}

	}
}
