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

		public CachedItem(object value) 
		{
			this.value = value;
			freshTimestamp = Timestamper.Next();
			fresh = true;
			unlockTimestamp = -1;
		}

		public long FreshTimestamp 
		{
			get { return freshTimestamp; }
		}

		public long UnlockTimestamp 
		{
			get { return unlockTimestamp; }
		}

		public object Value 
		{
			get { return value; }
		}

		public bool IsFresh 
		{
			get { return fresh; }
		}

		public void Lock() 
		{
			if ( 0 == theLock++ ) 
			{
				fresh = false;
				value = null;
			}
		}
		public void Unlock() 
		{
			if ( --theLock == 0 ) 
			{
				unlockTimestamp = Timestamper.Next();
			}
		}
		public bool IsUnlocked 
		{
			get { return theLock == 0; }
		}

	}
}
