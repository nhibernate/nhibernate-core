using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	public partial class CollectionFilterKeyParameterSpecification : IParameterSpecification
	{
		private const string CollectionFilterParameterIdTemplate = "<collfilter{0}{1}_{2}>";

		private readonly string collectionRole;
		private readonly IType keyType;
		private readonly int queryParameterPosition;

		/// <summary>
		/// Creates a specialized collection-filter collection-key parameter spec.
		/// </summary>
		/// <param name="collectionRole">The collection role being filtered.</param>
		/// <param name="keyType">The mapped collection-key type.</param>
		/// <param name="queryParameterPosition">The position within QueryParameters where we can find the appropriate param value to bind.</param>
		public CollectionFilterKeyParameterSpecification(string collectionRole, IType keyType, int queryParameterPosition)
		{
			this.collectionRole = collectionRole;
			this.keyType = keyType;
			this.queryParameterPosition = queryParameterPosition;
		}

		#region IParameterSpecification Members

		public void Bind(DbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			IType type = keyType;
			object value = queryParameters.PositionalParameterValues[queryParameterPosition];

			string backTrackId = GetIdsForBackTrack(session.Factory).First(); // just the first because IType suppose the oders in certain sequence
			int position = sqlQueryParametersList.GetEffectiveParameterLocations(backTrackId).Single(); // an HQL positional parameter can't appear more than once
			type.NullSafeSet(command, value, position + singleSqlParametersOffset, session);
		}

		public IType ExpectedType
		{
			get { return keyType; }
			set { throw new InvalidOperationException(); }
		}

		public string RenderDisplayInfo()
		{
			return "collection-filter-key=" + collectionRole;
		}

		public IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			int paremeterSpan = keyType.GetColumnSpan(sessionFactory);
			for (int i = 0; i < paremeterSpan; i++)
			{
				yield return string.Format(CollectionFilterParameterIdTemplate, collectionRole, queryParameterPosition, i);
			}
		}

		public void Bind(DbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			Bind(command, sqlQueryParametersList, 0, sqlQueryParametersList, queryParameters, session);
		}

		#endregion

		public override bool Equals(object obj)
		{
			return base.Equals(obj as CollectionFilterKeyParameterSpecification);
		}

		public bool Equals(CollectionFilterKeyParameterSpecification other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.queryParameterPosition == queryParameterPosition;
		}

		public override int GetHashCode()
		{
			return queryParameterPosition ^ 877;
		}
	}
}