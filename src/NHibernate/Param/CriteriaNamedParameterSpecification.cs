using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	public class CriteriaNamedParameterSpecification : IParameterSpecification
	{
		private const string CriteriaNamedParameterIdTemplate = "<crnh-{0}_span{1}>";
		private readonly string name;

		public CriteriaNamedParameterSpecification(string name, IType expectedType)
		{
			this.name = name;
			ExpectedType = expectedType;
		}

		#region IParameterSpecification Members

		public void Bind(IDbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			Bind(command, sqlQueryParametersList, 0, sqlQueryParametersList, queryParameters, session);
		}

		public void Bind(IDbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			TypedValue typedValue = queryParameters.NamedParameters[name];
			string backTrackId = GetIdsForBackTrack(session.Factory).First();
			foreach (int position in sqlQueryParametersList.GetEffectiveParameterLocations(backTrackId))
			{
				ExpectedType.NullSafeSet(command, typedValue.Value, position + singleSqlParametersOffset, session);
			}
		}

		public IType ExpectedType { get; set; }

		public string RenderDisplayInfo()
		{
			const string format = "criteria: parameter-name={0}, expectedType={1}";
			return ExpectedType != null ? string.Format(format, name, ExpectedType) : string.Format(format, name, "Unknow");
		}

		public IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			int paremeterSpan = GetParemeterSpan(sessionFactory);
			for (int i = 0; i < paremeterSpan; i++)
			{
				yield return string.Format(CriteriaNamedParameterIdTemplate, name, i);
			}
		}

		#endregion

		protected int GetParemeterSpan(IMapping sessionFactory)
		{
			if (sessionFactory == null)
			{
				throw new ArgumentNullException("sessionFactory");
			}
			return ExpectedType.GetColumnSpan(sessionFactory);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj as CriteriaNamedParameterSpecification);
		}

		public bool Equals(CriteriaNamedParameterSpecification other)
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