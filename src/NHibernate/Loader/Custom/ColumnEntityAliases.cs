using System;
using System.Collections;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// <see cref="IEntityAliases" /> that chooses the column names over the alias names.
	/// </summary>
	public class ColumnEntityAliases : DefaultEntityAliases
	{
		public ColumnEntityAliases(IDictionary returnProperties, ILoadable persister, string suffix)
			: base(returnProperties, persister, suffix)
		{
		}

		protected override string[] GetIdentifierAliases(ILoadable persister, string suffix)
		{
			return persister.IdentifierColumnNames;
		}

		protected override string GetDiscriminatorAlias(ILoadable persister, string suffix)
		{
			return persister.DiscriminatorColumnName;
		}

		protected override string[] GetPropertyAliases(ILoadable persister, int j)
		{
			return persister.GetPropertyColumnNames(j);
		}
	}
}