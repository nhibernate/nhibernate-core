using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
    /// <summary>
    /// Performs bottom-up analysis to determine which nodes that match a certain predicate
    /// </summary>
    class Nominator : NhExpressionTreeVisitor
    {
        private readonly Func<Expression, bool> _fnIsCandidate;
        private readonly Func<Expression, bool> _fnIsCandidateShortcut;
        private HashSet<Expression> _candidates;
        private bool _canBeCandidate;

        internal Nominator(Func<Expression, bool> fnIsCandidate, Func<Expression, bool> fnIsCandidateShortcut)
        {
            _fnIsCandidate = fnIsCandidate;
            _fnIsCandidateShortcut = fnIsCandidateShortcut;
        }

        internal HashSet<Expression> Nominate(Expression expression)
        {
            _candidates = new HashSet<Expression>();
            _canBeCandidate = true;
            VisitExpression(expression);
            return _candidates;
        }

        protected override Expression VisitExpression(Expression expression)
        {
            if (expression != null)
            {
                bool saveCanBeCandidate = _canBeCandidate;
                _canBeCandidate = true;

                if (_fnIsCandidateShortcut(expression))
                {
                    _candidates.Add(expression);
                    return expression;
                }

                base.VisitExpression(expression);

                if (_canBeCandidate)
                {
                    if (_fnIsCandidate(expression))
                    {
                        _candidates.Add(expression);
                    }
                    else
                    {
                        _canBeCandidate = false;
                    }
                }

                _canBeCandidate = _canBeCandidate & saveCanBeCandidate;
            }

            return expression;
        }
    }
}