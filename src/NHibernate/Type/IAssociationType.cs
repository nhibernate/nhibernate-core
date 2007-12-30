using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that represents some kind of association between entities.
	/// </summary>
	public interface IAssociationType : IType
	{
		/// <summary>
		/// When implemented by a class, gets the type of foreign key directionality 
		/// of this association.
		/// </summary>
		/// <value>The <see cref="ForeignKeyDirection"/> of this association.</value>
		ForeignKeyDirection ForeignKeyDirection { get; }

		/// <summary>
		/// Is the primary key of the owning entity table
		/// to be used in the join?
		/// </summary>
		bool UseLHSPrimaryKey { get; }

		/// <summary>
		/// Get the name of the property in the owning entity
		/// that provides the join key (null if the identifier)
		/// </summary>
		string LHSPropertyName { get; }

		/// <summary>
		/// The name of a unique property of the associated entity 
		/// that provides the join key (null if the identifier of
		/// an entity, or key of a collection)
		/// </summary>
		string RHSUniqueKeyPropertyName { get; }

		/// <summary>
		/// Get the "persister" for this association - a class or collection persister
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		IJoinable GetAssociatedJoinable(ISessionFactoryImplementor factory);

		/// <summary> Get the entity name of the associated entity</summary>
		string GetAssociatedEntityName(ISessionFactoryImplementor factory);

		/// <summary>
		/// Do we dirty check this association, even when there are
		/// no columns to be updated.
		/// </summary>
		bool IsAlwaysDirtyChecked { get; }

		/// <summary>
		/// Get the "filtering" SQL fragment that is applied in the
		/// SQL on clause, in addition to the usual join condition.
		/// </summary>
		string GetOnCondition(string alias, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters);

		bool IsEmbeddedInXML { get;}
	}
}
