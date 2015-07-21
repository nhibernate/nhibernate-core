using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	public interface GlarchProxy
	{
		int Version { get; set; }
		int DerivedVersion { get; }

		string Name { get; set; }

		GlarchProxy Next { get; set; }

		short Order { get; set; }

		IList<string> Strings { get; set; }

		IDictionary DynaBean { get; set; }

		ISet<string> StringSets { get; set; }
		IList<FooComponent> FooComponents { get; set; }
		GlarchProxy[] ProxyArray { get; set; }
		ISet<GlarchProxy> ProxySet { get; set; }
		Multiplicity Multiple { get; set; }
		object Any { get; set; }
	}
}