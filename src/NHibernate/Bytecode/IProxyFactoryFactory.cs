using NHibernate.Proxy;

namespace NHibernate.Bytecode
{
	/// <summary> 
	/// An interface for factories of <see cref="IProxyFactory">proxy factory</see> instances.
	/// </summary>
	/// <remarks>
	/// Used to abstract from the tupizer.
	/// </remarks>
	public interface IProxyFactoryFactory
	{
		/// <summary> 
		/// Build a proxy factory specifically for handling runtime
		/// lazy loading. 
		/// </summary>
		/// <returns> The lazy-load proxy factory. </returns>
		IProxyFactory BuildProxyFactory();

		/*
		/// <summary> Build a proxy factory for basic proxy concerns.  The return
		/// should be capable of properly handling newInstance() calls.
		/// <p/>
		/// Should build basic proxies essentially equivalent to JDK proxies in
		/// terms of capabilities, but should be able to deal with abstract super
		/// classes in addition to proxy interfaces.
		/// <p/>
		/// Must pass in either superClass or interfaces (or both).
		/// 
		/// </summary>
		/// <param name="superClass">The abstract super class (or null if none).</param>
		/// <param name="interfaces">Interfaces to be proxied (or null if none).</param>
		/// <returns> The proxy class</returns>
		// TODO: H3.2
		//BasicProxyFactory BuildBasicProxyFactory(System.Type superClass, System.Type[] interfaces);
		 */

		IProxyValidator ProxyValidator { get; }

		bool IsInstrumented(System.Type entityClass);

	    bool IsProxy(object entity);
	}
}