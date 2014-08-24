using System.Collections.Generic;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// EntityAliases which handles the logic of selecting user provided aliases (via return-property),
	/// before using the default aliases.
	/// </summary>
	public class DefaultEntityAliases : IEntityAliases
	{
		private readonly string[] suffixedKeyColumns;
		private readonly string[] suffixedVersionColumn;
		private readonly string[][] suffixedPropertyColumns;
		private readonly string suffixedDiscriminatorColumn;
		private readonly string suffix;
		private readonly string rowIdAlias;
		private readonly IDictionary<string, string[]> userProvidedAliases;

		public DefaultEntityAliases(ILoadable persister, string suffix)
			: this(new CollectionHelper.EmptyMapClass<string, string[]>(), persister, suffix) {}

		/// <summary>
		/// Calculate and cache select-clause suffixes.
		/// </summary>
		public DefaultEntityAliases(IDictionary<string, string[]> userProvidedAliases, ILoadable persister, string suffix)
		{
			this.suffix = suffix;
			this.userProvidedAliases = userProvidedAliases;

			string[] keyColumnsCandidates = GetUserProvidedAliases(persister.IdentifierPropertyName, null);
			if (keyColumnsCandidates == null)
			{
				suffixedKeyColumns = GetUserProvidedAliases(EntityPersister.EntityID, GetIdentifierAliases(persister, suffix));
			}
			else
			{
				suffixedKeyColumns = keyColumnsCandidates;
			}
			Intern(suffixedKeyColumns);

			suffixedPropertyColumns = GetSuffixedPropertyAliases(persister);
			suffixedDiscriminatorColumn =
				GetUserProvidedAlias(AbstractEntityPersister.EntityClass, GetDiscriminatorAlias(persister, suffix));
			if (persister.IsVersioned)
			{
				suffixedVersionColumn = suffixedPropertyColumns[persister.VersionProperty];
			}
			else
			{
				suffixedVersionColumn = null;
			}
			rowIdAlias = Loadable.RowIdAlias + suffix; // TODO: not visible to the user!
		}

		protected virtual string GetDiscriminatorAlias(ILoadable persister, string suffix)
		{
			return persister.GetDiscriminatorAlias(suffix);
		}

		protected virtual string[] GetIdentifierAliases(ILoadable persister, string suffix)
		{
			return persister.GetIdentifierAliases(suffix);
		}

		protected virtual string[] GetPropertyAliases(ILoadable persister, int j)
		{
			return persister.GetPropertyAliases(suffix, j);
		}

		private string[] GetUserProvidedAliases(string propertyPath, string[] defaultAliases)
		{
			string[] result = propertyPath == null ? null : GetUserProvidedAlias(propertyPath);
			if (result == null)
			{
				return defaultAliases;
			}
			else
			{
				return result;
			}
		}

		private string[] GetUserProvidedAlias(string propertyPath)
		{
			string[] result;
			userProvidedAliases.TryGetValue(propertyPath, out result);
			return result;
		}

		private string GetUserProvidedAlias(string propertyPath, string defaultAlias)
		{
			string[] columns = propertyPath == null ? null : GetUserProvidedAlias(propertyPath);
			if (columns == null)
			{
				return defaultAlias;
			}
			else
			{
				return columns[0];
			}
		}

		public string[][] GetSuffixedPropertyAliases(ILoadable persister)
		{
			int size = persister.PropertyNames.Length;
			string[][] suffixedPropertyAliases = new string[size][];
			for (int j = 0; j < size; j++)
			{
				suffixedPropertyAliases[j] = GetUserProvidedAliases(persister.PropertyNames[j], GetPropertyAliases(persister, j));
				Intern(suffixedPropertyAliases[j]);
			}
			return suffixedPropertyAliases;
		}

		public string[] SuffixedVersionAliases
		{
			get { return suffixedVersionColumn; }
		}

		public string[][] SuffixedPropertyAliases
		{
			get { return suffixedPropertyColumns; }
		}

		public string SuffixedDiscriminatorAlias
		{
			get { return suffixedDiscriminatorColumn; }
		}

		public string[] SuffixedKeyAliases
		{
			get { return suffixedKeyColumns; }
		}

		public string RowIdAlias
		{
			get { return rowIdAlias; }
		}

		private static void Intern(string[] strings)
		{
			for (int i = 0; i < strings.Length; i++)
			{
				strings[i] = StringHelper.InternedIfPossible(strings[i]);
			}
		}
	}
}