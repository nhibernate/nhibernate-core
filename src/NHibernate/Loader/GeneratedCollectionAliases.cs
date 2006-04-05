using System;
using System.Collections;

using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// CollectionAliases which handles the logic of selecting user provided aliases (via return-property),
	/// before using the default aliases.
	/// </summary>
	public class GeneratedCollectionAliases : ICollectionAliases
	{
		private readonly string suffix;
		private readonly string[ ] keyAliases;
		private readonly string[ ] indexAliases;
		private readonly string[ ] elementAliases;
		private readonly string identifierAlias;
		private IDictionary userProvidedAliases;

		public GeneratedCollectionAliases( IDictionary userProvidedAliases, ICollectionPersister persister, string suffix )
		{
			this.suffix = suffix;
			this.userProvidedAliases = userProvidedAliases;

			this.keyAliases = GetUserProvidedAliases(
				"key",
				persister.GetKeyColumnAliases( suffix )
				);

			this.indexAliases = GetUserProvidedAliases(
				"index",
				persister.GetIndexColumnAliases( suffix )
				);

			this.elementAliases = GetUserProvidedAliases( "element",
			                                              persister.GetElementColumnAliases( suffix )
				);

			this.identifierAlias = GetUserProvidedAlias( "id",
			                                             persister.GetIdentifierColumnAlias( suffix )
				);
		}

		public GeneratedCollectionAliases( ICollectionPersister persister, string str )
			: this( CollectionHelper.EmptyMap, persister, str )
		{
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for columns making up the key for this collection (i.e., its FK to
		/// its owner).
		/// </summary>
		public string[ ] SuffixedKeyAliases
		{
			get { return keyAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the collumns making up the collection's index (map or list).
		/// </summary>
		public string[ ] SuffixedIndexAliases
		{
			get { return indexAliases; }
		}

		/// <summary>
		/// Returns the suffixed result-set column-aliases for the columns making up the collection's elements.
		/// </summary>
		public string[ ] SuffixedElementAliases
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
			return base.ToString() + " [suffix=" + suffix +
			       ", suffixedKeyAliases=[" + Join( keyAliases ) +
			       "], suffixedIndexAliases=[" + Join( indexAliases ) +
			       "], suffixedElementAliases=[" + Join( elementAliases ) +
			       "], suffixedIdentifierAlias=[" + identifierAlias + "]]";
		}

		private string Join( string[ ] aliases )
		{
			if( aliases == null )
			{
				return null;
			}

			return StringHelper.Join( ", ", aliases );
		}

		private string[ ] GetUserProvidedAliases( string propertyPath, string[ ] defaultAliases )
		{
			string[ ] result = ( string[ ] ) userProvidedAliases[ propertyPath ];
			if( result == null )
			{
				return defaultAliases;
			}
			else
			{
				return result;
			}
		}

		private string GetUserProvidedAlias( string propertyPath, string defaultAlias )
		{
			string[ ] columns = ( string[ ] ) userProvidedAliases[ propertyPath ];
			if( columns == null )
			{
				return defaultAlias;
			}
			else
			{
				return columns[ 0 ];
			}
		}
	}
}