using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate {
	/// <summary>
	/// An object-oriented representation of a Hibernate query.
	/// </summary>
	/// <remarks>
	/// A <c>Query</c> instance is obtained by calling <c>ISession.CreateQuery()</c>. This interface
	/// exposes some extra functionality beyond that provided by <c>ISession.Iterate()</c> and
	/// <c>ISession.Find()</c>;
	/// <list>
	///		<item>A particulare page of the result set may be selected by calling 
	///		<c>SetMaxResults()</c>, <c>SetFirstResult</c></item>
	///		<item>Named query parameters may be used</item>
	///		<item>The results may be returned as an instance of <c>ScrollableResults</c></item>
	/// </list>
	/// <para>
	/// Use of <c>SetFirstResult()</c> requires that the ADO driver implements scrollable record sets
	/// </para>
	/// <para>
	/// Named query parameters are tokens of the form <c>:name</c> in the query string. A value is bound
	/// to the <c>integer</c> parameter <c>:foo</c> by calling
	/// <code>
	///		SetParameter("foo", foo, Hibernate.Integer);
	/// </code>
	/// for example. A name may appear multiple times in the query string.
	/// </para>
	/// <para>
	///	JDBC-stype <c>?</c> parameters are also supported. To bind a value to a JDBC-style
	///	parameter use a set method that accepts an <c>int</c> positional argument (numbered from
	///	zero, contrary to JDBC).
	/// </para>
	/// <para>
	/// You may not mix and match JDBC-style parameters and named parameters in the same query.
	/// </para>
	/// <para>
	/// Queries are executed by calling <c>List()</c>, <c>Scroll()</c>, or <c>Iterate()</c>. A query
	/// may be re-executed by subsequent invocations. Its lifespan is, however, bounded by the lifespan
	/// of the <c>ISession</c> that created it.
	/// </para>
	/// <para>
	/// Implementors are not intended to be threadsafe.
	/// </para>
	/// </remarks>
	public interface IQuery {
		

		/// <summary>
		/// The query string
		/// </summary>
		string QueryString {
			get;
		}

		/// <summary>
		/// The Hibernate types of the query result set.
		/// </summary>
		HibernateType[] ReturnTypes {
			get;
		}

		/// <summary>
		/// The names of all named parameters of the query
		/// </summary>
		/// <value>The parameter names, in no particular order</value>
		string[] NamedParameters {
			get;
		}

		/// <summary>
		/// Return the query results as an <c>IEnuemrator</c>. If the query contains multiple results
		/// per row, the results are returned in an instance of <c>object[]</c>.
		/// </summary>
		/// <remarks>
		/// Entities returned as results are initialized on demand. The first SQL query returns
		/// identifiers only.
		/// </remarks>
		IEnumerator Enumerator();

		/// <summary>
		/// Return the query results as <c>ScrollableResults</c>. The scrollability of the returned
		/// results depends upon the ADO support for scrollable <c>ResultSet</c>
		/// </summary>
		/// <remarks>
		/// Entities returned as results are initialized on demand. The first SQL query returns
		/// identifier only.
		/// </remarks>
		IScrollableResults ScrollableResults();

		/// <summary>
		/// Return the query results as a <c>IList</c>. If the query contains multiple results per row,
		/// the results are returned in an instance of <c>object[]</c>.
		/// </summary>
		IList List();

		/// <summary>
		/// The maximum number of rows to retrieve
		/// </summary>
		int MaxResults {
			get; set;
		}

		/// <summary>
		/// The first row to retrieve.
		/// </summary>
		int FirstResult {
			get; set;
		}

		/// <summary>
		/// The timeout for the underlying ADO query
		/// </summary>
		int Timeout {
			get; set;
		}

		/// <summary>
		/// Bind a value to a JDBC-style query parameter
		/// </summary>
		/// <param name="position">Postion of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The Hibernate type</param>
		void SetParameter(int position, object val, HibernateType type);

		/// <summary>
		/// Bind a value to a named query parameter
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The possibly null parameter value</param>
		/// <param name="type">The hibernate type</param>
		void SetParameter(string name, object val, HibernateType type);

		/// <summary>
		/// Bind a value to a JDBC-style query parameter, guessing the Hibernate type from
		/// the class of the given object.
		/// </summary>
		/// <param name="position">The position of the parameter in the query, numbered from <c>0</c></param>
		/// <param name="val">The non-null parameter value</param>
		void SetParameter(int position, object val);

		/// <summary>
		/// Bind a value to a named query parameter, guessing the Hibernate type from the class of
		/// the given object.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">The non-null parameter value</param>
		void SetParameter(string name, object val);

		/// <summary>
		/// Bind multiple values to a named query parameter. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		/// <param name="type">The Hibernate type of the values</param>
		void SetParameterList(string name, ICollection vals, HibernateType type);

		/// <summary>
		/// Bind multiple values to a named query parameter, guessing the Hibernate
		/// type from the class of the first object in the collection. This is useful for binding a list
		/// of values to an expression such as <c>foo.bar in (:value_list)</c>
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="vals">A collection of values to list</param>
		void SetParameterList(string name, ICollection vals);

		/// <summary>
		/// Bind the property values of the given object to named paramters of the query,
		/// matching property names with parameter names and mapping property types to
		/// Hibernate types using hueristics.
		/// </summary>
		/// <param name="obj">Any PONO</param>
		void SetProperties(object obj);

		/// <summary>
		/// Bind an instance of a mapped persistent class to a JDBC-style query parameter.
		/// </summary>
		/// <param name="position">Position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent class</param>
		void SetEntity(int position, object val);

		/// <summary>
		/// Bind an instance of a persistent enumeration class to a JDBC-style query parameter.
		/// </summary>
		/// <param name="position">The position of the parameter in the query string, numbered from <c>0</c></param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		void SetEnum(int position, IPersistentEnum val);

		/// <summary>
		/// Bind an instance of a mapped persistent class to a named query parameter.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent class</param>
		void SetEntity(string name, object val);

		/// <summary>
		/// Bind an instance of a persistent enumeration class to a named query parameter.
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="val">A non-null instance of a persistent enumeration</param>
		void SetEnum(string name, IPersistentEnum val);
	}
}
