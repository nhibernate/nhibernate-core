using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
	public abstract class Abstract : Foo, AbstractProxy
	{
		private DateTime _time;
		private Iesi.Collections.ISet _abstracts;

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
		public Iesi.Collections.ISet Abstracts
		{
			get { return _abstracts; }
			set { _abstracts = value; }
		}
	}
}