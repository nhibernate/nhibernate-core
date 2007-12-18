using System;
using System.Collections;
using System.IO;
using log4net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using NHibernate.Search.Impl;
using NHibernate.Util;

namespace NHibernate.Search.Storage
{
	public class FSDirectoryProvider : IDirectoryProvider
	{
		private FSDirectory directory;
		private static ILog log = LogManager.GetLogger(typeof (FSDirectoryProvider));
		private String indexName;

		public void Initialize(String directoryProviderName, IDictionary properties, SearchFactory searchFactory)
		{
			DirectoryInfo indexDir = DirectoryProviderHelper.DetermineIndexDir(directoryProviderName, properties);
			try
			{
				bool create = !indexDir.Exists;
				indexName = indexDir.FullName;
				directory = FSDirectory.GetDirectory(indexName, create);
				if (create)
				{
					IndexWriter iw = new IndexWriter(directory, new StandardAnalyzer(), create);
					iw.Close();
				}
				searchFactory.RegisterDirectoryProviderForLocks(this);
			}
			catch (IOException e)
			{
				throw new HibernateException("Unable to initialize index: " + directoryProviderName, e);
			}
		}

		public Lucene.Net.Store.Directory Directory
		{
			get { return directory; }
		}

		public override bool Equals(Object obj)
		{
			// this code is actually broken since the value change after initialize call
			// but from a practical POV this is fine since we only call this method
			// after initialize call
			if (obj == this) return true;
			if (obj == null || !(obj is FSDirectoryProvider)) return false;
			return indexName.Equals(((FSDirectoryProvider) obj).indexName);
		}

		public override int GetHashCode()
		{
			// this code is actually broken since the value change after initialize call
			// but from a practical POV this is fine since we only call this method
			// after initialize call
			int hash = 11;
			return 37*hash + indexName.GetHashCode();
		}
	}
}