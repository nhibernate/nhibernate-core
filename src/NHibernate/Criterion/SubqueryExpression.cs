using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

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

		[NonSerialized] private CriteriaQueryTranslator innerQuery;

		protected SubqueryExpression(String op, String quantifier, DetachedCriteria dc)
			:this(op, quantifier, dc, true)
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

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			InitializeInnerQueryAndParameters(criteriaQuery);

			if (innerQuery.HasProjection == false)
			{
				throw new QueryException("Cannot use subqueries on a criteria without a projection.");
			}

			ISessionFactoryImplementor factory = criteriaQuery.Factory;

			IOuterJoinLoadable persister = (IOuterJoinLoadable)factory.GetEntityPersister(criteriaImpl.EntityOrClassName);

			//patch to generate joins on subqueries
			//stolen from CriteriaLoader
			CriteriaJoinWalker walker =
				new CriteriaJoinWalker(persister, innerQuery, factory, criteriaImpl, criteriaImpl.EntityOrClassName, enabledFilters);

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

			// during CriteriaImpl.Clone we are doing a shallow copy of each criterion.
			// this is not a problem for common criterion but not for SubqueryExpression because here we are holding the state of inner CriteriaTraslator (ICriteriaQuery).
			// After execution (ToSqlString) we have to clean the internal state because the next execution may be performed in a different tree reusing the same istance of SubqueryExpression.
			innerQuery = null;

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
			if(prefixOp)
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

		public void InitializeInnerQueryAndParameters(ICriteriaQuery criteriaQuery)
		{
			if (innerQuery == null)
			{
				ISessionFactoryImplementor factory = criteriaQuery.Factory;

				innerQuery = new CriteriaQueryTranslator(
					factory,
					criteriaImpl, //implicit polymorphism not supported (would need a union)
					criteriaImpl.EntityOrClassName,
					criteriaQuery.GenerateSQLAlias(),
					criteriaQuery);

				types = innerQuery.HasProjection ? innerQuery.ProjectedTypes : null;
			}
		}

		public ICriteria Criteria
		{
			// NH-1146
			get { return criteriaImpl; }
		}
	}
}