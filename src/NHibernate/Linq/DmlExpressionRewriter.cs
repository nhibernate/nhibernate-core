using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq
{
	public class DmlExpressionRewriter
	{
		static readonly ConstructorInfo DictionaryConstructorInfo = typeof(Dictionary<string, object>).GetConstructor(new[] {typeof(int)});

		static readonly MethodInfo DictionaryAddMethodInfo = ReflectHelper.GetMethod<Dictionary<string, object>>(d => d.Add(null, null));

		readonly IReadOnlyCollection<ParameterExpression> _parameters;
		readonly Dictionary<string, Expression> _assignments = new Dictionary<string, Expression>();

		DmlExpressionRewriter(IReadOnlyCollection<ParameterExpression> parameters)
		{
			_parameters = parameters;
		}

		void AddSettersFromBindings(IEnumerable<MemberBinding> bindings, string path)
		{
			foreach (var node in bindings)
				switch (node.BindingType)
				{
					case MemberBindingType.Assignment:
						AddSettersFromAssignment((MemberAssignment) node, path + "." + node.Member.Name);
						break;
					case MemberBindingType.MemberBinding:
						AddSettersFromBindings(((MemberMemberBinding) node).Bindings, path + "." + node.Member.Name);
						break;
					default:
						throw new InvalidOperationException($"{node.BindingType} is not supported");
				}
		}

		void AddSettersFromAssignment(MemberAssignment assignment, string path)
		{
			// {Property=new Instance{SubProperty="Value"}}
			if (assignment.Expression is MemberInitExpression memberInit)
				AddSettersFromBindings(memberInit.Bindings, path);
			else
				_assignments.Add(path.Substring(1), Expression.Lambda(assignment.Expression, _parameters));
		}

		/// <summary>
		///     Converts the assignments into a lambda expression, which creates a Dictionary&lt;string,object%gt;.
		/// </summary>
		/// <param name="assignments"></param>
		/// <returns>A lambda expression representing the assignments.</returns>
		static LambdaExpression ConvertAssignmentsToDictionaryExpression<TSource>(IReadOnlyDictionary<string, Expression> assignments)
		{
			var param = Expression.Parameter(typeof(TSource));
			var inits = new List<ElementInit>();
			foreach (var set in assignments)
			{
				var setter = set.Value;
				if (setter is LambdaExpression setterLambda)
					setter = setterLambda.Body.Replace(setterLambda.Parameters.First(), param);
				inits.Add(
					Expression.ElementInit(
						DictionaryAddMethodInfo,
						Expression.Constant(set.Key),
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

		public static Expression PrepareExpression<TSource, TTarget>(Expression sourceExpression, Expression<Func<TSource, TTarget>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));

			var memberInitExpression = expression.Body as MemberInitExpression ??
			                           throw new ArgumentException("The expression must be member initialization, e.g. x => new Dog { Name = x.Name, Age = x.Age + 5 }");

			var assignments = ExtractAssignments(expression, memberInitExpression);
			return PrepareExpression<TSource>(sourceExpression, assignments);
		}

		public static Expression PrepareExpression<TSource>(Expression sourceExpression, IReadOnlyDictionary<string, Expression> assignments)
		{
			var lambda = ConvertAssignmentsToDictionaryExpression<TSource>(assignments);

			return Expression.Call(
				ReflectionCache.QueryableMethods.SelectDefinition.MakeGenericMethod(typeof(TSource), lambda.Body.Type),
				sourceExpression,
				Expression.Quote(lambda));
		}

		static Dictionary<string, Expression> ExtractAssignments<TSource, TTarget>(Expression<Func<TSource, TTarget>> expression, MemberInitExpression memberInitExpression)
		{
			var instance = new DmlExpressionRewriter(expression.Parameters);
			instance.AddSettersFromBindings(memberInitExpression.Bindings, "");
			return instance._assignments;
		}
	}
}
