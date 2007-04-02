using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using log4net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;

namespace NHibernate.Search.Storage
{
	/// <summary>
	/// File based DirectoryProvider that takes care of index copy
	/// The base directory is represented by hibernate.search.<index>.indexBase
	/// The index is created in <base directory>/<index name>
	/// The source (aka copy) directory is built from <sourceBase>/<index name>
	/// A copy is triggered every refresh seconds
	/// </summary>
	public class FSMasterDirectoryProvider : IDirectoryProvider
	{
		private static ILog log = LogManager.GetLogger(typeof(FSMasterDirectoryProvider));
		private FSDirectory directory;
		private int current;
		private String indexName;
		private Timer timer;
		private SearchFactory searchFactory;

		public void Initialize(String directoryProviderName, IDictionary properties, SearchFactory searchFactory)
		{
			//source guessing
			String source = DirectoryProviderHelper.GetSourceDirectory(Environment.SourceBase, Environment.Source, directoryProviderName, properties);
			if (source == null)
				throw new ArgumentException("FSMasterDirectoryProvider requires a viable source directory");
			log.Debug("Source directory: " + source);
			DirectoryInfo indexDir = DirectoryProviderHelper.DetermineIndexDir(directoryProviderName, properties);
			log.Debug("Index directory: " + indexDir);
			String refreshPeriod = (string)(properties[Environment.Refresh] ?? "3600");
			long period = int.Parse(refreshPeriod);
			log.Debug("Refresh period " + period + " seconds");
			period *= 1000; //per second
			try
			{
				bool create = !File.Exists(Path.Combine(indexDir.FullName, "segments"));
				indexName = indexDir.FullName;
				if (create)
				{
					log.Debug("Index directory '" + indexName + "' will be initialized");
					indexDir.Create();
				}
				directory = FSDirectory.GetDirectory(indexName, create);

				if (create)
				{
					IndexWriter iw = new IndexWriter(directory, new StandardAnalyzer(), create);
					iw.Close();
				}

				//copy to source
				if (File.Exists(Path.Combine(source, "current1")))
				{
					current = 2;
				}
				else if (File.Exists(Path.Combine(source, "current2")))
				{
					current = 1;
				}
				else
				{
					log.Debug("Source directory for '" + indexName + "' will be initialized");
					current = 1;
				}
				String currentString = current.ToString();
				DirectoryInfo subDir = new DirectoryInfo(Path.Combine(source, currentString));
				FileHelper.Synchronize(indexDir, subDir, true);
				File.Delete(Path.Combine(source, "current1"));
				File.Delete(Path.Combine(source, "current2"));
				log.Debug("Current directory: " + current);
			}
			catch (IOException e)
			{
				throw new HibernateException("Unable to initialize index: " + directoryProviderName, e);
			}
			searchFactory.RegisterDirectoryProviderForLocks(this);
			timer = new Timer(
				new CopyDirectory(this, indexName, source).Run
				);
			timer.Change(period, period);
			this.searchFactory = searchFactory;
		}

		public Directory Directory
		{
			get { return directory; }
		}

		public override bool Equals(Object obj)
		{
			// this code is actually broken since the value change after initialize call
			// but from a practical POV this is fine since we only call this method
			// after initialize call
			if (obj == this) return true;
			if (obj == null || !(obj is FSMasterDirectoryProvider)) return false;
			return indexName.Equals(((FSMasterDirectoryProvider)obj).indexName);
		}

		public override int GetHashCode()
		{
			// this code is actually broken since the value change after initialize call
			// but from a practical POV this is fine since we only call this method
			// after initialize call
			int hash = 11;
			return 37 * hash + indexName.GetHashCode();
		}


		private class CopyDirectory
		{
			private readonly FSMasterDirectoryProvider parent;
			private String source;
			private String destination;
			private object directoryProviderLock;

			public CopyDirectory(FSMasterDirectoryProvider parent, String source, String destination)
			{
				this.parent = parent;
				this.source = source;
				this.destination = destination;
			}

			[System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.Synchronized)]
			public void Run(object ignored)
			{
				//TODO get rid of current and use the marker file instead?
				DateTime start = DateTime.Now;
				if (directoryProviderLock == null)
				{
					directoryProviderLock = parent.searchFactory.GetLockObjForDirectoryProvider(parent);
				}
				lock (directoryProviderLock)
				{
					int oldIndex = parent.current;
					int index = parent.current == 1 ? 2 : 1;
					DirectoryInfo sourceFile = new DirectoryInfo(source);

					DirectoryInfo destinationFile = new DirectoryInfo(Path.Combine(destination, index.ToString()));
					//TODO make smart a parameter
					try
					{
						log.Info("Copying " + sourceFile + " into " + destinationFile);
						FileHelper.Synchronize(sourceFile, destinationFile, true);
						parent.current = index;
					}
					catch (IOException e)
					{
						//don't change current
						log.Error("Unable to synchronize source of " + parent.indexName, e);
						return;
					}
					try
					{
						File.Delete(Path.Combine(destination, "current" + oldIndex));
					}
					catch (IOException e)
					{
						log.Warn("Unable to remove previous marker file from source of " + parent.indexName, e);
					}
					try
					{
						File.Create(Path.Combine(destination, "current" + index)).Dispose();
					}
					catch (IOException e)
					{
						log.Warn("Unable to create current marker in source of " + parent.indexName, e);
					}
				}
				log.Info("Copy for " + parent.indexName + " took " + (DateTime.Now- start) + ".");
			}
		}
	}
}