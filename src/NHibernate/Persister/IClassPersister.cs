using System;
using System.Reflection;
using NHibernate.Type;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;

namespace NHibernate.Persister {

	/// <summary>
	/// Concrete <c>IClassPersister</c>s implement mapping and persistence logic for a particular class.
	/// </summary>
	/// <remarks>
	/// Implementors must be threadsafe (preferrably immutable) and must provide a constructor of type
	/// (PersistentClass, SessionFactoryImplementor)
	/// </remarks>
	public interface IClassPersister {
		
		/// <summary>
		/// Finish the initialization of this object, once all <c>ClassPersisters</c> have been
		/// instantiated. Called only once, before any other method.
		/// </summary>
		/// <param name="factory"></param>
		void PostInstatiate(ISessionFactoryImplementor factory);

		/// <summary>
		/// Returns an object that identifies the space in which identifiers of this class hierarchy
		/// are uniue. eg. a table name, etc.
		/// </summary>
		object IdentifierSpace { get; }

		/// <summary>
		/// Returns an array of objects that identifies spaces in which properties of this class
		/// instance are persisted. eg. table names.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		object[] GetPropertySpaces(object instance);

		/// <summary>
		/// The persistent class
		/// </summary>
		System.Type MappedClass { get; }

		/// <summary>
		/// The classname of the persistent class (used only for messages)
		/// </summary>
		string ClassName { get; }

		/// <summary>
		/// Does it have a composite key?
		/// </summary>
		bool HasCompositeKey { get; }

		/// <summary>
		/// Does the class implement the <c>ILifecycle</c> inteface?
		/// </summary>
		bool ImplementsLifecycle { get; }

		/// <summary>
		/// Does the class implement the <c>IValidatable</c> interface?
		/// </summary>
		bool ImplementsValidatable { get; }

		/// <summary>
		/// Does this class support dynamic proxies?
		/// </summary>
		bool HasProxy { get; }

		/// <summary>
		/// Get an array of interfaces that the proxy object implements
		/// </summary>
		System.Type[] ProxyInterfaces { get; } //TODO: .NET proxy only can implement one???

		/// <summary>
		/// Get the proxy interface that instances of <c>this</c> concrete class will be cast to
		/// </summary>
		System.Type ConcreteProxyClass { get; }

		/// <summary>
		/// Do instances of this class contain collections?
		/// </summary>
		bool HasCollections { get; }

		/// <summary>
		/// Does this class declare any cascading save/update/deletes?
		/// </summary>
		bool HasCascades { get; }

		/// <summary>
		/// Are instances of this class mutable?
		/// </summary>
		bool IsMutable { get; }

		/// <summary>
		/// Is the identifier assigned before the insert by an <c>IDGenerator</c> or is it returned
		/// by the <c>Insert()</c> method?
		/// </summary>
		/// <remarks>
		/// This determines which form of <c>Insert()</c> will be called.
		/// </remarks>
		bool IsIdentifierAssignedByInsert { get; }

		/// <summary>
		/// Does the given identifier value indicate that this is a new transient instance?
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		bool IsUnsaved(object id);

		/// <summary>
		/// Set the given values to the mapped properties of the given object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="values"></param>
		void SetPropertyValues(object obj, object[] values);

		/// <summary>
		/// Return the values of the mapped properties of the object
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object[] GetPropertyValues(object obj);

		/// <summary>
		/// Set the value of a particular property
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <param name="value"></param>
		void SetPropertyValue(object obj, int i, object value);

		/// <summary>
		/// Get the value of a particular property
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		object GetPropertyValue(object obj, int i);

		/// <summary>
		/// Compare two snapshots of the state of an instance to determine if the persistent state
		/// was modified
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		/// <returns><c>null</c> or the indices of the dirty properties</returns>
		int[] FindDirty(object[] x, object[] y, object owner, ISessionImplementor session);

		/// <summary>
		/// Does the class have a property holding the identifier value?
		/// </summary>
		bool HasIdentifierProperty { get; }

		/// <summary>
		/// Get the identifier of an instance ( throw an exception if no identifier property)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object GetIdentifier(object obj);

		/// <summary>
		/// Set the identifier of an instance (or do nothing if no identifier property)
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		void SetIdentifier(object obj, object id);

		/// <summary>
		/// A method of the proxy interface that returns the identifier value (optional operation)
		/// </summary>
		PropertyInfo ProxyIdentifierProperty { get; }

		/// <summary>
		/// Are instances of this class versioned by a timestamp or version number column?
		/// </summary>
		bool IsVersioned { get; }

		/// <summary>
		/// Get the type of versioning (optional operation)
		/// </summary>
		IVersionType VersionType { get; }

		/// <summary>
		/// Which property holds the version number? (optional operation)
		/// </summary>
		int VersionProperty { get; }

		/// <summary>
		/// Get the version number (or timestamp) from the object's version property (or return null if not versioned)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		object GetVersion(object obj);

		/// <summary>
		/// Create a class instance initialized with the given identifier
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		object Instantiate(object id);

		/// <summary>
		/// Return the <c>IIdentifierGenerator</c> for the class
		/// </summary>
		IIdentifierGenerator IdentifierGenerator { get; }

		/// <summary>
		/// Load an insatance of the persistent class.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session);
	
		/// <summary>
		/// Do a version check (optional operation)
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		/// <param name="session"></param>
		void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session);
	
		/// <summary>
		/// Persist an instance
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		void Insert(object id, object[] fields, object obj, ISessionImplementor session);

		/// <summary>
		/// Persist an instance, using a natively generated identifier (optional operation)
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object Insert(object[] fields, object obj, ISessionImplementor session);

		/// <summary>
		/// Delete a persistent instance
		/// </summary>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		void Delete(object id, object version, object obj, ISessionImplementor session);

		/// <summary>
		/// Update a persistent instance
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fields"></param>
		/// <param name="dirtyFields"></param>
		/// <param name="oldVersion"></param>
		/// <param name="obj"></param>
		/// <param name="session"></param>
		void Update(object id, object[] fields, int[] dirtyFields, object oldVersion, object obj, ISessionImplementor session);
		
		/// <summary>
		/// Get the Hibernate types of the class properties
		/// </summary>
		IType[] PropertyTypes { get; }

		/// <summary>
		/// Get the names of the class properties - doesn't have to be the names of the actual
		/// .NET properties (used for XML generation only)
		/// </summary>
		string[] PropertyNames { get; }

		bool[] PropertyUpdateability { get; }

		bool[] PropertyInsertability { get; }

		/// <summary>
		/// Get the cascade styles of the properties (optional operation)
		/// </summary>
		Cascades.CascadeStyle[] PropertyCascadeStyles { get; }

		/// <summary>
		/// Get the identifier type
		/// </summary>
		IType IdentifierType { get; }

		/// <summary>
		/// Get the name of the indentifier property (or return null) - need not return the
		/// name of an actual .NET property
		/// </summary>
		string IdentifierPropertyName { get; }

		/// <summary>
		/// Does this class have a cache?
		/// </summary>
		bool HasCache { get; }

		/// <summary>
		/// Get the cache (optional operation)
		/// </summary>
		ICacheConcurrencyStrategy Cache { get; }

		/// <summary>
		/// Get the user-visible metadata for the class (optional operation)
		/// </summary>
		IClassMetadata ClassMetadata { get; }
	
	
	}
}
