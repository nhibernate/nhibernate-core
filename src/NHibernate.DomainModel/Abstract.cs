using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public abstract class Abstract : Foo, AbstractProxy
	{
	
		/// <summary>
		/// Holds the _time
		/// </summary> 
		private DateTime _time;

		/// <summary>
		/// Gets or sets the _time
		/// </summary> 
		public DateTime Time
		{
			get 
			{
				return _time; 
			}
			set 
			{
				_time = value;
			}
		}
		/// <summary>
		/// Holds the _abstract
		/// </summary> 
		private IList _abstracts;

		/// <summary>
		/// Gets or sets the _abstract
		/// </summary> 
		public IList Abstracts
		{
			get 
			{
				return _abstracts; 
			}
			set 
			{
				_abstracts = value;
			}
		}
	}
}