using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader
{
	/// <summary>
	/// Metadata describing the SQL result set column aliases
	/// for a particular entity
	/// </summary>
	public interface IEntityAliases
	{
		/// <summary>
		/// The result set column aliases for the primary key columns
		/// </summary>
		string[] SuffixedKeyAliases { get; }

		/// <summary>
		/// The result set column aliases for the discriminator columns
		/// </summary>
		string SuffixedDiscriminatorAlias { get; }

		/// <summary>
		/// The result set column aliases for the version columns
		/// </summary>
		string[] SuffixedVersionAliases { get; }

		/// <summary>
		/// The result set column aliases for the property columns
		/// </summary>
		string[][] SuffixedPropertyAliases { get; }

		/// <summary>
		/// The result set column aliases for the property columns of a subclass
		/// </summary>
		string[][] GetSuffixedPropertyAliases(ILoadable persister);

		/// <summary>
		/// The result set column alias for the Oracle row id
		/// </summary>
		string RowIdAlias { get; }
	}
}