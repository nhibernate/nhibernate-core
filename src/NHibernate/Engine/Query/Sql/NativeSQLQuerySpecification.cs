using System.Collections;
using Iesi.Collections;
using NHibernate.Util;

namespace NHibernate.Engine.Query.Sql
{
	public class NativeSQLQuerySpecification
	{
		private readonly string queryString;
		private readonly INativeSQLQueryReturn[] sqlQueryReturns;
		private readonly ISet querySpaces;
		private readonly int hashCode;

		public NativeSQLQuerySpecification(
			string queryString,
			INativeSQLQueryReturn[] sqlQueryReturns,
			ICollection querySpaces)
		{
			this.queryString = queryString;
			this.sqlQueryReturns = sqlQueryReturns;

			if (querySpaces == null)
			{
				this.querySpaces = new HashedSet();
			}
			else
			{
				ISet tmp = new HashedSet();
				tmp.AddAll(querySpaces);
				// Can't use ImmutableSet here because it doesn't implement GetHashCode properly.
				this.querySpaces = tmp;
			}

			// pre-determine and cache the hashcode
			int hCode = queryString.GetHashCode();
			unchecked
			{
				hCode = 29 * hCode + this.querySpaces.GetHashCode();
				if (this.sqlQueryReturns != null)
				{
					hCode = 29 * hCode + sqlQueryReturns.Length;
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

		public ISet QuerySpaces
		{
			get { return querySpaces; }
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}

			if (o == null || GetType() != o.GetType())
			{
				return false;
			}

			NativeSQLQuerySpecification that = (NativeSQLQuerySpecification) o;

			return hashCode == that.hashCode &&
			       CollectionHelper.CollectionEquals(querySpaces, that.querySpaces) &&
			       querySpaces.Equals(that.querySpaces) &&
			       queryString.Equals(that.queryString) &&
			       CollectionHelper.CollectionEquals(sqlQueryReturns, that.sqlQueryReturns);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}