using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.Visitors;
using NHibernate.Type;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace NHibernate.Linq
{
	public class NhLinqExpression : IQueryExpression
	{
        public string Key { get; private set; }

        public System.Type Type { get; private set; }

        public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }

        public NhLinqExpressionReturnType ReturnType { get; private set; }

        public IDictionary<string, Tuple<object, IType>> ParameterValuesByName { get; private set; }

        public ExpressionToHqlTranslationResults ExpressionToHqlTranslationResults { get; private set; }

		private readonly Expression _expression;
	    private readonly IDictionary<ConstantExpression, NamedParameter> _constantToParameterMap;
	    private IASTNode _astNode;

	    public NhLinqExpression(Expression expression)
		{
			_expression = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees(expression);

		    _expression = NameUnNamedParameters.Visit(_expression);

			_constantToParameterMap = ExpressionParameterVisitor.Visit(_expression);

		    ParameterValuesByName = _constantToParameterMap.Values.ToDictionary(p => p.Name,
		                                                                        p =>
		                                                                        new Tuple<object, IType>
		                                                                            {First = p.Value, Second = p.Type});

			Key = ExpressionKeyVisitor.Visit(_expression, _constantToParameterMap);
			
            Type = _expression.Type;

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
            //if (_astNode == null)
            {
                var requiredHqlParameters = new List<NamedParameterDescriptor>();

                // TODO - can we cache any of this? 
                var queryModel = NhRelinqQueryParser.Parse(_expression);

                ExpressionToHqlTranslationResults = QueryModelVisitor.GenerateHqlQuery(queryModel,
                                                                                       _constantToParameterMap,
                                                                                       requiredHqlParameters);

                ParameterDescriptors = requiredHqlParameters.AsReadOnly();
                _astNode = ExpressionToHqlTranslationResults.Statement.AstNode;
            }

	        return _astNode;
		}
	}

    public static class NhRelinqQueryParser
    {
	    public static readonly MethodCallExpressionNodeTypeRegistry MethodCallRegistry =
	        MethodCallExpressionNodeTypeRegistry.CreateDefault();

        static NhRelinqQueryParser()
        {
            MethodCallRegistry.Register(
                new[]
                    {
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(ReflectionHelper.GetMethod(() => Queryable.Aggregate<object>(null, null))),
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(ReflectionHelper.GetMethod(() => Queryable.Aggregate<object, object>(null, null, null)))
                    },
                typeof (AggregateExpressionNode));

            MethodCallRegistry.Register(
                new []
                    {
                        MethodCallExpressionNodeTypeRegistry.GetRegisterableMethodDefinition(ReflectionHelper.GetMethod((List<object> l) => l.Contains(null))),
                        
                    },
                typeof(ContainsExpressionNode));
        }

        public static QueryModel Parse(Expression expression)
        {
            return new QueryParser(new ExpressionTreeParser(MethodCallRegistry)).GetParsedQuery(expression);
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
    }
}