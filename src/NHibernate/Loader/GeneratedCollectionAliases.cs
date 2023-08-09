using System;
using System.Collections.Generic;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader
{
	/// <summary>
	/// CollectionAliases which handles the logic of selecting user provided aliases (via return-property),
	/// before using the default aliases.
	/// </summary>
	public class GeneratedCollectionAliases : ICollectionAliases
	{
		private readonly string suffix;
		private readonly string[] keyAliases;
		private readonly string[] indexAliases;
		private readonly string[] elementAliases;
		private readonly string identifierAlias;

		public GeneratedCollectionAliases(IDictionary<string, string[]> userProvidedAliases, ICollectionPersister persister, string suffix)
		{
			this.suffix = suffix;

			keyAliases = GetUserProvidedAliases(userProvidedAliases, CollectionPersister.PropKey, persister.GetKeyColumnAliases(suffix));
			indexAliases = GetUserProvidedAliases(userProvidedAliases, CollectionPersister.PropIndex, persister.GetIndexColumnAliases(suffix));

			// NH-1612: Add aliases for all composite element properties to support access
			// to individual composite element properties in <return-property> elements.
			elementAliases = persister.ElementType.IsComponentType
			                 	? GetUserProvidedCompositeElementAliases(userProvidedAliases, persister.GetElementColumnAliases(suffix))
			                 	: GetUserProvidedAliases(userProvidedAliases, CollectionPersister.PropElement, persister.GetElementColumnAliases(suffix));

			identifierAlias = GetUserProvidedAlias(userProvidedAliases, CollectionPersister.PropId, persister.GetIdentifierColumnAlias(suffix));
		}

		public GeneratedCollectionAliases(ICollectionPersister persister, string str)
			: this(null, persister, str) {}

		private static string[] GetUserProvidedCompositeElementAliases(IDictionary<string, string[]> userProvidedAliases, string[] defaultAliases)
		{
			var aliases = new List<string>();
			if (userProvidedAliases != null)
			{
				foreach (var userProvidedAlias in userProvidedAliases)
				{
					if (userProvidedAlias.Key.StartsWith(
						CollectionPersister.PropElement + ".",
						StringComparison.Ordinal))
					{
						aliases.AddRange(userProvidedAlias.Value);
					}
				}
			}

			return aliases.Count > 0 ? aliases.ToArray() : defaultAliases;
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for columns making up the key for this collection (i.e., its FK to
		/// its owner).
		/// </summary>
		public string[] SuffixedKeyAliases
		{
			get { return keyAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the columns making up the collection's index (map or list).
		/// </summary>
		public string[] SuffixedIndexAliases
		{
			get { return indexAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the columns making up the collection's elements.
		/// </summary>
		public string[] SuffixedElementAliases
		{
			get { return elementAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the column defining the collection's identifier (if any).
		/// </summary>
		public string SuffixedIdentifierAlias
		{
			get { return identifierAlias; }
		}

		/// <summary>
		/// Returns the suffix used to unique the column aliases for this particular alias set.
		/// </summary>
		public string Suffix
		{
			get { return suffix; }
		}

		public override string ToString()
		{
			return
				string.Format(
					"{0} [suffix={1}, suffixedKeyAliases=[{2}], suffixedIndexAliases=[{3}], suffixedElementAliases=[{4}], suffixedIdentifierAlias=[{5}]]",
					base.ToString(), suffix, Join(keyAliases), Join(indexAliases), Join(elementAliases), identifierAlias);
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
