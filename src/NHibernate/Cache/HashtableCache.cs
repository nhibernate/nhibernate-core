using System.Collections;

namespace NHibernate.Cache
{
	/// <summary>
	/// A simple <c>Hashtable</c> based cache
	/// </summary>
	public class HashtableCache : ICache
	{
//		private static object synchObject = new object();
		private Hashtable cache = new Hashtable();
		private string region;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="region"></param>
		public HashtableCache( string region )
		{
			this.region = region;
		}

		#region ICache Members

		/// <summary></summary>
		public object Get( object key )
		{
			return cache[ key ];
		}

		/// <summary></summary>
		public void Put( object key, object value )
		{
			cache[ key ] = value;
		}

		/// <summary></summary>
		public void Remove( object key )
		{
			cache.Remove( key );
		}

		/// <summary></summary>
		public void Clear()
		{
			cache.Clear();
		}

		/// <summary>
		/// Destroys the existing cache by setting it to a new Hashtable.
		/// </summary>
		public void Destroy()
		{
			cache = new Hashtable();
		}

		/// <summary></summary>
		public string Region
		{
			set { region = value; }
		}

		#endregion
	}
}