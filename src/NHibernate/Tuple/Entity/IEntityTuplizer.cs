using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Tuple.Entity
{
	/// <summary> 
	/// Defines further responsibilities regarding tuplization based on a mapped entity.
	/// </summary>
	/// <remarks>
	/// EntityTuplizer implementations should have the following constructor signature:
	/// (<see cref="EntityMetamodel"/>, <see cref="Mapping.PersistentClass"/>)
	/// </remarks>
	public interface IEntityTuplizer : ITuplizer
	{
		/// <summary> 
		/// Does the <see cref="ITuplizer.MappedClass">class</see> managed by this tuplizer implement
		/// the <see cref="NHibernate.Classic.ILifecycle"/> interface. 
		/// </summary>
		/// <returns> True if the ILifecycle interface is implemented; false otherwise. </returns>
		bool IsLifecycleImplementor { get; }

		/// <summary> 
		/// Does the <see cref="ITuplizer.MappedClass">class</see> managed by this tuplizer implement
		/// the <see cref="NHibernate.Classic.IValidatable"/> interface. 
		/// </summary>
		/// <returns> True if the IValidatable interface is implemented; false otherwise. </returns>
		bool IsValidatableImplementor { get; }

		// TODO: getConcreteProxyClass() is solely used (externally) to perform narrowProxy()
		// would be great to fully encapsulate that narrowProxy() functionality within the
		// Tuplizer, itself, with a Tuplizer.narrowProxy(..., PersistentContext) method
		/// <summary> Returns the java class to which generated proxies will be typed. </summary>
		/// <returns> The .NET class to which generated proxies will be typed </returns>
		System.Type ConcreteProxyClass { get; }

		/// <summary> Is it an instrumented POCO?</summary>
		bool IsInstrumented { get; }

		/// <summary> Create an entity instance initialized with the given identifier. </summary>
		/// <param name="id">The identifier value for the entity to be instantiated. </param>
		/// <returns> The instantiated entity. </returns>
		object Instantiate(object id);

		/// <summary> Extract the identifier value from the given entity. </summary>
		/// <param name="entity">The entity from which to extract the identifier value. </param>
		/// <returns> The identifier value. </returns>
		object GetIdentifier(object entity);

		/// <summary> 
		/// Inject the identifier value into the given entity.
		/// </summary>
		/// <param name="entity">The entity to inject with the identifier value.</param>
		/// <param name="id">The value to be injected as the identifier. </param>
		/// <remarks>Has no effect if the entity does not define an identifier property</remarks>
		void SetIdentifier(object entity, object id);

		/// <summary> 
		/// Inject the given identifier and version into the entity, in order to
		/// "roll back" to their original values. 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="currentId">The identifier value to inject into the entity. </param>
		/// <param name="currentVersion">The version value to inject into the entity. </param>
		void ResetIdentifier(object entity, object currentId, object currentVersion);

		/// <summary> Extract the value of the version property from the given entity. </summary>
		/// <param name="entity">The entity from which to extract the version value. </param>
		/// <returns> The value of the version property, or null if not versioned. </returns>
		object GetVersion(object entity);

		/// <summary> Inject the value of a particular property. </summary>
		/// <param name="entity">The entity into which to inject the value. </param>
		/// <param name="i">The property's index. </param>
		/// <param name="value">The property value to inject. </param>
		void SetPropertyValue(object entity, int i, object value);

		/// <summary> Inject the value of a particular property. </summary>
		/// <param name="entity">The entity into which to inject the value. </param>
		/// <param name="propertyName">The name of the property. </param>
		/// <param name="value">The property value to inject. </param>
		void SetPropertyValue(object entity, string propertyName, object value);

		/// <summary> Extract the values of the insertable properties of the entity (including backrefs) </summary>
		/// <param name="entity">The entity from which to extract. </param>
		/// <param name="mergeMap">a map of instances being merged to merged instances </param>
		/// <param name="session">The session in which the request is being made. </param>
		/// <returns> The insertable property values. </returns>
		object[] GetPropertyValuesToInsert(object entity, IDictionary mergeMap, ISessionImplementor session);

		/// <summary> Extract the value of a particular property from the given entity. </summary>
		/// <param name="entity">The entity from which to extract the property value. </param>
		/// <param name="propertyName">The name of the property for which to extract the value. </param>
		/// <returns> The current value of the given property on the given entity. </returns>
		object GetPropertyValue(object entity, string propertyName);

		/// <summary> Called just after the entities properties have been initialized. </summary>
		/// <param name="entity">The entity being initialized. </param>
		/// <param name="lazyPropertiesAreUnfetched">Are defined lazy properties currently unfecthed </param>
		/// <param name="session">The session initializing this entity. </param>
		void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session);

		/// <summary> Does this entity, for this mode, present a possibility for proxying? </summary>
		/// <value> True if this tuplizer can generate proxies for this entity. </value>
		bool HasProxy { get;}

		/// <summary> 
		/// Generates an appropriate proxy representation of this entity for this entity-mode.
		///  </summary>
		/// <param name="id">The id of the instance for which to generate a proxy. </param>
		/// <param name="session">The session to which the proxy should be bound. </param>
		/// <returns> The generate proxies. </returns>
		object CreateProxy(object id, ISessionImplementor session);

		/// <summary> Does the given entity instance have any currently uninitialized lazy properties? </summary>
		/// <param name="entity">The entity to be check for uninitialized lazy properties. </param>
		/// <returns> True if uninitialized lazy properties were found; false otherwise. </returns>
		bool HasUninitializedLazyProperties(object entity);
	}
}
