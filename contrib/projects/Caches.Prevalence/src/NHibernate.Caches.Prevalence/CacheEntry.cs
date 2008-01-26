using System;

namespace NHibernate.Caches.Prevalence
{
	/// <summary>
	/// An item in the cache
	/// </summary>
	[Serializable]
	internal class CacheEntry
	{
		private object _key;
		private object _value;
		private DateTime _dateCreated;

		/// <summary>
		/// the unique identifier
		/// </summary>
		public object Key
		{
			get { return _key; }
			set { _key = value; }
		}

		/// <summary>
		/// the value
		/// </summary>
		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		/// <summary>
		/// the unique timestamp
		/// </summary>
		public DateTime DateCreated
		{
			get { return _dateCreated; }
			set { _dateCreated = value; }
		}
	}
}