using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq
{
	/// <summary>
	/// Specifies one assignment in an update or insert query
	/// </summary>
	public class Assignment
	{
		/// <summary>
		/// The assigned property.
		/// </summary>
		public string PropertyPath { get; }
		/// <summary>
		/// The value to assign.
		/// </summary>
		public Expression Expression { get; }

		public Assignment(string propertyPath, Expression expression)
		{
			PropertyPath = propertyPath;
			Expression = expression;
		}

		public static readonly ConstructorInfo DictionaryConstructorInfo = typeof(Dictionary<string, object>).GetConstructor(new[] { typeof(int) });
		public static readonly MethodInfo DictionaryAddMethodInfo = ReflectHelper.GetMethod<Dictionary<string, object>>(d => d.Add(null, null));

		/// <summary>
		/// Converts the assignments into a lambda expression, which creates a Dictionary&lt;string,object%gt;.
		/// </summary>
		/// <param name="assignments"></param>
		/// <returns>A lambda expression representing the assignments.</returns>
		public static LambdaExpression ConvertAssignmentsToDictionaryExpression<TSource>(IReadOnlyCollection<Assignment> assignments)
		{
			var param = Expression.Parameter(typeof(TSource));
			var inits = new List<ElementInit>();
			foreach (var set in assignments)
			{
				var setter = set.Expression;
				if (setter is LambdaExpression setterLambda)
				{
					setter = setterLambda.Body.Replace(setterLambda.Parameters.First(), param);
				}
				inits.Add(Expression.ElementInit(
					          DictionaryAddMethodInfo, Expression.Constant(set.PropertyPath),
				                                 Expression.Convert(setter, typeof(object))));
			}

			//The ListInit is intentionally "infected" with the lambda parameter (param), in the form of an IIF. 
			//The only relevance is to make sure that the ListInit is not evaluated by the PartialEvaluatingExpressionTreeVisitor,
			//which could turn it into a Constant
			var listInit = Expression.ListInit(
				Expression.New(
					DictionaryConstructorInfo,
					Expression.Condition(
						Expression.Equal(param, Expression.Constant(null, typeof(TSource))),
						Expression.Constant(assignments.Count),
						Expression.Constant(assignments.Count))),
				inits);

			return Expression.Lambda(listInit, param);
		}
	}
}
