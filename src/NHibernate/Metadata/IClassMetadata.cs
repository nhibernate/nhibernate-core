using System.Collections;
using NHibernate.Type;
using NHibernate.Engine;

namespace NHibernate.Metadata
{
	/// <summary>
	/// Exposes entity class metadata to the application
	/// </summary>
	/// <seealso cref="NHibernate.ISessionFactory.GetClassMetadata(System.Type)"/>
	public interface IClassMetadata
	{
		/// <summary>
		/// The name of the entity
		/// </summary>
		string EntityName{ get; }

		/// <summary>
		/// The name of the identifier property (or return null)
		/// </summary>
		string IdentifierPropertyName { get; }

		/// <summary>
		/// The names of the class' persistent properties
		/// </summary>
		string[] PropertyNames { get; }

		/// <summary>
		/// The identifier Hibernate type
		/// </summary>
		IType IdentifierType { get; }

		/// <summary>
		/// The Hibernate types of the classes properties
		/// </summary>
		IType[] PropertyTypes { get; }

		/// <summary>
		/// Are instances of this class mutable?
		/// </summary>
		bool IsMutable { get; }

		/// <summary>
		/// Are instances of this class versioned by a timestamp or version number column?
		/// </summary>
		bool IsVersioned { get; }

		/// <summary>
		/// Gets the index of the version property
		/// </summary>
		int VersionProperty { get; }

		/// <summary>
		/// Get the nullability of the class' persistent properties
		/// </summary>
		bool[] PropertyNullability { get; }

		/// <summary> Get the "laziness" of the properties of this class</summary>
		bool[] PropertyLaziness { get;}

		/// <summary> Which properties hold the natural id?</summary>
		int[] NaturalIdentifierProperties { get;}

		/// <summary> Does this entity extend a mapped superclass?</summary>
		bool IsInherited { get;}

		#region stuff that is persister-centric and/or EntityInfo-centric

		/// <summary> Get the type of a particular (named) property </summary>
		IType GetPropertyType(string propertyName);

		/// <summary> Does the class support dynamic proxies? </summary>
		bool HasProxy { get; }

		/// <summary> Does the class have an identifier property? </summary>
		bool HasIdentifierProperty { get; }

		/// <summary> Does this entity declare a natural id?</summary>
		bool HasNaturalIdentifier { get; }

		/// <summary> Does this entity have mapped subclasses?</summary>
		bool HasSubclasses { get; }

		#endregion

		#region stuff that is tuplizer-centric, but is passed a session

		/// <summary> Return the values of the mapped properties of the object</summary>
		object[] GetPropertyValuesToInsert(object entity, IDictionary mergeMap, ISessionImplementor session);

		#endregion

		#region stuff that is Tuplizer-centric

		/// <summary>
		/// The persistent class
		/// </summary>
		System.Type GetMappedClass(EntityMode entityMode);

		/// <summary>
		/// Create a class instance initialized with the given identifier
		/// </summary>
		object Instantiate(object id, EntityMode entityMode);

		/// <summary>
		/// Get the value of a particular (named) property 
		/// </summary>
		object GetPropertyValue(object obj, string propertyName, EntityMode entityMode);

		/// <summary> Extract the property values from the given entity. </summary>
		/// <param name="entity">The entity from which to extract the property values. </param>
		/// <param name="entityMode">The entity-mode of the given entity </param>
		/// <returns> The property values. </returns>
		object[] GetPropertyValues(object entity, EntityMode entityMode);

		/// <summary>
		/// Set the value of a particular (named) property 
		/// </summary>
		void SetPropertyValue(object obj, string propertyName, object value, EntityMode entityMode);

		/// <summary>
		/// Set the given values to the mapped properties of the given object
		/// </summary>
		void SetPropertyValues(object entity, object[] values, EntityMode entityMode);

		/// <summary>
		/// Get the identifier of an instance (throw an exception if no identifier property)
		/// </summary>
		object GetIdentifier(object entity, EntityMode entityMode);

		/// <summary>
		/// Set the identifier of an instance (or do nothing if no identifier property)
		/// </summary>
		void SetIdentifier(object entity, object id, EntityMode entityMode);

		/// <summary> Does the class implement the <see cref="Classic.ILifecycle"/> interface?</summary>
		bool ImplementsLifecycle(EntityMode entityMode);

		/// <summary> Does the class implement the <see cref="Classic.IValidatable"/> interface?</summary>
		bool ImplementsValidatable(EntityMode entityMode);

		/// <summary>
		/// Get the version number (or timestamp) from the object's version property 
		/// (or return null if not versioned)
		/// </summary>
		object GetVersion(object obj, EntityMode entityMode);

		#endregion
	}
}