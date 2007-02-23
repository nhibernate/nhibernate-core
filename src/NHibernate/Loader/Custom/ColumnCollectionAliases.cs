using System;
using System.Collections;
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
		private IDictionary userProvidedAliases;

		public ColumnCollectionAliases(IDictionary userProvidedAliases, ISqlLoadableCollection persister)
		{
			this.userProvidedAliases = userProvidedAliases;

			this.keyAliases = GetUserProvidedAliases(
				"key",
				persister.KeyColumnNames
				);

			this.indexAliases = GetUserProvidedAliases(
				"index",
				persister.IndexColumnNames
				);

			this.elementAliases = GetUserProvidedAliases("element",
			                                             persister.ElementColumnNames
				);

			this.identifierAlias = GetUserProvidedAlias("id",
			                                            persister.IdentifierColumnName
				);
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
		/// Returns the suffixed result-set column-aliases for the collumns making up the collection's index (map or list).
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
			get { return ""; }
		}

		public override string ToString()
		{
			return base.ToString() + " [ suffixedKeyAliases=[" + Join(keyAliases) +
			       "], suffixedIndexAliases=[" + Join(indexAliases) +
			       "], suffixedElementAliases=[" + Join(elementAliases) +
			       "], suffixedIdentifierAlias=[" + identifierAlias + "]]";
		}

		private string Join(string[] aliases)
		{
			if (aliases == null) return null;

			return StringHelper.Join(", ", aliases);
		}

		private string[] GetUserProvidedAliases(string propertyPath, string[] defaultAliases)
		{
			string[] result = (string[]) userProvidedAliases[propertyPath];
			if (result == null)
			{
				return defaultAliases;
			}
			else
			{
				return result;
			}
		}

		private string GetUserProvidedAlias(string propertyPath, string defaultAlias)
		{
			string[] columns = (string[]) userProvidedAliases[propertyPath];
			if (columns == null)
			{
				return defaultAlias;
			}
			else
			{
				return columns[0];
			}
		}
	}
}