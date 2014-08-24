using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	public class VersionTypeSeedParameterSpecification : IParameterSpecification
	{
		private const string IdBackTrack = "<nhv_seed_nh>";
		private readonly string[] idForBackTracks = new[] {IdBackTrack};
		private readonly IVersionType type;

		public VersionTypeSeedParameterSpecification(IVersionType type)
		{
			this.type = type;
		}

		#region IParameterSpecification Members

		public void Bind(IDbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			int position = sqlQueryParametersList.GetEffectiveParameterLocations(IdBackTrack).Single(); // version parameter can't appear more than once
			type.NullSafeSet(command, type.Seed(session), position, session);
		}

		public void Bind(IDbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			throw new NotSupportedException("Not supported for multiquery loader.");
		}

		public IType ExpectedType
		{
			get { return type; }
			set
			{
				// expected type is intrinsic here...
			}
		}

		public string RenderDisplayInfo()
		{
			return "version-seed, type=" + type;
		}

		public IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			return idForBackTracks;
		}

		#endregion
	}
}