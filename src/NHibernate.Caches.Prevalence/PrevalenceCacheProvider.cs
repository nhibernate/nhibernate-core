using System;
using System.Collections;
using System.IO;
using System.Text;
using Bamboo.Prevalence;
using Bamboo.Prevalence.Util;
using log4net;
using NHibernate.Cache;

namespace NHibernate.Caches.Prevalence
{
	/// <summary>
	/// Cache provider using <a href="http://bbooprevalence.sourceforge.net/">Bamboo Prevalence</a>,
	/// a Prevayler implementation in .NET.
	/// </summary>
	public class PrevalenceCacheProvider : ICacheProvider
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( PrevalenceCacheProvider ) );
		private PrevalenceEngine _engine;
		private CacheSystem _system;
		private SnapshotTaker _taker;
		private string _dataDir;

		/// <summary>
		/// build and return a new cache implementation
		/// </summary>
		/// <param name="regionName"></param>
		/// <param name="properties">cache configuration properties</param>
		/// <remarks>There is only one configurable parameter: prevalenceBase. This is
		/// the directory on the file system where the Prevalence engine will save data.
		/// It can be relative to the current directory or a full path. If the directory
		/// doesn't exist, it will be created.</remarks>
		/// <returns></returns>
		[CLSCompliant(false)]
		public ICache BuildCache( string regionName, IDictionary properties )
		{
			if( regionName == null )
			{
				regionName = "";
			}
			if( properties == null )
			{
				properties = new Hashtable();
			}
			if( log.IsDebugEnabled )
			{
				StringBuilder sb = new StringBuilder();
				foreach( DictionaryEntry de in properties )
				{
					sb.Append( "name=" );
					sb.Append( de.Key.ToString() );
					sb.Append( "&value=" );
					sb.Append( de.Value.ToString() );
					sb.Append( ";" );
				}
				log.Debug( "building cache with region: " + regionName + ", properties: " + sb.ToString() );
			}
			_dataDir = GetDataDirFromConfig( regionName, properties );
			if( _system == null )
			{
				SetupEngine();
			}
			
			return new PrevalenceCache( regionName, _system );
		}

		private void SetupEngine()
		{
			_engine = PrevalenceActivator.CreateTransparentEngine( typeof( CacheSystem ), _dataDir );
			_system = _engine.PrevalentSystem as CacheSystem;
			_taker = new SnapshotTaker( _engine, TimeSpan.FromMinutes( 5 ), CleanUpAllFilesPolicy.Default );
		}

		private string GetDataDirFromConfig( string region, IDictionary properties )
		{
			string dataDir = Path.Combine( Environment.CurrentDirectory, region );

			if( properties != null )
			{
				if( properties["prevalenceBase"] != null )
				{
					string prevalenceBase = properties["prevalenceBase"].ToString();
					if( Path.IsPathRooted( prevalenceBase ) )
					{
						dataDir = prevalenceBase;
					}
					else
					{
						dataDir = Path.Combine( Environment.CurrentDirectory, prevalenceBase );
					}
				}
			}
			if( Directory.Exists( dataDir ) == false )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( String.Format( "Data directory {0} doesn't exist: creating it.", dataDir ) );
				}
				Directory.CreateDirectory( dataDir );
			}
			if( log.IsDebugEnabled )
			{
				log.Debug( String.Format( "configuring cache in {0}.", dataDir ) );
			}
			return dataDir;
		}

		/// <summary></summary>
		/// <returns></returns>
		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <summary></summary>
		/// <param name="properties"></param>
		public void Start( IDictionary properties )
		{
			if( _dataDir == null || _dataDir.Length < 1 )
			{
				_dataDir = GetDataDirFromConfig( "", properties );
			}
			if( _system == null )
			{
				SetupEngine();
			}
		}

		/// <summary></summary>
		public void Stop()
		{
			try
			{
				_engine.HandsOffOutputLog();
				_taker.Dispose();
				if( Directory.Exists( _dataDir ) ) Directory.Delete( _dataDir, true );
			}
			catch
			{
				// not a big deal, probably a permissions issue
			}
		}
	}
}