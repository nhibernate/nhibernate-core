using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	public class Assignments 
	{
		readonly IReadOnlyCollection<ParameterExpression> _parameters;
		readonly List<Assignment> _sets  = new List<Assignment>();

		Assignments(IReadOnlyCollection<ParameterExpression> parameters)
		{
			_parameters = parameters;
		}

		void AddSetsFromBindings(IEnumerable<MemberBinding> bindings, string path)
		{
			foreach (var node in bindings)
			{
				switch (node.BindingType)
				{
					case MemberBindingType.Assignment:
						AddSetsFromAssignment((MemberAssignment)node, path + "." + node.Member.Name);
						break;
					case MemberBindingType.MemberBinding:
						AddSetsFromBindings(((MemberMemberBinding)node).Bindings, path + "." + node.Member.Name);
						break;
					default:
						throw new InvalidOperationException($"{node.BindingType} is not supported");
				}
			}
		}

		void AddSetsFromAssignment(MemberAssignment assignment, string path)
		{
			// {Property=new Instance{SubProperty="Value"}}
			if (assignment.Expression is MemberInitExpression memberInit)
			{
				AddSetsFromBindings(memberInit.Bindings, path);
			}
			else
			{
				_sets.Add(new Assignment(path.Substring(1), Expression.Lambda(assignment.Expression, _parameters)));
			}
		}

		/// <summary>
		/// Converts a members initialization expression to assignments. Unset members are ignored and left untouched.
		/// </summary>
		/// <param name="expression">The expression to convert.</param>
		/// <returns>The corresponding assignments.</returns>
		public static List<Assignment> FromExpression<TSource, TTarget>(Expression<Func<TSource, TTarget>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			var memberInitExpression = expression.Body as MemberInitExpression ??
			                           throw new ArgumentException("The expression must be member initialization, e.g. x => new Dog { Name = x.Name, Age = x.Age + 5 }");

			var instance = new Assignments(expression.Parameters);
			instance.AddSetsFromBindings(memberInitExpression.Bindings, "");
			return instance._sets;
		}
	}
}
