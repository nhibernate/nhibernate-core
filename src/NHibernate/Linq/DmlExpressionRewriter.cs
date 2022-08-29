using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq
{
	public class DmlExpressionRewriter
	{
		readonly IReadOnlyCollection<ParameterExpression> _parameters;
		readonly Dictionary<string, Expression> _assignments = new Dictionary<string, Expression>();

		DmlExpressionRewriter(IReadOnlyCollection<ParameterExpression> parameters)
		{
			_parameters = parameters;
		}

		void AddSettersFromBindings(IEnumerable<MemberBinding> bindings, string path)
		{
			foreach (var node in bindings)
			{
				var subPath = path + "." + node.Member.Name;
				switch (node.BindingType)
				{
					case MemberBindingType.Assignment:
						AddSettersFromAssignment((MemberAssignment) node, subPath);
						break;
					case MemberBindingType.MemberBinding:
						AddSettersFromBindings(((MemberMemberBinding) node).Bindings, subPath);
						break;
					default:
						throw new InvalidOperationException($"{node.BindingType} is not supported");
				}
			}
		}

		void AddSettersFromAnonymousConstructor(NewExpression newExpression, string path)
		{
			// See Members documentation, this property is specifically designed to match constructor arguments values
			// in the anonymous object case. It can be null otherwise, or non-matching.
			var argumentMatchingMembers = newExpression.Members;
			if (argumentMatchingMembers == null || argumentMatchingMembers.Count != newExpression.Arguments.Count)
				throw new ArgumentException("The expression must be an anonymous initialization, e.g. x => new { Name = x.Name, Age = x.Age + 5 }");

			var i = 0;
			foreach (var argument in newExpression.Arguments)
			{
				var argumentDefinition = argumentMatchingMembers[i];
				i++;
				var subPath = path + "." + argumentDefinition.Name;
				switch (argument.NodeType)
				{
					case ExpressionType.New:
						AddSettersFromAnonymousConstructor((NewExpression) argument, subPath);
						break;
					case ExpressionType.MemberInit:
						AddSettersFromBindings(((MemberInitExpression) argument).Bindings, subPath);
						break;
					default:
						_assignments.Add(subPath.Substring(1), Expression.Lambda(argument, _parameters));
						break;
				}
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
		///     Converts the assignments into block of assignments
		/// </summary>
		/// <param name="assignments"></param>
		/// <returns>A lambda expression representing the assignments.</returns>
		static LambdaExpression ConvertAssignmentsToBlockExpression<TSource>(IReadOnlyDictionary<string, Expression> assignments)
		{
			var param = Expression.Parameter(typeof(TSource));
			var variableAndAssignmentDic = new Dictionary<ParameterExpression, Expression>(assignments.Count);
			foreach (var set in assignments)
			{
				var setter = set.Value;
				if (setter is LambdaExpression setterLambda)
					setter = setterLambda.Body.Replace(setterLambda.Parameters.First(), param);

				var var = Expression.Variable(typeof(object), set.Key);
				variableAndAssignmentDic[var] = Expression.Assign(var, Expression.Convert(setter, typeof(object)));
			}

			return Expression.Lambda(Expression.Block(variableAndAssignmentDic.Keys, variableAndAssignmentDic.Values), param);
		}

		public static Expression PrepareExpression<TSource, TTarget>(Expression sourceExpression, Expression<Func<TSource, TTarget>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));

			var memberInitExpression = expression.Body as MemberInitExpression ??
				throw new ArgumentException("The expression must be a member initialization, e.g. x => new Dog { Name = x.Name, Age = x.Age + 5 }, " +
					// If someone call InsertSyntax<TSource>.As(source => new {...}), the code will fail here, so we have to hint at how to correctly
					// use anonymous initialization too.
					"or an anonymous initialization with an explicitly specified target type when inserting");

			if (memberInitExpression.Type != typeof(TTarget))
				throw new TypeMismatchException($"Expecting an expression of exact type {typeof(TTarget).AssemblyQualifiedName} " +
					$"but got {memberInitExpression.Type.AssemblyQualifiedName}");

			var instance = new DmlExpressionRewriter(expression.Parameters);
			instance.AddSettersFromBindings(memberInitExpression.Bindings, "");
			return PrepareExpression<TSource>(sourceExpression, instance._assignments);
		}

		public static Expression PrepareExpressionFromAnonymous<TSource>(Expression sourceExpression, Expression<Func<TSource, object>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));

			// Anonymous initializations are not implemented as member initialization but as plain constructor call.
			var newExpression = expression.Body as NewExpression ??
				throw new ArgumentException("The expression must be an anonymous initialization, e.g. x => new { Name = x.Name, Age = x.Age + 5 }");

			var instance = new DmlExpressionRewriter(expression.Parameters);
			instance.AddSettersFromAnonymousConstructor(newExpression, "");
			return PrepareExpression<TSource>(sourceExpression, instance._assignments);
		}

		public static Expression PrepareExpression<TSource>(Expression sourceExpression, IReadOnlyDictionary<string, Expression> assignments)
		{
			var lambda = ConvertAssignmentsToBlockExpression<TSource>(assignments);

			return Expression.Call(
				ReflectionCache.QueryableMethods.SelectDefinition.MakeGenericMethod(typeof(TSource), lambda.Body.Type),
				sourceExpression,
				Expression.Quote(lambda));
		}
	}
}
