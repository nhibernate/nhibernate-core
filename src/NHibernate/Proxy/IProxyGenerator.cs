using System;
using System.Reflection;

using NHibernate.Engine;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Summary description for IProxyGenerator.
	/// </summary>
	public interface IProxyGenerator
	{
		
		/// <summary>
		/// Build a proxy using the Castle.DynamicProxy library.
		/// </summary>
		/// <param name="persistentClass">The PersistentClass to proxy.</param>
		/// <param name="interfaces">The extra interfaces the Proxy should implement.</param>
		/// <param name="identifierPropertyInfo">The PropertyInfo to get/set the Id.</param>
		/// <param name="id">The value for the Id.</param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		INHibernateProxy GetProxy(System.Type persistentClass, System.Type concreteProxy, System.Type[] interfaces, PropertyInfo identifierPropertyInfo, object id, ISessionImplementor session);

		/// <summary>
		/// Gets the <see cref="LazyInitializer"/> that is used by the Proxy.
		/// </summary>
		/// <param name="proxy">The Proxy object</param>
		/// <returns>The <see cref="LazyInitializer"/> that contains the details of the Proxied object.</returns>
		LazyInitializer GetLazyInitializer(INHibernateProxy proxy); 

		/// <summary>
		/// Convenience method to figure out the underlying type for the object regardless of it
		/// is a Proxied object or the real object.
		/// </summary>
		/// <param name="obj">The object to get the type of.</param>
		/// <returns>The Underlying Type for the object regardless of if it is a Proxy.</returns>
		System.Type GetClass(object obj); 
	}
}
