using System;

namespace NHibernate.Cache
{
	[Serializable]
	public class QueryAliasesKey : IEquatable<QueryAliasesKey>
	{
		private readonly QueryKey _queryKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryAliasesKey"/> class.
		/// </summary>
		/// <param name="queryKey">The <see cref="QueryKey"/> of the query for which aliases have to be stored.</param>
		public QueryAliasesKey(QueryKey queryKey)
		{
			_queryKey = queryKey ?? throw new ArgumentNullException(nameof(queryKey));
		}

		public override bool Equals(object other)
		{
			return Equals(other as QueryAliasesKey);
		}

		public bool Equals(QueryAliasesKey other)
		{
			return other != null && _queryKey.Equals(other._queryKey);
		}

		public override int GetHashCode()
		{
			return _queryKey.GetHashCode();
		}

		public override string ToString()
		{
			return "QueryAlisesKey: " + _queryKey.ToString();
		}
	}
}
