using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Tuple.Entity;
using NHibernate.Type;
using System.Collections;

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
	/// Implementors must be threadsafe (preferably immutable) and must provide a constructor of type
	/// matching the signature of: (PersistentClass, SessionFactoryImplementor)
	/// </remarks>
	public partial interface IEntityPersister : IOptimisticCacheSource
	{
		/// <summary>
		/// The ISessionFactory to which this persister "belongs".
		/// </summary>
		ISessionFactoryImplementor Factory { get; }

		/// <summary> 
		/// Returns an object that identifies the space in which identifiers of
		/// this entity hierarchy are unique.
		/// </summary>
		string RootEntityName { get; }

		/// <summary>
		/// The entity name which this persister maps.
		/// </summary>
		string EntityName { get; }

		/// <summary> 
		/// Retrieve the underlying entity metamodel instance... 
		/// </summary>
		/// <returns> The metamodel </returns>
		EntityMetamodel EntityMetamodel { get; }

		/// <summary>
		/// Returns an array of objects that identify spaces in which properties of
		/// this entity are persisted, for instances of this class only.
		/// </summary>
		/// <returns>The property spaces.</returns>
		/// <remarks>
		/// For most implementations, this returns the complete set of table names
		/// to which instances of the mapped entity are persisted (not accounting
		/// for superclass entity mappings).
		/// </remarks>
		string[] PropertySpaces { get; }

		/// <summary>
		/// Returns an array of objects that identify spaces in which properties of
		/// this entity are persisted, for instances of this class and its subclasses.
		/// </summary>
		/// <remarks>
		/// Much like <see cref="PropertySpaces"/>, except that here we include subclass
		/// entity spaces.
		/// </remarks>
		/// <returns> The query spaces. </returns>
		string[] QuerySpaces { get; }

		/// <summary>
		/// Are instances of this class mutable?
		/// </summary>
		bool IsMutable { get; }

		/// <summary> 
		/// Determine whether the entity is inherited one or more other entities.
		/// In other words, is this entity a subclass of other entities. 
		/// </summary>
		/// <returns> True if other entities extend this entity; false otherwise. </returns>
		bool IsInherited { get; }

		/// <summary>
		/// Is the identifier assigned before the insert by an <c>IDGenerator</c> or is it returned
		/// by the <c>Insert()</c> method?
		/// </summary>
		/// <remarks>
		/// This determines which form of <c>Insert()</c> will be called.
		/// </remarks>
		bool IsIdentifierAssignedByInsert { get; }

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
		/// If the entity defines a natural id (<see cref="HasNaturalIdentifier"/>), which
		/// properties make up the natural id. 
		/// </summary>
		/// <returns> 
		/// The indices of the properties making of the natural id; or
		/// null, if no natural id is defined.
		/// </returns>
		int[] NaturalIdentifierProperties { get; }

		/// <summary>
		/// Return the <c>IIdentifierGenerator</c> for the class
		/// </summary>
		IIdentifierGenerator IdentifierGenerator { get; }

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
		/// Gets if the Property is insertable.
		/// </summary>
		/// <value><see langword="true" /> if the Property's value can be inserted.</value>
		/// <remarks>
		/// This is for formula columns and if the user sets the insert attribute on the &lt;property&gt; element.
		/// </remarks>
		bool[] PropertyInsertability { get; }

		/// <summary> Which of the properties of this class are database generated values on insert?</summary>
		ValueInclusion[] PropertyInsertGenerationInclusions { get; }

		/// <summary> Which of the properties of this class are database generated values on update?</summary>
		ValueInclusion[] PropertyUpdateGenerationInclusions { get; }

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
		/// Get the "versionability" of the properties of this class (is the property optimistic-locked)
		/// </summary>
		/// <value><see langword="true" /> if the property is optimistic-locked; otherwise, <see langword="false" />.</value>
		bool[] PropertyVersionability { get; }

		bool[] PropertyLaziness { get; }

		/// <summary>
		/// Get the cascade styles of the properties (optional operation)
		/// </summary>
		CascadeStyle[] PropertyCascadeStyles { get; }

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
		bool IsLazyPropertiesCacheable { get; }

		/// <summary>
		/// Get the cache (optional operation)
		/// </summary>
		ICacheConcurrencyStrategy Cache { get; }

		/// <summary> Get the cache structure</summary>
		ICacheEntryStructure CacheEntryStructure { get; }

		/// <summary>
		/// Get the user-visible metadata for the class (optional operation)
		/// </summary>
		IClassMetadata ClassMetadata { get; }

		/// <summary>
		/// Is batch loading enabled?
		/// </summary>
		bool IsBatchLoadable { get; }

		/// <summary> Is select snapshot before update enabled?</summary>
		bool IsSelectBeforeUpdateRequired { get; }

		/// <summary>
		/// Does this entity contain a version property that is defined
		/// to be database generated?
		/// </summary>
		bool IsVersionPropertyGenerated { get; }

		/// <summary>
		/// Finish the initialization of this object, once all <c>ClassPersisters</c> have been
		/// instantiated. Called only once, before any other method.
		/// </summary>
		void PostInstantiate();

		#region stuff that is persister-centric and/or EntityInfo-centric

		/// <summary> 
		/// Determine whether the given name represents a subclass entity
		/// (or this entity itself) of the entity mapped by this persister. 
		/// </summary>
		/// <param name="entityName">The entity name to be checked. </param>
		/// <returns> 
		/// True if the given entity name represents either the entity mapped by this persister or one of its subclass entities; 
		/// false otherwise.
		/// </returns>
		bool IsSubclassEntityName(string entityName);

		/// <summary>
		/// Does this class support dynamic proxies?
		/// </summary>
		bool HasProxy { get; }

		/// <summary>
		/// Do instances of this class contain collections?
		/// </summary>
		bool HasCollections { get; }

		/// <summary> 
		/// Determine whether any properties of this entity are considered mutable. 
		/// </summary>
		/// <returns> 
		/// True if any properties of the entity are mutable; false otherwise (meaning none are).
		/// </returns>
		bool HasMutableProperties { get; }

		/// <summary> 
		/// Determine whether this entity contains references to persistent collections
		/// which are fetchable by subselect? 
		/// </summary>
		/// <returns> 
		/// True if the entity contains collections fetchable by subselect; false otherwise.
		/// </returns>
		bool HasSubselectLoadableCollections { get; }

		/// <summary>
		/// Does this class declare any cascading save/update/deletes?
		/// </summary>
		bool HasCascades { get; }

		/// <summary>
		/// Get the type of a particular property
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		IType GetPropertyType(string propertyName);

		/// <summary> Locate the property-indices of all properties considered to be dirty. </summary>
		/// <param name="currentState">The current state of the entity (the state to be checked). </param>
		/// <param name="previousState">The previous state of the entity (the state to be checked against). </param>
		/// <param name="entity">The entity for which we are checking state dirtiness. </param>
		/// <param name="session">The session in which the check is occurring. </param>
		/// <returns> <see langword="null" /> or the indices of the dirty properties </returns>
		int[] FindDirty(object[] currentState, object[] previousState, object entity, ISessionImplementor session);

		/// <summary> Locate the property-indices of all properties considered to be dirty. </summary>
		/// <param name="old">The old state of the entity.</param>
		/// <param name="current">The current state of the entity. </param>
		/// <param name="entity">The entity for which we are checking state modification. </param>
		/// <param name="session">The session in which the check is occurring. </param>
		/// <returns>return <see langword="null" /> or the indicies of the modified properties</returns>
		int[] FindModified(object[] old, object[] current, object entity, ISessionImplementor session);

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
		/// The other option is the deprecated feature where users could supply
		/// the id during session calls.
		/// </remarks>
		bool CanExtractIdOutOfEntity { get; }

		/// <summary> 
		/// Determine whether this entity defines a natural identifier. 
		/// </summary>
		/// <returns> True if the entity defines a natural id; false otherwise. </returns>
		bool HasNaturalIdentifier { get; }

		/// <summary> 
		/// Retrieve the current state of the natural-id properties from the database. 
		/// </summary>
		/// <param name="id">
		/// The identifier of the entity for which to retrieve the natural-id values.
		/// </param>
		/// <param name="session">
		/// The session from which the request originated.
		/// </param>
		/// <returns> The natural-id snapshot. </returns>
		object[] GetNaturalIdentifierSnapshot(object id, ISessionImplementor session);

		/// <summary> 
		/// Determine whether this entity defines any lazy properties (ala
		/// bytecode instrumentation). 
		/// </summary>
		/// <returns> 
		/// True if the entity has properties mapped as lazy; false otherwise.
		/// </returns>
		bool HasLazyProperties { get; }

		/// <summary>
		/// Load an instance of the persistent class.
		/// </summary>
		object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session);

		/// <summary>
		/// Do a version check (optional operation)
		/// </summary>
		void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session);

		/// <summary>
		/// Persist an instance
		/// </summary>
		void Insert(object id, object[] fields, object obj, ISessionImplementor session);

		/// <summary>
		/// Persist an instance, using a natively generated identifier (optional operation)
		/// </summary>
		object Insert(object[] fields, object obj, ISessionImplementor session);

		/// <summary>
		/// Delete a persistent instance
		/// </summary>
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
		/// <param name="rowId">The rowId</param>
		/// <param name="session">The session.</param>
		void Update(
			object id,
			object[] fields,
			int[] dirtyFields,
			bool hasDirtyCollection,
			object[] oldFields,
			object oldVersion,
			object obj,
			object rowId,
			ISessionImplementor session);

		/// <summary>
		/// Gets if the Property is updatable
		/// </summary>
		/// <value><see langword="true" /> if the Property's value can be updated.</value>
		/// <remarks>
		/// This is for formula columns and if the user sets the update attribute on the &lt;property&gt; element.
		/// </remarks>
		bool[] PropertyUpdateability { get; }

		/// <summary>
		/// Does this class have a cache?
		/// </summary>
		bool HasCache { get; }

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

		/// <summary> Has the class actually been bytecode instrumented?</summary>
		bool IsInstrumented { get; }

		/// <summary>
		/// Does this entity define any properties as being database-generated on insert?
		/// </summary>
		bool HasInsertGeneratedProperties { get; }

		/// <summary>
		/// Does this entity define any properties as being database-generated on update?
		/// </summary>
		bool HasUpdateGeneratedProperties { get; }

		#endregion

		#region stuff that is tuplizer-centric, but is passed a session

		/// <summary> Called just after the entities properties have been initialized</summary>
		void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session);

		/// <summary> Called just after the entity has been reassociated with the session</summary>
		void AfterReassociate(object entity, ISessionImplementor session);

		/// <summary>
		/// Create a new proxy instance
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object CreateProxy(object id, ISessionImplementor session);

		/// <summary> Is this a new transient instance?</summary>
		bool? IsTransient(object obj, ISessionImplementor session);

		/// <summary> Return the values of the insertable properties of the object (including backrefs)</summary>
		object[] GetPropertyValuesToInsert(object obj, IDictionary mergeMap, ISessionImplementor session);

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

		#endregion

		#region stuff that is Tuplizer-centric

		/// <summary>
		/// The persistent class, or null
		/// </summary>
		System.Type MappedClass { get; }

		/// <summary>
		/// Does the class implement the <c>ILifecycle</c> inteface?
		/// </summary>
		bool ImplementsLifecycle { get; }

		/// <summary>
		/// Does the class implement the <c>IValidatable</c> interface?
		/// </summary>
		bool ImplementsValidatable { get; }

		/// <summary>
		/// Get the proxy interface that instances of <c>this</c> concrete class will be cast to
		/// </summary>
		System.Type ConcreteProxyClass { get; }

		/// <summary>
		/// Set the given values to the mapped properties of the given object
		/// </summary>
		void SetPropertyValues(object obj, object[] values);

		/// <summary>
		/// Set the value of a particular property
		/// </summary>
		void SetPropertyValue(object obj, int i, object value);

		/// <summary>
		/// Return the values of the mapped properties of the object
		/// </summary>
		object[] GetPropertyValues(object obj);

		/// <summary>
		/// Get the value of a particular property
		/// </summary>
		object GetPropertyValue(object obj, int i);

		/// <summary>
		/// Get the value of a particular property
		/// </summary>
		object GetPropertyValue(object obj, string name);

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
		/// Get the version number (or timestamp) from the object's version property (or return null if not versioned)
		/// </summary>
		object GetVersion(object obj);

		/// <summary>
		/// Create a class instance initialized with the given identifier
		/// </summary>
		object Instantiate(object id);

		/// <summary>
		/// Determines whether the specified entity is an instance of the class
		/// managed by this persister.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified entity is an instance; otherwise, <see langword="false"/>.
		/// </returns>
		bool IsInstance(object entity);

		/// <summary> Does the given instance have any uninitialized lazy properties?</summary>
		bool HasUninitializedLazyProperties(object obj);

		/// <summary> 
		/// Set the identifier and version of the given instance back
		/// to its "unsaved" value, returning the id
		/// </summary>
		void ResetIdentifier(object entity, object currentId, object currentVersion);

		/// <summary> Get the persister for an instance of this class or a subclass</summary>
		IEntityPersister GetSubclassEntityPersister(object instance, ISessionFactoryImplementor factory);

		#endregion

		/// <summary>
		/// Check the version value trough <see cref="VersionValue"/>.
		/// </summary>
		/// <param name="version">The snapshot entity state</param>
		/// <returns>The result of <see cref="VersionValue.IsUnsaved"/>.</returns>
		/// <remarks>NHibernate-specific feature, not present in H3.2</remarks>
		bool? IsUnsavedVersion(object version);

		/// <summary> 
		/// Gets EntityMode.
		/// </summary>
		EntityMode EntityMode { get; }

		IEntityTuplizer EntityTuplizer { get; }
	}
}
