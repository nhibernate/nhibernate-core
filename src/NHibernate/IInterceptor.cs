using System.Collections;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate
{
	/// <summary>
	/// Allows user code to inspect and/or change property values before they are written and after they
	/// are read from the database
	/// </summary>
	/// <remarks>
	///	<para>
	///	There might be a single instance of <c>IInterceptor</c> for a <c>SessionFactory</c>, or a new
	///	instance might be specified for each <c>ISession</c>. Whichever approach is used, the interceptor
	///	must be serializable if the <c>ISession</c> is to be serializable. This means that <c>SessionFactory</c>
	///	-scoped interceptors should implement <c>ReadResolve()</c>.
	///	</para>
	///	<para>
	///	The <c>ISession</c> may not be invoked from a callback (nor may a callback cause a collection or
	///	proxy to be lazily initialized).
	///	</para>
	/// </remarks>
	public interface IInterceptor
	{
		/// <summary>
		/// Called just before an object is initialized
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="propertyNames"></param>
		/// <param name="state"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// The interceptor may change the <c>state</c>, which will be propagated to the persistent
		/// object. Note that when this method is called, <c>entity</c> will be an empty
		/// uninitialized instance of the class.</remarks>
		/// <returns><see langword="true" /> if the user modified the <c>state</c> in any way</returns>
		bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types);

		/// <summary>
		/// Called when an object is detected to be dirty, during a flush.
		/// </summary>
		/// <param name="currentState"></param>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// The interceptor may modify the detected <c>currentState</c>, which will be propagated to
		/// both the database and the persistent object. Note that all flushes end in an actual
		/// synchronization with the database, in which as the new <c>currentState</c> will be propagated
		/// to the object, but not necessarily (immediately) to the database. It is strongly recommended
		/// that the interceptor <b>not</b> modify the <c>previousState</c>.
		/// </remarks>
		/// <returns><see langword="true" /> if the user modified the <c>currentState</c> in any way</returns>
		bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
		                  IType[] types);

		/// <summary>
		/// Called before an object is saved
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="propertyNames"></param>
		/// <param name="state"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// The interceptor may modify the <c>state</c>, which will be used for the SQL <c>INSERT</c>
		/// and propagated to the persistent object
		/// </remarks>
		/// <returns><see langword="true" /> if the user modified the <c>state</c> in any way</returns>
		bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types);

		/// <summary>
		/// Called before an object is deleted
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="id"></param>
		/// <param name="propertyNames"></param>
		/// <param name="state"></param>
		/// <param name="types"></param>
		/// <remarks>
		/// It is not recommended that the interceptor modify the <c>state</c>.
		/// </remarks>
		void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types);
		/// <summary> Called before a collection is (re)created.</summary>
		void OnCollectionRecreate(object collection, object key);
		/// <summary> Called before a collection is deleted.</summary>
		void OnCollectionRemove(object collection, object key);
		/// <summary> Called before a collection is updated.</summary>
		void OnCollectionUpdate(object collection, object key);
		/// <summary>
		/// Called before a flush
		/// </summary>
		/// <param name="entities">The entities</param>
		void PreFlush(ICollection entities);

		/// <summary>
		/// Called after a flush that actually ends in execution of the SQL statements required to
		/// synchronize in-memory state with the database.
		/// </summary>
		/// <param name="entities">The entitites</param>
		void PostFlush(ICollection entities);

		/// <summary>
		/// Called when a transient entity is passed to <c>SaveOrUpdate</c>.
		/// </summary>
		/// <remarks>
		///	The return value determines if the object is saved
		///	<list>
		///		<item><see langword="true" /> - the entity is passed to <c>Save()</c>, resulting in an <c>INSERT</c></item>
		///		<item><see langword="false" /> - the entity is passed to <c>Update()</c>, resulting in an <c>UPDATE</c></item>
		///		<item><see langword="null" /> - Hibernate uses the <c>unsaved-value</c> mapping to determine if the object is unsaved</item>
		///	</list>
		/// </remarks>
		/// <param name="entity">A transient entity</param>
		/// <returns>Boolean or <see langword="null" /> to choose default behaviour</returns>
		bool? IsTransient(object entity);

		/// <summary>
		/// Called from <c>Flush()</c>. The return value determines whether the entity is updated
		/// </summary>
		/// <remarks>
		///		<list>
		///			<item>an array of property indicies - the entity is dirty</item>
		///			<item>an empty array - the entity is not dirty</item>
		///			<item><see langword="null" /> - use Hibernate's default dirty-checking algorithm</item>
		///		</list>
		/// </remarks>
		/// <param name="entity">A persistent entity</param>
		/// <param name="currentState"></param>
		/// <param name="id"></param>
		/// <param name="previousState"></param>
		/// <param name="propertyNames"></param>
		/// <param name="types"></param>
		/// <returns>An array of dirty property indicies or <see langword="null" /> to choose default behavior</returns>
		int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
		                IType[] types);

		/// <summary>
		/// Instantiate the entity class. Return <see langword="null" /> to indicate that Hibernate should use the default
		/// constructor of the class
		/// </summary>
		/// <param name="entityName">the name of the entity </param>
		/// <param name="entityMode">The type of entity instance to be returned. </param>
		/// <param name="id">the identifier of the new instance </param>
		/// <returns>An instance of the class, or <see langword="null" /> to choose default behaviour</returns>
		/// <remarks>
		/// The identifier property of the returned instance
		/// should be initialized with the given identifier.
		/// </remarks>
		object Instantiate(System.String entityName, EntityMode entityMode, object id);

		/// <summary> Get the entity name for a persistent or transient instance</summary>
		/// <param name="entity">an entity instance </param>
		/// <returns> the name of the entity </returns>
		string GetEntityName(object entity);
		/// <summary> Get a fully loaded entity instance that is cached externally</summary>
		/// <param name="entityName">the name of the entity </param>
		/// <param name="id">the instance identifier </param>
		/// <returns> a fully initialized entity </returns>
		object GetEntity(string entityName, object id);

		/// <summary>
		/// Called when a NHibernate transaction is begun via the NHibernate <see cref="ITransaction" />
		/// API. Will not be called if transactions are being controlled via some other mechanism.
		/// </summary>
		void AfterTransactionBegin(ITransaction tx);

		/// <summary>
		/// Called before a transaction is committed (but not before rollback).
		/// </summary>
		void BeforeTransactionCompletion(ITransaction tx);

		/// <summary>
		/// Called after a transaction is committed or rolled back.
		/// </summary>
		void AfterTransactionCompletion(ITransaction tx);

		/// <summary>
		/// Called when a session-scoped (and <b>only</b> session scoped) interceptor is attached
		/// to a session
		/// </summary>
		void SetSession(ISession session);

		/// <summary> Called when sql string is being prepared. </summary>
		/// <param name="sql">sql to be prepared </param>
		/// <returns> original or modified sql </returns>
		SqlString OnPrepareStatement(SqlString sql);
	}
}