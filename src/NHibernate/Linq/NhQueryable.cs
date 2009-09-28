using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Impl;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing.Structure;

namespace NHibernate.Linq
{
    /// <summary>
    /// Provides the main entry point to a LINQ query.
    /// </summary>
    public class NhQueryable<T> : QueryableBase<T>
    {
        // This constructor is called by our users, create a new IQueryExecutor.
        public NhQueryable(ISession session)
            : base(new NhQueryProvider(session))
        {
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        public NhQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }

    public class NhQueryProvider : IQueryProvider
    {
        private readonly ISession _session;

        public NhQueryProvider(ISession session)
        {
            _session = session;
        }

        public object Execute(Expression expression)
        {
            var nhLinqExpression = new NhLinqExpression(expression);

            var query = _session.CreateQuery(nhLinqExpression).List();

            if (nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
            {
                return query.AsQueryable();
            }

            return query[0];
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult) Execute(expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new NhQueryable<T>(this, expression);
        }
    }

    public enum NhLinqExpressionReturnType
    {
        Sequence,
        Scalar
    }

    public class NhLinqExpression : IQueryExpression
    {
        private readonly Expression _expression;
        private CommandData _commandData;

        public NhLinqExpression(Expression expression)
        {
            _expression = expression;

            Key = expression.ToString();

            Type = expression.Type;

            // Note - re-linq handles return types via the GetOutputDataInfo method, and allows for SingleOrDefault here for the ChoiceResultOperator...
            ReturnType = NhLinqExpressionReturnType.Scalar;

            if (typeof(IQueryable).IsAssignableFrom(Type))
            {
                Type = Type.GetGenericArguments()[0];
                ReturnType = NhLinqExpressionReturnType.Sequence;
            }
        }

        public IASTNode Translate(ISessionFactory sessionFactory)
        {
            var queryModel = new QueryParser(new ExpressionTreeParser(MethodCallExpressionNodeTypeRegistry.CreateDefault())).GetParsedQuery(_expression);

            _commandData = QueryModelVisitor.GenerateHqlQuery(queryModel);

            return _commandData.Statement.AstNode;
        }

        public string Key { get; private set; }

        public NhLinqExpressionReturnType ReturnType { get; private set; }

        public System.Type Type { get; private set; }

        public void SetQueryParametersPriorToExecute(QueryImpl impl)
        {
            _commandData.SetParameters(impl);
            _commandData.SetResultTransformer(impl);
            _commandData.AddAdditionalCriteria(impl);
        }
    }
}