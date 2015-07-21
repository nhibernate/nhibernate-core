using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Cfg.Loquacious
{
	public interface INamedQueryDefinitionBuilder
	{
		bool IsCacheable { get; set; }
		string CacheRegion { get; set; }
		int FetchSize { get; set; }
		int Timeout { get; set; }
		FlushMode FlushMode { get; set; }
		string Query { get; set; }
		bool IsReadOnly { get; set; }
		string Comment { get; set; }
		CacheMode? CacheMode { get; set; }
	}

	internal class NamedQueryDefinitionBuilder : INamedQueryDefinitionBuilder
	{
		private int fetchSize = -1;
		private int timeout = -1;

		public NamedQueryDefinitionBuilder()
		{
			FlushMode = FlushMode.Unspecified;
		}

		#region INamedQueryDefinitionBuilder Members

		public bool IsCacheable { get; set; }
		public string CacheRegion { get; set; }

		public int FetchSize
		{
			get { return fetchSize; }
			set
			{
				if (value > 0)
				{
					fetchSize = value;
				}
				else
				{
					fetchSize = -1;
				}
			}
		}

		public int Timeout
		{
			get { return timeout; }
			set
			{
				if (value > 0)
				{
					timeout = value;
				}
				else
				{
					timeout = -1;
				}
			}
		}

		public FlushMode FlushMode { get; set; }
		public string Query { get; set; }
		public bool IsReadOnly { get; set; }
		public string Comment { get; set; }
		public CacheMode? CacheMode { get; set; }

		#endregion

		public NamedQueryDefinition Build()
		{
			return new NamedQueryDefinition(Query, IsCacheable, CacheRegion, Timeout, FetchSize, FlushMode, CacheMode ,IsReadOnly, Comment, new Dictionary<string, string>(1));
		}
	}
}