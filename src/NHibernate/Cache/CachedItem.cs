using System;

namespace NHibernate.Cache
{
	/// <summary>
	/// An item of cached data, timestamped with the time it was cached, when it was locked,
	/// when it was unlocked
	/// </summary>
	[Serializable]
	public class CachedItem
	{
		private long freshTimestamp;
		private bool fresh;
		private long unlockTimestamp;
		private int theLock;
		private object value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public CachedItem(object value)
		{
			this.value = value;
			freshTimestamp = Timestamper.Next();
			fresh = true;
			unlockTimestamp = -1;
		}

		/// <summary>
		/// The timestamp on the Cached Data.
		/// </summary>
		public long FreshTimestamp
		{
			get { return freshTimestamp; }
		}

		/// <summary>
		/// 
		/// </summary>
		public long UnlockTimestamp
		{
			get { return unlockTimestamp; }
		}

		/// <summary>
		/// The actual cached Data.
		/// </summary>
		public object Value
		{
			get { return value; }
		}

		/// <summary>
		/// A boolean indicating if the Cached Item is fresh.
		/// </summary>
		/// <value>true if the CachedItem has not ever been locked.</value>
		public bool IsFresh
		{
			get { return fresh; }
		}

		/// <summary>
		/// Lock the Item.
		/// </summary>
		public void Lock()
		{
			if( 0 == theLock++ )
			{
				fresh = false;
				value = null;
			}
		}

		/// <summary>
		/// Unlock the Item
		/// </summary>
		public void Unlock()
		{
			if( --theLock == 0 )
			{
				unlockTimestamp = Timestamper.Next();
			}
		}

		/// <summary>
		/// Value indicating if the CachedItem is unlocked. 
		/// </summary>
		/// <value>true if there are no locks on the CachedItem.</value>
		public bool IsUnlocked
		{
			get { return theLock == 0; }
		}

	}
}