using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	public class BasicEntityPropertyMapping : AbstractPropertyMapping
	{
		private readonly AbstractEntityPersister persister;

		public BasicEntityPropertyMapping(AbstractEntityPersister persister)
		{
			this.persister = persister;
		}

		public override string[] IdentifierColumnNames
		{
			get { return persister.IdentifierColumnNames; }
		}

		protected override string EntityName
		{
			get { return persister.EntityName; }
		}

		public override IType Type
		{
			get { return persister.Type; }
		}

		public override string[] ToColumns(string alias, string propertyName, bool useLastIndex)
		{
			var tableAlias = persister.GenerateTableAlias(alias, persister.GetSubclassPropertyTableNumber(propertyName, useLastIndex));
			return base.ToColumns(tableAlias, propertyName, useLastIndex);
		}
	}
}
