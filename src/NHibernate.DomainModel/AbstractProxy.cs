using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public interface AbstractProxy : FooProxy 
	{
		IList abstracts
		{
			get;
			set;
		}
		DateTime time
		{
			get;
			set;
		}
	}
}