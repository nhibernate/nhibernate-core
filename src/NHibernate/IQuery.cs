using System;
using System.Collections;
using NHibernate.Transform;
using NHibernate.Type;
using System.Collections.Generic;

namespace NHibernate
{
	/// <summary>
	/// An object-oriented representation of a NHibernate query.
	/// </summary>
	/// <remarks>
	/// An <c>IQuery</c> instance is obtained by calling <see cref="ISession.CreateQuery(string)" />.
	/// Key features of this interface include:
	/// <list type="bullet">
	///		<item>
	///			Paging: A particular page of the result set may be selected by calling
	///			<see cref="SetMaxResults(int)" />, <see cref="SetFirstResult(int)" />.  The generated SQL
	///			depends on the capabilities of the <see cref="Dialect.Dialect"/>.  Some
	///			Dialects are for databases that have built in paging (LIMIT) and those capabilities
	///			will be used to limit the number of records returned by the SQL statement.
	///			If the database does not support LIMITs then all of the records will be returned,
	///			but the objects created will be limited to the specific results requested.
	///		</item>
	///		<item>
	///			Named parameters
	///		</item>
	///		<item>
	///			Ability to return 'read-only' entities
	///		</item>
	/// </list>
	/// <para>
	/// Named query parameters are tokens of the form <c>:name</c> in the query string. For example, a
	/// value is bound to the <c>Int32</c> parameter <c>:foo</c> by calling:
	/// <code>
	/// SetParameter("foo", foo, NHibernateUtil.Int32);
	/// </code>
	/// A name may appear multiple times in the query string.
	/// </para>
	/// <para>
	/// Unnamed parameters <c>?</c> are also supported. To bind a value to an unnamed
	/// parameter use a Set method that accepts an <c>Int32</c> positional argument - numbered from
	/// zero.
	/// </para>
	/// <para>
	/// You may not mix and match unnamed parameters and named parameters in the same query.
	/// </para>
	/// <para>
	/// Queries are executed by calling <see cref="IQuery.List()" /> or <see cref="IQuery.Enumerable()" />. A query
	/// may be re-executed by subsequent invocations. Its lifespan is, however, bounded by the lifespan
	/// of the <c>ISession</c> that created it.
	/// </para>
	/// <para>
	/// Implementors are not intended to be threadsafe.
	/// </para>
	/// </remarks>
	public interface IQuery
	{
		/// <summary>
		/// The query string
		/// </summary>
		string QueryString { get; }

		/// <summary>
		/// The NHibernate types of the query result set.
		/// </summary>
		IType[] ReturnTypes { get; }

		/// <summary> Return the HQL select clause aliases (if any)</summary>
		/// <returns> An array of aliases as strings </returns>
		string[] ReturnAliases { get; }

		/// <summary>
		/// The names of all named parameters of the query
		/// </summary>
		/// <value>The parameter names, in no particular order</value>
		string[] NamedParameters { get; }
		
		/// <summary>
		/// Will entities (and proxies) returned by the query be loaded in read-only mode?
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the query's read-only setting is not initialized (with <see cref="SetReadOnly(bool)" />),
		/// the value of the session's <see cref="ISession.DefaultReadOnly" /> property is
		/// returned instead.
		/// </para>
		/// <para>
		/// The value of this property has no effect on entities or proxies returned by the
		/// query that existed in the session before the query was executed.
		/// </para>
		/// </remarks>
		/// <returns>
		/// <c>true</c> if entities and proxies loaded by the query will be put in read-only mode, otherwise <c>false</c>.
		/// </returns>
		/// <seealso cref="IQuery.SetReadOnly(bool)" />
		bool IsReadOnly { get; }

		/// <summary>
		/// Return the query results as an <see cref="IEnumerable"/>. If the query contains multiple results
		/// per row, the results are returned in an instance of <c>object[]</c>.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Entities returned as results are initialized on demand. The first SQL query returns
		/// identifiers only.
		/// </p>
		/// <p>
		/// This is a good strategy to use if you expect a high number of the objects
		/// returned to be already loaded in the <see cref="ISession"/> or in the 2nd level cache.
		/// </p>
		/// </remarks>
		IEnumerable Enumerable();

		/// <summary>
		/// Strongly-typed version of <see cref="Enumerable()"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<T> Enumerable<T>();

		/// <summary>
		/// Return the query results as an <see cref="IList"/>. If the query contains multiple results per row,
		/// the results are returned in an instance of <c>object[]</c>.
		/// </summary>
		/// <returns>The <see cref="IList"/> filled with the results.</returns>
		/// <remarks>
		/// This is a good strategy to use if you expect few of the objects being returned are already loaded
		/// or if you want to fill the 2nd level cache.
		/// </remarks>
		IList List();

		/// <summary>
		/// Return the query results an place them into the <see cref="IList"/>.
		/// </summary>
		/// <param name="results">The <see cref="IList"/> to place the results in.</param>
		void List(IList results);

		/// <summary>
		/// Strongly-typed version of <see cref="List()"/>.
		/// </summary>
		IList<T> List<T>();

		/// <summary>
		/// Convenience method to return a single instance that matches
		/// the query, or null if the query returns no results.
		/// </summary>
		/// <returns>the single result or <see langword="null" /></returns>
		/// <exception cref="HibernateException">
		/// Thrown when there is more than one matching result.
		/// </exception>
		object UniqueResult();

		/// <summary>
		/// Strongly-typed version of <see cref="UniqueResult()"/>.
		/// </summary>
		T UniqueResult<T>();

		/// <summary>
		/// Execute the update or delete statement.
		/// </summary>
		/// <returns> The number of entities updated or deleted. </returns>
		int ExecuteUpdate();

		/// <summary>
		/// Set the maximum number of rows to retrieve.
		/// </summary>
		/// <param name="maxResults">The maximum number of rows to retrieve.</param>
		IQuery SetMaxResults(int maxResults);

		/// <summary>
		/// Sets the first row to retrieve.
		/// </summary>
		/// <param name="firstResult">The first row to retrieve.</param>
		IQuery SetFirstResult(int firstResult);

		/// <summary>
		/// Set the read-only mode for entities (and proxies) loaded by this query. This setting 
		/// overrides the default setting for the session (see <see cref="ISession.DefaultReadOnly" />).
		/// </summary>
		/// <remarks>
		/// <para>
		/// Read-only entities can be modified, but changes are not persisted. They are not 
		/// dirty-checked and snapshots of persistent state are not maintained.
		/// </para>
		/// <para>
		/// When a proxy is initialized, the loaded entity will have the same read-only setting 
		/// as the uninitialized proxy, regardless of the session's current setting.
		/// </para>
		/// <para>
		/// The read-only setting has no impact on entities or proxies returned by the criteria
		/// that existed in the session before the criteria was executed.
		/// </para>
		/// </remarks>
		/// <param name="readOnly">
		/// If <c>true</c>, entities (and proxies) loaded by the query will be read-only.
		/// </param>
		/// <returns><c>this</c> (for method chaining)</returns>
		/// <seealso cref="IsReadOnly" />
		IQuery SetReadOnly(bool readOnly);

		/// <summary>
		/// Enable caching of this query result set.
		/// </summary>
		/// <param name="cacheable">Should the query results be cacheable?</param>
		IQuery SetCacheable(bool cacheable);

		/// Set the name of the cache region.
		/// <param name="cacheRegion">The name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		IQuery SetCacheRegion(string cacheRegion);

		/// <summary>
		/// The timeout for the underlying ADO query
		/// </summary>
		/// <param name="timeout"></param>
		IQuery SetTimeout(int timeout);

		/// <summary> Set a fetch size for the underlying ADO query.</summary>
		/// <param name="fetchSize">the fetch size </param>
		IQuery SetFetchSize(int fetchSize);

		/// <summary>
		/// Set the lockmode for the objects identified by the
		/// given alias that appears in the <c>FROM</c> clause.
		/// </summary>
		/// <param name="alias">alias a query alias, or <c>this</c> for a collection filter</param>
		/// <param name="lockMode"></param>
		IQuery SetLockMode(string alias, LockMode lockMode);

		/// <summary> Add a comment to the generated SQL.</summary>
		/// <param name="comment">a human-readable string </param>
		IQuery SetComment(string comment);

		/// <summary>
		/// Override the current session flush mode, just for this query.
		/// </summary>
		IQuery SetFlushMode(FlushMode flushMode);

		/// <summary> Override the current session cache mode, just for this query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		IQuery SetCacheMode(CacheMode cacheMode);

		/// <summary>
		/// Bind a value to an indexed parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The NHibernate type</param>
		IQuery SetParameter(int position, object val, IType type);

		/// <summary>
		/// Bind a value to a named query parameter
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The NHibernate <see cref="IType"/>.</param>
		IQuery SetParameter(string name, object val, IType type);

		/// <summary>
		/// Bind a value to an indexed parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The possibly null parameter value</param>
		/// <typeparam name="T">The parameter's <see cref="Type"/> </typeparam>
		IQuery SetParameter<T>(int position, T val);

		/// <summary>
		/// Bind a value to a named query parameter
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The possibly null parameter value</param>
		/// <typeparam name="T">The parameter's <see cref="Type"/> </typeparam>
		IQuery SetParameter<T>(string name, T val);

		/// <summary>
		/// Bind a value to an indexed parameter, guessing the NHibernate type from
		/// the class of the given object.
		/// </summary>
		/// <param name="position">The position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The non-null parameter value</param>
		IQuery SetParameter(int position, object val);

		/// <summary>
		/// Bind a value to a named query parameter, guessing the NHibernate <see cref="IType"/>
		/// from the class of the given object.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The non-null parameter value</param>
		IQuery SetParameter(string name, object val);

		/// <summary>
		/// Bind multiple values to a named query parameter. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		/// <param name="type">The NHibernate type of the values</param>
		IQuery SetParameterList(string name, IEnumerable vals, IType type);

		/// <summary>
		/// Bind multiple values to a named query parameter, guessing the NHibernate
		/// type from the class of the first object in the collection. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		IQuery SetParameterList(string name, IEnumerable vals);

		/// <summary>
		/// Bind the property values of the given object to named parameters of the query,
		/// matching property names with parameter names and mapping property types to
		/// NHibernate types using heuristics.
		/// </summary>
		/// <param name="obj">Any PONO</param>
		IQuery SetProperties(object obj);

		/// <summary>
		/// Bind an instance of a <see cref="String" /> to an indexed parameter
		/// using an NHibernate <see cref="AnsiStringType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="String"/>.</param>
		IQuery SetAnsiString(int position, string val);

		/// <summary>
		/// Bind an instance of a <see cref="String" /> to a named parameter
		/// using an NHibernate <see cref="AnsiStringType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="String"/>.</param>
		IQuery SetAnsiString(string name, string val);

		/// <summary>
		/// Bind an instance of a <see cref="Byte" /> array to an indexed parameter
		/// using an NHibernate <see cref="BinaryType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Byte"/> array.</param>
		IQuery SetBinary(int position, byte[] val);

		/// <summary>
		/// Bind an instance of a <see cref="Byte" /> array to a named parameter
		/// using an NHibernate <see cref="BinaryType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Byte"/> array.</param>
		IQuery SetBinary(string name, byte[] val);

		/// <summary>
		/// Bind an instance of a <see cref="Boolean" /> to an indexed parameter
		/// using an NHibernate <see cref="BooleanType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Boolean"/>.</param>
		IQuery SetBoolean(int position, bool val);

		/// <summary>
		/// Bind an instance of a <see cref="Boolean" /> to a named parameter
		/// using an NHibernate <see cref="BooleanType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Boolean"/>.</param>
		IQuery SetBoolean(string name, bool val);

		/// <summary>
		/// Bind an instance of a <see cref="Byte" /> to an indexed parameter
		/// using an NHibernate <see cref="ByteType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Byte"/>.</param>
		IQuery SetByte(int position, byte val);

		/// <summary>
		/// Bind an instance of a <see cref="Byte" /> to a named parameter
		/// using an NHibernate <see cref="ByteType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Byte"/>.</param>
		IQuery SetByte(string name, byte val);

		/// <summary>
		/// Bind an instance of a <see cref="Char" /> to an indexed parameter
		/// using an NHibernate <see cref="CharType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Char"/>.</param>
		IQuery SetCharacter(int position, char val);

		/// <summary>
		/// Bind an instance of a <see cref="Char" /> to a named parameter
		/// using an NHibernate <see cref="CharType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Char"/>.</param>
		IQuery SetCharacter(string name, char val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to an indexed parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IQuery SetDateTime(int position, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to a named parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		/// <param name="name">The name of the parameter</param>
		IQuery SetDateTime(string name, DateTime val);

		IQuery SetDateTime2(int position, DateTime val);
		IQuery SetDateTime2(string name, DateTime val);
		IQuery SetTimeSpan(int position, TimeSpan val);
		IQuery SetTimeSpan(string name, TimeSpan val);
		IQuery SetTimeAsTimeSpan(int position, TimeSpan val);
		IQuery SetTimeAsTimeSpan(string name, TimeSpan val);
		IQuery SetDateTimeOffset(int position, DateTimeOffset val);
		IQuery SetDateTimeOffset(string name, DateTimeOffset val);

		/// <summary>
		/// Bind an instance of a <see cref="Decimal" /> to an indexed parameter
		/// using an NHibernate <see cref="DecimalType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Decimal"/>.</param>
		IQuery SetDecimal(int position, decimal val);

		/// <summary>
		/// Bind an instance of a <see cref="Decimal" /> to a named parameter
		/// using an NHibernate <see cref="DecimalType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Decimal"/>.</param>
		IQuery SetDecimal(string name, decimal val);

		/// <summary>
		/// Bind an instance of a <see cref="Double" /> to an indexed parameter
		/// using an NHibernate <see cref="DoubleType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Double"/>.</param>
		IQuery SetDouble(int position, double val);

		/// <summary>
		/// Bind an instance of a <see cref="Double" /> to a named parameter
		/// using an NHibernate <see cref="DoubleType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Double"/>.</param>
		IQuery SetDouble(string name, double val);

		/// <summary>
		/// Bind an instance of a persistent enumeration class to an indexed parameter
		/// using an NHibernate <see cref="PersistentEnumType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		IQuery SetEnum(int position, Enum val);

		/// <summary>
		/// Bind an instance of a persistent enumeration class to a named parameter
		/// using an NHibernate <see cref="PersistentEnumType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		IQuery SetEnum(string name, Enum val);

		/// <summary>
		/// Bind an instance of a <see cref="Int16" /> to an indexed parameter
		/// using an NHibernate <see cref="Int16Type"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Int16"/>.</param>
		IQuery SetInt16(int position, short val);

		/// <summary>
		/// Bind an instance of a <see cref="Int16" /> to a named parameter
		/// using an NHibernate <see cref="Int16Type"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Int16"/>.</param>
		IQuery SetInt16(string name, short val);

		/// <summary>
		/// Bind an instance of a <see cref="Int32" /> to an indexed parameter
		/// using an NHibernate <see cref="Int32Type"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Int32"/>.</param>
		IQuery SetInt32(int position, int val);

		/// <summary>
		/// Bind an instance of a <see cref="Int32" /> to a named parameter
		/// using an NHibernate <see cref="Int32Type"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Int32"/>.</param>
		IQuery SetInt32(string name, int val);

		/// <summary>
		/// Bind an instance of a <see cref="Int64" /> to an indexed parameter
		/// using an NHibernate <see cref="Int64Type"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Int64"/>.</param>
		IQuery SetInt64(int position, long val);

		/// <summary>
		/// Bind an instance of a <see cref="Int64" /> to a named parameter
		/// using an NHibernate <see cref="Int64Type"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Int64"/>.</param>
		IQuery SetInt64(string name, long val);

		/// <summary>
		/// Bind an instance of a <see cref="Single" /> to an indexed parameter
		/// using an NHibernate <see cref="SingleType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Single"/>.</param>
		IQuery SetSingle(int position, float val);

		/// <summary>
		/// Bind an instance of a <see cref="Single" /> to a named parameter
		/// using an NHibernate <see cref="SingleType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Single"/>.</param>
		IQuery SetSingle(string name, float val);

		/// <summary>
		/// Bind an instance of a <see cref="String" /> to an indexed parameter
		/// using an NHibernate <see cref="StringType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="String"/>.</param>
		IQuery SetString(int position, string val);

		/// <summary>
		/// Bind an instance of a <see cref="String" /> to a named parameter
		/// using an NHibernate <see cref="StringType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="String"/>.</param>
		IQuery SetString(string name, string val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to an indexed parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IQuery SetTime(int position, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to a named parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IQuery SetTime(string name, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to an indexed parameter
		/// using an NHibernate <see cref="TimestampType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IQuery SetTimestamp(int position, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to a named parameter
		/// using an NHibernate <see cref="TimestampType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IQuery SetTimestamp(string name, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="Guid" /> to a named parameter
		/// using an NHibernate <see cref="GuidType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">An instance of a <see cref="Guid"/>.</param>
		IQuery SetGuid(int position, Guid val);

		/// <summary>
		/// Bind an instance of a <see cref="Guid" /> to a named parameter
		/// using an NHibernate <see cref="GuidType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">An instance of a <see cref="Guid"/>.</param>
		IQuery SetGuid(string name, Guid val);

		/// <summary>
		/// Bind an instance of a mapped persistent class to an indexed parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent class</param>
		IQuery SetEntity(int position, object val);

		/// <summary>
		/// Bind an instance of a mapped persistent class to a named parameter.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent class</param>
		IQuery SetEntity(string name, object val);

		/// <summary>
		/// Set a strategy for handling the query results. This can be used to change
		/// "shape" of the query result.
		/// </summary>
		IQuery SetResultTransformer(IResultTransformer resultTransformer);

		/// <summary>
		/// Get a enumerable that when enumerated will execute
		/// a batch of queries in a single database roundtrip
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<T> Future<T>();

		/// <summary>
		/// Get an IFutureValue instance, whose value can be retrieved through
		/// its Value property. The query is not executed until the Value property
		/// is retrieved, which will execute other Future queries as well in a
		/// single roundtrip
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IFutureValue<T> FutureValue<T>();

#if ASYNC
		/// <summary>
		/// Get a enumerable that when enumerated will execute
		/// a batch of queries in a single database roundtrip
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IAsyncEnumerable<T> FutureAsync<T>();

		/// <summary>
		/// Get an IFutureValueAsync instance, whose value can be retrieved through
		/// its GetValue method. The query is not executed until the GetValue method
		/// is called, which will execute other Future queries as well in a
		/// single roundtrip
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IFutureValueAsync<T> FutureValueAsync<T>();
#endif
	}
}
