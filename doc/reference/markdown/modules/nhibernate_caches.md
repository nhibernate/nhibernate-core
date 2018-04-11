# NHibernate.Caches

> **NHibernate.Caches namespace contains several second-level cache
> providers for NHibernate..**
> 
> A cache is a place where entities are kept after being loaded from the
> database; once cached, they can be retrieved without going to the
> database. This means that they are faster to (re)load.
> 
> An NHibernate session has an internal (first-level) cache where it
> keeps its entities. There is no sharing between these caches - a
> first-level cache belongs to a given session and is destroyed with it.
> NHibernate provides a *second-level cache* system; it works at the
> session factory level. A second-level cache is shared by all sessions
> created by the same session factory.
> 
> An important point is that the second-level cache *does not* cache
> instances of the object type being cached; instead it caches the
> individual values of the properties of that object. This provides two
> benefits. One, NHibernate doesn't have to worry that your client code
> will manipulate the objects in a way that will disrupt the cache. Two,
> the relationships and associations do not become stale, and are easy
> to keep up-to-date because they are simply identifiers. The cache is
> not a tree of objects but rather a map of arrays.
> 
> With the *session-per-request* model, a high number of sessions can
> concurrently access the same entity without hitting the database each
> time; hence the performance gain.
> 
> Depending on the chosen cache provider, the second level cache may be
> actually shared between different session factories. If you need to
> avoid this for some session factories, configure each of them with a
> different `cache.region_prefix`. See [???](#configuration-optional).
> 
> Several cache providers have been contributed by NHibernate users:
> 
>   - `NHibernate.Caches.Prevalence`  
>     Uses `Bamboo.Prevalence` as the cache provider. Open the file
>     `Bamboo.Prevalence.license.txt` for more information about its
>     license; you can also visit its
>     [website](http://bbooprevalence.sourceforge.net/). This provider
>     is available for the .Net Framework only. Also see
>     [section\_title](#NHibernate.Caches.Prevalence).
> 
>   - `NHibernate.Caches.SysCache`  
>     Uses `System.Web.Caching.Cache` as the cache provider. This means
>     that you can rely on ASP.NET caching feature to understand how it
>     works. For more information, read (on the MSDN): [Caching
>     Application
>     Data](https://msdn.microsoft.com/en-us/library/6hbbsfk6.aspx).
>     This provider is available for the .Net Framework only. Also see
>     [section\_title](#NHibernate.Caches.SysCache).
> 
>   - `NHibernate.Caches.SysCache2`  
>     Similar to `NHibernate.Caches.SysCache`, uses ASP.NET cache. This
>     provider also supports SQL dependency-based expiration, meaning
>     that it is possible to configure certain cache regions to
>     automatically expire when the relevant data in the database
>     changes.
>     
>     SysCache2 requires Microsoft SQL Server 2000 or higher. This
>     provider is available for the .Net Framework only.
>     
>     See [section\_title](#NHibernate.Caches.SysCache2).
> 
>   - `NHibernate.Caches.EnyimMemcached`  
>     Uses `Memcached`. See [memcached homepage](https://memcached.org/)
>     for more information on Memcached. This provider is available for
>     the .Net Framework only. Also see
>     [section\_title](#NHibernate.Caches.EnyimMemcached).
> 
>   - `NCache provider for NHibernate`  
>     Uses `NCache`. NCache is a commercial distributed caching system
>     with a provider for NHibernate. The NCache Express version is free
>     for use, see [NCache Express
>     homepage](http://www.alachisoft.com/ncache/) for more information.
> 
>   - `NHibernate.Caches.RtMemoryCache`  
>     Uses `System.Runtime.Caching.MemoryCache.Default` as the cache
>     provider. This provider is available for the .Net Framework only.
>     See [section\_title](#NHibernate.Caches.RtMemoryCache).
> 
>   - `NHibernate.Caches.CoreMemoryCache`  
>     Uses `Microsoft.Extensions.Caching.Memory.MemoryCache` as the
>     cache provider. This provider is available as a .Net Standard
>     NuGet package. See
>     [section\_title](#NHibernate.Caches.CoreMemoryCache).
> 
>   - `NHibernate.Caches.CoreDistributedCache`  
>     Uses `Microsoft.Extensions.Caching.Abstractions.IDistributedCache`
>     implementations as the cache provider. The implementation has to
>     be provided through an `IDistributedCacheFactory`. Distributed
>     cache factories for `Memcached`, `Redis`, `SqlServer` and `Memory`
>     caches are available through their own package, prefixed by
>     `NHibernate.Caches.CoreDistributedCache.`.
>     
>     This provider is available as a .Net Standard NuGet package. See
>     [section\_title](#NHibernate.Caches.CoreDistributedCache).

# How to use a cache?

Here are the steps to follow to enable the second-level cache in
NHibernate:

  - Choose the cache provider you want to use and copy its assembly in
    your assemblies directory. (For example,
    `NHibernate.Caches.Prevalence.dll` or
    `NHibernate.Caches.SysCache.dll`.)

  - To tell NHibernate which cache provider to use, add in your
    NHibernate configuration file (can be `YourAssembly.exe.config` or
    `web.config` or a `.cfg.xml` file):
    
    ``` 
    <property name="cache.provider_class">XXX</property>
    <property name="cache.default_expiration">120</property>
    <property name="cache.use_sliding_expiration">true</property>
                            
    ```
    
      - "`XXX`" is the assembly-qualified class name of a class
        implementing `ICacheProvider`, eg.
        "`NHibernate.Caches.SysCache.SysCacheProvider,
                                                                                                                                                                                                                                                                                                        NHibernate.Caches.SysCache`".
    
      - The `expiration` value is the number of seconds you wish to
        cache each entry (here two minutes). Not all providers support
        this setting, it may be ignored. Check their respective
        documentation.
    
      - The `use_sliding_expiration` value is whether you wish to use a
        sliding expiration or not. Defaults to `false`. Not all
        providers support this setting, it may be ignored. Check their
        respective documentation.

  - Add `<cache usage="read-write|nonstrict-read-write|read-only"/>`
    (just after `<class>`) in the mapping of the entities you want to
    cache. It also works for collections (bag, list, map, set, ...).

**Be careful.**

  - Most caches are never aware of changes made to the persistent store
    by another process (though they may be configured to regularly
    expire cached data). As the caches are created at the session
    factory level, they are destroyed with the SessionFactory instance;
    so you must keep them alive as long as you need them.

  - The second level cache requires the use of transactions, be it
    through transaction scopes or NHibernate transactions. Interacting
    with the data store without an explicit transaction is discouraged,
    and will not allow the second level cache to work as intended.

  - To avoid issues with composite ids and some cache providers,
    `ToString()` needs to be overridden on composite id classes. It
    should yield an unique string representing the id. If the composite
    id is mapped as a component, overriding the component `ToString()`
    is enough. See [???](#components-compositeid).

See also [???](#performance-cache).

# Prevalence Cache Configuration

There is only one configurable parameter: `prevalenceBase`. This is the
directory on the file system where the Prevalence engine will save data.
It can be relative to the current directory or a full path. If the
directory doesn't exist, it will be created.

The `prevalenceBase` setting can only be set programmatically through on
the NHibernate configuration object, by example with
`Configuration.SetProperty`.

# SysCache Configuration

SysCache relies on `System.Web.Caching.Cache` for the underlying
implementation. The following NHibernate configuration settings are
available:

  - `cache.default_expiration`  
    Number of seconds to wait before expiring each item. Defaults to
    300
    . It can also be set programmatically on the NHibernate
    configuration object under the name
    expiration
    , which then takes precedence over
    cache.default\_expiration
    .
  - `cache.use_sliding_expiration`  
    Should the expiration be sliding? A sliding expiration is
    reinitialized at each get. Defaults to
    false
    .
  - `priority`  
    A numeric cost of expiring each item, where 1 is a low cost, 5 is
    the highest, and 3 is normal. Only values 1 through 6 are valid. 6
    is a special value corresponding to
    NotRemovable
    . This setting can only be set programmatically through on the
    NHibernate configuration object, by example with
    Configuration.SetProperty
    .

SysCache has a config file section handler to allow configuring
different expirations and priorities for different regions. Here is an
example:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
        <section name="syscache"
          type="NHibernate.Caches.SysCache.SysCacheSectionHandler,NHibernate.Caches.SysCache" />
      </configSections>
    
      <syscache>
        <cache region="foo" expiration="500" priority="4" />
        <cache region="bar" expiration="300" priority="3" sliding="true" />
      </syscache>
    </configuration>

# SysCache2 Configuration

SysCache2 can use SqlCacheDependencies to invalidate cache regions when
data in an underlying SQL Server table or query changes. Query
dependencies are only available for SQL Server 2005 or higher. To use
the cache provider, the application must be setup and configured to
support SQL notifications as described in the MSDN documentation.

The following NHibernate configuration settings are available:

  - `cache.default_expiration`  
    Number of seconds to wait before expiring each item. Defaults to
    300
    . It can also be set programmatically on the NHibernate
    configuration object under the name
    expiration
    , which then takes precedence over
    cache.default\_expiration
    .
  - `cache.use_sliding_expiration`  
    Should the expiration be sliding? A sliding expiration is
    reinitialized at each get. Defaults to
    false
    .

To configure cache regions with SqlCacheDependencies a `syscache2`
config section must be defined in the application's configuration file.
See the sample below.

    <configSections>
      <section name="syscache2"
        type="NHibernate.Caches.SysCache2.SysCacheSection, NHibernate.Caches.SysCache2"/>
    </configSections>

## Table-based Dependency

A table-based dependency will monitor the data in a database table for
changes. Table-based dependencies are generally used for a SQL Server
2000 database but will work with SQL Server 2005 or superior as well.
Before you can use SQL Server cache invalidation with table based
dependencies, you need to enable notifications for the database. This
task is performed with the `aspnet_regsql` command. With table-based
notifications, the application will poll the database for changes at a
predefined interval. A cache region will not be invalidated immediately
when data in the table changes. The cache will be invalidated the next
time the application polls the database for changes.

To configure the data in a cache region to be invalidated when data in
an underlying table is changed, a cache region must be configured in the
application's configuration file. See the sample below.

    <syscache2>
      <cacheRegion name="Product">
        <dependencies>
          <tables>
            <add name="price"
              databaseEntryName="Default"
              tableName="VideoTitle" />
          </tables>
        </dependencies>
      </cacheRegion>
    </syscache2>

  - `name`  
    Unique name for the dependency
  - `tableName`  
    The name of the database table that the dependency is associated
    with. The table must be enabled for notification support with the
    AspNet\_SqlCacheRegisterTableStoredProcedure
    .
  - `databaseEntryName`  
    The name of a database defined in the
    databases
    element for
    sqlCacheDependency
    for caching (ASP.NET Settings Schema) element of the application's
    Web.config
    file.

## Command-Based Dependencies

A command-based dependency will use a SQL command to identify records to
monitor for data changes. Command-based dependencies work only with SQL
Server 2005.

Before you can use SQL Server cache invalidation with command-based
dependencies, you need to enable the Service Broker for query
notifications. The application must also start the listener for
receiving change notifications from SQL Server. With command based
notifications, SQL Server will notify the application when the data of a
record returned in the results of a SQL query has changed. Note that a
change will be indicated if the data in any of the columns of a record
change, not just the columns returned by a query. The query is a way to
limit the number of records monitored for changes, not the columns. As
soon as data in one of the records is modified, the data in the cache
region will be invalidated immediately.

To configure the data in a cache region to be invalidated based on a SQL
command, a cache region must be configured in the application's
configuration file. See the samples below.

    <cacheRegion name="Product" priority="High" >
      <dependencies>
        <commands>
          <add name="price"
            command="ActiveProductsStoredProcedure" 
            isStoredProcedure="true"/>
        </commands>
      </dependencies>
    </cacheRegion>

    <cacheRegion name="Product" priority="High">
      <dependencies>
        <commands>
          <add name="price"
            command="Select VideoTitleId from dbo.VideoTitle where Active = 1"
            connectionName="default"
            connectionStringProviderType="NHibernate.Caches.SysCache2.ConfigConnectionStringProvider, NHibernate.Caches.SysCache2"/>
        </commands>
      </dependencies>
    </cacheRegion>

  - `name`  
    Unique name for the dependency
  - `command` (required)  
    SQL command that returns results which should be monitored for data
    changes
  - `isStoredProcedure` (optional)  
    Indicates if command is a stored procedure. The default is
    false
    .
  - `connectionName` (optional)  
    The name of the connection in the applications configuration file to
    use for registering the cache dependency for change notifications.
    If no value is supplied for
    connectionName
    or
    connectionStringProviderType
    , the connection properties from the NHibernate configuration will
    be used.
  - `connectionStringProviderType` (optional)  
    IConnectionStringProvider
    to use for retrieving the connection string to use for registering
    the cache dependency for change notifications. If no value is
    supplied for
    connectionName
    , the unnamed connection supplied by the provider will be used.

## Aggregate Dependencies

Multiple cache dependencies can be specified. If any of the dependencies
triggers a change notification, the data in the cache region will be
invalidated. See the samples below.

    <cacheRegion name="Product">
      <dependencies>
        <commands>
          <add name="price"
            command="ActiveProductsStoredProcedure" 
            isStoredProcedure="true"/>
          <add name="quantity"
            command="Select quantityAvailable from dbo.VideoAvailability"/>
        </commands>
      </dependencies>
    </cacheRegion>

    <cacheRegion name="Product">
      <dependencies>
        <commands>
          <add name="price"
            command="ActiveProductsStoredProcedure" 
            isStoredProcedure="true"/>
        </commands>
        <tables>
          <add name="quantity"
            databaseEntryName="Default"
            tableName=" VideoAvailability" />
        </tables>
      </dependencies>
    </cacheRegion>

## Additional Settings

In addition to data dependencies for the cache regions, time based
expiration policies can be specified for each item added to the cache.
Time based expiration policies will not invalidate the data dependencies
for the whole cache region, but serve as a way to remove items from the
cache after they have been in the cache for a specified amount of time.
See the samples
    below.

    <cacheRegion name="Product" relativeExpiration="300" priority="High" useSlidingExpiration="true" />

    <cacheRegion name="Product" timeOfDayExpiration="2:00:00" priority="High" />

  - `relativeExpiration`  
    Number of seconds that an individual item will exist in the cache
    before being removed. Defaults to `300` if neither
    `relativeExpiration` nor `timeOfDayExpiration` are defined, and if
    no expiration settings are defined in NHibernate configuration.

  - `useSlidingExpiration`  
    Should the expiration be sliding? A sliding expiration is
    reinitialized at each get. Defaults to `false` if not defined in
    NHibernate configuration.

  - `timeOfDayExpiration`  
    24 hour based time of day that an item will exist in the cache
    until. 12am is specified as 00:00:00; 10pm is specified as 22:00:00.
    Only valid if relativeExpiration is not specified. Time of Day
    Expiration is useful for scenarios where items should be expired
    from the cache after a daily process completes.

  - `priority`  
    System.Web.Caching.CacheItemPriority
    
    that identifies the relative priority of items stored in the cache.

# EnyimMemcached Configuration

Its configuration relies on the EnyimMemcached library own
configuration, through its `enyim.com/memcached` configuration section.
See [project site](https://github.com/enyim/EnyimMemcached).

# RtMemoryCache Configuration

RtMemoryCache relies on `System.Runtime.Caching.MemoryCache` for the
underlying implementation. The following NHibernate configuration
settings are available:

  - `cache.default_expiration`  
    Number of seconds to wait before expiring each item. Defaults to
    300
    . It can also be set programmatically on the NHibernate
    configuration object under the name
    expiration
    , which then takes precedence over
    cache.default\_expiration
    .
  - `cache.use_sliding_expiration`  
    Should the expiration be sliding? A sliding expiration is
    reinitialized at each get. Defaults to
    false
    .

RtMemoryCache has a config file section handler to allow configuring
different expirations for different regions. Here is an example:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
        <section name="rtmemorycache"
          type="NHibernate.Caches.RtMemoryCache.RtMemoryCacheSectionHandler,NHibernate.Caches.RtMemoryCache" />
      </configSections>
    
      <rtmemorycache>
        <cache region="foo" expiration="500" />
        <cache region="bar" expiration="300" sliding="true" />
      </rtmemorycache>
    </configuration>

# CoreMemoryCache Configuration

CoreMemoryCache relies on
`Microsoft.Extensions.Caching.Memory.MemoryCache` for the underlying
implementation. The following NHibernate configuration settings are
available:

  - `cache.default_expiration`  
    Number of seconds to wait before expiring each item. Defaults to
    300
    . It can also be set programmatically on the NHibernate
    configuration object under the name
    expiration
    , which then takes precedence over
    cache.default\_expiration
    .
  - `cache.use_sliding_expiration`  
    Should the expiration be sliding? A sliding expiration is
    reinitialized at each get. Defaults to
    false
    .

CoreMemoryCache has a config file section handler to allow configuring
different expirations for different regions, and configuring the
`MemoryCache` expiration scan frequency. Here is an example:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
        <section name="corememorycache"
          type="NHibernate.Caches.CoreMemoryCache.CoreMemoryCacheSectionHandler,NHibernate.Caches.CoreMemoryCache"
        />
      </configSections>
    
      <corememorycache expiration-scan-frequency="00:05:00">
        <cache region="foo" expiration="500" />
        <cache region="bar" expiration="300" sliding="true" />
      </corememorycache>
    </configuration>

# CoreDistributedCache Configuration

CoreDistributedCache relies on
`Microsoft.Extensions.Caching.Abstractions.IDistributedCache`
implementations. The implementation has to be provided through an
`IDistributedCacheFactory`, either supplied through configuration or
programmatically by affecting
`CoreDistributedCacheProvider.CacheFactory` before building a session
factory. The following NHibernate configuration settings are available:

  - `cache.default_expiration`  
    Number of seconds to wait before expiring each item. Defaults to
    300
    . It can also be set programmatically on the NHibernate
    configuration object under the name
    expiration
    , which then takes precedence over
    cache.default\_expiration
    .
  - `cache.use_sliding_expiration`  
    Should the expiration be sliding? A sliding expiration is
    reinitialized at each get. Defaults to
    false
    .

CoreDistributedCache has a config file section handler to allow
configuring different expirations for different regions, configuring the
`IDistributedCacheFactory` to use, and configuring additional properties
specific to the chosen `IDistributedCache` implementation. Here is an
example:

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
        <section name="coredistributedcache"
          type="NHibernate.Caches.CoreDistributedCache.CoreDistributedCacheSectionHandler,
                NHibernate.Caches.CoreDistributedCache" />
      </configSections>
    
      <coredistributedcache
        factory-class="NHibernate.Caches.CoreDistributedCache.Memory.MemoryFactory,
                       NHibernate.Caches.CoreDistributedCache.Memory">
        <properties>
          <property name="expiration-scan-frequency">00:10:00</property>
          <property name="size-limit">1048576</property>
        </properties>
        <cache region="foo" expiration="500" sliding="true" />
        <cache region="noExplicitExpiration" sliding="true" />
      </coredistributedcache>
    </configuration>

CoreDistributedCache does not support `NHibernate.Cache.ICache.Clear`.
Clearing the NHibernate cache has no effects with CoreDistributedCache.

## Memcached distributed cache factory

`NHibernate.Caches.CoreDistributedCache.Memcached` provides a Redis
distributed cache factory. This factory yields a
`Enyim.Caching.MemcachedClient` from `EnyimMemcachedCore`. For using it,
reference the cache factory package and set the `factory-class`
attribute of the `coredistributedcache` configuration section to
`NHibernate.Caches.CoreDistributedCache.Memcached.MemcachedFactory,
NHibernate.Caches.CoreDistributedCache.Memcached`.

Memcached does not support sliding expirations.
`cache.use_sliding_expiration` setting or `sliding` region setting do
not have any effect with Memcached.

The following additional properties can be configured:

  - `configuration`  
    The JSON configuration of
    EnyimMemcachedCore
    , see its
    project website
    . It has to be structured like the value part of the
    "enyimMemcached"
    property in an
    appsettings.json
    file.
        {
          "Servers": [
            {
              "Address": "localhost",
              "Port": 11211
            }
          ]
        }

## Redis distributed cache factory

`NHibernate.Caches.CoreDistributedCache.Redis` provides a Redis
distributed cache factory. This factory yields a
`Microsoft.Extensions.Caching.Redis.RedisCache`. For using it, reference
the cache factory package and set the `factory-class` attribute of the
`coredistributedcache` configuration section to
`NHibernate.Caches.CoreDistributedCache.Redis.RedisFactory,
NHibernate.Caches.CoreDistributedCache.Redis`.

The following additional properties can be configured:

  - `configuration`  
    Its value will be used to set the
    Configuration
    property of the
    RedisCache
    options (
    RedisCacheOptions
    ).
  - `instance-name`  
    Its value will be used to set the
    InstanceName
    property of the
    RedisCache
    options (
    RedisCacheOptions
    ).

## SQL Server distributed cache factory

`NHibernate.Caches.CoreDistributedCache.SqlServer` provides a SQL Server
distributed cache factory. This factory yields a
`Microsoft.Extensions.Caching.SqlServer.SqlServerCache`. For using it,
reference the cache factory package and set the `factory-class`
attribute of the `coredistributedcache` configuration section to
`NHibernate.Caches.CoreDistributedCache.SqlServer.SqlServerFactory,
NHibernate.Caches.CoreDistributedCache.SqlServer`.

The following additional properties can be configured:

  - `connection-string`  
    Its value will be used to set the
    ConnectionString
    property of the
    SqlServerCache
    options (
    SqlServerCacheOptions
    ).
  - `schema-name`  
    Its value will be used to set the
    SchemaName
    property of the
    SqlServerCache
    options (
    SqlServerCacheOptions
    ).
  - `table-name`  
    Its value will be used to set the
    TableName
    property of the
    SqlServerCache
    options (
    SqlServerCacheOptions
    ).
  - `expired-items-deletion-interval`  
    Its value will be used to set the
    ExpiredItemsDeletionInterval
    property of the
    SqlServerCache
    options (
    SqlServerCacheOptions
    ). It can be provided either as an integer being a number of minutes
    or as a
    TimeSpan
    string representation.

## Memory distributed cache factory

`NHibernate.Caches.CoreDistributedCache.Memory` provides a memory
"distributed" cache factory. This factory yields a
`Microsoft.Extensions.Caching.Memory.MemoryDistributedCache`. For using
it, reference the cache factory package and set the `factory-class`
attribute of the `coredistributedcache` configuration section to
`NHibernate.Caches.CoreDistributedCache.Memory.MemoryFactory,
NHibernate.Caches.CoreDistributedCache.Memory`.

As implied by its name, this cache is not actually distributed. It is
meant for testing purpose. For other usages, consider using another
memory cache provider, like `CoreMemoryCache`. Due to the distributed
cache implementation, using the `MemoryDistributedCache` has some
drawbacks compared to most other memory cache providers: it will
serialize cached objects, incurring some overhead; it does not support
clearing the cache. But due to the serialization of cached objects, it
is able of computing its consumed memory size, thus the availability of
the `SizeLimit` option.

The following additional properties can be configured:

  - `expiration-scan-frequency`  
    Its value will be used to set the
    ExpirationScanFrequency
    property of the
    MemoryDistributedCache
    options (
    MemoryDistributedCacheOptions
    ). It can be provided either as an integer being a number of minutes
    or as a
    TimeSpan
    string representation.
  - `size-limit`  
    Its value will be used to set the
    SizeLimit
    property of the
    MemoryDistributedCache
    options (
    MemoryDistributedCacheOptions
    ). Its value is an integer, representing the maximal bytes count to
    be stored in the cache.
