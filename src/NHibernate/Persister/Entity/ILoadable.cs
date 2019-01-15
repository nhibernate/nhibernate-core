using System;
using NHibernate.Type;
using NHibernate.Engine;
using System.Data.Common;

namespace NHibernate.Persister.Entity
{
	public struct Loadable
	{
		public readonly static string RowIdAlias = "rowid_";
	}

	/// <summary>
	/// Implemented by <c>ClassPersister</c> that uses <c>Loader</c>. There are several optional
	/// operations used only by loaders that inherit <c>OuterJoinLoader</c>
	/// </summary>
	public partial interface ILoadable : IEntityPersister
	{
		/// <summary>
		/// The discriminator type
		/// </summary>
		IType DiscriminatorType { get; }

		/// <summary>
		/// Get the names of columns used to persist the identifier
		/// </summary>
		string[] IdentifierColumnNames { get; }

		/// <summary>
		/// Get the name of the column used as a discriminator
		/// </summary>
		string DiscriminatorColumnName { get; }

		bool IsAbstract { get; }

		/// <summary>
		/// Does the persistent class have subclasses?
		/// </summary>
		bool HasSubclasses { get; }

		/// <summary>
		/// Get the concrete subclass corresponding to the given discriminator value
		/// </summary>
		string GetSubclassForDiscriminatorValue(object value);

		/// <summary>
		/// Get the result set aliases used for the identifier columns, given a suffix
		/// </summary>
		string[] GetIdentifierAliases(string suffix);

		/// <summary>
		/// Get the result set aliases used for the property columns, given a suffix (properties of this class, only).
		/// </summary>
		string[] GetPropertyAliases(string suffix, int i);

		/// <summary>
		/// Get the result set column names mapped for this property (properties of this class, only).
		/// </summary>
		string[] GetPropertyColumnNames(int i);

		/// <summary>
		/// Get the alias used for the discriminator column, given a suffix
		/// </summary>
		string GetDiscriminatorAlias(string suffix);

		/// <summary> Does the result set contain rowids?</summary>
		bool HasRowId { get;}

		/// <summary>
		/// Retrieve property values from one row of a result set
		/// </summary>
		//Since 5.3
		[Obsolete("Use the extension method without the rootLoadable parameter instead")]
		object[] Hydrate(DbDataReader rs, object id, object obj, ILoadable rootLoadable, string[][] suffixedPropertyColumns,
						 bool allProperties, ISessionImplementor session);
	}

	public static partial class LoadableExtensions
	{
		public static object[] Hydrate(
			this ILoadable loadable,
			DbDataReader rs,
			object id,
			object obj,
			string[][] suffixedPropertyColumns,
			bool allProperties,
			ISessionImplementor session)
		{
			if (loadable is AbstractEntityPersister entityPersister)
			{
				return entityPersister.Hydrate(rs, id, obj, suffixedPropertyColumns, allProperties, session);
			}

			var rootLoadable = loadable.RootEntityName == loadable.EntityName
				? loadable
				: (ILoadable) loadable.Factory.GetEntityPersister(loadable.RootEntityName);

#pragma warning disable 618
			return loadable.Hydrate(rs, id, obj, rootLoadable, suffixedPropertyColumns, allProperties, session);
#pragma warning restore 618
		}
	}
}
