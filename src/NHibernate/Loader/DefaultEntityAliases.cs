using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader
{
	/// <summary>
	/// EntityAliases which handles the logic of selecting user provided aliases (via return-property),
	/// before using the default aliases.
	/// Based on https://raw.githubusercontent.com/hibernate/hibernate-orm/master/hibernate-core/src/main/java/org/hibernate/loader/DefaultEntityAliases.java
	/// </summary>
	public class DefaultEntityAliases : IEntityAliases
	{
		private readonly string[] suffixedKeyColumns;
		private readonly string[] suffixedVersionColumn;
		private readonly string[][] suffixedPropertyColumns;
		private readonly string suffixedDiscriminatorColumn;
		private readonly string suffix;
		private string rowIdAlias;
		private readonly IDictionary<string, string[]> userProvidedAliases;

		public DefaultEntityAliases(ILoadable persister, string suffix)
			: this(null, persister, suffix) {}

		/// <summary>
		/// Calculate and cache select-clause aliases.
		/// </summary>
		public DefaultEntityAliases(IDictionary<string, string[]> userProvidedAliases, ILoadable persister, string suffix)
		{
			this.suffix = suffix;
			this.userProvidedAliases = userProvidedAliases?.Count > 0 ? userProvidedAliases : null;

			suffixedKeyColumns = DetermineKeyAliases(persister, suffix);
			suffixedPropertyColumns = GetSuffixedPropertyAliases(persister);
			suffixedDiscriminatorColumn = DetermineDiscriminatorAlias(persister, suffix);

			suffixedVersionColumn = persister.IsVersioned ? suffixedPropertyColumns[persister.VersionProperty] : null;
			//rowIdAlias is generated on demand in property
		}

		private string[] DetermineKeyAliases(ILoadable persister, string suffix)
		{
			if (userProvidedAliases == null)
				return GetIdentifierAliases(persister, suffix);

			return GetUserProvidedAliases(
				persister.IdentifierPropertyName,
				() => GetUserProvidedAliases(EntityPersister.EntityID, () => GetIdentifierAliases(persister, suffix)));
		}

		private string DetermineDiscriminatorAlias(ILoadable persister, string suffix)
		{
			if (userProvidedAliases == null)
				return GetDiscriminatorAlias(persister, suffix);

			return GetUserProvidedAlias(AbstractEntityPersister.EntityClass, () => GetDiscriminatorAlias(persister, suffix));
		}

		/// <summary>
		/// Returns default aliases for all the properties
		/// </summary>
		protected string[][] GetPropertiesAliases(ILoadable persister)
		{
			return Enumerable.Range(0, persister.PropertyNames.Length).Select(i => GetPropertyAliases(persister, i)).ToArray();
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

		private string[] GetUserProvidedAliases(string propertyPath, Func<string[]> getDefaultAliases)
		{
			string[] result = propertyPath == null ? null : GetUserProvidedAlias(propertyPath);
			if (result == null)
			{
				return getDefaultAliases();
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

		private string GetUserProvidedAlias(string propertyPath, Func<string> getDefaultAlias)
		{
			string[] columns = propertyPath == null ? null : GetUserProvidedAlias(propertyPath);
			if (columns == null)
			{
				return getDefaultAlias();
			}
			else
			{
				return columns[0];
			}
		}

		/// <summary>
		/// Returns aliases for subclass persister
		/// </summary>
		public string[][] GetSuffixedPropertyAliases(ILoadable persister)
		{
			if (userProvidedAliases == null)
				return GetPropertiesAliases(persister);

			int size = persister.PropertyNames.Length;
			string[][] suffixedPropertyAliases = new string[size][];
			for (int j = 0; j < size; j++)
			{
				suffixedPropertyAliases[j] = GetUserProvidedAliases(persister.PropertyNames[j], () => GetPropertyAliases(persister, j));
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
			// TODO: not visible to the user!
			get { return rowIdAlias ?? (rowIdAlias = Loadable.RowIdAlias + suffix); }
		}
	}
}
