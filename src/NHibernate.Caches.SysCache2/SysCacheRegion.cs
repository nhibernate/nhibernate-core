#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using NHibernateExtensions.Caches.SysCache;
using log4net;
using NHibernate.Cache;
using Environment = NHibernate.Cfg.Environment;
#endregion

namespace NHibernateExtensions.Caches.SysCache {
	/// <summary>
	/// Pluggable cache implementation using the System.Web.Caching classes
	/// </summary>
	public class SysCacheRegion : ICache {
	
		#region Private Fields
		/// <summary>logger for the type</summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(SysCacheRegion));
		/// <summary>The cache for the web application</summary>
		private System.Web.Caching.Cache _webCache;
		/// <summary>the name of the cache region</summary>
		private string _name;
		/// <summary>The priority of the cache item</summary>
		private CacheItemPriority _priority;
		/// <summary>relative expiration for the cache items</summary>
		private TimeSpan? _relativeExpiration;
		/// <summary>time of day expiration for the cache items</summary>
		private TimeSpan? _timeOfDayExpiration;
		/// <summary>The name of the cache key for the region</summary>
		private string _rootCacheKey;
		/// <summary>Indicates if the root cache item has been stored or not</summary>
		private bool _isRootItemCached;
		/// <summary>The default expiration to use if one is not specified</summary>
		private static readonly TimeSpan _defaultRelativeExpiration = TimeSpan.FromSeconds(300);
		/// <summary>The name of the cache prefix to differntaite the nhibernate cache elements from
		/// other items in the cache</summary>
		private const string CacheKeyPrefix = "NHibernate-Cache:";
		/// <summary>
		/// List of dependencies that need to be enlisted before being hooked to a cache item
		/// </summary>
		private List<ICacheDependencyEnlister> _dependencyEnlisters = new List<ICacheDependencyEnlister>();
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SysCacheRegion"/> class with
		/// the default region name and configuration properties
		/// </summary>
		public SysCacheRegion(): this(null, null, null){}

		/// <summary>
		/// Initializes a new instance of the <see cref="SysCacheRegion"/> class with the default configuration
		/// properties
		/// </summary>
		/// <param name="name">The name of the region</param>
		/// <param name="additionalProperties">additional NHibernate configuration properties</param>
		public SysCacheRegion(string name, IDictionary additionalProperties) : this(name, null, additionalProperties){}

		/// <summary>
		/// Initializes a new instance of the <see cref="SysCacheRegion"/> class.
		/// </summary>
		/// <param name="name">The name of the region</param>
		/// <param name="settings">The configuration settings for the cache region</param>
		/// <param name="additionalProperties">additional NHibernate configuration properties</param>
		public SysCacheRegion(string name, CacheRegionElement settings, IDictionary additionalProperties){
		
			//validate the params
			if(String.IsNullOrEmpty(name)){
				_log.Info("No region name specified for cache region. Using default name of 'nhibernate'");
				name = "nhibernate";
			}
			
			this._webCache = HttpRuntime.Cache;
			this._name = name;
			
			//configure the cache region based on the configured settings and any relevant nhibernate settings
			this.Configure(settings, additionalProperties);

			//creaet the cache key that will be used for the root cache item which all other
			//cache items are dependent on
			this._rootCacheKey = this.GenerateRootCacheKey();
		}
		#endregion
		
		#region Private Methods
		/// <summary>
		/// Configures the cache region from configuration values
		/// </summary>
		/// <param name="settings">Configuration settings for the region</param>
		/// <param name="additionalProperties">The additional properties supplied by NHibernate engine</param>
		private void Configure(CacheRegionElement settings, IDictionary additionalProperties){
			_log.Debug("Configuring cache region");
			
			//these are some default conenction values that can be later used by the data dependencies
			//if no custome settings are specified
			string connectionName = null;
			string connectionString = null;
			
			
			if (additionalProperties != null) {
	
				//pick up connection settings that might be used later for data dependencis if any are specified
				if(additionalProperties.Contains(Environment.ConnectionStringName)){
					connectionName = additionalProperties[Environment.ConnectionStringName].ToString();
				}
				
				if(additionalProperties.Contains(Environment.ConnectionString)){
					connectionString = additionalProperties[Environment.ConnectionString].ToString();
				}
				
			}

			if (settings != null) {
				this._priority = settings.Priority;
				this._timeOfDayExpiration = settings.TimeOfDayExpiration;
				this._relativeExpiration = settings.RelativeExpiration;
				
				if(_log.IsDebugEnabled){
					_log.DebugFormat("using priority: {0}", settings.Priority.ToString("g"));
					
					if(this._relativeExpiration.HasValue){
						_log.DebugFormat("using relative expiration :{0}", this._relativeExpiration); 
					}
					
					if(this._timeOfDayExpiration.HasValue){
						_log.DebugFormat("using time of day expiration : {0}", this._timeOfDayExpiration);
					}
				}

				this.CreateDependencyEnlisters(settings.Dependencies, connectionName, connectionString);
			}else{
				this._priority = CacheItemPriority.Default;
				
				if(_log.IsDebugEnabled){
					_log.DebugFormat("no priority specified using default : {0}", this._priority.ToString("g"));
				}
				
			}
			
			//use the default expiration of no expiration was set
			if (this._relativeExpiration.HasValue == false && this._timeOfDayExpiration.HasValue == false) {
				this._relativeExpiration = SysCacheRegion._defaultRelativeExpiration;
				
				if(_log.IsDebugEnabled){
					_log.DebugFormat("no expiration specified using default : {0}", this._relativeExpiration);	
				}		
			}

			
		}
		/// <summary>
		/// Creates the dependency enlisters for any dependecies that require notification enlistment
		/// </summary>
		/// <param name="dependencyConfig">Settings for the dependencies</param>
		/// <param name="defaultConnectionName">connection name to use when setting up data dependencies if no connection string provider is specified</param>
		/// <param name="defaultConnectionString">default connection string to use for data dependencies if no connection string provider is specified </param>
		private void CreateDependencyEnlisters(CacheDependenciesElement dependencyConfig, string defaultConnectionName, string defaultConnectionString){
			//dont do anything if there is no config
			if(dependencyConfig == null){
				_log.Debug("no data dependencies specified");
				return;
			}
			
			//build the table dependency enlisters
			if(dependencyConfig.TableDependencies.Count > 0){
				foreach(TableCacheDependencyElement tableConfig in dependencyConfig.TableDependencies){
					if(_log.IsDebugEnabled){
						_log.DebugFormat("configuring sql table dependency, '{0}' using table, '{1}', and database entry. '{2}'", 
							tableConfig.Name, tableConfig.TableName, tableConfig.DatabaseEntryName);
					}

					SqlTableCacheDependencyEnlister tableEnlister = 
					new SqlTableCacheDependencyEnlister(tableConfig.TableName, tableConfig.DatabaseEntryName);
					
					this._dependencyEnlisters.Add(tableEnlister);
				}
			}
			
			//build the command dependency enlisters
			if(dependencyConfig.CommandDependencies.Count > 0){
				foreach(CommandCacheDependencyElement commandConfig in dependencyConfig.CommandDependencies){
					
					//construct the correct connection string provider, we will do are best fallback to a connection string provider
					//that will help us find a connection string even if one isnt specified
					
					if (_log.IsDebugEnabled) {
						_log.DebugFormat("configuring sql command dependency, '{0}', using command, '{1}'", 
							commandConfig.Name, commandConfig.Command);
						_log.DebugFormat("command configured as stored procedure: {0}", commandConfig.IsStoredProcedure);
					}
					
					IConnectionStringProvider connectionStringProvider;
					string connectionName = null;
					
					if(commandConfig.ConnectionStringProviderType != null){
						if(_log.IsDebugEnabled){
							_log.DebugFormat("Activating configured connection string provider, '{0}'", commandConfig.ConnectionStringProviderType.ToString());
						}
						
						connectionStringProvider = Activator.CreateInstance(commandConfig.ConnectionStringProviderType) as IConnectionStringProvider;	
						connectionName = commandConfig.ConnectionName;
					}else{
						//no connection string provider specified so use the appropriate default
						//if a connection string was specified and we dont have a specifi name in the cache regions settings
						//then just use the default connection string
						if(String.IsNullOrEmpty(defaultConnectionName)&& String.IsNullOrEmpty(commandConfig.ConnectionName)){
							
							_log.DebugFormat("no connection string provider specified using nhibernate configured connection string");
							
							connectionStringProvider = new StaticConnectionStringProvider(defaultConnectionString);
						
						}else{
							//we dont have any connection strings specified so we must get it from config
							connectionStringProvider = new ConfigConnectionStringProvider();
							
							//tweak the connection name based on whether the region has one specified or not
							if(String.IsNullOrEmpty(commandConfig.ConnectionName) == false){
								connectionName = commandConfig.ConnectionName;
							}else{
								connectionName = defaultConnectionName;
							}
							
							if(_log.IsDebugEnabled){
								_log.DebugFormat("no connection string provider specified, using connection with name : {0}", connectionName);	
							}
						}						
					}
					
					SqlCommandCacheDependencyEnlister commandEnlister = 
						new SqlCommandCacheDependencyEnlister(commandConfig.Command, commandConfig.IsStoredProcedure, 
						connectionName, connectionStringProvider);
					 
					 this._dependencyEnlisters.Add(commandEnlister);

				}
			}
		}
		
		/// <summary>
		/// Gets a valid cache key for the element in the cache with <paramref name="identifier"/>.
		/// </summary>
		/// <param name="identifier">The identifier of a cache element</param>
		/// <returns>Key to use for retrieving an element from the cache</returns>
		private string GetCacheKey(object identifier){
			return String.Concat(CacheKeyPrefix, this._name, ":", identifier.ToString(), "@", identifier.GetHashCode());
		}

		/// <summary>
		/// Generates the root cache key for the cache region
		/// </summary>
		/// <returns>Cache key that can be used for the root cache dependency</returns>
		private string GenerateRootCacheKey(){
			return this.GetCacheKey(Guid.NewGuid());
		}
		
		/// <summary>
		/// Creates the cache item for the cache region which all other cache items in the region
		/// will be dependent upon
		/// </summary>
		/// <remarks>
		///		<para>Specified Region dependencies will be associated to the cache item</para>
		/// </remarks>
		private void CacheRootItem(){
			
			if(_log.IsDebugEnabled){
				_log.DebugFormat("Creating root cache entry for cache region: {0}", this._name); 
			}
			
			//register ant cache dependencies for change notifications
			//and build an aggragate dependency if multiple dependencies exist
			CacheDependency rootCacheDependency = null;
						
			if(this._dependencyEnlisters.Count > 0){
				List<CacheDependency> dependencies = new List<CacheDependency>(this._dependencyEnlisters.Count);
				
				foreach(ICacheDependencyEnlister enlister in this._dependencyEnlisters){
					_log.Debug("Enlisting cache dependency for change notification");
					dependencies.Add(enlister.Enlist());
				}
				
				if(dependencies.Count == 1){
					rootCacheDependency = dependencies[0];
				}else{
					AggregateCacheDependency jointDependency = new AggregateCacheDependency();
					jointDependency.Add(dependencies.ToArray());
					
					rootCacheDependency = jointDependency;
				}
				
				_log.Debug("Attaching cache dependencies to root cache entry. Cache entry will be removed when change is detected.");
			} 
		
			this._webCache.Add(this._rootCacheKey, this._rootCacheKey, 
				rootCacheDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
				this._priority, new CacheItemRemovedCallback(this.RootCacheItemRemovedCallback));
			
			
			//flag the root cache item as beeing cached
			this._isRootItemCached = true;
		}
		
		/// <summary>
		/// Called when the root cache item has been removed from the cache
		/// </summary>
		/// <param name="key">the key of the cache item that wwas removed</param>
		/// <param name="value">the value of the cache item that was removed</param>
		/// <param name="reason">The <see cref="CacheItemRemovedReason"/> for the removal of the 
		///		item from the cache</param>
		///	<remarks>
		///		<para>Since all cache items are dependent on the root cache item, if this method is called, 
		///		all the cache items for this region have also been removed</para>
		///	</remarks>
		private void RootCacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason){
			if(_log.IsDebugEnabled){
				_log.DebugFormat("Cache items for region '{0}' have been removed from the cache for the following reason : {1}", 
					this._name, reason.ToString("g"));
			}
				
			//lets us know that we need to reestablish the root cache item for this region
			this._isRootItemCached = false;
		}		
		
		/// <summary>
		/// Gets the expiration time for a new item added to the cache
		/// </summary>
		/// <returns></returns>
		private DateTime GetCacheItemExpiration(){
			DateTime expiration = Cache.NoAbsoluteExpiration;
			
			//use the relative expiration if one is specified, otherwise use the 
			//time of day expiration if that is specified
			if(this._relativeExpiration.HasValue){
				expiration = DateTime.Now.Add(this._relativeExpiration.Value);
				
			}else if(this._timeOfDayExpiration.HasValue){
			
				//calculate the expiration by starting at 12 am of today
				DateTime timeExpiration = DateTime.Today;
				
				//add a day to the expiration time if the time of day has already passed,
				//this will cause the item to expire tommorrow
				if(DateTime.Now.TimeOfDay > this._timeOfDayExpiration.Value){
					timeExpiration = timeExpiration.AddDays(1);
				}
				
				//adding the specified time of day expiration to the adjusted base date
				//will provide us with the time of day expiration specified
				timeExpiration = timeExpiration.Add(this._timeOfDayExpiration.Value);
				
				expiration = timeExpiration;
			}
			
			return expiration;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Clear the Cache
		/// </summary>
		/// <exception cref="T:NHibernate.Cache.CacheException"></exception>
		public void Clear() {
			//remove the root cache item, this will cause all of the individual items to be removed from the cache
			this._webCache.Remove(this._rootCacheKey);
			this._isRootItemCached = false;
		
			_log.Debug("All items cleared from the cache.");
			
		}

		/// <summary>
		/// Clean up.
		/// </summary>
		/// <exception cref="T:NHibernate.Cache.CacheException"></exception>
		public void Destroy() {
			this.Clear();
		}

		/// <summary>
		/// Get the object from the Cache
		/// </summary>
		/// <param name="key">the id of the item to get from the cache</param>
		/// <returns>the item stored in the cache with the id specified by <paramref name="key"/></returns>
		public object Get(object key) {
			if(key == null || this._isRootItemCached == false){
				return null;
			}
			
			//get the full key to use to locate the item in the cache
			string cacheKey = this.GetCacheKey(key);
			
			if(_log.IsDebugEnabled){
				_log.DebugFormat("Fetching object '{0}' from the cache.", cacheKey);
			}
			
			object cachedObject = this._webCache.Get(cacheKey);
			if(cachedObject == null){
				return null;
			}
			
			//casting the object to a dictionary entry so we can verify that the item for the correct key was retrieved
			DictionaryEntry entry = (DictionaryEntry)cachedObject;
			if(key.Equals(entry.Key)){
				return entry.Value;
			}
			
			return null;			
			
		}

		/// <summary>
		/// If this is a clustered cache, lock the item
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to lock.</param>
		/// <exception cref="T:NHibernate.Cache.CacheException"></exception>
		public void Lock(object key) {
			//nothing to do here
		}

		/// <summary>
		/// Generate a timestamp
		/// </summary>
		/// <returns>a timestamp</returns>
		public long NextTimestamp() {
			return Timestamper.Next();
		}

		/// <summary>Puts an item into the cache
		/// </summary>
		/// <param name="key">the key of the item to cache</param>
		/// <param name="value">the actual value/object to cache</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or
		/// <paramref name="value"/> is null.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
		public void Put(object key, object value) {
			//validate the params
			if(key == null){
				throw new ArgumentNullException("key");
			}
			
			if(value == null){
				throw new ArgumentNullException("value");
			}

			//If the root cache item is not cached then we should reestablish it now
			if (this._isRootItemCached == false) {
				_log.DebugFormat("root cache item for region not found.");
			
				this.CacheRootItem();
			}
			
			//get the full key for the cache key
			string cacheKey = this.GetCacheKey(key);
			
			if(this._webCache[cacheKey] != null){
				if(_log.IsDebugEnabled){
					_log.DebugFormat("updating value of key '{0}' to '{1}'.", cacheKey, value.ToString());
				}
				
			}else{
				if(_log.IsDebugEnabled){
					_log.DebugFormat("adding new data: key={0} & value={1}",cacheKey, value.ToString());
				}
			}
			//get the expiration time for the cache item
			DateTime expiration = this.GetCacheItemExpiration();
			
			if(_log.IsDebugEnabled){
				if(expiration.Equals(Cache.NoAbsoluteExpiration) == false){
					_log.DebugFormat("item will expire at: {0}", expiration);
				}
			}
			
			this._webCache.Insert(cacheKey, new DictionaryEntry(key, value),
				new CacheDependency(null, new string[]{this._rootCacheKey}),
				expiration, Cache.NoSlidingExpiration, this._priority, null);
			
		}

		/// <summary>
		/// Gets the name of the cache region
		/// </summary>
		public string RegionName {
			get {
				return this._name;
			}
		}

		/// <summary>
		/// Remove an item from the Cache.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <exception cref="T:NHibernate.Cache.CacheException"></exception>
		/// <exception cref="ArgumentNullException">thrown if <paramref name="key"/> is null.</exception>
		public void Remove(object key) {
			if(key == null){
				throw new ArgumentNullException("key");
			}
			
			//get the full cache key
			string cacheKey = this.GetCacheKey(key);
			
			if(_log.IsDebugEnabled){
				_log.DebugFormat("removing item with key:", cacheKey);
			}
			
			this._webCache.Remove(cacheKey);
		}

		/// <summary>
		/// Get a reasonable "lock timeout"
		/// </summary>
		/// <value></value>
		public int Timeout {
			get {
				return Timestamper.OneMs * 60000;// 60 seconds
			} 
		}

		/// <summary>
		/// If this is a clustered cache, unlock the item
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to unlock.</param>
		/// <exception cref="T:NHibernate.Cache.CacheException"></exception>
		public void Unlock(object key) {
			//nothing to do since we arent locking
		}
		#endregion
	}
		
}
