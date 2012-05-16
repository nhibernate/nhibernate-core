using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Id
{
	// 6.0 TODO: remove
	internal static class PostInsertIdentityPersisterExtension
	{
		public static SqlString GetSelectByUniqueKeyString(
			this IPostInsertIdentityPersister persister,
			string[] propertyNames)
		{
			if (persister is ICompositeKeyPostInsertIdentityPersister compositeKeyPersister)
				return compositeKeyPersister.GetSelectByUniqueKeyString(propertyNames);

			if (propertyNames.Length > 1)
			{
				throw new IdentifierGenerationException(
					$"persister {persister} does not implement {nameof(ICompositeKeyPostInsertIdentityPersister)}, " +
					$"which is required for selecting by an unique key spanning multiple properties.");
			}

#pragma warning disable 618
			return persister.GetSelectByUniqueKeyString(propertyNames[0]);
#pragma warning restore 618
		}
	}

	/// <summary> 
	/// A persister that may have an identity assigned by execution of a SQL <tt>INSERT</tt>. 
	/// </summary>
	public interface IPostInsertIdentityPersister
	{
		/* NH consideration:
		 * this interface was de-wired from IEntityPersister because we want use it for ICollectionPersister too.
		 * More exactly we want use it for id-bag.
		 */
		/// <summary> 
		/// Get the database-specific SQL command to retrieve the last
		/// generated IDENTITY value.
		/// </summary>
		string IdentitySelectString { get;}

		/// <summary> The names of the primary key columns in the root table. </summary>
		/// <returns> The primary key column names. </returns>
		string[] RootTableKeyColumnNames { get;}

		/// <summary> 
		/// Get a SQL select string that performs a select based on a unique
		/// key determined by the given property name). 
		/// </summary>
		/// <param name="propertyName">
		/// The name of the property which maps to the
		/// column(s) to use in the select statement restriction.
		/// </param>
		/// <returns> The SQL select string </returns>
		// Since 5.2
		[Obsolete("Have the persister implement ICompositeKeyPostInsertIdentityPersister and use its GetSelectByUniqueKeyString(string[] propertyNames).")]
		SqlString GetSelectByUniqueKeyString(string propertyName);

		#region NH specific
		/// <summary>
		/// Get the identifier type
		/// </summary>
		IType IdentifierType { get; }

		string GetInfoString();
		#endregion
	}

	// 6.0 TODO: merge in IPostInsertIdentityPersister
	/// <summary> 
	/// An <see cref="IPostInsertIdentityPersister" /> that supports selecting by an unique key spanning
	/// multiple properties.
	/// </summary>
	public interface ICompositeKeyPostInsertIdentityPersister
	{
		/// <summary>
		/// Get a SQL select string that performs a select based on a unique
		/// key determined by the given array of property names.
		/// </summary>
		/// <param name="propertyNames">
		/// The names of the properties which map to the
		/// column(s) to use in the select statement restriction.
		/// </param>
		/// <returns>The SQL select string.</returns>
		SqlString GetSelectByUniqueKeyString(string[] propertyNames);
	}
}
