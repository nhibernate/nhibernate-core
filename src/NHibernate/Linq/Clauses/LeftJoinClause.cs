using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;

namespace NHibernate.Linq.Visitors
{
    public class LeftJoinClause : AdditionalFromClause
    {
        public LeftJoinClause(string itemName, System.Type itemType, Expression fromExpression) : base(itemName, itemType, fromExpression)
        {
        }
    }
}