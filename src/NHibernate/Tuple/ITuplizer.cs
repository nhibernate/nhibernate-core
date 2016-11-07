using NHibernate;

namespace NHibernate.Tuple
{
	/// <summary> 
	/// A tuplizer defines the contract for things which know how to manage
	/// a particular representation of a piece of data, given that
	/// representation's <see cref="EntityMode"/> (the entity-mode
	/// essentially defining which representation).
	/// </summary>
	/// <remarks>
	/// If that given piece of data is thought of as a data structure, then a tuplizer
	/// is the thing which knows how to:
	/// <list type="bullet">
	/// <item><description>create such a data structure appropriately</description></item>
	/// <item><description>extract values from and inject values into such a data structure</description></item>
	/// </list>
	/// <para/>
	/// For example, a given piece of data might be represented as a POCO class.
	/// Here, it's representation and entity-mode is POCO.  Well a tuplizer for POCO
	/// entity-modes would know how to:
	/// <list type="bullet">
	/// <item><description>create the data structure by calling the POCO's constructor</description></item>
	/// <item><description>extract and inject values through getters/setter, or by direct field access, etc</description></item>
	/// </list>
	/// <para/>
	/// That same piece of data might also be represented as a DOM structure, using
	/// the tuplizer associated with the XML entity-mode, which would generate instances
	/// of <see cref="System.Xml.XmlElement"/> as the data structure and know how to access the
	/// values as either nested <see cref="System.Xml.XmlElement"/>s or as <see cref="System.Xml.XmlAttribute"/>s.
	/// </remarks>
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
		/// Is the given object considered an instance of the the entity (accounting
		/// for entity-mode) managed by this tuplizer. 
		/// </summary>
		/// <param name="obj">The object to be checked. </param>
		/// <returns> True if the object is considered as an instance of this entity within the given mode. </returns>
		bool IsInstance(object obj);
	}
}