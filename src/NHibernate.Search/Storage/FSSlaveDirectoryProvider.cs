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
	/// File based directory provider that takes care of geting a version of the index
	/// from a given source
	/// The base directory is represented by hibernate.search.<index>.indexBase
	/// The index is created in <base directory>/<index name>
	/// The source (aka copy) directory is built from <sourceBase>/<index name>
	/// 
	/// A copy is triggered every refresh seconds
	/// </summary>
	public class FSSlaveDirectoryProvider : IDirectoryProvider
	{
		private static ILog log = LogManager.GetLogger(typeof(FSSlaveDirectoryProvider));
		private FSDirectory directory1;
		private FSDirectory directory2;
		private int current;
		private String indexName;
		private Timer timer;

		public void Initialize(String directoryProviderName, IDictionary properties, SearchFactory searchFactory)
		{
			//source guessing
			String source = DirectoryProviderHelper.GetSourceDirectory(Environment.SourceBase, Environment.Source, directoryProviderName, properties);
			if (source == null)
				throw new ArgumentException("FSSlaveDirectoryProvider requires a viable source directory");
			if (!File.Exists(Path.Combine(source, "current1")) &&
				!File.Exists(Path.Combine(source, "current2")))
			{
				log.Warn("No current marker in source directory: " + source);
			}
			log.Debug("Source directory: " + source);
			DirectoryInfo indexDir = DirectoryProviderHelper.DetermineIndexDir(directoryProviderName, properties);
			log.Debug("Index directory: " + indexDir.FullName);
			String refreshPeriod = (string)(properties[Environment.Refresh] ?? "3600");
			long period = long.Parse(refreshPeriod);
			log.Debug("Refresh period " + period + " seconds");
			period *= 1000; //per second
			try
			{
				bool create = !indexDir.Exists;
				indexName = indexDir.FullName;
				if (create)
				{
					indexDir.Create();
					log.Debug("Initializing index directory " + indexName);
				}

				DirectoryInfo subDir = new DirectoryInfo(Path.Combine(indexName, "1"));
				create = !File.Exists(Path.Combine(subDir.FullName, "segments"));
				directory1 = FSDirectory.GetDirectory(subDir.FullName, create);

				if (create)
				{
					IndexWriter iw1 = new IndexWriter(directory1, new StandardAnalyzer(), create);
					iw1.Close();
				}

				subDir = new DirectoryInfo(Path.Combine(indexName, "2"));
				create = !File.Exists(Path.Combine(subDir.FullName, "segments"));
				directory2 = FSDirectory.GetDirectory(subDir.FullName, create);

				if (create)
				{
					IndexWriter iw2 = new IndexWriter(directory2, new StandardAnalyzer(), create);
					iw2.Close();
				}

				string current1Master = Path.Combine(indexName, "current1");
				string current2Master = Path.Combine(indexName, "current2");
				if (File.Exists(current1Master))
				{
					current = 1;
				}
				else if (File.Exists(current2Master))
				{
					current = 2;
				}
				else
				{
					//no default
					log.Debug("Setting directory 1 as current");
					current = 1;
					DirectoryInfo srcDir = new DirectoryInfo(source);
					DirectoryInfo destDir = new DirectoryInfo(Path.Combine(indexName, current.ToString()));
					int sourceCurrent = -1;
					if (File.Exists(Path.Combine(srcDir.Name, "current1")))
					{
						sourceCurrent = 1;
					}
					else if (File.Exists(Path.Combine(srcDir.Name, "current2")))
					{
						sourceCurrent = 2;
					}

					if (sourceCurrent != -1)
					{
						try
						{
							FileHelper.Synchronize(new DirectoryInfo(Path.Combine(source, sourceCurrent.ToString())),
												   destDir, true);
						}
						catch (IOException e)
						{
							throw new HibernateException("Umable to synchonize directory: " + indexName, e);
						}
					}
					try
					{
						File.Create(current1Master).Dispose();
					}
					catch (IOException e)
					{
						throw new HibernateException("Unable to create the directory marker file: " + indexName, e);
					}
				}
				log.Debug("Current directory: " + current);
			}
			catch (IOException e)
			{
				throw new HibernateException("Unable to initialize index: " + directoryProviderName, e);
			}
			searchFactory.RegisterDirectoryProviderForLocks(this);
			timer = new Timer(
				new CopyDirectory(this, source, indexName).Run
				);
			timer.Change(period, period);
		}

		public Directory Directory
		{
			get
			{
				if (current == 1)
				{
					return directory1;
				}
				else if (current == 2)
				{
					return directory2;
				}
				else
				{
					throw new AssertionFailure("Illegal current directory: " + current);
				}
			}
		}

		public override bool Equals(Object obj)
		{
			// this code is actually broken since the value change after initialize call
			// but from a practical POV this is fine since we only call this method
			// after initialize call
			if (obj == this) return true;
			if (obj == null || !(obj is FSSlaveDirectoryProvider)) return false;
			return indexName.Equals(((FSSlaveDirectoryProvider)obj).indexName);
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
			private readonly FSSlaveDirectoryProvider parent;
			private String source;
			private String destination;
			private volatile bool inProgress;

			public CopyDirectory(FSSlaveDirectoryProvider parent, String source, String destination)
			{
				this.parent = parent;
				this.source = source;
				this.destination = destination;
			}

			[System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.Synchronized)]
			public void Run(object ignoed)
			{
				DateTime start = DateTime.Now;
				try
				{
					inProgress = true;
					int oldIndex = parent.current;
					int index = parent.current == 1 ? 2 : 1;
					DirectoryInfo sourceFile;
					string current1Slave = Path.Combine(source, "current1");
					string current2Slave = Path.Combine(source, "current2");
					if (File.Exists(current1Slave))
					{
						sourceFile = new DirectoryInfo(Path.Combine(source, "1"));
					}
					else if (File.Exists(current2Slave))
					{
						sourceFile = new DirectoryInfo(Path.Combine(source, "2"));
					}
					else
					{
						log.Warn("Unable to determine current in source directory");
						inProgress = false;
						return;
					}

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
						log.Error("Unable to synchronize " + parent.indexName, e);
						inProgress = false;
						return;
					}
					try
					{
						File.Delete(Path.Combine(parent.indexName, "current" + oldIndex));
					}
					catch (Exception e)
					{
						log.Warn("Unable to remove previous marker file in " + parent.indexName, e);
					}
					try
					{
						File.Create(Path.Combine(parent.indexName, "current" + index)).Dispose();
					}
					catch (IOException e)
					{
						log.Warn("Unable to create current marker file in " + parent.indexName, e);
					}
				}
				finally
				{
					inProgress = false;
				}
				log.Info("Copy for " + parent.indexName + " took " + (DateTime.Now - start) + ".");
			}
		}
	}
}