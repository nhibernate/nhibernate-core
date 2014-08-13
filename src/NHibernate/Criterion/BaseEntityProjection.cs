using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	[Serializable]
	public abstract class BaseEntityProjection : IProjection
	{
		private String alias = null;
		private String [] columnAliases = null;
		private Boolean lazy = true;

		protected BaseEntityProjection(System.Type rootEntity, String alias)
		{
			this.RootEntity = rootEntity;
			this.alias = alias;
		}

		protected System.Type RootEntity
		{
			get;
			private set;
		}

		public BaseEntityProjection SetLazy(Boolean lazy)
		{
			this.lazy = lazy;

			return (this);
		}

		String[] IProjection.Aliases
		{
			get
			{
				return (this.columnAliases.ToArray());
			}
		}

		TypedValue[] IProjection.GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new NotImplementedException();
		}

		IType[] IProjection.GetTypes(String alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new NotImplementedException();
		}

		IType[] IProjection.GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (this.RootEntity == null)
			{
				this.RootEntity = criteria.GetRootEntityTypeIfAvailable();
			}

			if (this.alias == null)
			{
				this.alias = criteria.Alias;
			}

			return (new IType[] { new ManyToOneType(this.RootEntity.FullName, this.lazy) });
		}

		Boolean IProjection.IsAggregate
		{
			get { return (false); }
		}

		Boolean IProjection.IsGrouped
		{
			get { return (false); }
		}

		SqlString IProjection.ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<String, IFilter> enabledFilters)
		{
			throw new NotImplementedException();
		}

		SqlString IProjection.ToSqlString(ICriteria criteria, Int32 position, ICriteriaQuery criteriaQuery, IDictionary<String, IFilter> enabledFilters)
		{
			var builder = new SqlStringBuilder();
			var persister = criteriaQuery.Factory.TryGetEntityPersister(this.RootEntity.FullName) as AbstractEntityPersister;
			var subcriteria = criteria.GetCriteriaByAlias(this.alias);

			this.columnAliases = persister.GetIdentifierAliases(String.Empty);

			var columnNames = persister.GetPropertyColumnNames(persister.IdentifierPropertyName).Select(x => String.Concat(criteriaQuery.GetSQLAlias(subcriteria, persister.IdentifierPropertyName), ".", criteriaQuery.Factory.Dialect.QuoteForColumnName(x))).ToArray();

			for (var i = 0; i < columnNames.Length; ++i)
			{
				builder.Add(String.Format("{0} as {1}", columnNames[i], this.columnAliases[i]));

				if (i < columnNames.Length - 1)
				{
					builder.Add(", ");
				}
			}

			return (builder.ToSqlString());
		}

		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return ((this as IProjection).Aliases);
		}

		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return ((this as IProjection).Aliases);
		}
	}
}
