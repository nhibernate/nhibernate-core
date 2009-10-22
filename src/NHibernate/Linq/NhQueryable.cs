using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Impl;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
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

        	var query = _session.CreateQuery(nhLinqExpression);

			SetParameters(query, nhLinqExpression.ParameterValuesByName);

        	var results = query.List();

            if (nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
            {
                return results.AsQueryable();
            }

            return results[0];
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

		void SetParameters(IQuery query, IDictionary<string, object> parameters)
		{
			foreach (var parameterName in query.NamedParameters)
			{
				query.SetParameter(parameterName, parameters[parameterName]);
			}
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
    	private readonly IDictionary<ConstantExpression, NamedParameter> _queryParameters;

    	public NhLinqExpression(Expression expression)
        {
            _expression = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees(expression);

			_queryParameters = ExpressionParameterVisitor.Visit(_expression);
			ParameterValuesByName = _queryParameters.Values.ToDictionary(p => p.Name, p => p.Value);

			Key = ExpressionKeyVisitor.Visit(_expression, _queryParameters);
            Type = _expression.Type;

			ParameterValues = _queryParameters.Values;

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
			var requiredHqlParameters = new List<NamedParameterDescriptor>();
            var queryModel = new QueryParser(new ExpressionTreeParser(MethodCallExpressionNodeTypeRegistry.CreateDefault())).GetParsedQuery(_expression);

			_commandData = QueryModelVisitor.GenerateHqlQuery(queryModel, _queryParameters, requiredHqlParameters);

    		ParameterDescriptors = requiredHqlParameters.AsReadOnly();

            return _commandData.Statement.AstNode;
        }

        public string Key { get; private set; }

		public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }

		public ICollection<NamedParameter> ParameterValues { get; private set; }

        public NhLinqExpressionReturnType ReturnType { get; private set; }

        public System.Type Type { get; private set; }

    	public IDictionary<string, object> ParameterValuesByName { get; private set; }

    	public void SetQueryPropertiesPriorToExecute(IQuery impl)
        {
            _commandData.SetResultTransformer(impl);
            _commandData.AddAdditionalCriteria(impl);
        }
    }

	public class ExpressionParameterVisitor : ExpressionTreeVisitor
	{
		private readonly Dictionary<ConstantExpression, NamedParameter> _parameters = new Dictionary<ConstantExpression, NamedParameter>();

		public static IDictionary<ConstantExpression, NamedParameter> Visit(Expression expression)
		{
			var visitor = new ExpressionParameterVisitor();
			
			visitor.VisitExpression(expression);

			return visitor._parameters;
		}

		protected override Expression VisitConstantExpression(ConstantExpression expression)
		{
			if (!typeof(IQueryable).IsAssignableFrom(expression.Type))
			{
				_parameters.Add(expression, new NamedParameter("p" + (_parameters.Count + 1), expression.Value));
			}

			return base.VisitConstantExpression(expression);
		}
	}
}