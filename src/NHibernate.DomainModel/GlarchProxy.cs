using System;
using System.Collections;

using Iesi.Collections;

namespace NHibernate.DomainModel
{
	public interface GlarchProxy
	{
		int Version { get; set; }
		int DerivedVersion { get; }

		string Name { get; set; }

		GlarchProxy Next { get; set; }

		short Order { get; set; }

		IList Strings { get; set; }

		IDictionary DynaBean { get; set; }

		ISet StringSets { get; set; }
		IList FooComponents { get; set; }
		GlarchProxy[] ProxyArray { get; set; }
		ISet ProxySet { get; set; }
		Multiplicity Multiple { get; set; }
		object Any { get; set; }
	}
}