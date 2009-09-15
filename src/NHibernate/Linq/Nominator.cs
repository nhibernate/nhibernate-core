using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
    /// <summary>
    /// Performs bottom-up analysis to determine which nodes that match a certain predicate
    /// </summary>
    class Nominator : NhExpressionTreeVisitor
    {
        readonly Func<Expression, bool> _fnIsCandidate;
        HashSet<Expression> _candidates;
        bool _cannotBeCandidate;

        internal Nominator(Func<Expression, bool> fnIsCandidate)
        {
            _fnIsCandidate = fnIsCandidate;
        }

        internal HashSet<Expression> Nominate(Expression expression)
        {
            _candidates = new HashSet<Expression>();
            VisitExpression(expression);
            return _candidates;
        }

        protected override Expression VisitExpression(Expression expression)
        {
            if (expression != null)
            {
                bool saveCannotBeEvaluated = _cannotBeCandidate;
                _cannotBeCandidate = false;

                base.VisitExpression(expression);

                if (!_cannotBeCandidate)
                {
                    if (_fnIsCandidate(expression))
                    {
                        _candidates.Add(expression);
                    }
                    else
                    {
                        _cannotBeCandidate = true;
                    }
                }

                _cannotBeCandidate |= saveCannotBeEvaluated;
            }

            return expression;
        }
    }
}