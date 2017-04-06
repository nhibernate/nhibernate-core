using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	/// <summary>
	/// Parameter bind specification for an explicit  positional (or ordinal) parameter.
	/// </summary>
	public class PositionalParameterSpecification : AbstractExplicitParameterSpecification
	{
		private const string PositionalParameterIdTemplate = "<pos{0}_span{1}>";

		private readonly int hqlPosition;

		/// <summary>
		/// Constructs a position/ordinal parameter bind specification.
		/// </summary>
		/// <param name="sourceLine">sourceLine</param>
		/// <param name="sourceColumn">sourceColumn</param>
		/// <param name="hqlPosition">The position in the source query, relative to the other source positional parameters.</param>
		public PositionalParameterSpecification(int sourceLine, int sourceColumn, int hqlPosition) : base(sourceLine, sourceColumn)
		{
			this.hqlPosition = hqlPosition;
		}

		/// <summary>
		/// Getter for property 'hqlPosition'.
		/// </summary>
		public int HqlPosition
		{
			get { return hqlPosition; }
		}

		public override string RenderDisplayInfo()
		{
			return "ordinal=" + hqlPosition + ", expectedType=" + ExpectedType;
		}

		public override IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			int paremeterSpan = GetParemeterSpan(sessionFactory);
			for (int i = 0; i < paremeterSpan; i++)
			{
				yield return string.Format(PositionalParameterIdTemplate, hqlPosition, i);
			}
		}

		public override void Bind(DbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			Bind(command, sqlQueryParametersList, 0, sqlQueryParametersList, queryParameters, session);
		}

		public override void Bind(DbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			IType type = ExpectedType;
			object value = queryParameters.PositionalParameterValues[hqlPosition];

			string backTrackId = GetIdsForBackTrack(session.Factory).First(); // just the first because IType suppose the oders in certain sequence
			// an HQL positional parameter can appear more than once because a custom HQL-Function can duplicate it
			foreach (int position in sqlQueryParametersList.GetEffectiveParameterLocations(backTrackId))
			{
				type.NullSafeSet(command, GetPagingValue(value, session.Factory.Dialect, queryParameters), position + singleSqlParametersOffset, session);
			}
		}

		public override int GetSkipValue(QueryParameters queryParameters)
		{
			return (int)queryParameters.PositionalParameterValues[hqlPosition];
		}

		public override void SetEffectiveType(QueryParameters queryParameters)
		{
			ExpectedType = queryParameters.PositionalParameterTypes[hqlPosition];
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as PositionalParameterSpecification);
		}

		public bool Equals(PositionalParameterSpecification other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.hqlPosition == hqlPosition;
		}

		public override int GetHashCode()
		{
			return hqlPosition ^ 751;
		}
	}
}