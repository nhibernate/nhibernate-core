// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using MemberBinding = Remotion.Linq.Parsing.ExpressionVisitors.MemberBindings.MemberBinding;

namespace NHibernate.Linq.Visitors
{
	// Copied from Relinq and added a fallback for comparing two member info by DeclaringType and Name
	// 6.0 TODO: drop if https://github.com/OData/WebApi/issues/2108 is fixed and add a possible breaking
	// change requiring to upgrade OData. (See https://github.com/nhibernate/nhibernate-core/pull/2322#discussion_r401215456 )
	// Use this version in order to support expressions that were created programmatically and do not mimic what the C# compiler generates.
	// Consider removing this if https://re-motion.atlassian.net/projects/RMLNQ/issues/RMLNQ-121 is fixed and we upgrade ReLinq.
	/// <summary>
	/// Replaces expression patterns of the form <c>new T { x = 1, y = 2 }.x</c> (<see cref="MemberInitExpression"/>) or 
	/// <c>new T ( x = 1, y = 2 ).x</c> (<see cref="NewExpression"/>) to <c>1</c> (or <c>2</c> if <c>y</c> is accessed instead of <c>x</c>).
	/// Expressions are also replaced within subqueries; the <see cref="QueryModel"/> is changed by the replacement operations, it is not copied. 
	/// </summary>
	internal sealed class TransparentIdentifierRemovingExpressionVisitor : RelinqExpressionVisitor
	{
		public static Expression ReplaceTransparentIdentifiers(Expression expression)
		{
			Expression expressionBeforeRemove;
			Expression expressionAfterRemove = expression;

			// Run again and again until no replacements have been made.
			do
			{
				expressionBeforeRemove = expressionAfterRemove;
				expressionAfterRemove = new TransparentIdentifierRemovingExpressionVisitor().Visit(expressionAfterRemove);
			} while (expressionAfterRemove != expressionBeforeRemove);

			return expressionAfterRemove;
		}

		private TransparentIdentifierRemovingExpressionVisitor()
		{
		}

		protected override Expression VisitMember(MemberExpression memberExpression)
		{
			var memberBindings = GetMemberBindingsCreatedByExpression(memberExpression.Expression);
			if (memberBindings == null)
				return base.VisitMember(memberExpression);

			var matchingAssignment = memberBindings
									 .Where(binding => binding.MatchesReadAccess(memberExpression.Member))
									 .LastOrDefault();

			// Added logic: In some cases (e.g OData), the member can be from a different derived class, in such case
			// we need to check the member DeclaringType instead of ReflectedType
			if (matchingAssignment == null && memberExpression.Expression.NodeType == ExpressionType.MemberInit)
			{
				matchingAssignment = memberBindings
									 .Where(binding => AreEqual(binding.BoundMember, memberExpression.Member))
									 .LastOrDefault();
			}

			if (matchingAssignment == null)
				return base.VisitMember(memberExpression);
			else
				return matchingAssignment.AssociatedExpression;
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(ReplaceTransparentIdentifiers);
			return expression; // Note that we modifiy the (mutable) QueryModel, we return an unchanged expression
		}

		private IEnumerable<MemberBinding> GetMemberBindingsCreatedByExpression(Expression expression)
		{
			if (expression is MemberInitExpression memberInitExpression)
			{
				return memberInitExpression.Bindings
					.Where(binding => binding is MemberAssignment)
					.Select(assignment => MemberBinding.Bind(assignment.Member, ((MemberAssignment) assignment).Expression));
			}

			if (expression is NewExpression newExpression && newExpression.Members != null)
			{
				return GetMemberBindingsForNewExpression(newExpression);
			}

			return null;
		}

		private IEnumerable<MemberBinding> GetMemberBindingsForNewExpression(NewExpression newExpression)
		{
			for (int i = 0; i < newExpression.Members.Count; ++i)
				yield return MemberBinding.Bind(newExpression.Members[i], newExpression.Arguments[i]);
		}

		private static bool AreEqual(MemberInfo memberInfo, MemberInfo toComapre)
		{
			return memberInfo.DeclaringType == toComapre.DeclaringType && memberInfo.Name == toComapre.Name;
		}
	}
}
