using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public interface AbstractProxy : FooProxy 
	{
		IList Abstracts
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