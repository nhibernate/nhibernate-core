using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
    public class LeftJoinDetector : NhExpressionTreeVisitor
    {
        private readonly NameGenerator _nameGenerator;
        private readonly ISessionFactory _sessionFactory;
        private readonly Dictionary<string, LeftJoinClause> _joins = new Dictionary<string, LeftJoinClause>();
        private readonly Dictionary<Expression, Expression> _expressionMap = new Dictionary<Expression, Expression>();

        private LeftJoinDetector(NameGenerator nameGenerator, ISessionFactory sessionFactory)
        {
            _nameGenerator = nameGenerator;
            _sessionFactory = sessionFactory;
        }

        public static Results Detect(Expression selector, NameGenerator nameGenerator, ISessionFactory sessionFactory)
        {
            var detector = new LeftJoinDetector(nameGenerator, sessionFactory);

            var newSelector = detector.VisitExpression(selector);

            return new Results(newSelector, detector._joins.Values, detector._expressionMap);
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            if (expression.Type.IsNonPrimitive() && IsEntity(expression.Type))
            {
                var newExpr = AddJoin(expression);
                _expressionMap.Add(expression, newExpr);
                return newExpr;
            }

            return base.VisitMemberExpression(expression);
        }

        private bool IsEntity(System.Type type)
        {
            return _sessionFactory.GetClassMetadata(type) != null;
        }

        private Expression AddJoin(MemberExpression expression)
        {
            string key = ExpressionKeyVisitor.Visit(expression, null);
            LeftJoinClause join;

            if (!_joins.TryGetValue(key, out join))
            {
                join = new LeftJoinClause(_nameGenerator.GetNewName(), expression.Type, expression);
                _joins.Add(key, join);
            }

            return new QuerySourceReferenceExpression(join);
        }

        public class Results
        {
            public Expression Selector { get; private set; }
            public ICollection<LeftJoinClause> Joins { get; private set; }
            public Dictionary<Expression, Expression> ExpressionMap { get; private set; }

            public Results(Expression selector, ICollection<LeftJoinClause> joins, Dictionary<Expression, Expression> expressionMap)
            {
                Selector = selector;
                Joins = joins;
                ExpressionMap = expressionMap;
            }
        }
    }
}