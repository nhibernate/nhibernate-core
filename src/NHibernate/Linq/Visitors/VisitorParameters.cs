using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Param;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors
{
	public class VisitorParameters
	{
		public ISessionFactoryImplementor SessionFactory { get; }

		public IDictionary<ConstantExpression, NamedParameter> ConstantToParameterMap { get; }

		public List<NamedParameterDescriptor> RequiredHqlParameters { get; }

		public QuerySourceNamer QuerySourceNamer { get; }

		public NhLinqExpressionReturnType RootReturnType { get; }

		private readonly HashSet<WhereClause> _havingClauses = new HashSet<WhereClause>();
		private readonly HashSet<AdditionalFromClause> _leftJoins = new HashSet<AdditionalFromClause>();
		private readonly HashSet<WhereClause> _withClauses = new HashSet<WhereClause>();
		private readonly Dictionary<AdditionalFromClause, IEnumerable<WhereClause>> _joinRestrictions = new Dictionary<AdditionalFromClause, IEnumerable<WhereClause>>();

		public VisitorParameters(
			ISessionFactoryImplementor sessionFactory,
			IDictionary<ConstantExpression, NamedParameter> constantToParameterMap,
			List<NamedParameterDescriptor> requiredHqlParameters,
			QuerySourceNamer querySourceNamer,
			NhLinqExpressionReturnType rootReturnType)
		{
			SessionFactory = sessionFactory;
			ConstantToParameterMap = constantToParameterMap;
			RequiredHqlParameters = requiredHqlParameters;
			QuerySourceNamer = querySourceNamer;
			RootReturnType = rootReturnType;
		}

		/// <summary>
		/// Indicates if a Linq where clause needs to be converted to a HQL having clause.
		/// </summary>
		/// <param name="clause">The clause to test.</param>
		/// <returns><c>true</c> if the clause needs to be converted to a HQL having clause, <c>false</c> otherwise.</returns>
		public bool IsHavingClause(WhereClause clause)
			=> _havingClauses.Contains(clause);

		/// <summary>
		/// Indicates if a Linq where clause needs to be converted to a HQL with clause.
		/// </summary>
		/// <param name="clause">The clause to test.</param>
		/// <returns><c>true</c> if the clause needs to be converted to a HQL with clause, <c>false</c> otherwise.</returns>
		public bool IsWithClause(WhereClause clause)
			=> _withClauses.Contains(clause);

		/// <summary>
		/// Indicates if a Linq join clause needs to be converted to a HQL left join.
		/// </summary>
		/// <param name="join">The join to test.</param>
		/// <returns><c>true</c> if the clause needs to be converted to a HQL left join, <c>false</c> otherwise.</returns>
		public bool IsLeftJoin(AdditionalFromClause join)
			=> _leftJoins.Contains(join);

		/// <summary>
		/// Get the clauses to apply to the join as HQL with clauses.
		/// </summary>
		/// <param name="join">The join.</param>
		/// <returns>A list of where clauses to apply as HQL with to the join.</returns>
		public IEnumerable<WhereClause> GetRestrictions(AdditionalFromClause join)
		{
			if (_joinRestrictions.TryGetValue(join, out var restrictions))
				return restrictions;
			return new List<WhereClause>();
		}

		/// <summary>
		/// Add a detected having clause.
		/// </summary>
		/// <param name="clause">The clause to add.</param>
		public void AddHavingClause(WhereClause clause)
		{
			_havingClauses.Add(clause);
		}

		/// <summary>
		/// Add a detected join.
		/// </summary>
		/// <param name="join">The join to add.</param>
		/// <param name="restrictions">Its restrictions if any.</param>
		public void AddLeftJoin(AdditionalFromClause join, IEnumerable<WhereClause> restrictions)
		{
			_leftJoins.Add(join);
			if (restrictions != null)
			{
				_joinRestrictions.Add(join, restrictions);
				foreach (var with in restrictions)
				{
					_withClauses.Add(with);
				}
			}
		}

		/// <summary>
		/// Remove a join clause from left join clauses if it was one.
		/// </summary>
		/// <param name="join">The join clause to handle as inner.</param>
		public void MakeInnerJoin(AdditionalFromClause join)
		{
			_leftJoins.Remove(join);
		}
	}
}