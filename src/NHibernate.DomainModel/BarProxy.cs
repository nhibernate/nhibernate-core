using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public interface BarProxy : AbstractProxy 
	{
		Baz baz
		{
			get;
			set;
		}
		FooComponent barComponent
		{
			get;
			set;
		}
		string barString
		{
			get;
		}
		object @object
		{
			get;
			set;
		}
	}
}