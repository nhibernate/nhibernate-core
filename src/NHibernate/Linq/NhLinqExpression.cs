using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.Structure;

namespace NHibernate.Linq
{
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
}