using System;

namespace NHibernate.Proxy {
	
	public interface IHibernateProxy {
		object WriteReplace();
	}
}
