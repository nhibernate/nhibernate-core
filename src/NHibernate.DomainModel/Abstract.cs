using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public abstract class Abstract : Foo, AbstractProxy
	{
		// added an initialization because MsSql errors out when inserting
		// dates outside of the range - TODO: fix this to be a DATE type
		private DateTime _time = new DateTime(2001, 12, 1, 1, 1, 1);
		private ISet<object> _abstracts;

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
		public ISet<object> Abstracts
		{
			get { return _abstracts; }
			set { _abstracts = value; }
		}
	}
}