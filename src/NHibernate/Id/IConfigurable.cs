using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate.Id {
	
	public interface IConfigurable {
		
		void Configure(IType type, IDictionary parms, Dialect.Dialect d);
	}
}
