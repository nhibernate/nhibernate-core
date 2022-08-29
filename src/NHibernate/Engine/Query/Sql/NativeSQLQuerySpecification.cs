using System;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Engine.Query.Sql
{
	public class NativeSQLQuerySpecification
	{
		private readonly string queryString;
		private readonly INativeSQLQueryReturn[] sqlQueryReturns;
		private readonly HashSet<string> querySpaces;
		private readonly int hashCode;

		public NativeSQLQuerySpecification(
			string queryString,
			INativeSQLQueryReturn[] sqlQueryReturns,
			ICollection<string> querySpaces)
		{
			this.queryString = queryString;
			this.sqlQueryReturns = sqlQueryReturns;

			this.querySpaces = new HashSet<string>();
			if (querySpaces != null)
				this.querySpaces.UnionWith(querySpaces);

			// pre-determine and cache the hashcode
			int hCode = queryString.GetHashCode();
			unchecked
			{
				hCode = 29 * hCode + CollectionHelper.GetHashCode(this.querySpaces, this.querySpaces.Comparer);
				if (this.sqlQueryReturns != null)
				{
					hCode = 29 * hCode + ArrayHelper.ArrayGetHashCode(this.sqlQueryReturns);
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

			// NH-3956: hashcode inequality rules out equality, but hashcode equality is not enough.
			// Code taken back from 8e92af3f and amended according to NH-1931.
			return
				hashCode == that.hashCode &&
				queryString.Equals(that.queryString) &&
				CollectionHelper.SetEquals(querySpaces, that.querySpaces) &&
				ArrayHelper.ArrayEquals(sqlQueryReturns, that.sqlQueryReturns);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}
