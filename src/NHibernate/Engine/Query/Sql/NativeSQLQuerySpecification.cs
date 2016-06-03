using System.Collections.Generic;
using System.Collections.ObjectModel;

using NHibernate.Util;

namespace NHibernate.Engine.Query.Sql
{
	public class NativeSQLQuerySpecification
	{
		private readonly string queryString;
		private readonly ReadOnlyCollection<INativeSQLQueryReturn> sqlQueryReturns;
		private readonly ReadOnlyCollection<string> querySpaces;
		private readonly int hashCode;

	    public NativeSQLQuerySpecification(
	        string queryString,
	        IList<INativeSQLQueryReturn> sqlQueryReturns,
	        IList<string> querySpaces)
	    {
	        this.queryString = queryString;
	        this.sqlQueryReturns = new ReadOnlyCollection<INativeSQLQueryReturn>(sqlQueryReturns);
	        this.querySpaces = new ReadOnlyCollection<string>(querySpaces ?? new string[0]);

	        // pre-determine and cache the hashcode
	        int hCode = queryString.GetHashCode();

	        unchecked
	        {
	            hCode = 29*hCode + CollectionHelper.GetHashCode(this.querySpaces);

                if (this.sqlQueryReturns.Count > 0)
	            {
	                hCode = 29*hCode + CollectionHelper.GetHashCode(this.sqlQueryReturns);
	            }
	        }

	        hashCode = hCode;
	    }

	    public string QueryString
		{
			get { return queryString; }
		}

		public ReadOnlyCollection<INativeSQLQueryReturn> SqlQueryReturns
		{
			get { return sqlQueryReturns; }
		}

		public ReadOnlyCollection<string> QuerySpaces
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

			// NH-3956: hashcode inequality rules out equality, but hashcode equality is not enough.
			// Code taken back from 8e92af3f and amended according to NH-1931.
			return hashCode == that.hashCode &&
				queryString.Equals(that.queryString) &&
				CollectionHelper.CollectionEquals(querySpaces, that.querySpaces) &&
				CollectionHelper.CollectionEquals<INativeSQLQueryReturn>(sqlQueryReturns, that.sqlQueryReturns);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}