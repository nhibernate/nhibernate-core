using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader
{
	// Based on https://raw.githubusercontent.com/hibernate/hibernate-orm/master/hibernate-core/src/main/java/org/hibernate/loader/DefaultEntityAliases.java
	/// <summary>
	/// EntityAliases which handles the logic of selecting user provided aliases (via return-property),
	/// before using the default aliases.
	/// </summary>
	public class DefaultEntityAliases : IEntityAliases
	{
		private readonly string _suffix;
		private string _rowIdAlias;
		private readonly IDictionary<string, string[]> _userProvidedAliases;

		public DefaultEntityAliases(ILoadable persister, string suffix)
			: this(null, persister, suffix) {}

		/// <summary>
		/// Calculate and cache select-clause aliases.
		/// </summary>
		public DefaultEntityAliases(IDictionary<string, string[]> userProvidedAliases, ILoadable persister, string suffix)
		{
			_suffix = suffix;
			_userProvidedAliases = userProvidedAliases?.Count > 0 ? userProvidedAliases : null;

			SuffixedKeyAliases = DetermineKeyAliases(persister, suffix);
			SuffixedPropertyAliases = GetSuffixedPropertyAliases(persister);
			SuffixedDiscriminatorAlias = DetermineDiscriminatorAlias(persister, suffix);

			SuffixedVersionAliases = persister.IsVersioned ? SuffixedPropertyAliases[persister.VersionProperty] : null;
			//rowIdAlias is generated on demand in property
		}

		private string[] DetermineKeyAliases(ILoadable persister, string suffix)
		{
			if (_userProvidedAliases == null)
				return GetIdentifierAliases(persister, suffix);

			return GetUserProvidedAliases(persister.IdentifierPropertyName)
					?? GetUserProvidedAliases(EntityPersister.EntityID)
					?? GetIdentifierAliases(persister, suffix);
		}

		private string DetermineDiscriminatorAlias(ILoadable persister, string suffix)
		{
			if (_userProvidedAliases == null)
				return GetDiscriminatorAlias(persister, suffix);

			return GetUserProvidedAlias(AbstractEntityPersister.EntityClass)
					?? GetDiscriminatorAlias(persister, suffix);
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
			return persister.GetPropertyAliases(_suffix, j);
		}

		private string[] GetUserProvidedAliases(string propertyPath)
		{
			if (propertyPath == null)
				return null;

			string[] result;
			_userProvidedAliases.TryGetValue(propertyPath, out result);
			return result;
		}

		private string GetUserProvidedAlias(string propertyPath)
		{
			return GetUserProvidedAliases(propertyPath)?[0];

		}

		/// <summary>
		/// Returns aliases for subclass persister
		/// </summary>
		public string[][] GetSuffixedPropertyAliases(ILoadable persister)
		{
			if (_userProvidedAliases == null)
				return GetPropertiesAliases(persister);

			int size = persister.PropertyNames.Length;
			string[][] suffixedPropertyAliases = new string[size][];
			for (int j = 0; j < size; j++)
			{
				suffixedPropertyAliases[j] = GetUserProvidedAliases(persister.PropertyNames[j]) ?? GetPropertyAliases(persister, j);
			}
			return suffixedPropertyAliases;
		}

		public string[] SuffixedVersionAliases { get; }

		public string[][] SuffixedPropertyAliases { get; }

		public string SuffixedDiscriminatorAlias { get; }

		public string[] SuffixedKeyAliases { get; }

		// TODO: not visible to the user!
		public string RowIdAlias => _rowIdAlias ?? (_rowIdAlias = Loadable.RowIdAlias + _suffix);
	}
}
