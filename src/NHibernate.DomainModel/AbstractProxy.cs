using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public interface AbstractProxy : FooProxy 
	{
		Iesi.Collections.ISet Abstracts
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