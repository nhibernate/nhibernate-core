namespace NHibernate.Loader
{
	/// <summary>
	/// Type definition of CollectionAliases.
	/// </summary>
	public interface ICollectionAliases
	{
		/// <summary>
		/// Returns the suffixed result-set column-aliases for columns making
		/// up the key for this collection (i.e., its FK to its owner).
		/// </summary>
		/// <value>The key result-set column aliases.</value>
		string[] SuffixedKeyAliases { get; }

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the columns
		/// making up the collection's index (map or list).
		/// </summary>
		/// <value>The index result-set column aliases.</value>
		string[] SuffixedIndexAliases { get; }

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the columns
		/// making up the collection's elements.
		/// </summary>
		/// <value>The element result-set column aliases.</value>
		string[] SuffixedElementAliases { get; }

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the column
		/// defining the collection's identifier (if any).
		/// </summary>
		/// <value>The identifier result-set column aliases.</value>
		string SuffixedIdentifierAlias { get; }

		/// <summary>
		/// Returns the suffix used to unique the column aliases for this
		/// particular alias set.
		/// </summary>
		/// <value>The uniqued column alias suffix.</value>
		string Suffix { get; }
	}
}
