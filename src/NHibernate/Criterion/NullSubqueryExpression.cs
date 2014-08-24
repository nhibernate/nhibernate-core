using System;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
    [Serializable]
    public class NullSubqueryExpression : SubqueryExpression
    {
        protected override SqlString ToLeftSqlString(ICriteria criteria, ICriteriaQuery outerQuery)
        {
            return SqlString.Empty;
        }

        internal NullSubqueryExpression(String quantifier, DetachedCriteria dc)
            : base(null, quantifier, dc, false)
        {
        }
    }
}