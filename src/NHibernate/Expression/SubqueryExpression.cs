using System;
using System.Collections.Generic;
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

        [NonSerialized]
        private CriteriaQueryTranslator innerQuery;

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

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
        {
            InitializeInnerQueryAndParameters(criteriaQuery);

            if (innerQuery.HasProjection == false)
                throw new QueryException("Cannot use subqueries on a criteria without a projection.");

            ISessionFactoryImplementor factory = criteriaQuery.Factory;

            IOuterJoinLoadable persister = (IOuterJoinLoadable)factory.GetEntityPersister(this.criteriaImpl.CriteriaClass);

            //patch to generate joins on subqueries
            //stolen from CriteriaLoader
            CriteriaJoinWalker walker = new CriteriaJoinWalker(
                persister,
                innerQuery,
                factory,
                criteriaImpl,
                criteriaImpl.CriteriaClass,
                enabledFilters);

            SqlString sql = walker.SqlString;

            SqlStringBuilder buf = new SqlStringBuilder().Add(ToLeftSqlString(criteria, criteriaQuery));
            if (op != null)
            {
                buf.Add(" ").Add(op).Add(" ");
            }

            if (quantifier != null)
            {
                buf.Add(quantifier).Add(" ");
            }
            return buf.Add("(").Add(sql).Add(")").ToSqlString();
        }

        public ICriteria Criteria
        {
            get { return criteriaImpl; }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", op, quantifier, criteriaImpl);
        }

        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
        {
            InitializeInnerQueryAndParameters(criteriaQuery);
            IType[] paramTypes = parameters.PositionalParameterTypes;
            Object[] values = parameters.PositionalParameterValues;
            TypedValue[] tv = new TypedValue[paramTypes.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                tv[i] = new TypedValue(paramTypes[i], values[i]);
            }
            return tv;
        }

        // NH: This feels like a hack but I don't understand the code enough yet to code a better solution
        private void InitializeInnerQueryAndParameters(ICriteriaQuery criteriaQuery)
        {
            if (innerQuery != null)
            {
                // Already initialized
                return;
            }

            ISessionFactoryImplementor factory = criteriaQuery.Factory;
            innerQuery = new CriteriaQueryTranslator(factory,
                                                     this.criteriaImpl,
                //implicit polymorphism not supported (would need a union) 
                                                     this.criteriaImpl.CriteriaClass,
                                                     criteriaQuery.GenerateSQLAlias(),
                                                     criteriaQuery);

            parameters = innerQuery.GetQueryParameters();
            types = innerQuery.ProjectedTypes;
        }
    }
}
