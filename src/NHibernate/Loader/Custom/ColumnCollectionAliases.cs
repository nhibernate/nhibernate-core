using System.Collections.Generic;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// <see cref="ICollectionAliases" /> that uses columnnames instead of generated aliases.
	/// Aliases can still be overwritten via <c>&lt;return-property&gt;</c>
	/// </summary>
	public class ColumnCollectionAliases : ICollectionAliases
	{
		private readonly string[] keyAliases;
		private readonly string[] indexAliases;
		private readonly string[] elementAliases;
		private readonly string identifierAlias;

		public ColumnCollectionAliases(IDictionary<string, string[]> userProvidedAliases, ISqlLoadableCollection persister)
		{
			keyAliases = GetUserProvidedAliases(userProvidedAliases, CollectionPersister.PropKey, persister.KeyColumnNames);
			indexAliases = GetUserProvidedAliases(userProvidedAliases, CollectionPersister.PropIndex, persister.IndexColumnNames);
			elementAliases = GetUserProvidedAliases(userProvidedAliases, CollectionPersister.PropElement, persister.ElementColumnNames);
			identifierAlias = GetUserProvidedAlias(userProvidedAliases, CollectionPersister.PropId, persister.IdentifierColumnName);
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for columns making up the key for this collection (i.e., its FK to
		/// its owner).
		/// </summary>
		/// <value>The key result-set column aliases.</value>
		public string[] SuffixedKeyAliases
		{
			get { return keyAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the columns making up the collection's index (map or list).
		/// </summary>
		/// <value>The index result-set column aliases.</value>
		public string[] SuffixedIndexAliases
		{
			get { return indexAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the columns making up the collection's elements.
		/// </summary>
		/// <value>The element result-set column aliases.</value>
		public string[] SuffixedElementAliases
		{
			get { return elementAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the column defining the collection's identifier (if any).
		/// </summary>
		/// <value>The identifier result-set column aliases.</value>
		public string SuffixedIdentifierAlias
		{
			get { return identifierAlias; }
		}

		/// <summary>
		/// Returns the suffix used to unique the column aliases for this particular alias set.
		/// </summary>
		/// <value>The uniqued column alias suffix.</value>
		public string Suffix
		{
			get { return string.Empty; }
		}

		public override string ToString()
		{
			return
				base.ToString() + " [ suffixedKeyAliases=[" + Join(keyAliases) + "], suffixedIndexAliases=[" + Join(indexAliases)
				+ "], suffixedElementAliases=[" + Join(elementAliases) + "], suffixedIdentifierAlias=[" + identifierAlias + "]]";
		}

		private static string Join(string[] aliases)
		{
			return aliases == null
				? null
				: string.Join(", ", aliases);
		}

		private static string[] GetUserProvidedAliases(IDictionary<string, string[]> userProvidedAliases, string propertyPath, string[] defaultAliases)
		{
			if (userProvidedAliases != null && userProvidedAliases.TryGetValue(propertyPath, out var result))
			{
				return result;
			}

			return defaultAliases;
		}

		private static string GetUserProvidedAlias(IDictionary<string, string[]> userProvidedAliases, string propertyPath, string defaultAlias)
		{
			if (userProvidedAliases != null && userProvidedAliases.TryGetValue(propertyPath, out var columns))
			{
				return columns[0];
			}

			return defaultAlias;
		}
	}
}
