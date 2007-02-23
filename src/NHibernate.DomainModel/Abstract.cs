using System;

using Iesi.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
	public abstract class Abstract : Foo, AbstractProxy
	{
		// added an initialization because MsSql errors out when inserting
		// dates outside of the range - TODO: fix this to be a DATE type
		private DateTime _time = new DateTime(2001, 12, 1, 1, 1, 1);
		private ISet _abstracts;

		/// <summary>
		/// Gets or sets the _time
		/// </summary> 
		public DateTime Time
		{
			get { return _time; }
			set { _time = value; }
		}

		/// <summary>
		/// Gets or sets the _abstract
		/// </summary> 
		public ISet Abstracts
		{
			get { return _abstracts; }
			set { _abstracts = value; }
		}
	}
}