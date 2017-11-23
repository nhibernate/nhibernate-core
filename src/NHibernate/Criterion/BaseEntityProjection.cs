using System;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	[Serializable]
	public abstract class BaseEntityProjection : IProjection
	{
		private string alias = null;
		private string[] columnAliases = null;
		private bool lazy = true;

		protected BaseEntityProjection(System.Type rootEntity, string alias)
		{
			this.RootEntity = rootEntity;
			this.alias = alias;
		}

		protected System.Type RootEntity
		{
			get;
			private set;
		}

		public BaseEntityProjection SetLazy(bool lazy)
		{
			this.lazy = lazy;

			return this;
		}

		public string[] Aliases
		{
			get
			{
				return this.columnAliases;
			}
		}

		TypedValue[] IProjection.GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new NotImplementedException();
		}

		IType[] IProjection.GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { criteriaQuery.GetType(criteria, alias) };
		}

		private void SetFields(ICriteria criteria)
		{
			if (this.RootEntity == null)
			{
				this.RootEntity = criteria.GetRootEntityTypeIfAvailable();
			}

			if (this.alias == null)
			{
				this.alias = criteria.Alias;
			}
		}

		IType[] IProjection.GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			this.SetFields(criteria);

			return new IType[] { new ManyToOneType(this.RootEntity.FullName, this.lazy) };
		}

		bool IProjection.IsAggregate
		{
			get { return false; }
		}

		bool IProjection.IsGrouped
		{
			get { return false; }
		}

		SqlString IProjection.ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new NotImplementedException();
		}

		SqlString IProjection.ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			this.SetFields(criteria);

			SqlStringBuilder builder = new SqlStringBuilder();
			var persister = (ILoadable)criteriaQuery.Factory.GetEntityPersister(this.RootEntity.FullName);
			ICriteria subcriteria = criteria.GetCriteriaByAlias(this.alias);

			this.columnAliases = persister.GetIdentifierAliases(string.Empty);
			var columnNamesWithAliases = persister.IdentifierColumnNames
				.Select(
					(x, i) => string.Concat(
						criteriaQuery.GetSQLAlias(
							subcriteria, persister.IdentifierPropertyName ?? string.Empty),
						".",
						criteriaQuery.Factory.Dialect.QuoteForColumnName(x),
						" as ",
						columnAliases[i]
					));

			builder.Add(string.Join(", ", columnNamesWithAliases));
			return builder.ToSqlString();
		}

		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return Aliases;
		}

		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return Aliases;
		}
	}
}
