using System.Collections.Generic;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Proxy
{
	/// <summary> Contract for run-time, proxy-based lazy initialization proxies. </summary>
	public interface IProxyFactory
	{
		/// <summary> Called immediately after instantiation of this factory. </summary>
		/// <param name="entityName">
		/// The name of the entity for which this factory should generate proxies. 
		/// </param>
		/// <param name="persistentClass">
		/// The entity class for which to generate proxies; not always the same as the entityName.
		/// </param>
		/// <param name="interfaces">
		/// The interfaces to expose in the generated proxy;
		/// <see cref="INHibernateProxy"/> is already included in this collection.
		/// </param>
		/// <param name="getIdentifierMethod">
		/// Reference to the identifier getter method; invocation on this method should not force initialization
		/// </param>
		/// <param name="setIdentifierMethod">
		/// Reference to the identifier setter method; invocation on this method should not force initialization
		/// </param>
		/// <param name="componentIdType">
		/// For composite identifier types, a reference to
		/// the <see cref="ComponentType">type</see> of the identifier
		/// property; again accessing the id should generally not cause
		/// initialization - but need to bear in mind key-many-to-one
		/// mappings.
		/// </param>
		///  <exception cref="HibernateException"> Indicates a problem completing post </exception>
		/// <remarks>
		/// Essentially equivalent to constructor injection, but contracted
		/// here via interface.
		/// </remarks>
		void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces,
			MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType);

		/// <summary>
		/// Create a new proxy
		/// </summary>
		/// <param name="id">The id value for the proxy to be generated.</param>
		/// <param name="session">The session to which the generated proxy will be associated.</param>
		/// <returns>The generated proxy.</returns>
		/// <exception cref="HibernateException">Indicates problems generating requested proxy.</exception>
		INHibernateProxy GetProxy(object id, ISessionImplementor session);

		object GetFieldInterceptionProxy(object instanceToWrap);
	}
}
