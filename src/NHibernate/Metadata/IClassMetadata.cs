using NHibernate.Type;

namespace NHibernate.Metadata
{
	/// <summary>
	/// Exposes entity class metadata to the application
	/// </summary>
	public interface IClassMetadata
	{
		/// <summary>
		/// The name of the entity
		/// </summary>
		string EntityName{ get; }

		/// <summary>
		/// The persistent class
		/// </summary>
		System.Type MappedClass { get; }

		/// <summary>
		/// Create a class instance initialized with the given identifier
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		object Instantiate(object id);

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
		/// Get the type of a particular (named) property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		IType GetPropertyType(string propertyName);

		/// <summary>
		/// Get the value of a particular (named) property 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		object GetPropertyValue(object obj, string propertyName);

		/// <summary>
		/// Set the value of a particular (named) property 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		void SetPropertyValue(object obj, string propertyName, object value);

		/// <summary>
		/// Return the values of the mapped properties of the object
		/// </summary>
		/// <param name="entity"></param>
		object[] GetPropertyValues(object entity);

		/// <summary>
		/// Set the given values to the mapped properties of the given object
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="values"></param>
		void SetPropertyValues(object entity, object[] values);

		/// <summary>
		/// Get the identifier of an instance (throw an exception if no identifier property)
		/// </summary>
		object GetIdentifier(object entity);

		/// <summary>
		/// Set the identifier of an instance (or do nothing if no identifier property)
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		void SetIdentifier(object entity, object id);

		/// <summary>
		/// Does the class implement the <c>ILifecycle</c> interface?
		/// </summary>
		bool ImplementsLifecycle { get; }

		/// <summary>
		/// Does the class implement the <c>IValidatable</c> inteface?
		/// </summary>
		bool ImplementsValidatable { get; }

		/// <summary>
		/// Does the class support dynamic proxies?
		/// </summary>
		bool HasProxy { get; }

		/// <summary>
		/// Are instances of this class mutable?
		/// </summary>
		bool IsMutable { get; }

		/// <summary>
		/// Are instances of this class versioned by a timestamp or version number column?
		/// </summary>
		bool IsVersioned { get; }

		/// <summary>
		/// Get the version number (or timestamp) from the object's version property 
		/// (or return null if not versioned)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object GetVersion(object obj);

		/// <summary>
		/// Gets the index of the version property
		/// </summary>
		int VersionProperty { get; }

		/// <summary>
		/// Get the nullability of the class' persistent properties
		/// </summary>
		bool[] PropertyNullability { get; }

		/// <summary>
		/// Does the class have an identifier property?
		/// </summary>
		bool HasIdentifierProperty { get; }
	}
}