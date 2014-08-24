using System;
using System.Collections;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate
{
	/// <summary>
	/// Interface  to create queries in "detached mode" where the NHibernate session is not available.
	/// All methods have the same semantics as the corresponding methods of the <see cref="IQuery"/> interface.
	/// </summary>
	public interface IDetachedQuery
	{
		/// <summary>
		/// Get an executable instance of <see cref="IQuery"/>,
		/// to actually run the query.</summary>
		IQuery GetExecutableQuery(ISession session);

		/// <summary>
		/// Set the maximum number of rows to retrieve.
		/// </summary>
		/// <param name="maxResults">The maximum number of rows to retreive.</param>
		IDetachedQuery SetMaxResults(int maxResults);

		/// <summary>
		/// Sets the first row to retrieve.
		/// </summary>
		/// <param name="firstResult">The first row to retreive.</param>
		IDetachedQuery SetFirstResult(int firstResult);

		/// <summary>
		/// Enable caching of this query result set.
		/// </summary>
		/// <param name="cacheable">Should the query results be cacheable?</param>
		IDetachedQuery SetCacheable(bool cacheable);

		/// Set the name of the cache region.
		/// <param name="cacheRegion">The name of a query cache region, or <see langword="null" />
		/// for the default query cache</param>
		IDetachedQuery SetCacheRegion(string cacheRegion);

		/// <summary> 
		/// Entities retrieved by this query will be loaded in 
		/// a read-only mode where Hibernate will never dirty-check
		/// them or make changes persistent. 
		/// </summary>
		/// <param name="readOnly">Enable/Disable read -only mode</param>
		IDetachedQuery SetReadOnly(bool readOnly);

		/// <summary>
		/// The timeout for the underlying ADO query
		/// </summary>
		/// <param name="timeout"></param>
		IDetachedQuery SetTimeout(int timeout);

		/// <summary> Set a fetch size for the underlying ADO query.</summary>
		/// <param name="fetchSize">the fetch size </param>
		IDetachedQuery SetFetchSize(int fetchSize);

		/// <summary>
		/// Set the lockmode for the objects idententified by the
		/// given alias that appears in the <c>FROM</c> clause.
		/// </summary>
		/// <param name="alias">alias a query alias, or <c>this</c> for a collection filter</param>
		/// <param name="lockMode"></param>
		void SetLockMode(string alias, LockMode lockMode);

		/// <summary> Add a comment to the generated SQL.</summary>
		/// <param name="comment">a human-readable string </param>
		IDetachedQuery SetComment(string comment);

		/// <summary>
		/// Bind a value to an indexed parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The Hibernate type</param>
		IDetachedQuery SetParameter(int position, object val, IType type);

		/// <summary>
		/// Bind a value to a named query parameter
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The NHibernate <see cref="IType"/>.</param>
		IDetachedQuery SetParameter(string name, object val, IType type);

		/// <summary>
		/// Bind a value to an indexed parameter, guessing the Hibernate type from
		/// the class of the given object.
		/// </summary>
		/// <param name="position">The position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The non-null parameter value</param>
		IDetachedQuery SetParameter(int position, object val);

		/// <summary>
		/// Bind a value to a named query parameter, guessing the NHibernate <see cref="IType"/>
		/// from the class of the given object.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The non-null parameter value</param>
		IDetachedQuery SetParameter(string name, object val);

		/// <summary>
		/// Bind multiple values to a named query parameter. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		/// <param name="type">The Hibernate type of the values</param>
		IDetachedQuery SetParameterList(string name, IEnumerable vals, IType type);

		/// <summary>
		/// Bind multiple values to a named query parameter, guessing the Hibernate
		/// type from the class of the first object in the collection. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		IDetachedQuery SetParameterList(string name, IEnumerable vals);

		/// <summary>
		/// Bind the property values of the given object to named parameters of the query,
		/// matching property names with parameter names and mapping property types to
		/// Hibernate types using heuristics.
		/// </summary>
		/// <param name="obj">Any POCO</param>
		IDetachedQuery SetProperties(object obj);

		/// <summary>
		/// Bind an instance of a <see cref="string" /> to an indexed parameter
		/// using an NHibernate <see cref="AnsiStringType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="string"/>.</param>
		IDetachedQuery SetAnsiString(int position, string val);

		/// <summary>
		/// Bind an instance of a <see cref="string" /> to a named parameter
		/// using an NHibernate <see cref="AnsiStringType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="string"/>.</param>
		IDetachedQuery SetAnsiString(string name, string val);

		/// <summary>
		/// Bind an instance of a <see cref="byte" /> array to an indexed parameter
		/// using an NHibernate <see cref="BinaryType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="byte"/> array.</param>
		IDetachedQuery SetBinary(int position, byte[] val);

		/// <summary>
		/// Bind an instance of a <see cref="byte" /> array to a named parameter
		/// using an NHibernate <see cref="BinaryType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="byte"/> array.</param>
		IDetachedQuery SetBinary(string name, byte[] val);

		/// <summary>
		/// Bind an instance of a <see cref="bool" /> to an indexed parameter
		/// using an NHibernate <see cref="BooleanType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="bool"/>.</param>
		IDetachedQuery SetBoolean(int position, bool val);

		/// <summary>
		/// Bind an instance of a <see cref="bool" /> to a named parameter
		/// using an NHibernate <see cref="BooleanType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="bool"/>.</param>
		IDetachedQuery SetBoolean(string name, bool val);

		/// <summary>
		/// Bind an instance of a <see cref="byte" /> to an indexed parameter
		/// using an NHibernate <see cref="ByteType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="byte"/>.</param>
		IDetachedQuery SetByte(int position, byte val);

		/// <summary>
		/// Bind an instance of a <see cref="byte" /> to a named parameter
		/// using an NHibernate <see cref="ByteType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="byte"/>.</param>
		IDetachedQuery SetByte(string name, byte val);

		/// <summary>
		/// Bind an instance of a <see cref="char" /> to an indexed parameter
		/// using an NHibernate <see cref="CharType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="char"/>.</param>
		IDetachedQuery SetCharacter(int position, char val);

		/// <summary>
		/// Bind an instance of a <see cref="char" /> to a named parameter
		/// using an NHibernate <see cref="CharType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="char"/>.</param>
		IDetachedQuery SetCharacter(string name, char val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to an indexed parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IDetachedQuery SetDateTime(int position, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to a named parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		/// <param name="name">The name of the parameter</param>
		IDetachedQuery SetDateTime(string name, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="Decimal" /> to an indexed parameter
		/// using an NHibernate <see cref="DecimalType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Decimal"/>.</param>
		IDetachedQuery SetDecimal(int position, decimal val);

		/// <summary>
		/// Bind an instance of a <see cref="Decimal" /> to a named parameter
		/// using an NHibernate <see cref="DecimalType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Decimal"/>.</param>
		IDetachedQuery SetDecimal(string name, decimal val);

		/// <summary>
		/// Bind an instance of a <see cref="Double" /> to an indexed parameter
		/// using an NHibernate <see cref="DoubleType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Double"/>.</param>
		IDetachedQuery SetDouble(int position, double val);

		/// <summary>
		/// Bind an instance of a <see cref="Double" /> to a named parameter
		/// using an NHibernate <see cref="DoubleType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Double"/>.</param>
		IDetachedQuery SetDouble(string name, double val);

		/// <summary>
		/// Bind an instance of a mapped persistent class to an indexed parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent class</param>
		IDetachedQuery SetEntity(int position, object val);

		/// <summary>
		/// Bind an instance of a mapped persistent class to a named parameter.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent class</param>
		IDetachedQuery SetEntity(string name, object val);

		/// <summary>
		/// Bind an instance of a persistent enumeration class to an indexed parameter
		/// using an NHibernate <see cref="PersistentEnumType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		IDetachedQuery SetEnum(int position, Enum val);

		/// <summary>
		/// Bind an instance of a persistent enumeration class to a named parameter
		/// using an NHibernate <see cref="PersistentEnumType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		IDetachedQuery SetEnum(string name, Enum val);

		/// <summary>
		/// Bind an instance of a <see cref="Int16" /> to an indexed parameter
		/// using an NHibernate <see cref="Int16Type"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Int16"/>.</param>
		IDetachedQuery SetInt16(int position, short val);

		/// <summary>
		/// Bind an instance of a <see cref="Int16" /> to a named parameter
		/// using an NHibernate <see cref="Int16Type"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Int16"/>.</param>
		IDetachedQuery SetInt16(string name, short val);

		/// <summary>
		/// Bind an instance of a <see cref="Int32" /> to an indexed parameter
		/// using an NHibernate <see cref="Int32Type"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Int32"/>.</param>
		IDetachedQuery SetInt32(int position, int val);

		/// <summary>
		/// Bind an instance of a <see cref="Int32" /> to a named parameter
		/// using an NHibernate <see cref="Int32Type"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Int32"/>.</param>
		IDetachedQuery SetInt32(string name, int val);

		/// <summary>
		/// Bind an instance of a <see cref="Int64" /> to an indexed parameter
		/// using an NHibernate <see cref="Int64Type"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Int64"/>.</param>
		IDetachedQuery SetInt64(int position, long val);

		/// <summary>
		/// Bind an instance of a <see cref="Int64" /> to a named parameter
		/// using an NHibernate <see cref="Int64Type"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Int64"/>.</param>
		IDetachedQuery SetInt64(string name, long val);

		/// <summary>
		/// Bind an instance of a <see cref="Single" /> to an indexed parameter
		/// using an NHibernate <see cref="SingleType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="Single"/>.</param>
		IDetachedQuery SetSingle(int position, float val);

		/// <summary>
		/// Bind an instance of a <see cref="Single" /> to a named parameter
		/// using an NHibernate <see cref="SingleType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="Single"/>.</param>
		IDetachedQuery SetSingle(string name, float val);

		/// <summary>
		/// Bind an instance of a <see cref="String" /> to an indexed parameter
		/// using an NHibernate <see cref="StringType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="String"/>.</param>
		IDetachedQuery SetString(int position, string val);

		/// <summary>
		/// Bind an instance of a <see cref="String" /> to a named parameter
		/// using an NHibernate <see cref="StringType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="String"/>.</param>
		IDetachedQuery SetString(string name, string val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to an indexed parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IDetachedQuery SetTime(int position, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to a named parameter
		/// using an NHibernate <see cref="DateTimeType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IDetachedQuery SetTime(string name, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to an indexed parameter
		/// using an NHibernate <see cref="TimestampType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IDetachedQuery SetTimestamp(int position, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="DateTime" /> to a named parameter
		/// using an NHibernate <see cref="TimestampType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a <see cref="DateTime"/>.</param>
		IDetachedQuery SetTimestamp(string name, DateTime val);

		/// <summary>
		/// Bind an instance of a <see cref="Guid" /> to a named parameter
		/// using an NHibernate <see cref="GuidType"/>.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">An instance of a <see cref="Guid"/>.</param>
		IDetachedQuery SetGuid(int position, Guid val);

		/// <summary>
		/// Bind an instance of a <see cref="Guid" /> to a named parameter
		/// using an NHibernate <see cref="GuidType"/>.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">An instance of a <see cref="Guid"/>.</param>
		IDetachedQuery SetGuid(string name, Guid val);

		/// <summary>
		/// Override the current session flush mode, just for this query.
		/// </summary>
		IDetachedQuery SetFlushMode(FlushMode flushMode);

		/// <summary>
		/// Set a strategy for handling the query results. This can be used to change
		/// "shape" of the query result.
		/// </summary>
		IDetachedQuery SetResultTransformer(IResultTransformer resultTransformer);

		/// <summary>
		/// Set the value to ignore unknown parameters names.
		/// </summary>
		/// <param name="ignoredUnknownNamedParameters">True to ignore unknown parameters names.</param>
		IDetachedQuery SetIgnoreUknownNamedParameters(bool ignoredUnknownNamedParameters);

		/// <summary> Override the current session cache mode, just for this query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		IDetachedQuery SetCacheMode(CacheMode cacheMode);
	}
}
