using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	public struct EntityPersister
	{
		/// <summary> The property name of the "special" identifier property in HQL</summary>
		public readonly static string EntityID = "id";
	}

	/// <summary>
	/// Concrete <c>IEntityPersister</c>s implement mapping and persistence logic for a particular class.
	/// </summary>
	/// <remarks>
	/// Implementors must be threadsafe (preferrably immutable) and must provide a constructor of type
	/// (PersistentClass, SessionFactoryImplementor)
	/// </remarks>
	public interface IEntityPersister : IOptimisticCacheSource
	{
		/// <summary>
		/// Finish the initialization of this object, once all <c>ClassPersisters</c> have been
		/// instantiated. Called only once, before any other method.
		/// </summary>
		void PostInstantiate();

		/// <summary>
		/// Returns an object that identifies the space in which identifiers of this class hierarchy
		/// are unique. eg. a table name, etc.
		/// </summary>
		object IdentifierSpace { get; }

		/// <summary>
		/// Returns an array of objects that identifies spaces in which properties of this class
		/// instance are persisted. eg. table names.
		/// </summary>
		/// <returns></returns>
		object[] PropertySpaces { get; }

		/// <summary>
		/// The persistent class
		/// </summary>
		System.Type MappedClass { get; }

		/// <summary>
		/// The classname of the persistent class (used only for messages)
		/// </summary>
		string ClassName { get; }

		/// <summary> 
		/// Returns an object that identifies the space in which identifiers of
		/// this entity hierarchy are unique.
		/// </summary>
		string RootEntityName{get;}

		// TODO H3: http://www.hibernate.org/hib_docs/v3/reference/en/html/mapping.html#mapping-entityname
		// introduced in NH to prepare the work to port the feature during the port of other features (listener+events)
		/// <summary>
		/// The entity name which this persister maps.
		/// </summary>
		string EntityName { get;}

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
		/// Get the proxy interface that instances of <c>this</c> concrete class will be cast to
		/// </summary>
		System.Type ConcreteProxyClass { get; }

		/// <summary>
		/// Create a new proxy instance
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object CreateProxy(object id, ISessionImplementor session);

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
		/// Is this a new transient instance?
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		bool IsUnsaved(object obj);

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
		/// Get the value of a particular property
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		object GetPropertyValue(object obj, string name);

		/// <summary>
		/// Get the type of a particular property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		IType GetPropertyType(string propertyName);

		/// <summary>
		/// Compare two snapshots of the state of an instance to determine if the persistent state
		/// was modified
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		/// <returns><see langword="null" /> or the indices of the dirty properties</returns>
		int[] FindDirty(object[] x, object[] y, object owner, ISessionImplementor session);

		/// <summary>
		/// Compare the state of an instance to the current database state
		/// </summary>
		/// <param name="old"></param>
		/// <param name="current"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		/// <returns>return <see langword="null" /> or the indicies of the modified properties</returns>
		int[] FindModified(object[] old, object[] current, object owner, ISessionImplementor session);

		/// <summary>
		/// Does the class have a property holding the identifier value?
		/// </summary>
		bool HasIdentifierProperty { get; }

		/// <summary> 
		/// Determine whether detahced instances of this entity carry their own
		/// identifier value.
		/// </summary>
		/// <returns> 
		/// True if either (1) <see cref="HasIdentifierProperty"/> or
		/// (2) the identifier is an embedded composite identifier; false otherwise.
		/// </returns>
		/// <remarks>
		/// The other option is the deperecated feature where users could supply
		/// the id during session calls.
		/// </remarks>
		bool CanExtractIdOutOfEntity { get; }

		/// <summary>
		/// Get the identifier of an instance ( throw an exception if no identifier property)
		/// </summary>
		object GetIdentifier(object obj);

		/// <summary>
		/// Set the identifier of an instance (or do nothing if no identifier property)
		/// </summary>
		/// <param name="obj">The object to set the Id property on.</param>
		/// <param name="id">The value to set the Id property to.</param>
		void SetIdentifier(object obj, object id);

		/// <summary>
		/// Are instances of this class versioned by a timestamp or version number column?
		/// </summary>
		new bool IsVersioned { get; }

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
		/// <param name="id">The id.</param>
		/// <param name="fields">The fields.</param>
		/// <param name="dirtyFields">The dirty fields.</param>
		/// <param name="hasDirtyCollection">if set to <see langword="true" /> [has dirty collection].</param>
		/// <param name="oldFields">The old fields.</param>
		/// <param name="oldVersion">The old version.</param>
		/// <param name="obj">The obj.</param>
		/// <param name="session">The session.</param>
		void Update(object id, object[] fields, int[] dirtyFields, bool hasDirtyCollection, object[] oldFields,
		            object oldVersion, object obj, ISessionImplementor session);

		/// <summary>
		/// Get the Hibernate types of the class properties
		/// </summary>
		IType[] PropertyTypes { get; }

		/// <summary>
		/// Get the names of the class properties - doesn't have to be the names of the actual
		/// .NET properties (used for XML generation only)
		/// </summary>
		string[] PropertyNames { get; }

		/// <summary>
		/// Gets if the Property is updatable
		/// </summary>
		/// <value><see langword="true" /> if the Property's value can be updated.</value>
		/// <remarks>
		/// This is for formula columns and if the user sets the update attribute on the &lt;property&gt; element.
		/// </remarks>
		bool[] PropertyUpdateability { get; }

		/// <summary>
		/// Properties that may be dirty (and thus should be dirty-checked). These
		/// include all updatable properties and some associations.
		/// </summary>
		bool[] PropertyCheckability { get; }

		/// <summary>
		/// Get the nullability of the properties of this class
		/// </summary>
		bool[] PropertyNullability { get; }

		/// <summary>
		/// Gets if the Property is insertable.
		/// </summary>
		/// <value><see langword="true" /> if the Property's value can be inserted.</value>
		/// <remarks>
		/// This is for formula columns and if the user sets the insert attribute on the &lt;property&gt; element.
		/// </remarks>
		bool[] PropertyInsertability { get; }

		/// <summary>
		/// Get the "versionability" of the properties of this class (is the property optimistic-locked)
		/// </summary>
		/// <value><see langword="true" /> if the property is optimistic-locked; otherwise, <see langword="false" />.</value>
		bool[] PropertyVersionability { get; }

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
		/// Should we always invalidate the cache instead of recaching updated state
		/// </summary>
		bool IsCacheInvalidationRequired { get; }

		/// <summary>
		/// Should lazy properties of this entity be cached?
		/// </summary>
		bool IsLazyPropertiesCacheable { get;}

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

		/// <summary>
		/// Is batch loading enabled?
		/// </summary>
		bool IsBatchLoadable { get; }

		/// <summary>
		/// Returns an array of objects that identify spaces in which properties of this class are persisted,
		/// for instances of this class and its subclasses.
		/// </summary>
		object[] QuerySpaces { get; }

		/// <summary>
		/// The session factory this instance is associated with.
		/// </summary>
		ISessionFactoryImplementor Factory { get; }

		/// <summary>
		/// Does this entity contain a version property that is defined
		/// to be database generated?
		/// </summary>
		bool IsVersionPropertyGenerated { get; }

		/// <summary> Has the class actually been bytecode instrumented?</summary>
		bool IsInstrumented(EntityMode entityMode);

		/// <summary>
		/// Does this entity define any properties as being database-generated on insert?
		/// </summary>
		bool HasInsertGeneratedProperties { get; }

		/// <summary>
		/// Does this entity define any properties as being database-generated on update?
		/// </summary>
		bool HasUpdateGeneratedProperties { get; }

		/// <summary> Is select snapshot before update enabled?</summary>
		bool IsSelectBeforeUpdateRequired { get; }

		/// <summary>
		/// Get the current database state of the object, in a "hydrated" form, without resolving identifiers
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns><see langword="null" /> if select-before-update is not enabled or not supported</returns>
		object[] GetDatabaseSnapshot(object id, ISessionImplementor session);

		/// <summary>
		/// Get the current version of the object, or return null if there is no row for
		/// the given identifier. In the case of unversioned data, return any object
		/// if the row exists.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object GetCurrentVersion(object id, ISessionImplementor session);

		object ForceVersionIncrement(object id, object currentVersion, ISessionImplementor session);

		/// <summary>
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		/// <remarks>NHibernate-specific feature, not present in H2.1</remarks>
		bool IsUnsavedVersion(object[] values);


		/// <summary>
		/// Determines whether the specified entity is an instance of the class
		/// managed by this persister.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified entity is an instance; otherwise, <see langword="false"/>.
		/// </returns>
		bool IsInstance(object entity);

		/// <summary>
		/// Perform a select to retrieve the values of any generated properties
		/// back from the database, injecting these generated values into the
		/// given entity as well as writing this state to the persistence context.
		/// </summary>
		/// <remarks>
		/// Note, that because we update the persistence context here, callers
		/// need to take care that they have already written the initial snapshot
		/// to the persistence context before calling this method. 
		/// </remarks>
		/// <param name="id">The entity's id value.</param>
		/// <param name="entity">The entity for which to get the state.</param>
		/// <param name="state">The entity state (at the time of Save).</param>
		/// <param name="session">The session.</param>
		void ProcessInsertGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session);

		/// <summary>
		/// Perform a select to retrieve the values of any generated properties
		/// back from the database, injecting these generated values into the
		/// given entity as well as writing this state to the persistence context.
		/// </summary>
		/// <remarks>
		/// Note, that because we update the persistence context here, callers
		/// need to take care that they have already written the initial snapshot
		/// to the persistence context before calling this method. 
		/// </remarks>
		/// <param name="id">The entity's id value.</param>
		/// <param name="entity">The entity for which to get the state.</param>
		/// <param name="state">The entity state (at the time of Save).</param>
		/// <param name="session">The session.</param>
		void ProcessUpdateGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session);
	}
}
