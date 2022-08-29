using System.Collections.Generic;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader
{
	/// <summary>
	/// EntityAliases which handles the logic of selecting user provided aliases (via return-property),
	/// before using the default aliases.
	/// </summary>
	public class DefaultEntityAliases : IEntityAliases
	{
		private readonly string _suffix;
		private readonly IDictionary<string, string[]> _userProvidedAliases;
		private string _rowIdAlias;

		public DefaultEntityAliases(ILoadable persister, string suffix)
			: this(null, persister, suffix)
		{
		}

		/// <summary>
		/// Calculate and cache select-clause aliases.
		/// </summary>
		public DefaultEntityAliases(IDictionary<string, string[]> userProvidedAliases, ILoadable persister, string suffix)
		{
			_suffix = suffix;
			_userProvidedAliases = userProvidedAliases?.Count > 0 ? userProvidedAliases : null;

			SuffixedKeyAliases = DetermineKeyAliases(persister);
			SuffixedPropertyAliases = DeterminePropertyAliases(persister);
			SuffixedDiscriminatorAlias = DetermineDiscriminatorAlias(persister);

			SuffixedVersionAliases = persister.IsVersioned ? SuffixedPropertyAliases[persister.VersionProperty] : null;
			//rowIdAlias is generated on demand in property
		}

		/// <summary>
		/// Returns aliases for subclass persister
		/// </summary>
		public string[][] GetSuffixedPropertyAliases(ILoadable persister)
		{
			if (_userProvidedAliases == null)
				return GetAllPropertyAliases(persister);

			var propertyNames = persister.PropertyNames;
			var suffixedPropertyAliases = new string[propertyNames.Length][];
			for (var i = 0; i < propertyNames.Length; i++)
			{
				suffixedPropertyAliases[i] =
					SafeGetUserProvidedAliases(propertyNames[i]) ??
					GetPropertyAliases(persister, i);
			}

			return suffixedPropertyAliases;
		}

		public string[] SuffixedVersionAliases { get; }

		public string[][] SuffixedPropertyAliases { get; }

		public string SuffixedDiscriminatorAlias { get; }

		public string[] SuffixedKeyAliases { get; }

		// TODO: not visible to the user!
		public string RowIdAlias => _rowIdAlias ?? (_rowIdAlias = Loadable.RowIdAlias + _suffix);

		/// <summary>
		/// Returns default aliases for all the properties
		/// </summary>
		private string[][] GetAllPropertyAliases(ILoadable persister)
		{
			var propertyNames = persister.PropertyNames;
			var suffixedPropertyAliases = new string[propertyNames.Length][];
			for (var i = 0; i < propertyNames.Length; i++)
			{
				suffixedPropertyAliases[i] = GetPropertyAliases(persister, i);
			}

			return suffixedPropertyAliases;
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

		private string[] DetermineKeyAliases(ILoadable persister)
		{
			if (_userProvidedAliases != null)
			{
				var result = SafeGetUserProvidedAliases(persister.IdentifierPropertyName) ??
							 GetUserProvidedAliases(EntityPersister.EntityID);

				if (result != null)
					return result;
			}

			return GetIdentifierAliases(persister, _suffix);
		}

		private string[][] DeterminePropertyAliases(ILoadable persister)
		{
			return GetSuffixedPropertyAliases(persister);
		}

		private string DetermineDiscriminatorAlias(ILoadable persister)
		{
			if (_userProvidedAliases != null)
			{
				var columns = GetUserProvidedAliases(AbstractEntityPersister.EntityClass);
				if (columns != null)
					return columns[0];
			}

			return GetDiscriminatorAlias(persister, _suffix);
		}

		private string[] SafeGetUserProvidedAliases(string propertyPath)
		{
			if (propertyPath == null)
				return null;

			return GetUserProvidedAliases(propertyPath);
		}

		private string[] GetUserProvidedAliases(string propertyPath)
		{
			_userProvidedAliases.TryGetValue(propertyPath, out var result);
			return result;
		}
	}
}
