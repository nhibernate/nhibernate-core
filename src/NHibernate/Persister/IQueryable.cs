using System;
using NHibernate.Type;

namespace NHibernate.Persister {
	/// <summary>
	/// Extends the generic <c>IClassPersister</c> contract to add operations required
	/// by the query language
	/// </summary>
	public interface IQueryable : ILoadable {
		
		/// <summary>
		/// Is this class mapped as a subclass of another class?
		/// </summary>
		bool IsInherited { get; }

		/// <summary>
		/// Is this class explicit polymorphism only?
		/// </summary>
		bool IsExplicitPolymorphism { get; }

		/// <summary>
		/// The class that this class is mapped as a subclass of - not necessarily the direct superclass
		/// </summary>
		System.Type MappedSuperclass { get; }

		/// <summary>
		/// The discriminator value for this particular concrete subclass, as a string that may be
		/// embedded in a select statement
		/// </summary>
		string DiscriminatorSQLString { get; }

		/// <summary>
		/// Given a component path expression, get the type of the property
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IType GetPropertyType(string path);

		/// <summary>
		/// Given a component path expression, get the type of the composite identifier property
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IType GetIdentifierPropertyType(string path);

		/// <summary>
		/// Get the where clause fragment, give a query alias
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSublcasses"></param>
		/// <returns></returns>
		string QueryWhereFragment(string alias, bool innerJoin, bool includeSublcasses);

		/// <summary>
		/// Given a query alias and a property path, return the qualified column name
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		string[] ToColumns(string alias, string property);
	}
}
