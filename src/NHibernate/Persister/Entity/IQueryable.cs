using System;
using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	public enum Declarer
	{
		Class,
		SubClass,
		SuperClass
	}

	// 6.0 TODO: Move into IQueryable
	public static class AbstractEntityPersisterExtensions
	{
		/// <summary>
		/// Gets the properties select fragment.
		/// </summary>
		/// <param name="query">The <see cref="IQueryable"/> instance.</param>
		/// <param name="alias">The table alias</param>
		/// <param name="suffix">The column suffix.</param>
		/// <param name="fetchProperties">Lazy properties to fetch.</param>
		/// <returns>The properties select fragment.</returns>
		internal static SelectFragment GetPropertiesSelectFragment(this IQueryable query, string alias, string suffix, ICollection<string> fetchProperties)
		{
			return ReflectHelper.CastOrThrow<AbstractEntityPersister>(query, "individual lazy property fetches")
			                    .GetPropertiesSelectFragment(alias, suffix, fetchProperties);
		}

		/// <summary>
		/// Gets the identifier select fragment.
		/// </summary>
		/// <param name="queryable">The <see cref="IQueryable"/> instance.</param>
		/// <param name="alias">The table alias</param>
		/// <param name="suffix">The column suffix.</param>
		/// <returns>The identifier select fragment.</returns>
		public static SelectFragment GetIdentifierSelectFragment(this IQueryable queryable, string alias, string suffix)
		{
			if (queryable is AbstractEntityPersister entityPersister)
			{
				return entityPersister.GetIdentifierSelectFragment(alias, suffix);
			}

			return new SelectFragment(queryable.Factory.Dialect)
			       .SetSuffix(suffix)
			       .AddColumns(alias, queryable.IdentifierColumnNames, queryable.GetIdentifierAliases(null));
		}

		/// <summary>
		/// Gets the properties select fragment.
		/// </summary>
		/// <param name="queryable">The <see cref="IQueryable"/> instance.</param>
		/// <param name="alias">The table alias</param>
		/// <param name="suffix">The column suffix.</param>
		/// <param name="allProperties">Whether to fetch all lazy properties.</param>
		/// <returns>The properties select fragment.</returns>
		public static SelectFragment GetPropertiesSelectFragment(this IQueryable queryable, string alias, string suffix, bool allProperties)
		{
			if (queryable is AbstractEntityPersister entityPersister)
			{
				return entityPersister.GetPropertiesSelectFragment(alias, suffix, allProperties);
			}

#pragma warning disable 618
			var text = queryable.PropertySelectFragment(alias, suffix, allProperties);
#pragma warning restore 618
			return new SelectFragment(queryable.Factory.Dialect, text, null)
				.SetSuffix(suffix);
		}
	}

	/// <summary>
	/// Extends the generic <c>ILoadable</c> contract to add operations required by HQL
	/// </summary>
	public interface IQueryable : ILoadable, IPropertyMapping, IJoinable
	{
		/// <summary>
		/// Is this class explicit polymorphism only?
		/// </summary>
		bool IsExplicitPolymorphism { get; }

		/// <summary>
		/// The class that this class is mapped as a subclass of - not necessarily the direct superclass
		/// </summary>
		string MappedSuperclass { get; }

		/// <summary>
		/// The discriminator value for this particular concrete subclass, as a string that may be
		/// embedded in a select statement
		/// </summary>
		string DiscriminatorSQLValue { get; }

		/// <summary>
		/// The discriminator value for this particular concrete subclass
		/// </summary>
		/// <remarks>The DiscriminatorValue is specific of NH since we are using strongly typed parameters for SQL query.</remarks>
		object DiscriminatorValue { get; }

		/// <summary> 
		/// Is the inheritance hierarchy described by this persister contained across
		/// multiple tables? 
		/// </summary>
		/// <returns> True if the inheritance hierarchy is spread across multiple tables; false otherwise. </returns>
		bool IsMultiTable { get; }

		/// <summary> 
		/// Get the names of all tables used in the hierarchy (up and down) ordered such
		/// that deletes in the given order would not cause constraint violations. 
		/// </summary>
		/// <returns> The ordered array of table names. </returns>
		string[] ConstraintOrderedTableNameClosure { get; }

		/// <summary> 
		/// For each table specified in <see cref="ConstraintOrderedTableNameClosure"/>, get
		/// the columns that define the key between the various hierarchy classes.
		/// </summary>
		/// <returns>
		/// The first dimension here corresponds to the table indexes returned in
		/// <see cref="ConstraintOrderedTableNameClosure"/>.
		/// <para/>
		/// The second dimension should have the same length across all the elements in
		/// the first dimension.  If not, that'd be a problem ;) 
		/// </returns>
		string[][] ConstraintOrderedTableKeyColumnClosure { get; }

		/// <summary> 
		/// Get the name of the temporary table to be used to (potentially) store id values
		/// when performing bulk update/deletes. 
		/// </summary>
		/// <returns> The appropriate temporary table name. </returns>
		string TemporaryIdTableName { get; }

		/// <summary> 
		/// Get the appropriate DDL command for generating the temporary table to
		/// be used to (potentially) store id values when performing bulk update/deletes. 
		/// </summary>
		/// <returns> The appropriate temporary table creation command. </returns>
		string TemporaryIdTableDDL { get; }

		/// <summary> Is the version property included in insert statements?</summary>
		bool VersionPropertyInsertable { get; }

		/// <summary>
		/// Given a query alias and an identifying suffix, render the identifier select fragment.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		// Since v5.4
		[Obsolete("Use GetIdentifierSelectFragment extension method instead.")]
		string IdentifierSelectFragment(string name, string suffix);

		/// <summary>
		/// Given a query alias and an identifying suffix, render the property select fragment.
		/// </summary>
		// Since v5.4
		[Obsolete("Use GetPropertiesSelectFragment extension method instead.")]
		string PropertySelectFragment(string alias, string suffix, bool allProperties);

		/// <summary> 
		/// Given a property name, determine the number of the table which contains the column
		/// to which this property is mapped.
		/// </summary>
		/// <param name="propertyPath">The name of the property. </param>
		/// <returns> The number of the table to which the property is mapped. </returns>
		/// <remarks>
		/// Note that this is <b>not</b> relative to the results from {@link #getConstraintOrderedTableNameClosure()}.
		/// It is relative to the subclass table name closure maintained internal to the persister (yick!).
		/// It is also relative to the indexing used to resolve {@link #getSubclassTableName}...
		/// </remarks>
		int GetSubclassPropertyTableNumber(string propertyPath);

		/// <summary> Determine whether the given property is declared by our
		/// mapped class, our super class, or one of our subclasses...
		/// <p/>
		/// Note: the method is called 'subclass property...' simply
		/// for consistency sake (e.g. {@link #getSubclassPropertyTableNumber}
		///  </summary>
		/// <param name="propertyPath">The property name. </param>
		/// <returns> The property declarer </returns>
		Declarer GetSubclassPropertyDeclarer(string propertyPath);

		/// <summary> 
		/// Get the name of the table with the given index from the internal array. 
		/// </summary>
		/// <param name="number">The index into the internal array. </param>
		/// <returns> </returns>
		string GetSubclassTableName(int number);

		/// <summary> 
		/// The alias used for any filter conditions (mapped where-fragments or
		/// enabled-filters).
		/// </summary>
		/// <param name="rootAlias">The root alias </param>
		/// <returns> The alias used for "filter conditions" within the where clause. </returns>
		/// <remarks>
		/// This may or may not be different from the root alias depending upon the
		/// inheritance mapping strategy. 
		/// </remarks>
		string GenerateFilterConditionAlias(string rootAlias);
	}
}
