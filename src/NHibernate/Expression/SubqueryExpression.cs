using System;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	[Serializable]
	public abstract class SubqueryExpression : AbstractCriterion
	{
		private CriteriaImpl criteriaImpl;
		private String quantifier;
		private String op;
		private QueryParameters parameters;
		private IType[] types;

		protected IType[] GetTypes()
		{
			return types;
		}

		protected SubqueryExpression(String op, String quantifier, DetachedCriteria dc)
		{
			criteriaImpl = dc.GetCriteriaImpl();
			this.quantifier = quantifier;
			this.op = op;
		}

		protected abstract SqlString ToLeftSqlString(ICriteria criteria, ICriteriaQuery outerQuery);

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			ISessionImplementor session = ((CriteriaImpl) criteria).Session; //ugly!
			ISessionFactoryImplementor factory = session.Factory;

			IOuterJoinLoadable persister = (IOuterJoinLoadable) factory.GetEntityPersister(criteriaImpl.CriteriaClass);
			CriteriaQueryTranslator innerQuery =
				new CriteriaQueryTranslator(factory,
				                            criteriaImpl,
				                            criteriaImpl.CriteriaClass,
				                            //implicit polymorphism not supported (would need a union) 
				                            criteriaQuery.GenerateSQLAlias(),
				                            criteriaQuery);

			parameters = innerQuery.GetQueryParameters(); //TODO: bad lifecycle....

			if (innerQuery.HasProjection == false)
				throw new QueryException("Cannot use sub queries on a criteria without a projection.");
			types = innerQuery.ProjectedTypes;

			SqlString sql = new SqlSelectBuilder(factory)
				.SetOuterJoins(SqlString.Empty, SqlString.Empty) // NH Specific: throws null ref otherwise.
				.SetWhereClause(innerQuery.GetWhereCondition())
				.SetGroupByClause(innerQuery.GetGroupBy().ToString())
				.SetSelectClause(innerQuery.GetSelect().ToString())
				.SetFromClause(persister.FromTableFragment(innerQuery.RootSQLAlias) +
					persister.FromJoinFragment(innerQuery.RootSQLAlias, true, false))
				.ToSqlString();

			SqlStringBuilder buf = new SqlStringBuilder().Add(ToLeftSqlString(criteria, criteriaQuery));
			if (op != null)
				buf.Add(" ").Add(op).Add(" ");
			if (quantifier != null)
				buf.Add(quantifier).Add(" ");
			return buf.Add("(").Add(sql).Add(")").ToSqlString();
		}

		public override string ToString()
		{
			return string.Format("{0} {1} ({2})", op, quantifier, criteriaImpl.ToString());
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IType[] paramTypes = parameters.PositionalParameterTypes;
			Object[] values = parameters.PositionalParameterValues;
			TypedValue[] tv = new TypedValue[paramTypes.Length];
			for (int i = 0; i < paramTypes.Length; i++)
			{
				tv[i] = new TypedValue(paramTypes[i], values[i]);
			}
			return tv;
		}
	}
}
