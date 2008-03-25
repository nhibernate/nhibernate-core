using System;

namespace NHibernate.DomainModel
{
	public interface BarProxy : AbstractProxy
	{
		Baz Baz { get; set; }
		FooComponent BarComponent { get; set; }
		string BarString { get; }
		object Object { get; set; }
	}
}