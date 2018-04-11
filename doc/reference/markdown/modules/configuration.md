# ISessionFactory Configuration

Because NHibernate is designed to operate in many different
environments, there are a large number of configuration parameters.
Fortunately, most have sensible default values and NHibernate is
distributed with an example `App.config` file (found in
`src\NHibernate.Test`) that shows the various options. You usually only
have to put that file in your project and customize it.

# Programmatic Configuration

An instance of `NHibernate.Cfg.Configuration` represents an entire set
of mappings of an application's .NET types to a SQL database. The
`Configuration` is used to build an (immutable) `ISessionFactory`. The
mappings are compiled from various XML mapping files.

You may obtain a `Configuration` instance by instantiating it directly.
Here is an example of setting up a datastore from mappings defined in
two XML configuration files:

    Configuration cfg = new Configuration()
        .AddFile("Item.hbm.xml")
        .AddFile("Bid.hbm.xml");

An alternative (sometimes better) way is to let NHibernate load a
mapping file from an embedded resource:

    Configuration cfg = new Configuration()
        .AddClass(typeof(NHibernate.Auction.Item))
        .AddClass(typeof(NHibernate.Auction.Bid));

Then NHibernate will look for mapping files named
`NHibernate.Auction.Item.hbm.xml` and `NHibernate.Auction.Bid.hbm.xml`
embedded as resources in the assembly that the types are contained in.
This approach eliminates any hardcoded filenames.

Another alternative (probably the best) way is to let NHibernate load
all of the mapping files contained in an Assembly:

    Configuration cfg = new Configuration()
        .AddAssembly( "NHibernate.Auction" );

Then NHibernate will look through the assembly for any resources that
end with `.hbm.xml`. This approach eliminates any hardcoded filenames
and ensures the mapping files in the assembly get added.

If a tool like Visual Studio .NET or NAnt is used to build the assembly,
then make sure that the `.hbm.xml` files are compiled into the assembly
as `Embedded Resources`.

A `Configuration` also specifies various optional properties:

    var props = new Dictionary<string, string>();
    ...
    Configuration cfg = new Configuration()
        .AddClass(typeof(NHibernate.Auction.Item))
        .AddClass(typeof(NHibernate.Auction.Bind))
        .SetProperties(props);

A `Configuration` is intended as a configuration-time object, to be
discarded once an `ISessionFactory` is built.

# Obtaining an ISessionFactory

When all mappings have been parsed by the `Configuration`, the
application must obtain a factory for `ISession` instances. This factory
is intended to be shared by all application threads:

    ISessionFactory sessions = cfg.BuildSessionFactory();

However, NHibernate does allow your application to instantiate more than
one `ISessionFactory`. This is useful if you are using more than one
database.

# User provided ADO.NET connection

An `ISessionFactory` may open an `ISession` on a user-provided ADO.NET
connection. This design choice frees the application to obtain ADO.NET
connections wherever it pleases:

    var conn = myApp.GetOpenConnection();
    var session = sessions.OpenSession(conn);
    
    // do some data access work

The application must be careful not to open two concurrent `ISession`s
on the same ADO.NET connection\!

# NHibernate provided ADO.NET connection

Alternatively, you can have the `ISessionFactory` open connections for
you. The `ISessionFactory` must be provided with ADO.NET connection
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

    ISession session = sessions.OpenSession(); // open a new Session
    // do some data access work, an ADO.NET connection will be used on demand

All NHibernate property names and semantics are defined on the class
`NHibernate.Cfg.Environment`. We will now describe the most important
settings for ADO.NET connection configuration.

NHibernate will obtain (and pool) connections using an ADO.NET data
provider if you set the following properties:

<table>
<caption>NHibernate ADO.NET Properties</caption>
<colgroup>
<col style="width: 50%" />
<col style="width: 50%" />
</colgroup>
<thead>
<tr class="header">
<th>Property name</th>
<th>Purpose</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td><code>connection.provider</code></td>
<td>The type of a custom <code>IConnectionProvider</code> implementation.
<p><strong>eg.</strong> <code>full.classname.of.ConnectionProvider</code> if the Provider is built into NHibernate, or <code>full.classname.of.ConnectionProvider, 
                        assembly</code> if using an implementation of <code>IConnectionProvider</code> not included in NHibernate. The default is <code>NHibernate.Connection.DriverConnectionProvider</code>.</p></td>
</tr>
<tr class="even">
<td><code>connection.driver_class</code></td>
<td>The type of a custom <code>IDriver</code>, if using <code>DriverConnectionProvider</code>.
<p><strong>eg.</strong> <code>full.classname.of.Driver</code> if the Driver is built into NHibernate, or <code>full.classname.of.Driver, assembly</code> if using an implementation of IDriver not included in NHibernate.</p>
<p>This is usually not needed, most of the time the <code>dialect</code> will take care of setting the <code>IDriver</code> using a sensible default. See the API documentation of the specific dialect for the defaults.</p></td>
</tr>
<tr class="odd">
<td><code>connection.connection_string</code></td>
<td>Connection string to use to obtain the connection.</td>
</tr>
<tr class="even">
<td><code>connection.connection_string_name</code></td>
<td>The name of the connection string (defined in <code>&lt;connectionStrings&gt;</code> configuration file element) to use to obtain the connection.</td>
</tr>
<tr class="odd">
<td><code>connection.isolation</code></td>
<td>Set the ADO.NET transaction isolation level. Check <code>System.Data.IsolationLevel</code> for meaningful values and the database's documentation to ensure that level is supported.
<p><strong>eg.</strong> <code>Chaos</code> | <code>ReadCommitted</code> | <code>ReadUncommitted</code> | <code>RepeatableRead</code> | <code>Serializable</code> | <code>Unspecified</code></p></td>
</tr>
<tr class="even">
<td><code>connection.release_mode</code></td>
<td>Specify when NHibernate should release ADO.NET connections. See <a href="#transactions-connection-release">???</a>.
<p><strong>eg.</strong> <code>auto</code> (default) | <code>on_close</code> | <code>after_transaction</code></p>
<p>Note that for <code>ISession</code>s obtained through <code>ISessionFactory.GetCurrentSession</code>, the <code>ICurrentSessionContext</code> implementation configured for use may control the connection release mode for those <code>ISession</code>s. See <a href="#architecture-current-session">???</a>.</p></td>
</tr>
<tr class="odd">
<td><code>prepare_sql</code></td>
<td>Specify to prepare <code>DbCommand</code>s generated by NHibernate. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>command_timeout</code></td>
<td>Specify the default timeout in seconds of <code>DbCommand</code>s generated by NHibernate. Negative values disable it.
<p><strong>eg.</strong> <code>30</code></p></td>
</tr>
<tr class="odd">
<td><code>adonet.batch_size</code></td>
<td>Specify the batch size to use when batching update statements. Setting this to 0 (the default) disables the functionality. See <a href="#performance-batch-updates">???</a>.
<p><strong>eg.</strong> <code>20</code></p></td>
</tr>
<tr class="even">
<td><code>order_inserts</code></td>
<td>Enable ordering of insert statements for the purpose of more efficient batching. Defaults to <code>true</code> if batching is enabled, <code>false</code> otherwise.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>order_updates</code></td>
<td>Enable ordering of update statements for the purpose of more efficient batching. Defaults to <code>true</code> if batching is enabled, <code>false</code> otherwise.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>adonet.batch_versioned_data</code></td>
<td>If batching is enabled, specify that versioned data can also be batched. Requires a dialect which batcher correctly returns rows count. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>adonet.factory_class</code></td>
<td>The class name of a <code>IBatcherFactory</code> implementation.
<p>This is usually not needed, most of the time the <code>driver</code> will take care of setting the <code>IBatcherFactory</code> using a sensible default according to the database capabilities.</p>
<p><strong>eg.</strong> <code>classname.of.BatcherFactory, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>adonet.wrap_result_sets</code></td>
<td>Some database vendor data reader implementation have inefficient columnName-to-columnIndex resolution. Enabling this setting allows to wrap them in a data reader that will cache those resolutions. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
</tbody>
</table>

This is an example of how to specify the database connection properties
inside a `web.config`:

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

NHibernate relies on the ADO.NET data provider implementation of
connection pooling.

You may define your own plug-in strategy for obtaining ADO.NET
connections by implementing the interface
`NHibernate.Connection.IConnectionProvider`. You may select a custom
implementation by setting `connection.provider`.

# Optional configuration properties

There are a number of other properties that control the behaviour of
NHibernate at runtime. All are optional and have reasonable default
values.

Some properties are system-level properties. They can only be set
manually by setting static properties of `NHibernate.Cfg.Environment`
class or be defined in the `<hibernate-configuration>` section of the
application configuration file. These properties cannot be set using
`Configuration.SetProperties` or the `hibernate.cfg.xml` configuration
file.

<table>
<caption>NHibernate Configuration Properties</caption>
<colgroup>
<col style="width: 50%" />
<col style="width: 50%" />
</colgroup>
<thead>
<tr class="header">
<th>Property name</th>
<th>Purpose</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td><code>dialect</code></td>
<td>The class name of a NHibernate <code>Dialect</code> - enables certain platform dependent features. See <a href="#configuration-optional-dialects">SQL Dialects</a>.
<p><strong>eg.</strong> <code>full.classname.of.Dialect, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>default_catalog</code></td>
<td>Qualify unqualified table names with the given catalog name in generated SQL.
<p><strong>eg.</strong> <code>CATALOG_NAME</code></p></td>
</tr>
<tr class="odd">
<td><code>default_schema</code></td>
<td>Qualify unqualified table names with the given schema/table-space in generated SQL.
<p><strong>eg.</strong> <code>SCHEMA_NAME</code></p></td>
</tr>
<tr class="even">
<td><code>max_fetch_depth</code></td>
<td>Set a maximum &quot;depth&quot; for the outer join fetch tree for single-ended associations (one-to-one, many-to-one). A <code>0</code> disables default outer join fetching.
<p><strong>eg.</strong> recommended values between <code>0</code> and <code>3</code></p></td>
</tr>
<tr class="odd">
<td><code>use_reflection_optimizer</code></td>
<td>Enables use of a runtime-generated class to set or get properties of an entity or component instead of using runtime reflection. This is a system-level property. The use of the reflection optimizer inflicts a certain startup cost on the application but should lead to better performance in the long run. Defaults to <code>true</code>.
<p>You can not set this property in <code>hibernate.cfg.xml</code>, but only in <code>&lt;hibernate-configuration&gt;</code> section of the application configuration file or by code by setting <code>NHibernate.Cfg.Environment.UseReflectionOptimizer</code> before creating any <code>NHibernate.Cfg.Configuration</code> instance.</p>
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>bytecode.provider</code></td>
<td>Specifies the bytecode provider to use to optimize the use of reflection in NHibernate. This is a system-level property. Use <code>null</code> to disable the optimization completely, <code>lcg</code> to use built-in lightweight code generation, or the class name of a custom <code>IBytecodeProvider</code> implementation. Defaults to <code>lcg</code>.
<p>You can not set this property in <code>hibernate.cfg.xml</code>, but only in <code>&lt;hibernate-configuration&gt;</code> section of the application configuration file or by code by setting <code>NHibernate.Cfg.Environment.BytecodeProvider</code> before creating any <code>NHibernate.Cfg.Configuration</code> instance.</p>
<p><strong>eg.</strong> <code>null</code> | <code>lcg</code> | <code>classname.of.BytecodeProvider, assembly</code></p></td>
</tr>
<tr class="odd">
<td><code>cache.use_second_level_cache</code></td>
<td>Enable the second level cache. Requires specifying a <code>cache.provider_class</code>. See <a href="#caches">???</a>. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>cache.provider_class</code></td>
<td>The class name of a <code>ICacheProvider</code> implementation.
<p><strong>eg.</strong> <code>classname.of.CacheProvider, assembly</code></p></td>
</tr>
<tr class="odd">
<td><code>cache.use_minimal_puts</code></td>
<td>Optimize second-level cache operation to minimize writes, at the cost of more frequent reads (useful for clustered caches). Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>cache.use_query_cache</code></td>
<td>Enable the query cache, individual queries still have to be set cacheable. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>cache.query_cache_factory</code></td>
<td>The class name of a custom <code>IQueryCacheFactory</code> implementation. Defaults to the built-in <code>StandardQueryCacheFactory</code>.
<p><strong>eg.</strong> <code>classname.of.QueryCacheFactory, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>cache.region_prefix</code></td>
<td>A prefix to use for second-level cache region names.
<p><strong>eg.</strong> <code>prefix</code></p></td>
</tr>
<tr class="odd">
<td><code>cache.default_expiration</code></td>
<td>The default expiration delay in seconds for cached entries, for providers supporting this setting.
<p><strong>eg.</strong> <code>300</code></p></td>
</tr>
<tr class="even">
<td><code>query.substitutions</code></td>
<td>Mapping from tokens in NHibernate queries to SQL tokens (tokens might be function or literal names, for example).
<p><strong>eg.</strong> <code>hqlLiteral=SQL_LITERAL, hqlFunction=SQLFUNC</code></p></td>
</tr>
<tr class="odd">
<td><code>query.default_cast_length</code></td>
<td>Set the default length used in casting when the target type is length bound and does not specify it. Defaults to <code>4000</code>, automatically trimmed down according to dialect type registration.
<p><strong>eg.</strong> <code>255</code></p></td>
</tr>
<tr class="even">
<td><code>query.default_cast_precision</code></td>
<td>Set the default precision used in casting when the target type is decimal and does not specify it. Defaults to <code>28</code>, automatically trimmed down according to dialect type registration.
<p><strong>eg.</strong> <code>19</code></p></td>
</tr>
<tr class="odd">
<td><code>query.default_cast_scale</code></td>
<td>Set the default scale used in casting when the target type is decimal and does not specify it. Defaults to <code>10</code>, automatically trimmed down according to dialect type registration.
<p><strong>eg.</strong> <code>5</code></p></td>
</tr>
<tr class="even">
<td><code>query.startup_check</code></td>
<td>Should named queries be checked during startup (the default is enabled).
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>query.factory_class</code></td>
<td>The class name of a custom <code>IQueryTranslatorFactory</code> implementation (HQL query parser factory). Defaults to the built-in <code>ASTQueryTranslatorFactory</code>.
<p><strong>eg.</strong> <code>classname.of.QueryTranslatorFactory, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>query.linq_provider_class</code></td>
<td>The class name of a custom <code>INhQueryProvider</code> implementation (LINQ provider). Defaults to the built-in <code>DefaultQueryProvider</code>.
<p><strong>eg.</strong> <code>classname.of.LinqProvider, assembly</code></p></td>
</tr>
<tr class="odd">
<td><code>query.query_model_rewriter_factory</code></td>
<td>The class name of a custom <code>IQueryModelRewriterFactory</code> implementation (LINQ query model rewriter factory). Defaults to <code>null</code> (no rewriter).
<p><strong>eg.</strong> <code>classname.of.QueryModelRewriterFactory, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>linqtohql.generatorsregistry</code></td>
<td>The class name of a custom <code>ILinqToHqlGeneratorsRegistry</code> implementation. Defaults to the built-in <code>DefaultLinqToHqlGeneratorsRegistry</code>. See <a href="#querylinq-extending-generator">???</a>.
<p><strong>eg.</strong> <code>classname.of.LinqToHqlGeneratorsRegistry, assembly</code></p></td>
</tr>
<tr class="odd">
<td><code>sql_exception_converter</code></td>
<td>The class name of a custom <code>ISQLExceptionConverter</code> implementation. Defaults to <code>Dialect.BuildSQLExceptionConverter()</code>.
<p><strong>eg.</strong> <code>classname.of.SQLExceptionConverter, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>show_sql</code></td>
<td>Write all SQL statements to console. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>format_sql</code></td>
<td>Log formatted SQL. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>use_sql_comments</code></td>
<td>Generate SQL with comments. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>hbm2ddl.auto</code></td>
<td>Automatically export schema DDL to the database when the <code>ISessionFactory</code> is created. With <code>create-drop</code>, the database schema will be dropped when the <code>ISessionFactory</code> is closed explicitly.
<p><strong>eg.</strong> <code>create</code> | <code>create-drop</code></p></td>
</tr>
<tr class="even">
<td><code>hbm2ddl.keywords</code></td>
<td>Automatically import <code>reserved/keywords</code> from the database when the <code>ISessionFactory</code> is created.
<p><strong>none :</strong> disable any operation regarding RDBMS KeyWords (the default).</p>
<p><strong>keywords :</strong> imports all RDBMS KeyWords where the <code>Dialect</code> can provide the implementation of <code>IDataBaseSchema</code>.</p>
<p><strong>auto-quote :</strong> imports all RDBMS KeyWords and auto-quote all table-names/column-names.</p>
<p><strong>eg.</strong> <code>none</code> | <code>keywords</code> | <code>auto-quote</code></p></td>
</tr>
<tr class="odd">
<td><code>use_proxy_validator</code></td>
<td>Enables or disables validation of interfaces or classes specified as proxies. Enabled by default.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>proxyfactory.factory_class</code></td>
<td>The class name of a custom <code>IProxyFactoryFactory</code> implementation. Defaults to the built-in <code>DefaultProxyFactoryFactory</code>.
<p><strong>eg.</strong> <code>classname.of.ProxyFactoryFactory, assembly</code></p></td>
</tr>
<tr class="odd">
<td><code>collectiontype.factory_class</code></td>
<td>The class name of a custom <code>ICollectionTypeFactory</code> implementation. Defaults to the built-in <code>DefaultCollectionTypeFactory</code>.
<p><strong>eg.</strong> <code>classname.of.CollectionTypeFactory, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>transaction.factory_class</code></td>
<td>The class name of a custom <code>ITransactionFactory</code> implementation. Defaults to the built-in <code>AdoNetWithSystemTransactionFactory</code>.
<p><strong>eg.</strong> <code>classname.of.TransactionFactory, assembly</code></p></td>
</tr>
<tr class="odd">
<td><code>transaction.use_connection_on_system_prepare</code></td>
<td>When a system transaction is being prepared, is using connection during this process enabled?
<p>Default is <code>true</code>, for supporting <code>FlushMode.Commit</code> with transaction factories supporting system transactions. But this requires enlisting additional connections, retaining disposed sessions and their connections until transaction end, and may trigger undesired transaction promotions to distributed.</p>
<p>Set to <code>false</code> for disabling using connections from system transaction preparation, while still benefiting from <code>FlushMode.Auto</code> on querying.</p>
<p>See <a href="#transactions-scopes">???</a>.</p>
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>transaction.system_completion_lock_timeout</code></td>
<td>Timeout duration in milliseconds for the system transaction completion lock.
<p>When a system transaction completes, it may have its completion events running on concurrent threads, after scope disposal. This occurs when the transaction is distributed. This notably concerns <code>ISessionImplementor.AfterTransactionCompletion(bool, ITransaction)</code>. NHibernate protects the session from being concurrently used by the code following the scope disposal with a lock. To prevent any application freeze, this lock has a default timeout of five seconds. If the application appears to require longer (!) running transaction completion events, this setting allows to raise this timeout. <code>-1</code> disables the timeout.</p>
<p><strong>eg.</strong> <code>10000</code></p></td>
</tr>
<tr class="odd">
<td><code>default_flush_mode</code></td>
<td>The default <code>FlushMode</code>, <code>Auto</code> when not specified. See <a href="#manipulatingdata-flushing">???</a>.
<p><strong>eg.</strong> <code>Manual</code> | <code>Commit</code> | <code>Auto</code> | <code>Always</code></p></td>
</tr>
<tr class="even">
<td><code>default_batch_fetch_size</code></td>
<td>The default batch fetch size to use when lazily loading an entity or collection. Defaults to <code>1</code>. See <a href="#performance-fetching-batch">???</a>.
<p><strong>eg.</strong> <code>20</code></p></td>
</tr>
<tr class="odd">
<td><code>current_session_context_class</code></td>
<td>The class name of an <code>ICurrentSessionContext</code> implementation. See <a href="#architecture-current-session">???</a>.
<p><strong>eg.</strong> <code>classname.of.CurrentSessionContext, assembly</code></p></td>
</tr>
<tr class="even">
<td><code>id.optimizer.pooled.prefer_lo</code></td>
<td>When using an enhanced id generator and pooled optimizers (see <a href="#mapping-declaration-id-enhanced">???</a>), prefer interpreting the database value as the lower (lo) boundary. The default is to interpret it as the high boundary.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>generate_statistics</code></td>
<td>Enable statistics collection within <code>ISessionFactory.Statistics</code> property. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>track_session_id</code></td>
<td>Set whether the session id should be tracked in logs or not. When <code>true</code>, each session will have an unique <code>Guid</code> that can be retrieved with <code>ISessionImplementor.SessionId</code>, otherwise <code>ISessionImplementor.SessionId</code> will be <code>Guid.Empty</code>.
<p>Session id is used for logging purpose and can also be retrieved on the static property <code>NHibernate.Impl.SessionIdLoggingContext.SessionId</code>, when tracking is enabled.</p>
<p>Disabling tracking by setting <code>track_session_id</code> to <code>false</code> increases performances. Default is <code>true</code>.</p>
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>sql_types.keep_datetime</code></td>
<td>Since NHibernate v5.0 and if the dialect supports it, <code>DbType.DateTime2</code> is used instead of <code>DbType.DateTime</code>. This may be disabled by setting <code>sql_types.keep_datetime</code> to <code>true</code>. Defaults to <code>false</code>.
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="even">
<td><code>oracle.use_n_prefixed_types_for_unicode</code></td>
<td><p>Oracle has a dual Unicode support model.</p>
<p>Either the whole database use an Unicode encoding, and then all string types will be Unicode. In such case, Unicode strings should be mapped to non <code>N</code> prefixed types, such as <code>Varchar2</code>. This is the default.</p>
<p>Or <code>N</code> prefixed types such as <code>NVarchar2</code> are to be used for Unicode strings, the others type are using a non Unicode encoding. In such case this setting needs to be set to <code>true</code>.</p>
<p>See <a href="https://docs.oracle.com/cd/B19306_01/server.102/b14225/ch6unicode.htm#CACHCAHF">Implementing a Unicode Solution in the Database</a>. This setting applies only to Oracle dialects and ODP.Net managed or unmanaged driver.</p>
<p><strong>eg.</strong> <code>true</code> | <code>false</code></p></td>
</tr>
<tr class="odd">
<td><code>odbc.explicit_datetime_scale</code></td>
<td>This may need to be set to <code>3</code> if you are using the <code>OdbcDriver</code> with MS SQL Server 2008+.
<p>This is intended to work around issues like:</p>
<pre><code>System.Data.Odbc.OdbcException :
ERROR [22008]
[Microsoft][SQL Server Native Client 11.0]
Datetime field overflow. Fractional second
precision exceeds the scale specified
in the parameter binding.</code></pre>
<p><strong>eg.</strong> <code>3</code></p></td>
</tr>
<tr class="even">
<td><code>nhibernate-logger</code></td>
<td>The class name of an <code>ILoggerFactory</code> implementation. It allows using another logger than log4net.
<p>The default is not defined, which causes NHibernate to search for log4net assembly. If this search succeeds, NHibernate will log with log4net. Otherwise, its internal logging will be disabled.</p>
<p>This is a very special system-level property. It can only be set through an <a href="https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/appsettings/">appSetting</a> named <code>nhibernate-logger</code> in the application configuration file. It cannot be set neither with <code>NHibernate.Cfg.Environment</code> class, nor be defined in the <code>&lt;hibernate-configuration&gt;</code> section of the application configuration file, nor supplied by using <code>Configuration.SetProperties</code>, nor set in the <code>hibernate.cfg.xml</code> configuration file.</p>
<p><strong>eg.</strong> <code>classname.of.LoggerFactory, assembly</code></p></td>
</tr>
</tbody>
</table>

## SQL Dialects

You should always set the `dialect` property to the correct
`NHibernate.Dialect.Dialect` subclass for your database. This is not
strictly essential unless you wish to use `native` or `sequence` primary
key generation or pessimistic locking (with, eg. `ISession.Lock()` or
`IQuery.SetLockMode()`). However, if you specify a dialect, NHibernate
will use sensible defaults for some of the other properties listed
above, saving you the effort of specifying them manually.

<table>
<caption>NHibernate SQL Dialects (<code>dialect</code>)</caption>
<colgroup>
<col style="width: 16%" />
<col style="width: 41%" />
<col style="width: 41%" />
</colgroup>
<thead>
<tr class="header">
<th>RDBMS</th>
<th>Dialect</th>
<th>Remarks</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>DB2</td>
<td><code>NHibernate.Dialect.DB2Dialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>DB2 for iSeries (OS/400)</td>
<td><code>NHibernate.Dialect.DB2400Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Firebird</td>
<td><code>NHibernate.Dialect.FirebirdDialect</code></td>
<td>Set <code>driver_class</code> to <code>NHibernate.Driver.FirebirdClientDriver</code> for Firebird ADO.NET provider 2.0.</td>
</tr>
<tr class="even">
<td>Informix</td>
<td><code>NHibernate.Dialect.InformixDialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Informix 9.40</td>
<td><code>NHibernate.Dialect.InformixDialect0940</code></td>
<td></td>
</tr>
<tr class="even">
<td>Informix 10.00</td>
<td><code>NHibernate.Dialect.InformixDialect1000</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Ingres</td>
<td><code>NHibernate.Dialect.IngresDialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Ingres 9</td>
<td><code>NHibernate.Dialect.Ingres9Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Microsoft SQL Server 7</td>
<td><code>NHibernate.Dialect.MsSql7Dialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Microsoft SQL Server 2000</td>
<td><code>NHibernate.Dialect.MsSql2000Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Microsoft SQL Server 2005</td>
<td><code>NHibernate.Dialect.MsSql2005Dialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Microsoft SQL Server 2008</td>
<td><code>NHibernate.Dialect.MsSql2008Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Microsoft SQL Azure Server 2008</td>
<td><code>NHibernate.Dialect.MsSqlAzure2008Dialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Microsoft SQL Server 2012</td>
<td><code>NHibernate.Dialect.MsSql2012Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Microsoft SQL Server Compact Edition</td>
<td><code>NHibernate.Dialect.MsSqlCeDialect</code></td>
</tr>
<tr class="even">
<td>Microsoft SQL Server Compact Edition 4.0</td>
<td><code>NHibernate.Dialect.MsSqlCe40Dialect</code></td>
</tr>
<tr class="odd">
<td>MySQL 3 or 4</td>
<td><code>NHibernate.Dialect.MySQLDialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>MySQL 5</td>
<td><code>NHibernate.Dialect.MySQL5Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>MySQL 5 Inno DB</td>
<td><code>NHibernate.Dialect.MySQL5InnoDBDialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>MySQL 5.5</td>
<td><code>NHibernate.Dialect.MySQL55Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>MySQL 5.5 Inno DB</td>
<td><code>NHibernate.Dialect.MySQL55InnoDBDialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Oracle</td>
<td><code>NHibernate.Dialect.Oracle8iDialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Oracle 9i</td>
<td><code>NHibernate.Dialect.Oracle9iDialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Oracle 10g, Oracle 11g</td>
<td><code>NHibernate.Dialect.Oracle10gDialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Oracle 12c</td>
<td><code>NHibernate.Dialect.Oracle12cDialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>PostgreSQL</td>
<td><code>NHibernate.Dialect.PostgreSQLDialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>PostgreSQL</td>
<td><code>NHibernate.Dialect.PostgreSQLDialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>PostgreSQL 8.1</td>
<td><code>NHibernate.Dialect.PostgreSQL81Dialect</code></td>
<td>This dialect supports <code>FOR UPDATE NOWAIT</code> available in PostgreSQL 8.1.</td>
</tr>
<tr class="odd">
<td>PostgreSQL 8.2</td>
<td><code>NHibernate.Dialect.PostgreSQL82Dialect</code></td>
<td>This dialect supports <code>IF EXISTS</code> keyword in <code>DROP TABLE</code> and <code>DROP SEQUENCE</code> available in PostgreSQL 8.2.</td>
</tr>
<tr class="even">
<td>PostgreSQL 8.3</td>
<td><code>NHibernate.Dialect.PostgreSQL83Dialect</code></td>
<td>This dialect supports <code>XML</code> type.</td>
</tr>
<tr class="odd">
<td>SQLite</td>
<td><code>NHibernate.Dialect.SQLiteDialect</code></td>
<td>Set <code>driver_class</code> to <code>NHibernate.Driver.SQLite20Driver</code> for System.Data.SQLite provider for .NET 2.0.
<p>Due to <a href="https://system.data.sqlite.org/index.html/tktview/44a0955ea344a777ffdbcc077831e1adc8b77a36">the behavior of System.Data.SQLite</a> with <code>DateTime</code>, consider using <code>DateTimeFormatString=yyyy-MM-dd HH:mm:ss.FFFFFFF;</code> in the SQLite connection string for preventing undesired time shifts with its default configuration.</p></td>
</tr>
<tr class="even">
<td>Sybase Adaptive Server Anywhere 9</td>
<td><code>NHibernate.Dialect.SybaseASA9Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Sybase Adaptive Server Enterprise 15</td>
<td><code>NHibernate.Dialect.SybaseASE15Dialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Sybase SQL Anywhere 10</td>
<td><code>NHibernate.Dialect.SybaseSQLAnywhere10Dialect</code></td>
<td></td>
</tr>
<tr class="odd">
<td>Sybase SQL Anywhere 11</td>
<td><code>NHibernate.Dialect.SybaseSQLAnywhere11Dialect</code></td>
<td></td>
</tr>
<tr class="even">
<td>Sybase SQL Anywhere 12</td>
<td><code>NHibernate.Dialect.SybaseSQLAnywhere12Dialect</code></td>
<td></td>
</tr>
</tbody>
</table>

Additional dialects may be available in the NHibernate.Dialect
namespace.

## Outer Join Fetching

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

See [???](#performance-fetching) for more information.

In NHibernate 1.0, `outer-join` attribute could be used to achieve a
similar effect. This attribute is now deprecated in favor of `fetch`.

## Custom `ICacheProvider`

You may integrate a process-level (or clustered) second-level cache
system by implementing the interface `NHibernate.Cache.ICacheProvider`.
You may select the custom implementation by setting
`cache.provider_class`. See the [???](#performance-cache) for more
details.

## Query Language Substitution

You may define new NHibernate query tokens using `query.substitutions`.
For example:

    query.substitutions true=1, false=0

would cause the tokens `true` and `false` to be translated to integer
literals in the generated SQL.

    query.substitutions toLowercase=LOWER

would allow you to rename the SQL `LOWER` function.

# Logging

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

# Implementing an `INamingStrategy`

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

    ISessionFactory sf = new Configuration()
        .SetNamingStrategy(ImprovedNamingStrategy.Instance)
        .AddFile("Item.hbm.xml")
        .AddFile("Bid.hbm.xml")
        .BuildSessionFactory();

`NHibernate.Cfg.ImprovedNamingStrategy` is a built-in strategy that
might be a useful starting point for some applications.

# XML Configuration File

An alternative approach is to specify a full configuration in a file
named `hibernate.cfg.xml`. This file can be used as a replacement for
the `<hibernate-configuration>` sections of the application
configuration file.

The XML configuration file is by default expected to be in your
application directory. Here is an example:

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

Configuring NHibernate is then as simple
    as

    ISessionFactory sf = new Configuration().Configure().BuildSessionFactory();

You can pick a different XML configuration file using

    ISessionFactory sf = new Configuration()
        .Configure("/path/to/config.cfg.xml")
        .BuildSessionFactory();
