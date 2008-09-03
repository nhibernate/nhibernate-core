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
	/// An <c>IQuery</c> instance is obtained by calling <c>ISession.CreateQuery()</c>. This interface
	/// exposes some extra functionality beyond that provided by <c>ISession.Iterate()</c> and
	/// <c>ISession.List()</c>;
	/// <list>
	///		<item>
	///			A particulare page of the result set may be selected by calling 
	///			<c>SetMaxResults()</c>, <c>SetFirstResult()</c>.  The generated sql
	///			depends on the capabilities of the <see cref="Dialect.Dialect"/>.  Some
	///			Dialects are for databases that have built in paging (LIMIT) and those capabilities
	///			will be used to limit the number of records returned by the sql statement. 
	///			If the database does not support LIMITs then all of the records will be returned,
	///			but the objects created will be limited to the specific results requested.
	///		</item>
	///		<item>Named query parameters may be used</item>
	/// </list>
	/// <para>
	/// Named query parameters are tokens of the form <c>:name</c> in the query string. A value is bound
	/// to the <c>Int32</c> parameter <c>:foo</c> by calling
	/// <code>
	///		SetParameter("foo", foo, NHibernateUtil.Int32);
	/// </code>
	/// for example. A name may appear multiple times in the query string.
	/// </para>
	/// <para>
	///	Unnamed parameters <c>?</c> are also supported. To bind a value to an unnamed
	///	parameter use a Set method that accepts an <c>Int32</c> positional argument - numbered from
	///	zero.
	/// </para>
	/// <para>
	/// You may not mix and match unnamed parameters and named parameters in the same query.
	/// </para>
	/// <para>
	/// Queries are executed by calling <c>List()</c> or <c>Iterate()</c>. A query
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
		/// The Hibernate types of the query result set.
		/// </summary>
		IType[] ReturnTypes { get; }

		/// <summary>
		/// The names of all named parameters of the query
		/// </summary>
		/// <value>The parameter names, in no particular order</value>
		string[] NamedParameters { get; }

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
		/// Set the maximum number of rows to retrieve.
		/// </summary>
		/// <param name="maxResults">The maximum number of rows to retreive.</param>
		IQuery SetMaxResults(int maxResults);

		/// <summary>
		/// Sets the first row to retrieve.
		/// </summary>
		/// <param name="firstResult">The first row to retreive.</param>
		IQuery SetFirstResult(int firstResult);

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
		/// Entities retrieved by this query will be loaded in 
		/// a read-only mode where Hibernate will never dirty-check
		/// them or make changes persistent. 
		/// </summary>
		IQuery SetReadOnly(bool readOnly);

		/// <summary>
		/// The timeout for the underlying ADO query
		/// </summary>
		/// <param name="timeout"></param>
		IQuery SetTimeout(int timeout);

		/// <summary>
		/// Set the lockmode for the objects idententified by the
		/// given alias that appears in the <c>FROM</c> clause.
		/// </summary>
		/// <param name="alias">alias a query alias, or <c>this</c> for a collection filter</param>
		/// <param name="lockMode"></param>
		IQuery SetLockMode(string alias, LockMode lockMode);

		/// <summary>
		/// Bind a value to an indexed parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The Hibernate type</param>
		IQuery SetParameter(int position, object val, IType type);

		/// <summary>
		/// Bind a value to a named query parameter
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The NHibernate <see cref="IType"/>.</param>
		IQuery SetParameter(string name, object val, IType type);

		/// <summary>
		/// Bind a value to an indexed parameter, guessing the Hibernate type from
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
		/// <param name="type">The Hibernate type of the values</param>
		IQuery SetParameterList(string name, ICollection vals, IType type);

		/// <summary>
		/// Bind multiple values to a named query parameter, guessing the Hibernate
		/// type from the class of the first object in the collection. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		IQuery SetParameterList(string name, ICollection vals);

		/// <summary> 
		/// Bind multiple values to a named query parameter. This is useful for binding
		/// a list of values to an expression such as <tt>foo.bar in (:value_list)</tt>.
		/// </summary>
		/// <param name="name">the name of the parameter </param>
		/// <param name="vals">a collection of values to list </param>
		/// <param name="type">the Hibernate type of the values </param>
		IQuery SetParameterList(string name, object[] vals, IType type);

		/// <summary> 
		/// Bind multiple values to a named query parameter. The Hibernate type of the parameter is
		/// first detected via the usage/position in the query and if not sufficient secondly 
		/// guessed from the class of the first object in the array. This is useful for binding a list of values
		/// to an expression such as <tt>foo.bar in (:value_list)</tt>.
		/// </summary>
		/// <param name="name">the name of the parameter </param>
		/// <param name="vals">a collection of values to list </param>
		IQuery SetParameterList(string name, object[] vals);

		/// <summary>
		/// Bind the property values of the given object to named parameters of the query,
		/// matching property names with parameter names and mapping property types to
		/// Hibernate types using heuristics.
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
		/// Override the current session flush mode, just for this query.
		/// </summary>
		IQuery SetFlushMode(FlushMode flushMode);

		/// <summary>
		/// Set a strategy for handling the query results. This can be used to change
		/// "shape" of the query result.
		/// </summary>
		IQuery SetResultTransformer(IResultTransformer resultTransformer);

		/// <summary> Override the current session cache mode, just for this query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		IQuery SetCacheMode(CacheMode cacheMode);

		/// <summary> 
		/// Execute the update or delete statement.
		/// </summary>
		/// <returns> The number of entities updated or deleted. </returns>
		int ExecuteUpdate();
	}
}
