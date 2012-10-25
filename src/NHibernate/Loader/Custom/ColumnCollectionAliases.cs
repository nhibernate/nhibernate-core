using System.Collections;
using System.Collections.Generic;
using NHibernate.Persister.Collection;
using NHibernate.Util;

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
		private readonly IDictionary<string, string[]> userProvidedAliases;

		public ColumnCollectionAliases(IDictionary<string, string[]> userProvidedAliases, ISqlLoadableCollection persister)
		{
			this.userProvidedAliases = userProvidedAliases;

			this.keyAliases = GetUserProvidedAliases("key", persister.KeyColumnNames);
			this.indexAliases = GetUserProvidedAliases("index", persister.IndexColumnNames);

			// NH-1612: Add aliases for all composite element properties to support access
			// to individual composite element properties in <return-property> elements.
			this.elementAliases = persister.ElementType.IsComponentType
				? GetUserProvidedCompositeElementAliases(persister.ElementColumnNames)
				: GetUserProvidedAliases("element", persister.ElementColumnNames);

			this.identifierAlias = GetUserProvidedAlias("id", persister.IdentifierColumnName);
		}

		private string[] GetUserProvidedCompositeElementAliases(string[] defaultAliases)
		{
			if (userProvidedAliases != null)
			{
				var aliases = new List<string>();
				foreach (var userProvidedAlias in userProvidedAliases)
				{
					if (userProvidedAlias.Key.StartsWith("element."))
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

		private static string Join(IEnumerable aliases)
		{
			if (aliases == null)
			{
				return null;
			}

			return StringHelper.Join(", ", aliases);
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
