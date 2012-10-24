using System;
using System.Collections.Generic;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Loader.Custom
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
		private readonly IDictionary<string, string[]> userProvidedAliases;

		public GeneratedCollectionAliases(IDictionary<string, string[]> userProvidedAliases, ICollectionPersister persister,
																			string suffix)
		{
			this.suffix = suffix;
			this.userProvidedAliases = userProvidedAliases;

			keyAliases = GetUserProvidedAliases("key", persister.GetKeyColumnAliases(suffix));

			indexAliases = GetUserProvidedAliases("index", persister.GetIndexColumnAliases(suffix));

			// NH-1612: Add aliases for all composite element properties to support access
			// to individual composite element properties in <return-property> elements.
			elementAliases = persister.ElementType.IsComponentType
			                 	? GetUserProvidedCompositeElementAliases(persister.GetElementColumnAliases(suffix))
			                 	: GetUserProvidedAliases("element", persister.GetElementColumnAliases(suffix));

			identifierAlias = GetUserProvidedAlias("id", persister.GetIdentifierColumnAlias(suffix));
		}

		public GeneratedCollectionAliases(ICollectionPersister persister, string str)
			: this(CollectionHelper.EmptyDictionary<string, string[]>(), persister, str) {}

		private string[] GetUserProvidedCompositeElementAliases(string[] defaultAliases)
		{
			if (userProvidedAliases != null)
			{
				var aliases = new List<string>();
				foreach (var userProvidedAlias in userProvidedAliases)
				{
					if (userProvidedAlias.Key.StartsWith("element.", StringComparison.Ordinal))
					{
						aliases.AddRange(userProvidedAlias.Value);
					}
				}

				if (aliases.Count > 0) return aliases.ToArray();
			}

			return defaultAliases;
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

		private static string Join(IEnumerable<string> aliases)
		{
			return aliases == null ? null : StringHelper.Join(", ", aliases);
		}

		private string[] GetUserProvidedAliases(string propertyPath, string[] defaultAliases)
		{
			string[] result;
			return userProvidedAliases == null || !userProvidedAliases.TryGetValue(propertyPath, out result)
				? defaultAliases
				: result;
		}

		private string GetUserProvidedAlias(string propertyPath, string defaultAlias)
		{
			string[] columns;
			return userProvidedAliases == null || !userProvidedAliases.TryGetValue(propertyPath, out columns)
				? defaultAlias
				: columns[0];
		}
	}
}
