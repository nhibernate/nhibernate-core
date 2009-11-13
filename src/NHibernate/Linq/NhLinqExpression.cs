using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq
{
	public class NhLinqExpression : IQueryExpression
	{
	    private static readonly MethodCallExpressionNodeTypeRegistry MethodCallRegistry =
	        MethodCallExpressionNodeTypeRegistry.CreateDefault();

        static NhLinqExpression()
        {
            MethodCallRegistry.Register(
                new[]
                    {
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(ReflectionHelper.GetMethod(() => Queryable.Aggregate<object>(null, null))),
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(ReflectionHelper.GetMethod(() => Queryable.Aggregate<object, object>(null, null, null)))
                    },
                typeof (AggregateExpressionNode));
        }


		private readonly Expression _expression;
		private CommandData _commandData;
		private readonly IDictionary<ConstantExpression, NamedParameter> _queryParameters;

		public NhLinqExpression(Expression expression)
		{
			_expression = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees(expression);

		    _expression = NameUnNamedParameters.Visit(_expression);

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

            // TODO - can we cache any of this? 
			var queryModel = new QueryParser(new ExpressionTreeParser(MethodCallRegistry)).GetParsedQuery(_expression);

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

    public class AggregateExpressionNode : ResultOperatorExpressionNodeBase
    {
        public MethodCallExpressionParseInfo ParseInfo { get; set; }
        public Expression OptionalSeed { get; set; }
        public LambdaExpression Accumulator { get; set; }
        public LambdaExpression OptionalSelector { get; set; }

        public AggregateExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression arg1, Expression arg2, LambdaExpression optionalSelector) : base(parseInfo, null, optionalSelector)
        {
            ParseInfo = parseInfo;

            if (arg2 != null)
            {
                OptionalSeed = arg1;
                Accumulator = (LambdaExpression) arg2;
            }
            else
            {
                Accumulator = (LambdaExpression) arg1;
            }

            OptionalSelector = optionalSelector;
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw new NotImplementedException();
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new AggregateResultOperator(ParseInfo, OptionalSeed, Accumulator, OptionalSelector);
        }
    }

    public class AggregateResultOperator : ClientSideTransformOperator
    {
        public MethodCallExpressionParseInfo ParseInfo { get; set; }
        public Expression OptionalSeed { get; set; }
        public LambdaExpression Accumulator { get; set; }
        public LambdaExpression OptionalSelector { get; set; }

        public AggregateResultOperator(MethodCallExpressionParseInfo parseInfo, Expression optionalSeed, LambdaExpression accumulator, LambdaExpression optionalSelector)
        {
            ParseInfo = parseInfo;
            OptionalSeed = optionalSeed;
            Accumulator = accumulator;
            OptionalSelector = optionalSelector;
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            throw new NotImplementedException();
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            throw new NotImplementedException();
        }
    }
}