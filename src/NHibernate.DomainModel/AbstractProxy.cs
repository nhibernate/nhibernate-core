using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public interface AbstractProxy : FooProxy 
	{
		IDictionary Abstracts
		{
			get;
			set;
		}
		DateTime Time
		{
			get;
			set;
		}
	}
}