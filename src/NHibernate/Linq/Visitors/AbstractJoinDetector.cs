using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
    public abstract class AbstractJoinDetector : NhExpressionTreeVisitor
    {
        private readonly NameGenerator _nameGenerator;
        internal readonly IIsEntityDecider _isEntityDecider;
        protected readonly Dictionary<string, NhJoinClause> _joins;
        protected readonly Dictionary<MemberExpression, QuerySourceReferenceExpression> _expressionMap;

        internal AbstractJoinDetector(NameGenerator nameGenerator, IIsEntityDecider isEntityDecider, Dictionary<string, NhJoinClause> joins, Dictionary<MemberExpression, QuerySourceReferenceExpression> expressionMap)
        {
            _nameGenerator = nameGenerator;
            _expressionMap = expressionMap;
            _joins = joins;
            _isEntityDecider = isEntityDecider;
        }

        protected internal Expression AddJoin(MemberExpression expression)
        {
            string key = ExpressionKeyVisitor.Visit(expression, null);
            NhJoinClause join;

            if (!_joins.TryGetValue(key, out join))
            {
                join = new NhJoinClause(_nameGenerator.GetNewName(), expression.Type, expression);
                _joins.Add(key, join);
            }

            QuerySourceReferenceExpression newExpr = new QuerySourceReferenceExpression(join);

            if (!_expressionMap.ContainsKey(expression))
                _expressionMap.Add(expression, newExpr);

            return newExpr;
        }

        protected void MakeInnerIfJoined(string memberExpressionKey)
        {
            // memberExpressionKey is not joined if it occurs only at tails of expressions, e.g.
            // a.B == null, a.B != null, a.B == c.D etc.
            if (_joins.ContainsKey(memberExpressionKey))
            {
                _joins[memberExpressionKey].MakeInner();
            }
        }
    }
}
