using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using NHibernate.Type;

namespace NHibernate.Linq
{
	public class ExpressionToHqlTranslationResults
	{
		public HqlTreeNode Statement { get;  }
		public ResultTransformer ResultTransformer { get; }
		public Delegate PostExecuteTransformer { get; }
		public List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> AdditionalCriteria { get; }

		/// <summary>
		/// If execute result type does not match expected final result type (implying a post execute transformer
		/// will yield expected result type), the intermediate execute type.
		/// </summary>
		public System.Type ExecuteResultTypeOverride { get; }

		public ExpressionToHqlTranslationResults(HqlTreeNode statement, 
			IList<LambdaExpression> itemTransformers, 
			IList<LambdaExpression> listTransformers,
			IList<LambdaExpression> postExecuteTransformers,
			List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> additionalCriteria,
			System.Type executeResultTypeOverride)
		{
			Statement = statement;

			PostExecuteTransformer = MergeLambdasAndCompile(postExecuteTransformers);

			var itemTransformer = MergeLambdasAndCompile<Func<object[], object>>(itemTransformers);
			var listTransformer = MergeLambdasAndCompile<Func<IEnumerable<object>, object>>(listTransformers);

			if (itemTransformer != null || listTransformer != null)
			{
				 ResultTransformer = new ResultTransformer(itemTransformer, listTransformer);
			}

			AdditionalCriteria = additionalCriteria;
			ExecuteResultTypeOverride = executeResultTypeOverride;
		}

		private static TDelegate MergeLambdasAndCompile<TDelegate>(IList<LambdaExpression> itemTransformers) 
		{
			var lambda = MergeLambdas(itemTransformers);
			if (lambda == null)
				return default(TDelegate);

			var body = lambda.ReturnType.IsValueType
						   ? Expression.Convert(lambda.Body, typeof (object))
						   : lambda.Body;
			
			return Expression.Lambda<TDelegate>(body, lambda.Parameters).Compile();
		}

		private static Delegate MergeLambdasAndCompile(IList<LambdaExpression> transformations)
		{
			var lambda = MergeLambdas(transformations);
			if (lambda == null)
				return null;
			
			return lambda.Compile();
		}

		private static LambdaExpression MergeLambdas(IList<LambdaExpression> transformations)
		{
			if (transformations == null || transformations.Count == 0)
				return null;

			var lambda = transformations[0];

			for (int i = 1; i < transformations.Count; i++)
			{
				var invoked = Expression.Invoke(transformations[i], lambda.Body);

				lambda = Expression.Lambda(invoked, lambda.Parameters);
			}

			return lambda;
		}
	}
}