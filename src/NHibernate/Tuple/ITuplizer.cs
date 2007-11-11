namespace NHibernate.Tuple
{
	/// <summary> 
	/// A tuplizer defines the contract for things which know how to manage
	/// a particular representation of a piece of data, given that
	/// representation's {@link org.hibernate.EntityMode} (the entity-mode
	/// essentially defining which representation).
	/// </p>
	/// If that given piece of data is thought of as a data structure, then a tuplizer
	/// is the thing which knows how to<ul>
	/// <li>create such a data structure appropriately
	/// <li>extract values from and inject values into such a data structure
	/// </ul>
	/// </p>
	/// For example, a given piece of data might be represented as a POJO class.
	/// Here, it's representation and entity-mode is POJO.  Well a tuplizer for POJO
	/// entity-modes would know how to<ul>
	/// <li>create the data structure by calling the POJO's constructor
	/// <li>extract and inject values through getters/setter, or by direct field access, etc
	/// </ul>
	/// </p>
	/// That same piece of data might also be represented as a DOM structure, using
	/// the tuplizer associated with the DOM4J entity-mode, which would generate instances
	/// of {@link org.dom4j.Element} as the data structure and know how to access the
	/// values as either nested {@link org.dom4j.Element}s or as {@link org.dom4j.Attribute}s.
	///  </summary>
	/// <seealso cref="Entity.IEntityTuplizer"/>
	/// <seealso cref="Component.IComponentTuplizer"/>
	public interface ITuplizer
	{
		// TODO : be really nice to not have this here since it is essentially pojo specific...

		/// <summary> 
		/// Return the pojo class managed by this tuplizer.
		/// </summary>
		/// <returns> The persistent class. </returns>
		/// <remarks>
		/// Need to determine how to best handle this for the Tuplizers for EntityModes
		/// other than POCO.
		/// </remarks>
		System.Type MappedClass { get;}

		/// <summary> 
		/// Extract the current values contained on the given entity. 
		/// </summary>
		/// <param name="entity">The entity from which to extract values. </param>
		/// <returns> The current property values. </returns>
		/// <throws>  HibernateException </throws>
		object[] GetPropertyValues(object entity);

		/// <summary> Inject the given values into the given entity. </summary>
		/// <param name="entity">The entity. </param>
		/// <param name="values">The values to be injected. </param>
		void SetPropertyValues(object entity, object[] values);

		/// <summary> Extract the value of a particular property from the given entity. </summary>
		/// <param name="entity">The entity from which to extract the property value. </param>
		/// <param name="i">The index of the property for which to extract the value. </param>
		/// <returns> The current value of the given property on the given entity. </returns>
		object GetPropertyValue(object entity, int i);

		/// <summary> Generate a new, empty entity. </summary>
		/// <returns> The new, empty entity instance. </returns>
		object Instantiate();

		/// <summary> 
		/// Is the given object considered an instance of the the entity (acconting
		/// for entity-mode) managed by this tuplizer. 
		/// </summary>
		/// <param name="obj">The object to be checked. </param>
		/// <returns> True if the object is considered as an instance of this entity within the given mode. </returns>
		bool IsInstance(object obj);
	}
}