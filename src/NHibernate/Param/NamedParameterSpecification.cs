using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Param
{
	/// <summary>
	/// Parameter bind specification for an explicit named parameter.
	/// </summary>
	public class NamedParameterSpecification : AbstractExplicitParameterSpecification
	{
		private const string NamedParameterIdTemplate = "<nnh{0}_span{1}>";

		private readonly string name;

		/// <summary>
		/// Constructs a named parameter bind specification.
		/// </summary>
		/// <param name="sourceLine">sourceLine</param>
		/// <param name="sourceColumn">sourceColumn</param>
		/// <param name="name">The named parameter name.</param>
		public NamedParameterSpecification(int sourceLine, int sourceColumn, string name) : base(sourceLine, sourceColumn)
		{
			this.name = name;
		}

		/// <summary>
		/// The user parameter name.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		public override string RenderDisplayInfo()
		{
			const string format = "name={0}, expectedType={1}";
			return ExpectedType != null ? string.Format(format, name, ExpectedType) : string.Format(format, name, "Unknow");
		}

		public override IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			int paremeterSpan = GetParemeterSpan(sessionFactory);
			for (int i = 0; i < paremeterSpan; i++)
			{
				yield return string.Format(NamedParameterIdTemplate, name, i);
			}
		}

		public override void Bind(IDbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			Bind(command, sqlQueryParametersList, 0, sqlQueryParametersList, queryParameters, session);
		}

		public override void Bind(IDbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			TypedValue typedValue = queryParameters.NamedParameters[name];
			string backTrackId = GetIdsForBackTrack(session.Factory).First(); // just the first because IType suppose the oders in certain sequence
			foreach (int position in sqlQueryParametersList.GetEffectiveParameterLocations(backTrackId))
			{
				ExpectedType.NullSafeSet(command, GetPagingValue(typedValue.Value, session.Factory.Dialect, queryParameters), position + singleSqlParametersOffset, session);
			}
		}

		public override int GetSkipValue(QueryParameters queryParameters)
		{
			return (int)queryParameters.NamedParameters[name].Value;
		}

		public override void SetEffectiveType(QueryParameters queryParameters)
		{
			ExpectedType = queryParameters.NamedParameters[name].Type;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as NamedParameterSpecification);
		}

		public bool Equals(NamedParameterSpecification other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.name, name);
		}

		public override int GetHashCode()
		{
			return name.GetHashCode() ^ 211;
		}
	}
}