using System;
using System.Collections;
using System.Data;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	/// <summary>
	/// This class maps a DbType to names.
	/// </summary>
	/// <remarks>
	/// Associations may be marked with a capacity. Calling the <c>Get()</c>
	/// method with a type and actual size n will return the associated
	/// name with smallest capacity >= n, if available and an unmarked
	/// default type otherwise.
	/// Eg, setting
	/// <code>
	///		Names.Put(DbType,			"TEXT" );
	///		Names.Put(DbType,	255,	"VARCHAR($1)" );
	///		Names.Put(DbType,	65534,	"LONGVARCHAR($1)" );
	/// </code>
	/// will give you back the following:
	/// <code>
	///		Names.Get(DbType)			// --> "TEXT" (default)
	///		Names.Get(DbType,100)		// --> "VARCHAR(100)" (100 is in [0:255])
	///		Names.Get(DbType,1000)	// --> "LONGVARCHAR(1000)" (100 is in [256:65534])
	///		Names.Get(DbType,100000)	// --> "TEXT" (default)
	/// </code>
	/// On the other hand, simply putting
	/// <code>
	///		Names.Put(DbType, "VARCHAR($1)" );
	/// </code>
	/// would result in
	/// <code>
	///		Names.Get(DbType)			// --> "VARCHAR($1)" (will cause trouble)
	///		Names.Get(DbType,100)		// --> "VARCHAR(100)" 
	///		Names.Get(DbType,1000)	// --> "VARCHAR(1000)"
	///		Names.Get(DbType,10000)	// --> "VARCHAR(10000)"
	/// </code>
	/// </remarks>
	public class TypeNames
	{
		private string placeholder;
		private Hashtable weighted = new Hashtable();
		private Hashtable defaults = new Hashtable();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="placeholder">String to be replaced by actual size/length in type names</param>
		public TypeNames( string placeholder )
		{
			this.placeholder = placeholder;
		}

		/// <summary>
		/// Get default type name for specified type
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <returns>the default type name associated with the specified key</returns>
		public string Get( DbType typecode )
		{
			string result = ( string ) defaults[ typecode ];

			if( result == null )
			{
				throw new ArgumentException("Dialect does not support DbType." + typecode,
					"typecode");
			}
			return result;
		}

		/// <summary>
		/// Get the type name specified type and size
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <param name="size">the (maximum) type size/length</param>
		/// <returns>
		/// The associated name with smallest capacity >= size if available and the
		/// default type name otherwise
		/// </returns>
		public string Get( DbType typecode, int size )
		{
			IDictionary map = weighted[ typecode ] as IDictionary;
			if( map != null && map.Count > 0 )
			{
				foreach( int entrySize in map.Keys )
				{
					if( size <= entrySize )
					{
						return StringHelper.ReplaceOnce(
							( string ) map[ entrySize ],
							placeholder,
							size.ToString()
							);
					}
				}
			}
			return StringHelper.ReplaceOnce( Get( typecode ), placeholder, size.ToString() );
		}

		/// <summary>
		/// Set a type name for specified type key and capacity
		/// </summary>
		/// <param name="typecode">the type key</param>
		/// <param name="capacity">the (maximum) type size/length</param>
		/// <param name="value">The associated name</param>
		public void Put( DbType typecode, int capacity, string value )
		{
			SequencedHashMap map = weighted[ typecode ] as SequencedHashMap;
			if( map == null )
			{
				weighted[ typecode ] = map = new SequencedHashMap();
			}
			map[ capacity ] = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typecode"></param>
		/// <param name="value"></param>
		public void Put( DbType typecode, string value )
		{
			defaults[ typecode ] = value;
		}

	}
}