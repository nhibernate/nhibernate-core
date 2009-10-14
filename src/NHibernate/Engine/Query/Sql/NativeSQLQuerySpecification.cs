using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Engine.Query.Sql
{
	public class NativeSQLQuerySpecification
	{
		private readonly string queryString;
		private readonly INativeSQLQueryReturn[] sqlQueryReturns;
		private readonly ISet<string> querySpaces;
		private readonly int hashCode;

		public NativeSQLQuerySpecification(
			string queryString,
			INativeSQLQueryReturn[] sqlQueryReturns,
			ICollection<string> querySpaces)
		{
			this.queryString = queryString;
			this.sqlQueryReturns = sqlQueryReturns;

			if (querySpaces == null)
			{
				this.querySpaces = new HashedSet<string>();
			}
			else
			{
				ISet<string> tmp = new HashedSet<string>();
				tmp.AddAll(querySpaces);
				// Can't use ImmutableSet here because it doesn't implement GetHashCode properly.
				this.querySpaces = tmp;
			}

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

		public string QueryString
		{
			get { return queryString; }
		}

		public INativeSQLQueryReturn[] SqlQueryReturns
		{
			get { return sqlQueryReturns; }
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			var that = obj as NativeSQLQuerySpecification;

			if (that == null)
				return false;

			// NHibernate different impl.: NativeSQLQuerySpecification is immutable and the hash is calculated at Ctor
			return hashCode == that.hashCode;
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}