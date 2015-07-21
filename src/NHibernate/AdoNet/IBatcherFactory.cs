using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	/// <summary> Factory for <see cref="IBatcher"/> instances.</summary>
	public interface IBatcherFactory
	{
		IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor);
	}
}