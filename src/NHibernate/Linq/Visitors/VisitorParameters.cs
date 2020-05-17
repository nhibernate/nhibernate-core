using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Param;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class VisitorParameters
	{
		public ISessionFactoryImplementor SessionFactory { get; private set; }

		public IDictionary<ConstantExpression, NamedParameter> ConstantToParameterMap { get; private set; }

		public List<NamedParameterDescriptor> RequiredHqlParameters { get; private set; }

		public QuerySourceNamer QuerySourceNamer { get; set; }

		/// <summary>
		/// Entity type to insert or update when the operation is a DML.
		/// </summary>
		public System.Type TargetEntityType { get; }

		public QueryMode RootQueryMode { get; }

		internal bool CanCachePlan { get; private set; } = true;

		public VisitorParameters(
			ISessionFactoryImplementor sessionFactory, 
			IDictionary<ConstantExpression, NamedParameter> constantToParameterMap, 
			List<NamedParameterDescriptor> requiredHqlParameters, 
			QuerySourceNamer querySourceNamer,
			System.Type targetEntityType,
			QueryMode rootQueryMode)
		{
			SessionFactory = sessionFactory;
			ConstantToParameterMap = constantToParameterMap;
			RequiredHqlParameters = requiredHqlParameters;
			QuerySourceNamer = querySourceNamer;
			TargetEntityType = targetEntityType;
			RootQueryMode = rootQueryMode;
		}

		internal void UpdateCanCachePlan(
			System.Action action,
			Action<RelinqExpressionVisitor> visitAction)
		{
			UpdateCanCachePlan(
				() =>
				{
					action();
					return true;
				},
				visitAction);
		}
		internal T UpdateCanCachePlan<T>(
			Func<T> function,
			Action<RelinqExpressionVisitor> visitAction)
		{
			var totalHqlParameters = RequiredHqlParameters.Count;
			var result = function();
			var visitor = new ParameterMatcher(this, totalHqlParameters);
			if (!visitor.MatchHqlParameters(visitAction))
			{
				CanCachePlan = false;
			}

			return result;
		}

		private class ParameterMatcher : RelinqExpressionVisitor
		{
			private readonly VisitorParameters _parameters;
			private readonly List<NamedParameter> _namedParameters = new List<NamedParameter>();
			private readonly int _totalHqlParameters;

			public ParameterMatcher(VisitorParameters parameters, int totalHqlParameters)
			{
				_parameters = parameters;
				_totalHqlParameters = totalHqlParameters;
			}

			public bool MatchHqlParameters(Action<RelinqExpressionVisitor> visitAction)
			{
				visitAction(this);
				if (_namedParameters.Count == 0 && _parameters.RequiredHqlParameters.Count == _totalHqlParameters)
				{
					return true;
				}

				return MatchHqlParameters(_parameters.RequiredHqlParameters.Skip(_totalHqlParameters).ToList());
			}

			protected override Expression VisitConstant(ConstantExpression node)
			{
				if (_parameters.ConstantToParameterMap.TryGetValue(node, out var parameter))
				{
					_namedParameters.Add(parameter);
				}

				return base.VisitConstant(node);
			}

			private bool MatchHqlParameters(List<NamedParameterDescriptor> hqlParameters)
			{
				if (_namedParameters.Count != hqlParameters.Count)
				{
					return false;
				}

				for (var i = 0; i < hqlParameters.Count; i++)
				{
					if (_namedParameters[i].Name != hqlParameters[i].Name)
					{
						return false;
					}
				}

				return true;
			}
		}
	}
}
