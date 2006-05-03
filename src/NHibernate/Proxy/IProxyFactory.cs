using System;
using System.Reflection;

using Iesi.Collections;

using NHibernate.Engine;

namespace NHibernate.Proxy
{
	public interface IProxyFactory
	{
		/// <summary>
		/// Called immediately after instantiation
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <param name="interfaces"></param>
		/// <param name="getIdentifierMethod"></param>
		/// <param name="setIdentifierMethod"></param>
		void PostInstantiate(
			System.Type persistentClass,
			ISet interfaces,
			MethodInfo getIdentifierMethod,
			MethodInfo setIdentifierMethod );
		
		/// <summary>
		/// Create a new proxy
		/// </summary>
		/// <param name="id">The id value for the proxy to be generated.</param>
		/// <param name="session">The session to which the generated proxy will be
		/// associated.</param>
		/// <returns>The generated proxy.</returns>
		/// <exception cref="HibernateException">Indicates problems generating
		/// requested proxy.</exception>
		INHibernateProxy GetProxy( object id, ISessionImplementor session );
	}
}