using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
    public class SelectClauseHqlNominator : NhExpressionTreeVisitor
    {
        private ILinqToHqlGeneratorsRegistry _functionRegistry;

        private HashSet<Expression> _candidates;
        private bool _canBeCandidate;
        Stack<bool> _stateStack;
        public SelectClauseHqlNominator(VisitorParameters parameters)
        {
            _functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
        }


        public override Expression VisitExpression(Expression expression)
        {
            try
            {
                bool projectConstantsInHql = _stateStack.Peek();
                if (!projectConstantsInHql && expression != null && IsRegisteredFunction(expression))
                {
                    projectConstantsInHql = true;
                }
                _stateStack.Push(projectConstantsInHql);

                if (expression == null)
                    return expression;


                bool saveCanBeCandidate = _canBeCandidate;
                _canBeCandidate = true;

                if (CanBeEvaluatedInHqlStatementShortcut(expression))
                {
                    _candidates.Add(expression);
                    return expression;
                }

                base.VisitExpression(expression);

                if (_canBeCandidate)
                {
                    if (CanBeEvaluatedInHqlSelectStatement(expression, projectConstantsInHql))
                    {
                        _candidates.Add(expression);
                    }
                    else
                    {
                        _canBeCandidate = false;
                    }
                }

                _canBeCandidate = _canBeCandidate & saveCanBeCandidate;

                return expression;
            }
            finally
            {
                _stateStack.Pop();
            }
        }

        private bool IsRegisteredFunction(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                IHqlGeneratorForMethod methodGenerator;
                if (_functionRegistry.TryGetGenerator(((MethodCallExpression)expression).Method, out methodGenerator))
                {
                    return true;
                }
            }
            return false;
        }




        private bool CanBeEvaluatedInHqlSelectStatement(Expression expression, bool projectConstantsInHql)
        {

            // Hql can't do New or Member Init
            if ((expression.NodeType == ExpressionType.MemberInit) || (expression.NodeType == ExpressionType.New))
            {
                return false;
            }

            //Constants will only be evaluated in Hql if they're inside a method call
            if (expression.NodeType == ExpressionType.Constant)
            {
                return projectConstantsInHql;
            }



            if (expression.NodeType == ExpressionType.Call)
            {
                // Depends if it's in the function registry
                if (!IsRegisteredFunction(expression))
                    return false;
            }

            // Assume all is good
            return true;
        }



        private static bool CanBeEvaluatedInHqlStatementShortcut(Expression expression)
        {
            return ((NhExpressionType)expression.NodeType) == NhExpressionType.Count;
        }

        internal HashSet<Expression> Nominate(Expression expression)
        {
            _candidates = new HashSet<Expression>();
            _canBeCandidate = true;
            _stateStack = new Stack<bool>();
            _stateStack.Push(false);
            VisitExpression(expression);
            return _candidates;
        }

    }
}
