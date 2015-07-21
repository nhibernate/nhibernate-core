using System.Collections.Generic;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// <see cref="IEntityAliases" /> that chooses the column names over the alias names.
	/// </summary>
	public class ColumnEntityAliases : DefaultEntityAliases
	{
		public ColumnEntityAliases(IDictionary<string, string[]> returnProperties, ILoadable persister, string suffix)
			: base(returnProperties, persister, suffix) {}

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