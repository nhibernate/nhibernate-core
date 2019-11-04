using System;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	[Serializable]
	public abstract class SubqueryExpression : AbstractCriterion
	{
		private readonly CriteriaImpl criteriaImpl;
		private readonly String quantifier;
		private readonly bool prefixOp;
		private readonly String op;
		private QueryParameters parameters;
		private IType[] types;

		protected SubqueryExpression(String op, String quantifier, DetachedCriteria dc)
			: this(op, quantifier, dc, true)
		{
		}

		protected SubqueryExpression(String op, String quantifier, DetachedCriteria dc, bool prefixOp)
		{
			criteriaImpl = dc.GetCriteriaImpl();
			this.quantifier = quantifier;
			this.prefixOp = prefixOp;
			this.op = op;
		}

		public IType[] GetTypes()
		{
			return types;
		}

		protected abstract SqlString ToLeftSqlString(ICriteria criteria, ICriteriaQuery outerQuery);

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			ISessionFactoryImplementor factory = criteriaQuery.Factory;

			var innerQuery = new CriteriaQueryTranslator(
				factory,
				criteriaImpl, //implicit polymorphism not supported (would need a union)
				criteriaImpl.EntityOrClassName,
				criteriaQuery.GenerateSQLAlias(),
				criteriaQuery);

			types = innerQuery.HasProjection ? innerQuery.ProjectedTypes : null;

			if (innerQuery.HasProjection == false)
			{
				throw new QueryException("Cannot use subqueries on a criteria without a projection.");
			}

			IOuterJoinLoadable persister = (IOuterJoinLoadable) factory.GetEntityPersister(criteriaImpl.EntityOrClassName);

			criteriaImpl.Session = DeriveRootSession(criteria);

			var walker = new CriteriaJoinWalker(persister, innerQuery, factory, criteriaImpl, criteriaImpl.EntityOrClassName, criteriaImpl.Session.EnabledFilters);

			parameters = innerQuery.GetQueryParameters(); // parameters can be inferred only after initialize the walker

			SqlString sql = walker.SqlString;

			if (criteriaImpl.FirstResult != 0 || criteriaImpl.MaxResults != RowSelection.NoValue)
			{
				int? offset = Loader.Loader.GetOffsetUsingDialect(parameters.RowSelection, factory.Dialect);
				int? limit = Loader.Loader.GetLimitUsingDialect(parameters.RowSelection, factory.Dialect);
				Parameter offsetParameter = offset.HasValue ? innerQuery.CreateSkipParameter(offset.Value) : null;
				Parameter limitParameter = limit.HasValue ? innerQuery.CreateTakeParameter(limit.Value) : null;
				sql = factory.Dialect.GetLimitString(sql, offset, limit, offsetParameter, limitParameter);
			}

			SqlStringBuilder buf = new SqlStringBuilder().Add(ToLeftSqlString(criteria, criteriaQuery));
			if (op != null)
			{
				buf.Add(" ").Add(op).Add(" ");
			}

			if (quantifier != null && prefixOp)
			{
				buf.Add(quantifier).Add(" ");
			}

			buf.Add("(").Add(sql).Add(")");

			if (quantifier != null && prefixOp == false)
			{
				buf.Add(" ").Add(quantifier);
			}

			return buf.ToSqlString();
		}

		public override string ToString()
		{
			if (prefixOp)
				return string.Format("{0} {1} ({2})", op, quantifier, criteriaImpl);

			return string.Format("{0} ({1}) {2}", op, criteriaImpl, quantifier);
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return parameters.NamedParameters.Values.ToArray();
		}

		public override IProjection[] GetProjections()
		{
			return null;
		}

		// NH-1146
		public ICriteria Criteria => criteriaImpl;

		static ISessionImplementor DeriveRootSession(ICriteria criteria)
		{
			while (criteria is CriteriaImpl.Subcriteria subcriteria)
			{
				criteria = subcriteria.Parent;
			}
			if (criteria is CriteriaImpl criteriaImpl)
			{
				return criteriaImpl.Session;
			}
			// could happen for custom Criteria impls.  Not likely, but...
			// for long term solution, see HHH-3514
			return null;
		}
	}
}
