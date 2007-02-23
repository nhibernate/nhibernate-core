using System;

using Iesi.Collections;

namespace NHibernate.DomainModel
{
	public interface AbstractProxy : FooProxy
	{
		ISet Abstracts { get; set; }
		DateTime Time { get; set; }
	}
}