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
		/// Build a proxy using the Apache.Avalon.DynamicProxy library.
		/// </summary>
		/// <param name="persistentClass">The PersistentClass to proxy.</param>
		/// <param name="interfaces">The extra interfaces the Proxy should implement.</param>
		/// <param name="identifierPropertyInfo">The PropertyInfo to get/set the Id.</param>
		/// <param name="id">The value for the Id.</param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		INHibernateProxy GetProxy(System.Type persistentClass, System.Type[] interfaces, PropertyInfo identifierPropertyInfo, object id, ISessionImplementor session);
	}
}
