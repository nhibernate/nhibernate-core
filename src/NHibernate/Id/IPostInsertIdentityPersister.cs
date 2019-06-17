using System;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Id.Insert;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	// 6.0 TODO: remove
	internal static class PostInsertIdentityPersisterExtension
	{
		public static SqlString GetSelectByUniqueKeyString(
			this IPostInsertIdentityPersister persister,
			string[] suppliedPropertyNames,
			out IType[] parameterTypes)
		{
			if (persister is ICompositeKeyPostInsertIdentityPersister compositeKeyPersister)
				return compositeKeyPersister.GetSelectByUniqueKeyString(suppliedPropertyNames, out parameterTypes);

			if (suppliedPropertyNames.Length > 1)
			{
				throw new IdentifierGenerationException(
					$"persister {persister} does not implement {nameof(ICompositeKeyPostInsertIdentityPersister)}, " +
					$"which is required for selecting by an unique key spanning multiple properties.");
			}

			if (persister is IEntityPersister entityPersister)
			{
				var uniqueKeyPropertyNames = suppliedPropertyNames ?? DetermineNameOfPropertiesToUse(entityPersister);

				parameterTypes = uniqueKeyPropertyNames.ToArray(p => entityPersister.GetPropertyType(p));
			}
			else if (persister is IQueryableCollection collectionPersister)
			{
				parameterTypes = new[] { collectionPersister.KeyType, collectionPersister.ElementType };
			}
			else 
			{
				throw new IdentifierGenerationException(
					$"Persister type {persister.GetType()} is not supported by post insert identity persisters");
			}
#pragma warning disable 618
			return persister.GetSelectByUniqueKeyString(suppliedPropertyNames[0]);
#pragma warning restore 618
		}

		internal static string[] DetermineNameOfPropertiesToUse(IEntityPersister persister)
		{
			var naturalIdPropertyIndices = persister.NaturalIdentifierProperties;
			if (naturalIdPropertyIndices == null)
			{
				throw new IdentifierGenerationException(
					"no natural-id property defined; need to specify [key] in generator parameters");
			}

			foreach (var naturalIdPropertyIndex in naturalIdPropertyIndices)
			{
				var inclusion = persister.PropertyInsertGenerationInclusions[naturalIdPropertyIndex];
				if (inclusion != ValueInclusion.None)
				{
					throw new IdentifierGenerationException(
						"natural-id also defined as insert-generated; need to specify [key] in generator parameters");
				}
			}

			var result = new string[naturalIdPropertyIndices.Length];
			for (var i = 0; i < naturalIdPropertyIndices.Length; i++)
			{
				result[i] = persister.PropertyNames[naturalIdPropertyIndices[i]];
			}
			return result;
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
		[Obsolete("Have the persister implement ICompositeKeyPostInsertIdentityPersister and use its GetSelectByUniqueKeyString(string[] suppliedPropertyNames, out IType[] parameterTypes).")]
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
	public partial interface ICompositeKeyPostInsertIdentityPersister
	{
		/// <summary>
		/// Get a SQL select string that performs a select based on an unique key, optionnaly determined by
		/// the given array of property names.
		/// </summary>
		/// <param name="suppliedPropertyNames">The names of the properties which map to the column(s) to use
		/// in the select statement restriction. If supplied, they override the persister logic for determining
		/// them.</param>
		/// <param name="parameterTypes">In return, the parameter types used by the select string.</param>
		/// <returns>The SQL select string.</returns>
		/// <exception cref="NotSupportedException">thrown if <paramref name="suppliedPropertyNames"/> are
		/// specified on a persister which does not allow a custom key.</exception>
		SqlString GetSelectByUniqueKeyString(string[] suppliedPropertyNames, out IType[] parameterTypes);

		/// <summary>
		/// Bind the parameter values of a SQL select command that performs a select based on an unique key.
		/// </summary>
		/// <param name="session">The current <see cref="ISession" />.</param>
		/// <param name="selectCommand">The command.</param>
		/// <param name="binder">The id insertion binder.</param>
		/// <param name="suppliedPropertyNames">The names of the properties which map to the column(s) to use
		/// in the select statement restriction. If supplied, they override the persister logic for determining
		/// them.</param>
		/// <exception cref="NotSupportedException">thrown if <paramref name="suppliedPropertyNames"/> are
		/// specified on a persister which does not allow a custom key.</exception>
		void BindSelectByUniqueKey(
			ISessionImplementor session,
			DbCommand selectCommand,
			IBinder binder,
			string[] suppliedPropertyNames);
	}
}
