using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Persister
{
	/// <summary>
	/// Extends the generic <c>ILoadable</c> contract to add operations required by HQL
	/// </summary>
	public interface IQueryable : ILoadable, IPropertyMapping, IJoinable
	{
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

		/*
		/// <summary>
		/// The discriminator value for this particular concrete subclass, as a string that may be
		/// embedded in a select statement
		/// </summary>
		string DiscriminatorSQLString { get; }
		*/

		/// <summary>
		/// The discriminator value for this particular concrete subclass, as a string that may be
		/// embedded in a select statement
		/// </summary>
		object DiscriminatorSQLValue { get; }

		/// <summary>
		/// Get the where clause fragment, give a query alias
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSublcasses"></param>
		/// <returns></returns>
		SqlString QueryWhereFragment( string alias, bool innerJoin, bool includeSublcasses );

		/// <summary>
		/// Given a query alias and an identifying suffix, render the intentifier select fragment.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		SqlString IdentifierSelectFragment( string name, string suffix );

		/// <summary>
		/// Given a query alias and an identifying suffix, render the property select fragment.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		SqlString PropertySelectFragment( string alias, string suffix );

		/// <summary>
		/// Get the names of columns used to persist the identifier
		/// </summary>
		string[] IdentifierColumnNames { get; }
	}
}