using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
    public class NameUnNamedParameters : NhExpressionTreeVisitor
    {
        public static Expression Visit(Expression expression)
        {
            var visitor = new NameUnNamedParameters();

            return visitor.VisitExpression(expression);
        }

        private readonly Dictionary<ParameterExpression, ParameterExpression> _renamedParameters = new Dictionary<ParameterExpression, ParameterExpression>();

        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            if (string.IsNullOrEmpty(expression.Name))
            {
                ParameterExpression renamed;
                
                if (_renamedParameters.TryGetValue(expression, out renamed))
                {
                    return renamed;
                }

                renamed = Expression.Parameter(expression.Type, Guid.NewGuid().ToString());

                _renamedParameters.Add(expression, renamed);

                return renamed;
            }

            return base.VisitParameterExpression(expression);
        }
    }
}