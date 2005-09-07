using System;
using NHibernate.SqlCommand;

namespace NHibernate.Persister
{
	/// <summary>
	/// Anything that can be loaded by outer join - namely persisters for classes or collections.
	/// </summary>
	public interface IJoinable
	{
		// Should this interface extend PropertyMapping?

		/// <summary>
		/// An identifying name; a class name or collection role name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The table to join to.
		/// </summary>
		string TableName { get; }

		/// <summary>
		/// All columns to select, when loading.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <param name="includeCollectionColumns"></param>
		/// <returns></returns>
		SqlString SelectFragment( string alias, string suffix, bool includeCollectionColumns );

		/// <summary>
		/// Get the where clause part of any joins (optional operation)
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses );

		/// <summary>
		/// Get the from clause part of any joins (optional operation)
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses );

		/// <summary>
		/// The columns to join on.
		/// </summary>
		string[] JoinKeyColumnNames { get; }

		/// <summary>
		/// Is this instance actually a ICollectionPersister?
		/// </summary>
		bool IsCollection { get; }

		/// <summary>
		/// Is this instance actually a many-to-many association?
		/// </summary>
		bool IsManyToMany { get; }

		/// <summary>
		/// Ugly, very ugly....
		/// </summary>
		/// <returns></returns>
		bool ConsumesAlias( );
	}
}
