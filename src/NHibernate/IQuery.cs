using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate
{
	/// <summary>
	/// An object-oriented representation of a NHibernate query.
	/// </summary>
	/// <remarks>
	/// An <c>IQuery</c> instance is obtained by calling <c>ISession.CreateQuery()</c>. This interface
	/// exposes some extra functionality beyond that provided by <c>ISession.Iterate()</c> and
	/// <c>ISession.Find()</c>;
	/// <list>
	///		<item>A particulare page of the result set may be selected by calling 
	///		<c>SetMaxResults()</c>, <c>SetFirstResult</c></item>
	///		<item>Named query parameters may be used</item>
	/// </list>
	/// <para>
	/// Named query parameters are tokens of the form <c>:name</c> in the query string. A value is bound
	/// to the <c>Int32</c> parameter <c>:foo</c> by calling
	/// <code>
	///		SetParameter("foo", foo, NHibernate.Int32);
	/// </code>
	/// for example. A name may appear multiple times in the query string.
	/// </para>
	/// <para>
	///	JDBC-stype <c>?</c> parameters are also supported. To bind a value to a JDBC-style
	///	parameter use a set method that accepts an <c>int</c> positional argument - numbered from
	///	zero.
	/// </para>
	/// <para>
	/// You may not mix and match JDBC-style parameters and named parameters in the same query.
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
		/// Return the query results as an <see cref="IList"/>. If the query contains multiple results per row,
		/// the results are returned in an instance of <c>object[]</c>.
		/// </summary>
		/// <remarks>
		/// This is a good strategy to use if you expect few of the objects being returned are already loaded
		/// or if you want to fill the 2nd level cache.
		/// </remarks>
		IList List();

		/// <summary>
		/// The maximum number of rows to retrieve
		/// </summary>
		/// <param name="maxResults"></param>
		IQuery SetMaxResults( int maxResults );

		/// <summary>
		/// The first row to retrieve.
		/// </summary>
		/// <param name="firstResult"></param>
		IQuery SetFirstResult( int firstResult );

		/// <summary>
		/// The timeout for the underlying ADO query
		/// </summary>
		/// <param name="timeout"></param>
		IQuery SetTimeout( int timeout );

		/// <summary>
		/// Bind a value to a JDBC-style query parameter
		/// </summary>
		/// <param name="position">Postion of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The Hibernate type</param>
		IQuery SetParameter( int position, object val, IType type );

		/// <summary>
		/// Bind a value to a named query parameter
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The hibernate type</param>
		IQuery SetParameter( string name, object val, IType type );

		/// <summary>
		/// Bind a value to a JDBC-style query parameter, guessing the Hibernate type from
		/// the class of the given object.
		/// </summary>
		/// <param name="position">The position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The non-null parameter value</param>
		IQuery SetParameter( int position, object val );

		/// <summary>
		/// Bind a value to a named query parameter, guessing the Hibernate type from the class of
		/// the given object.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The non-null parameter value</param>
		IQuery SetParameter( string name, object val );

		/// <summary>
		/// Bind multiple values to a named query parameter. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		/// <param name="type">The Hibernate type of the values</param>
		IQuery SetParameterList( string name, ICollection vals, IType type );

		/// <summary>
		/// Bind multiple values to a named query parameter, guessing the Hibernate
		/// type from the class of the first object in the collection. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		IQuery SetParameterList( string name, ICollection vals );

		/// <summary>
		/// Bind the property values of the given object to named paramters of the query,
		/// matching property names with parameter names and mapping property types to
		/// Hibernate types using hueristics.
		/// </summary>
		/// <param name="obj">Any PONO</param>
		IQuery SetProperties( object obj );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetAnsiString( int position, string val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetBinary( int position, byte[] val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetBoolean( int position, bool val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetByte( int position, byte val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetCharacter( int position, char val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetDateTime( int position, DateTime val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetDecimal( int position, decimal val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetDouble( int position, double val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetInt16( int position, short val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetInt32( int position, int val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetInt64( int position, long val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetSingle( int position, float val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetString( int position, string val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetTime( int position, DateTime val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetTimestamp( int position, DateTime val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetAnsiString( string name, string val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetBinary( string name, byte[] val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetBoolean( string name, bool val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetByte( string name, byte val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetCharacter( string name, char val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetDateTime( string name, DateTime val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetDecimal( string name, decimal val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetDouble( string name, double val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetInt16( string name, short val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetInt32( string name, int val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetInt64( string name, long val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetSingle( string name, float val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetString( string name, string val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetTime( string name, DateTime val );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		IQuery SetTimestamp( string name, DateTime val );

		/// <summary>
		/// Bind an instance of a mapped persistent class to a JDBC-style query parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent class</param>
		IQuery SetEntity( int position, object val );

		/// <summary>
		/// Bind an instance of a persistent enumeration class to a JDBC-style query parameter.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		IQuery SetEnum( int position, Enum val );

		/// <summary>
		/// Bind an instance of a mapped persistent class to a named query parameter.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent class</param>
		IQuery SetEntity( string name, object val );

		/// <summary>
		/// Bind an instance of a persistent enumeration class to a named query parameter.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		IQuery SetEnum( string name, Enum val );

		/// <summary>
		/// Set the lockmode for the objects idententified by the
		/// given alias that appears in the <tt>FROM</tt> clause.
		/// </summary>
		/// <param name="alias">alias a query alias, or <tt>this</tt> for a collection filter</param>
		/// <param name="lockMode"></param>
		void SetLockMode( string alias, LockMode lockMode );
	}
}