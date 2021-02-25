using System;

namespace NHibernate.Test.DynamicEntity
{
	[Obsolete("Require dynamic proxies")]
	public interface IProxyMarker
	{
		DataProxyHandler DataHandler { get; }
	}
}
