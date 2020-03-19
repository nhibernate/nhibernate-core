using System;
using NHibernate.Criterion;
using NHibernate.Type;
using static NHibernate.Impl.CriteriaImpl;

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

		public override string[] ToColumns(ICriteria pathCriteria, string propertyName, Func<ICriteria, string> getSQLAlias)
		{
			var withClause = pathCriteria as Subcriteria != null ? ((Subcriteria) pathCriteria).WithClause as SimpleExpression : null;
			if (withClause != null && withClause.PropertyName == propertyName)
			{
				return base.ToColumns(persister.GenerateTableAlias(getSQLAlias(pathCriteria), 0), propertyName);
			}

			return base.ToColumns(pathCriteria, propertyName, getSQLAlias);
		}

		public override string[] ToColumns(string alias, string propertyName)
		{
			return
				base.ToColumns(persister.GenerateTableAlias(alias, persister.GetSubclassPropertyTableNumber(propertyName)),
				               propertyName);
		}
	}
}
