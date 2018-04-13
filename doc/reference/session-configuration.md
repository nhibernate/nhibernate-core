# ISessionFactory Configuration

Because NHibernate is designed to operate in many different
environments, there are a large number of configuration parameters.
Fortunately, most have sensible default values and NHibernate is
distributed with an example `App.config` file (found in
`src\NHibernate.Test`) that shows the various options. You usually only
have to put that file in your project and customize it.

# Programmatic Configuration <a name="configuration-programmatic"></a>

An instance of `NHibernate.Cfg.Configuration` represents an entire set
of mappings of an application's .NET types to a SQL database. The
`Configuration` is used to build an (immutable) `ISessionFactory`. The
mappings are compiled from various XML mapping files.

You may obtain a `Configuration` instance by instantiating it directly.
Here is an example of setting up a datastore from mappings defined in
two XML configuration files:

```csharp
    Configuration cfg = new Configuration()
        .AddFile("Item.hbm.xml")
        .AddFile("Bid.hbm.xml");
```

An alternative (sometimes better) way is to let NHibernate load a
mapping file from an embedded resource:

```csharp
    Configuration cfg = new Configuration()
        .AddClass(typeof(NHibernate.Auction.Item))
        .AddClass(typeof(NHibernate.Auction.Bid));
```

Then NHibernate will look for mapping files named
`NHibernate.Auction.Item.hbm.xml` and `NHibernate.Auction.Bid.hbm.xml`
embedded as resources in the assembly that the types are contained in.
This approach eliminates any hardcoded filenames.

Another alternative (probably the best) way is to let NHibernate load
all of the mapping files contained in an Assembly:

```csharp
    Configuration cfg = new Configuration()
        .AddAssembly( "NHibernate.Auction" );
```

Then NHibernate will look through the assembly for any resources that
end with `.hbm.xml`. This approach eliminates any hardcoded filenames
and ensures the mapping files in the assembly get added.

If a tool like Visual Studio .NET or NAnt is used to build the assembly,
then make sure that the `.hbm.xml` files are compiled into the assembly
as `Embedded Resources`.

A `Configuration` also specifies various optional properties:

```csharp
    var props = new Dictionary<string, string>();
    ...
    Configuration cfg = new Configuration()
        .AddClass(typeof(NHibernate.Auction.Item))
        .AddClass(typeof(NHibernate.Auction.Bind))
        .SetProperties(props);
```

A `Configuration` is intended as a configuration-time object, to be
discarded once an `ISessionFactory` is built.

# Obtaining an ISessionFactory <a name="configuration-sessionfactory"></a>

When all mappings have been parsed by the `Configuration`, the
application must obtain a factory for `ISession` instances. This factory
is intended to be shared by all application threads:

```csharp
    ISessionFactory sessions = cfg.BuildSessionFactory();
```

However, NHibernate does allow your application to instantiate more than
one `ISessionFactory`. This is useful if you are using more than one
database.

# User provided `ADO.NET` connection <a name="configuration-userjdbc"></a>

An `ISessionFactory` may open an `ISession` on a user-provided `ADO.NET` 
connection. This design choice frees the application to obtain `ADO.NET` 
connections wherever it pleases:

```csharp
    var conn = myApp.GetOpenConnection();
    var session = sessions.OpenSession(conn);
    
    // do some data access work
```

The application must be careful not to open two concurrent `ISession`s
on the same `ADO.NET` connection\!

# NHibernate provided `ADO.NET` connection <a name="configuration-hibernatejdbc"></a>

Alternatively, you can have the `ISessionFactory` open connections for
you. The `ISessionFactory` must be provided with `ADO.NET` connection
properties in one of the following ways:

1.  Pass an instance of `IDictionary` mapping property names to property
    values to `Configuration.SetProperties()`.

2.  Include `<property>` elements in a configuration section in the
    application configuration file. The section should be named
    `hibernate-configuration` and its handler set to
    `NHibernate.Cfg.ConfigurationSectionHandler`. The XML namespace of
    the section should be set to `urn:nhibernate-configuration-2.2`.

3.  Include `<property>` elements in `hibernate.cfg.xml` (discussed
    later).

If you take this approach, opening an `ISession` is as simple as:

```csharp
    ISession session = sessions.OpenSession(); // open a new Session
    // do some data access work, an ADO.NET connection will be used on demand
```

All NHibernate property names and semantics are defined on the class
`NHibernate.Cfg.Environment`. We will now describe the most important
settings for `ADO.NET` connection configuration.

NHibernate will obtain (and pool) connections using an `ADO.NET` data
provider if you set the following properties:

| Property name                     | Purpose                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |         |
|-----------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------|
| connection.provider               | The type of a custom `IConnectionProvider` implementation. eg. `full.classname.of.ConnectionProvider` if the Provider is built into NHibernate, or `full.classname.of.ConnectionProvider`, assembly if using an implementation of `IConnectionProvider` not included in NHibernate. The default is `NHibernate.Connection.DriverConnectionProvider.`                                                                                                                         |         |
| connection.driver_class           | The type of a custom `IDriver`, if using `DriverConnectionProvider`, eg.` full.classname.of.Driver` if the Driver is built into NHibernate, or `full.classname.of.Driver`, assembly if using an implementation of `IDriver` not included in NHibernate. This is usually not needed, most of the time the dialect will take care of setting the `IDriver` using a sensible default. See the API documentation of the specific dialect for the defaults.                       |         |
| connection.connection_string      | Connection string to use to obtain the connection.                                                                                                                                                                                                                                                                                                                                                                                                                           |         |
| connection.connection_string_name | The name of the connection string (defined in `<connectionStrings>` configuration file element) to use to obtain the connection.                                                                                                                                                                                                                                                                                                                                             |         |
| connection.isolation              | Set the `ADO.NET` transaction isolation level. Check `System.Data.IsolationLevel` for meaningful values and the database's documentation to ensure that level is supported. eg. `Chaos`, `ReadCommitted`,` ReadUncommitted`, `RepeatableRead`, `Serializable`, `Unspecified`                                                                                                                                                                                                 |         |
| connection.release_mode           | Specify when NHibernate should release `ADO.NET` connections. See [Connection Release Modes](transactions.md#connection-release-modes). eg. `auto` (default), `on_close`, `after_transaction`. Note that for `ISession`s obtained through `ISessionFactory.GetCurrentSession`, the `ICurrentSessionContext` implementation configured for use may control the connection release mode for those `ISession`s. See [Contextual Sessions](architecture.md#contextual-sessions). |         |
| prepare_sql                       | Specify to prepare `DbCommand`s generated by NHibernate. Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                            |         |
| command_timeout                   | Specify the default timeout in seconds of `DbCommand`s generated by NHibernate. Negative values disable it. eg. `30`                                                                                                                                                                                                                                                                                                                                                         |         |
| adonet.batch_size                 | Specify the batch size to use when batching update statements. Setting this to `0` (the default) disables the functionality. See [Batch updates](performance.md#batch-updates). eg. `20`                                                                                                                                                                                                                                                                                     |         |
| order_inserts                     | Enable ordering of insert statements for the purpose of more efficient batching. Defaults to `true` if batching is enabled, `false` otherwise. eg. `true`                                                                                                                                                                                                                                                                                                                    | `false` |
| order_updates                     | Enable ordering of update statements for the purpose of more efficient batching. Defaults to `true` if batching is enabled, `false` otherwise. eg. `true`                                                                                                                                                                                                                                                                                                                    | `false` |
| adonet.batch_versioned_data       | If batching is enabled, specify that versioned data can also be batched. Requires a dialect which batcher correctly returns rows count. Defaults to `false`. eg. `true`                                                                                                                                                                                                                                                                                                      | `false` |
| adonet.factory_class              | The class name of a `IBatcherFactory` implementation. This is usually not needed, most of the time the `driver` will take care of setting the `IBatcherFactory` using a sensible default according to the database capabilities. eg. `classname.of.BatcherFactory, assembly`                                                                                                                                                                                                 |         |
| adonet.wrap_result_sets           | Some database vendor data reader implementation have inefficient columnName-to-columnIndex resolution. Enabling this setting allows to wrap them in a data reader that will cache those resolutions. Defaults to `false`. eg. `true`                                                                                                                                                                                                                                         | `false` |

This is an example of how to specify the database connection properties
inside a `web.config`:

```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <configSections>
        <section name="hibernate-configuration"
            type="NHibernate.Cfg.ConfigurationSectionHandler, NHibernate" />
      </configSections>
    
      <hibernate-configuration xmlns="urn:nhibernate-configuration-2.2">
        <session-factory>
          <property name="dialect">NHibernate.Dialect.MsSql2012Dialect</property>
          <property name="connection.connection_string">
            Server=(local);initial catalog=theDb;Integrated Security=SSPI
          </property>
          <property name="connection.isolation">ReadCommitted</property>
        </session-factory>
      </hibernate-configuration>
    
      <!-- other app specific config follows -->
    </configuration>
```

NHibernate relies on the `ADO.NET` data provider implementation of
connection pooling.

You may define your own plug-in strategy for obtaining `ADO.NET` 
connections by implementing the interface
`NHibernate.Connection.IConnectionProvider`. You may select a custom
implementation by setting `connection.provider`.

# Optional configuration properties <a name="configuration-optional"></a>

There are a number of other properties that control the behaviour of
NHibernate at runtime. All are optional and have reasonable default
values.

Some properties are system-level properties. They can only be set
manually by setting static properties of `NHibernate.Cfg.Environment`
class or be defined in the `<hibernate-configuration>` section of the
application configuration file. These properties cannot be set using
`Configuration.SetProperties` or the `hibernate.cfg.xml` configuration
file.

| Property name                                | Purpose                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
|----------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| dialect                                      | The class name of a NHibernate `Dialect` - enables certain platform dependent features. See SQL Dialects. eg. `full.classname.of.Dialect, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| default_catalog                              | Qualify unqualified table names with the given catalog name in generated SQL. eg. `CATALOG_NAME`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| default_schema                               | Qualify unqualified table names with the given schema/table-space in generated SQL. eg. `SCHEMA_NAME`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| max_fetch_depth                              | Set a maximum "depth" for the outer join fetch tree for single-ended associations (one-to-one, many-to-one). A `0` disables default outer join fetching. eg. recommended values between `0` and `3`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| use_reflection_optimizer                     | Enables use of a runtime-generated class to set or get properties of an entity or component instead of using runtime reflection. This is a system-level property. The use of the reflection optimizer inflicts a certain startup cost on the application but should lead to better performance in the long run. Defaults to `true`. You can not set this property in `hibernate.cfg.xml`, but only in `<hibernate-configuration>` section of the application configuration file or by code by setting `NHibernate.Cfg.Environment.UseReflectionOptimizer` before creating any `NHibernate.Cfg.Configuration` instance. eg. `true`, `false`                                                                                                                                                  |
| bytecode.provider                            | Specifies the bytecode provider to use to optimize the use of reflection in NHibernate. This is a system-level property. Use `null` to disable the optimization completely, `lcg` to use built-in lightweight code generation, or the class name of a custom `IBytecodeProvider` implementation. Defaults to `lcg`. You can not set this property in `hibernate.cfg.xml`, but only in `<hibernate-configuration>` section of the application configuration file or by code by setting `NHibernate.Cfg.Environment.BytecodeProvider` before creating any `NHibernate.Cfg.Configuration` instance. eg. `null`, `lcg`, `classname.of.BytecodeProvider, assembly`                                                                                                                               |
| cache.use_second_level_cache                 | Enable the second level cache. Requires specifying a `cache.provider_class`. See [NHibernate.Caches](caches.md). Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| cache.provider_class                         | The class name of a `ICacheProvider` implementation. eg. `classname.of.CacheProvider, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| cache.use_minimal_puts                       | Optimize second-level cache operation to minimize writes, at the cost of more frequent reads (useful for clustered caches). Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| cache.use_query_cache                        | Enable the query cache, individual queries still have to be set cacheable. Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| cache.query_cache_factory                    | The class name of a custom `IQueryCacheFactory` implementation. Defaults to the built-in `StandardQueryCacheFactory`. eg. `classname.of.QueryCacheFactory, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| cache.region_prefix                          | A prefix to use for second-level cache region names. eg. `prefix`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| cache.default_expiration                     | The default expiration delay in seconds for cached entries, for providers supporting this setting. eg. `300`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| query.substitutions                          | Mapping from tokens in NHibernate queries to SQL tokens (tokens might be function or literal names, for example). eg. `hqlLiteral=SQL_LITERAL`, `hqlFunction=SQLFUNC`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| query.default_cast_length                    | Set the default length used in casting when the target type is length bound and does not specify it. Defaults to `4000`, automatically trimmed down according to dialect type registration. eg. `255`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| query.default_cast_precision                 | Set the default precision used in casting when the target type is `decimal` and does not specify it. Defaults to `28`, automatically trimmed down according to dialect type registration. eg. `19`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| query.default_cast_scale                     | Set the default scale used in casting when the target type is `decimal` and does not specify it. Defaults to `10`, automatically trimmed down according to dialect type registration. eg. `5`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| query.startup_check                          | Should named queries be checked during startup (the default is enabled). eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| query.factory_class                          | The class name of a custom `IQueryTranslatorFactory` implementation (HQL query parser factory). Defaults to the built-in `ASTQueryTranslatorFactory`. eg. `classname.of.QueryTranslatorFactory, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| query.linq_provider_class                    | The class name of a custom `INhQueryProvider` implementation (LINQ provider). Defaults to the built-in `DefaultQueryProvider`. eg. `classname.of.LinqProvider, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| query.query_model_rewriter_factory           | The class name of a custom `IQueryModelRewriterFactory` implementation (LINQ query model rewriter factory). Defaults to `null` (no rewriter). eg. `classname.of.QueryModelRewriterFactory, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| linqtohql.generatorsregistry                 | The class name of a custom `ILinqToHqlGeneratorsRegistry` implementation. Defaults to the built-in `DefaultLinqToHqlGeneratorsRegistry`. See [Adding a custom generator](querysql.md#adding-a-custom-generator). eg. `classname.of.LinqToHqlGeneratorsRegistry, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| sql_exception_converter                      | The class name of a custom `ISQLExceptionConverter` implementation. Defaults to `Dialect.BuildSQLExceptionConverter()`. eg. `classname.of.SQLExceptionConverter, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| show_sql                                     | Write all SQL statements to console. Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| format_sql                                   | Log formatted SQL. Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| use_sql_comments                             | Generate SQL with comments. Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| hbm2ddl.auto                                 | Automatically export schema DDL to the database when the `ISessionFactory` is created. With `create-drop`, the database schema will be dropped when the `ISessionFactory` is closed explicitly. eg. `create`, `create-drop`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| hbm2ddl.keywords                             | Automatically import reserved/keywords from the database when the `ISessionFactory` is created.<br>`none` : disable any operation regarding RDBMS KeyWords (the default). <br>`keywords` : imports all RDBMS keywords where the `Dialect` can provide the implementation of `IDataBaseSchema`. <br>`auto-quote` : imports all RDBMS keywords and auto-quote all table-names/column-names. eg. `none`, `keywords`, `auto-quote`                                                                                                                                                                                                                                                                                                                                                              |
| use_proxy_validator                          | Enables or disables validation of interfaces or classes specified as proxies. Enabled by default. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| proxyfactory.factory_class                   | The class name of a custom `IProxyFactoryFactory` implementation. Defaults to the built-in `DefaultProxyFactoryFactory`. eg. `classname.of.ProxyFactoryFactory, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| collectiontype.factory_class                 | The class name of a custom `ICollectionTypeFactory` implementation. Defaults to the built-in `DefaultCollectionTypeFactory`. eg. `classname.of.CollectionTypeFactory, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| transaction.factory_class                    | The class name of a custom `ITransactionFactory` implementation. Defaults to the built-in `AdoNetWithSystemTransactionFactory`. eg. `classname.of.TransactionFactory, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| transaction.use_connection_on_system_prepare | When a system transaction is being prepared, is using connection during this process enabled? Default is `true`, for supporting `FlushMode.Commit` with transaction factories supporting system transactions. But this requires enlisting additional connections, retaining disposed sessions and their connections until transaction end, and may trigger undesired transaction promotions to distributed. Set to `false` for disabling using connections from system transaction preparation, while still benefiting from `FlushMode.Auto` on querying. See [Transaction scopes (System.Transactions)](transactions.md#transaction-scopes-systemtransactions). eg. `true`, `false`                                                                                                        |
| transaction.system_completion_lock_timeout   | Timeout duration in milliseconds for the system transaction completion lock. When a system transaction completes, it may have its completion events running on concurrent threads, after scope disposal. This occurs when the transaction is distributed. This notably concerns `ISessionImplementor.AfterTransactionCompletion(bool, ITransaction)`. NHibernate protects the session from being concurrently used by the code following the scope disposal with a lock. To prevent any application freeze, this lock has a default timeout of five seconds. If the application appears to require longer (!) running transaction completion events, this setting allows to raise this timeout. `-1` disables the timeout. eg. `10000`                                                      |
| default_flush_mode                           | The default `FlushMode`, `Auto` when not specified. See [Flush](manipulatingdata.md#flush). eg. `Manual`, `Commit`, `Auto`, `Always`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| default_batch_fetch_size                     | The default batch fetch size to use when lazily loading an entity or collection. Defaults to `1`. See [Using batch fetching](performance.md#using-batch-fetching) eg. `20`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| current_session_context_class                | The class name of an `ICurrentSessionContext` implementation. See [Contextual Sessions](architecture.md#contextual-sessions). eg. `classname.of.CurrentSessionContext, assembly`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| id.optimizer.pooled.prefer_lo                | When using an enhanced id generator and pooled optimizers (see [Enhanced identifier generators](mapping.md#enhanced-identifier-generators)), prefer interpreting the database value as the lower (lo) boundary. The default is to interpret it as the high boundary. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
| generate_statistics                          | Enable statistics collection within `ISessionFactory.Statistics` property. Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| track_session_id                             | Set whether the session id should be tracked in logs or not. When `true`, each session will have an unique `Guid` that can be retrieved with `ISessionImplementor.SessionId`, otherwise `ISessionImplementor.SessionId` will be `Guid.Empty`. Session id is used for logging purpose and can also be retrieved on the static property `NHibernate.Impl.SessionIdLoggingContext.SessionId`, when tracking is enabled. Disabling tracking by setting `track_session_id` to `false` increases performances. Default is `true`. eg. `true`, `false`                                                                                                                                                                                                                                             |
| sql_types.keep_datetime                      | Since NHibernate v5.0 and if the dialect supports it, `DbType.DateTime2` is used instead of `DbType.DateTime`. This may be disabled by setting `sql_types.keep_datetime` to `true`. Defaults to `false`. eg. `true`, `false`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| oracle.use_n_prefixed_types_for_unicode      | Oracle has a dual Unicode support model. Either the whole database use an Unicode encoding, and then all string types will be Unicode. In such case, Unicode strings should be mapped to non N prefixed types, such as `Varchar2`. This is the default. Or N prefixed types such as `NVarchar2` are to be used for Unicode strings, the others type are using a non Unicode encoding. In such case this setting needs to be set to `true`. See [Implementing a Unicode Solution in the Database](https://docs.oracle.com/cd/B19306_01/server.102/b14225/ch6unicode.htm#CACHCAHF). This setting applies only to Oracle dialects and ODP.Net managed or unmanaged driver. eg. `true`, `false`                                                                                                 |
| odbc.explicit_datetime_scale                 | This may need to be set to 3 if you are using the OdbcDriver with MS SQL Server 2008+. This is intended to work around issues like: <br>`System.Data.Odbc.OdbcException :`<br>`ERROR [22008]`<br>`[Microsoft][SQL Server Native Client 11.0]`<br>`Datetime field overflow. Fractional second precision exceeds the scale specified in the parameter binding.` eg. `3`                                                                                                                                                                                                                                                                                                                                                                                                                       |
| nhibernate-logger                            | The class name of an `ILoggerFactory` implementation. It allows using another logger than log4net. The default is not defined, which causes NHibernate to search for log4net assembly. If this search succeeds, NHibernate will log with log4net. Otherwise, its internal logging will be disabled. <br>This is a very special system-level property. It can only be set through an `appSetting` named `nhibernate-logger` in the application configuration file. It cannot be set neither with `NHibernate.Cfg.Environment` class, nor be defined in the `<hibernate-configuration>` section of the application configuration file, nor supplied by using `Configuration.SetProperties`, nor set in the `hibernate.cfg.xml` configuration file. eg. `classname.of.LoggerFactory, assembly` |

## SQL Dialects <a name="configuration-optional-dialects"></a>

You should always set the `dialect` property to the correct
`NHibernate.Dialect.Dialect` subclass for your database. This is not
strictly essential unless you wish to use `native` or `sequence` primary
key generation or pessimistic locking (with, eg. `ISession.Lock()` or
`IQuery.SetLockMode()`). However, if you specify a dialect, NHibernate
will use sensible defaults for some of the other properties listed
above, saving you the effort of specifying them manually.

| RDBMS                                    | Dialect                                         | Remarks                                                                                                                                                                                                                                                                                                                                                                                                                                 |
|------------------------------------------|-------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| DB2                                      | `NHibernate.Dialect.DB2Dialect`                 |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| DB2 for iSeries (OS/400)                 | `NHibernate.Dialect.DB2400Dialect`              |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Firebird                                 | `NHibernate.Dialect.FirebirdDialect`            | Set `driver_class` to `NHibernate.Driver.FirebirdClientDriver` for Firebird `ADO.NET` provider 2.0.                                                                                                                                                                                                                                                                                                                                     |
| Informix                                 | `NHibernate.Dialect.InformixDialect`            |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Informix 9.40                            | `NHibernate.Dialect.InformixDialect0940`        |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Informix 10.00                           | `NHibernate.Dialect.InformixDialect1000`        |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Ingres                                   | `NHibernate.Dialect.IngresDialect`              |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Ingres 9                                 | `NHibernate.Dialect.Ingres9Dialect`             |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Server 7                   | `NHibernate.Dialect.MsSql7Dialect`              |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Server 2000                | `NHibernate.Dialect.MsSql2000Dialect`           |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Server 2005                | `NHibernate.Dialect.MsSql2005Dialect`           |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Server 2008                | `NHibernate.Dialect.MsSql2008Dialect`           |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Azure Server 2008          | `NHibernate.Dialect.MsSqlAzure2008Dialect`      |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Server 2012                | `NHibernate.Dialect.MsSql2012Dialect`           |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Server Compact Edition     | `NHibernate.Dialect.MsSqlCeDialect`             |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Microsoft SQL Server Compact Edition 4.0 | `NHibernate.Dialect.MsSqlCe40Dialect`           |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| MySQL 3 or 4                             | `NHibernate.Dialect.MySQLDialect`               |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| MySQL 5                                  | `NHibernate.Dialect.MySQL5Dialect`              |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| MySQL 5 Inno DB                          | `NHibernate.Dialect.MySQL5InnoDBDialect`        |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| MySQL 5.5                                | `NHibernate.Dialect.MySQL55Dialect`             |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| MySQL 5.5 Inno DB                        | `NHibernate.Dialect.MySQL55InnoDBDialect`       |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Oracle                                   | `NHibernate.Dialect.Oracle8iDialect`            |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Oracle 9i                                | `NHibernate.Dialect.Oracle9iDialect`            |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Oracle 10g, Oracle 11g                   | `NHibernate.Dialect.Oracle10gDialect`           |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Oracle 12c                               | `NHibernate.Dialect.Oracle12cDialect`           |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| PostgreSQL                               | `NHibernate.Dialect.PostgreSQLDialect`          |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| PostgreSQL                               | `NHibernate.Dialect.PostgreSQLDialect`          |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| PostgreSQL 8.1                           | `NHibernate.Dialect.PostgreSQL81Dialect`        | This dialect supports `FOR UPDATE NOWAIT` available in PostgreSQL 8.1.                                                                                                                                                                                                                                                                                                                                                                  |
| PostgreSQL 8.2                           | `NHibernate.Dialect.PostgreSQL82Dialect`        | This dialect supports `IF EXISTS` keyword in `DROP TABLE` and `DROP SEQUENCE` available in PostgreSQL 8.2.                                                                                                                                                                                                                                                                                                                              |
| PostgreSQL 8.3                           | `NHibernate.Dialect.PostgreSQL83Dialect`        | This dialect supports `XML` type.                                                                                                                                                                                                                                                                                                                                                                                                       |
| SQLite                                   | `NHibernate.Dialect.SQLiteDialect`              | Set `driver_class` to `NHibernate.Driver.SQLite20Driver` for System.Data.SQLite provider for .NET 2.0. Due to [the behavior of System.Data.SQLite](https://system.data.sqlite.org/index.html/tktview/44a0955ea344a777ffdbcc077831e1adc8b77a36) with `DateTime`, consider using `DateTimeFormatString=yyyy-MM-dd HH:mm:ss.FFFFFFF;` in the SQLite connection string for preventing undesired time shifts with its default configuration. |
| Sybase Adaptive Server Anywhere 9        | `NHibernate.Dialect.SybaseASA9Dialect`          |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Sybase Adaptive Server Enterprise 15     | `NHibernate.Dialect.SybaseASE15Dialect`         |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Sybase SQL Anywhere 10                   | `NHibernate.Dialect.SybaseSQLAnywhere10Dialect` |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Sybase SQL Anywhere 11                   | `NHibernate.Dialect.SybaseSQLAnywhere11Dialect` |                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| Sybase SQL Anywhere 12                   | `NHibernate.Dialect.SybaseSQLAnywhere12Dialect` |                                                                                                                                                                                                                                                                                                                                                                                                                                         |


Additional dialects may be available in the NHibernate.Dialect
namespace.

## Outer Join Fetching <a name="configuration-optional-outerjoin"></a>

If your database supports ANSI or Oracle style outer joins, *outer join
fetching* might increase performance by limiting the number of round
trips to and from the database (at the cost of possibly more work
performed by the database itself). Outer join fetching allows a graph of
objects connected by many-to-one, one-to-many or one-to-one associations
to be retrieved in a single SQL `SELECT`.

By default, the fetched graph when loading an objects ends at leaf
objects, collections, objects with proxies, or where circularities
occur.

For a *particular association*, fetching may be configured (and the
default behaviour overridden) by setting the `fetch` attribute in the
XML mapping.

Outer join fetching may be disabled *globally* by setting the property
`max_fetch_depth` to `0`. A setting of `1` or higher enables outer join
fetching for one-to-one and many-to-one associations which have been
mapped with `fetch="join"`.

See [Fetching strategies](performance.md#fetching-strategies) for more information.

In NHibernate 1.0, `outer-join` attribute could be used to achieve a
similar effect. This attribute is now deprecated in favor of `fetch`.

## Custom `ICacheProvider` <a name="configuration-optional-cacheprovider"></a>

You may integrate a process-level (or clustered) second-level cache
system by implementing the interface `NHibernate.Cache.ICacheProvider`.
You may select the custom implementation by setting
`cache.provider_class`. See the [The Second Level Cache](performance.md#the-second-level-cache) for more
details.

## Query Language Substitution <a name="configuration-optional-querysubstitution"></a>

You may define new NHibernate query tokens using `query.substitutions`.
For example:

    query.substitutions true=1, false=0

would cause the tokens `true` and `false` to be translated to integer
literals in the generated SQL.

    query.substitutions toLowercase=LOWER

would allow you to rename the SQL `LOWER` function.

# Logging <a name="configuration-logging"></a>

NHibernate logs various events using Apache log4net.

You may download log4net from [](https://logging.apache.org/log4net/),
or install it with NuGet. To use log4net you will need a `log4net`
configuration section in the application configuration file. An example
of the configuration section is distributed with NHibernate in the
`src/NHibernate.Test` project.

We strongly recommend that you familiarize yourself with NHibernate's
log messages. A lot of work has been put into making the NHibernate log
as detailed as possible, without making it unreadable. It is an
essential troubleshooting device. Also don't forget to enable SQL
logging as described above (`show_sql`), it is your first step when
looking for performance problems.

# Implementing an `INamingStrategy` <a name="configuration-namingstrategy"></a>

The interface `NHibernate.Cfg.INamingStrategy` allows you to specify a
"naming standard" for database objects and schema elements.

You may provide rules for automatically generating database identifiers
from .NET identifiers or for processing "logical" column and table names
given in the mapping file into "physical" table and column names. This
feature helps reduce the verbosity of the mapping document, eliminating
repetitive noise (`TBL_` prefixes, for example). The default strategy
used by NHibernate is quite minimal.

You may specify a different strategy by calling
`Configuration.SetNamingStrategy()` before adding mappings:

```csharp
    ISessionFactory sf = new Configuration()
        .SetNamingStrategy(ImprovedNamingStrategy.Instance)
        .AddFile("Item.hbm.xml")
        .AddFile("Bid.hbm.xml")
        .BuildSessionFactory();
```

`NHibernate.Cfg.ImprovedNamingStrategy` is a built-in strategy that
might be a useful starting point for some applications.

# XML Configuration File <a name="configuration-xmlconfig"></a>

An alternative approach is to specify a full configuration in a file
named `hibernate.cfg.xml`. This file can be used as a replacement for
the `<hibernate-configuration>` sections of the application
configuration file.

The XML configuration file is by default expected to be in your
application directory. Here is an example:

```xml
    <?xml version='1.0' encoding='utf-8'?>
    <hibernate-configuration xmlns="urn:nhibernate-configuration-2.2">
    
      <!-- an ISessionFactory instance -->
      <session-factory>
    
        <!-- properties -->
        <property name="connection.connection_string">
          Server=localhost;initial catalog=nhibernate;User Id=;Password=
        </property>
        <property name="show_sql">false</property>
        <property name="dialect">NHibernate.Dialect.MsSql2012Dialect</property>
    
        <!-- mapping files -->
        <mapping resource="NHibernate.Auction.Item.hbm.xml" assembly="NHibernate.Auction" />
        <mapping resource="NHibernate.Auction.Bid.hbm.xml" assembly="NHibernate.Auction" />
    
      </session-factory>
    
    </hibernate-configuration>
```

Configuring NHibernate is then as simple
    as

```csharp
    ISessionFactory sf = new Configuration().Configure().BuildSessionFactory();
```

You can pick a different XML configuration file using

```csharp
    ISessionFactory sf = new Configuration()
        .Configure("/path/to/config.cfg.xml")
        .BuildSessionFactory();
```
