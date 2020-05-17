using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Linq.Visitors;
using NHibernate.Param;
using NHibernate.Type;

namespace NHibernate.Linq
{
	public class NhLinqExpression : IQueryExpression, ICacheableQueryExpression
	{
		public string Key { get; protected set; }

		public bool CanCachePlan { get; private set; } = true;

		public System.Type Type { get; private set; }

		/// <summary>
		/// Entity type to insert or update when the expression is a DML.
		/// </summary>
		protected virtual System.Type TargetType => Type;

		// Since v5.3
		[Obsolete("This property has no usages and will be removed in a future version")]
		public IList<NamedParameterDescriptor> ParameterDescriptors { get; private set; }

		public NhLinqExpressionReturnType ReturnType { get; }

		// Since v5.3
		[Obsolete("Use NamedParameters property instead.")]
		public IDictionary<string, Tuple<object, IType>> ParameterValuesByName { get; }

		public ExpressionToHqlTranslationResults ExpressionToHqlTranslationResults { get; private set; }

		protected virtual QueryMode QueryMode { get; }

		public IDictionary<string, NamedParameter> NamedParameters { get; }

		internal object[] ParameterValues { get; }

		private readonly Expression _expression;
		private readonly IDictionary<ConstantExpression, NamedParameter> _constantToParameterMap;

		public NhLinqExpression(Expression expression, ISessionFactoryImplementor sessionFactory)
			: this(QueryMode.Select, expression, sessionFactory)
		{
		}

		internal NhLinqExpression(QueryMode queryMode, Expression expression, ISessionFactoryImplementor sessionFactory)
		{
			QueryMode = queryMode;
			var preTransformResult = NhRelinqQueryParser.PreTransform(
				expression,
				new PreTransformationParameters(queryMode, sessionFactory));
			_expression = preTransformResult.Expression;

			// We want logging to be as close as possible to the original expression sent from the
			// application. But if we log before partial evaluation done in PreTransform, the log won't
			// include e.g. sub-query expressions if those are defined by the application in a variable
			// referenced from the main query.
			LinqLogging.LogExpression("Expression (partially evaluated)", _expression);

			_constantToParameterMap = ExpressionParameterVisitor.Visit(preTransformResult);

			var parameterValuesByName = new Dictionary<string, Tuple<object, IType>>();
			var namedParameters = new Dictionary<string, NamedParameter>();
			var parameterValues = new object[_constantToParameterMap.Count];
			foreach (var pair in _constantToParameterMap)
			{
				var parameter = pair.Value;
				if (!parameterValuesByName.ContainsKey(parameter.Name))
				{
					parameterValuesByName.Add(parameter.Name, System.Tuple.Create(parameter.Value, parameter.Type));
					namedParameters.Add(parameter.Name, parameter);
				}

				parameterValues[parameter.Index] = parameter.Value;
			}
#pragma warning disable 618
			ParameterValuesByName = parameterValuesByName;
#pragma warning restore 618
			NamedParameters = namedParameters;
			ParameterValues = parameterValues;
			Key = ExpressionKeyVisitor.Visit(_expression, _constantToParameterMap, sessionFactory);

			Type = _expression.Type;

			// Note - re-linq handles return types via the GetOutputDataInfo method, and allows for SingleOrDefault here for the ChoiceResultOperator...
			ReturnType = NhLinqExpressionReturnType.Scalar;

			if (typeof(IQueryable).IsAssignableFrom(Type))
			{
				Type = Type.GetGenericArguments()[0];
				ReturnType = NhLinqExpressionReturnType.Sequence;
			}
		}

		public IASTNode Translate(ISessionFactoryImplementor sessionFactory, bool filter)
		{
			if (ExpressionToHqlTranslationResults != null)
			{
				// Query has already been translated. Arguments do not really matter, because queries are anyway tied
				// to a single session factory and cannot switch from being a filter query (query on a mapped collection)
				// or not.
				return DuplicateTree(ExpressionToHqlTranslationResults.Statement.AstNode);
			}

			var requiredHqlParameters = new List<NamedParameterDescriptor>();
			var queryModel = NhRelinqQueryParser.Parse(_expression);
			ParameterTypeLocator.SetParameterTypes(_constantToParameterMap, queryModel, TargetType, sessionFactory, true);
			var visitorParameters = new VisitorParameters(sessionFactory, _constantToParameterMap, requiredHqlParameters,
				new QuerySourceNamer(), TargetType, QueryMode);

			ExpressionToHqlTranslationResults = QueryModelVisitor.GenerateHqlQuery(queryModel, visitorParameters, true, ReturnType);

			if (ExpressionToHqlTranslationResults.ExecuteResultTypeOverride != null)
				Type = ExpressionToHqlTranslationResults.ExecuteResultTypeOverride;
#pragma warning disable CS0618
			ParameterDescriptors = requiredHqlParameters.AsReadOnly();
#pragma warning restore CS0618
			CanCachePlan &= visitorParameters.CanCachePlan;

			// The ast node may be altered by caller, duplicate it for preserving the original one.
			return DuplicateTree(ExpressionToHqlTranslationResults.Statement.AstNode);
		}

		internal void CopyExpressionTranslation(NhLinqExpression other)
		{
			ExpressionToHqlTranslationResults = other.ExpressionToHqlTranslationResults;
#pragma warning disable CS0618
			ParameterDescriptors = other.ParameterDescriptors;
#pragma warning restore CS0618
			// Type could have been overridden by translation.
			Type = other.Type;
		}

		internal void Prepare()
		{
			ExpressionToHqlTranslationResults = ExpressionToHqlTranslationResults?.WithParameterValues(ParameterValues);
		}

		private static IASTNode DuplicateTree(IASTNode ast)
		{
			var thisNode = ast.DupNode();
			foreach (var child in ast)
			{
				thisNode.AddChild(DuplicateTree(child));
			}
			return thisNode;
		}
	}
}
