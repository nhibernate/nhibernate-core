namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Interface describing an object that knows how to create Interceptors.
	/// Technically this is just an interceptor factory, but it is designed
	/// to be used by clients who want to use stateful interceptors in conjunction
	/// with sharded sessions.  Clients should make sure their Interceptor
	/// implementation implements this interface.  Furthermore, if the
	/// Interceptor implementation requires a reference to the Session, the
	/// Interceptor returned by NewInstance() should implement the <see cref="IRequiresSession"/>
	/// interface.
	/// </summary>
	public interface IStatefulInterceptorFactory
	{
		IInterceptor NewInstance();
	}
}