using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Engine.Query.Sql
{
	public class NativeSQLQuerySpecification : QueryPlanKey
	{
		private readonly INativeSQLQueryReturn[] sqlQueryReturns;
		private readonly ISet<string> querySpaces;
		private readonly int hashCode;

		public NativeSQLQuerySpecification(
			string queryString,
			INativeSQLQueryReturn[] sqlQueryReturns,
			ICollection<string> querySpaces) : base(queryString)
		{
			this.sqlQueryReturns = sqlQueryReturns;

			this.querySpaces = new HashSet<string>();
			if (querySpaces != null)
				this.querySpaces.UnionWith(querySpaces);

			// pre-determine and cache the hashcode
			int hCode = queryString.GetHashCode();
			unchecked
			{
				hCode = 29 * hCode + CollectionHelper.GetHashCode(this.querySpaces);
				if (this.sqlQueryReturns != null)
				{
					hCode = 29 * hCode + CollectionHelper.GetHashCode(this.sqlQueryReturns);
				}
			}

			hashCode = hCode;
		}

		public INativeSQLQueryReturn[] SqlQueryReturns
		{
			get { return sqlQueryReturns; }
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		public override bool Equals(QueryPlanKey other)
		{
			if (!base.Equals(other))
				return false;

			var that = other as NativeSQLQuerySpecification;

			if (that == null)
				return false;

			return CollectionHelper.SequenceEquals(querySpaces, that.querySpaces) &&
				CollectionHelper.SequenceEquals(sqlQueryReturns, that.sqlQueryReturns);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}
