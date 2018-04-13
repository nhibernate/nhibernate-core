# Improving performance

# Fetching strategies <a name="performance-fetching"></a>

A *fetching strategy* is the strategy NHibernate will use for retrieving
associated objects if the application needs to navigate the association.
Fetch strategies may be declared in the O/R mapping metadata, or
overridden by a particular HQL or `Criteria` query.

NHibernate defines the following fetching strategies:

  - *Join fetching* - NHibernate retrieves the associated instance or
    collection in the same `SELECT`, using an `OUTER JOIN`.

  - *Select fetching* - a second `SELECT` is used to retrieve the
    associated entity or collection. Unless you explicitly disable lazy
    fetching by specifying `lazy="false"`, this second select will only
    be executed when you actually access the association.

  - *Subselect fetching* - a second `SELECT` is used to retrieve the
    associated collections for all entities retrieved in a previous
    query or fetch. Unless you explicitly disable lazy fetching by
    specifying `lazy="false"`, this second select will only be executed
    when you actually access the association.

  - *"Extra-lazy" collection fetching* - individual elements of the
    collection are accessed from the database as needed. NHibernate
    tries not to fetch the whole collection into memory unless
    absolutely needed (suitable for very large collections)

  - *Batch fetching* - an optimization strategy for select fetching -
    NHibernate retrieves a batch of entity instances or collections in a
    single `SELECT`, by specifying a list of primary keys or foreign
    keys.

NHibernate also distinguishes between:

  - *Immediate fetching* - an association, collection or attribute is
    fetched immediately, when the owner is loaded.

  - *Lazy collection fetching* - a collection is fetched when the
    application invokes an operation upon that collection. (This is the
    default for collections.)

  - *Proxy fetching* - a single-valued association is fetched when a
    method other than the identifier getter is invoked upon the
    associated object.

We have two orthogonal notions here: *when* is the association fetched,
and *how* is it fetched (what SQL is used). Don't confuse them\! We use
`fetch` to tune performance. We may use `lazy` to define a contract for
what data is always available in any detached instance of a particular
class.

## Working with lazy associations <a name="performance-fetching-lazy"></a>

By default, NHibernate uses lazy select fetching for collections and
lazy proxy fetching for single-valued associations. These defaults make
sense for almost all associations in almost all applications.

However, lazy fetching poses one problem that you must be aware of.
Access to a lazy association outside of the context of an open
NHibernate session will result in an exception. For example:

```csharp
    IDictionary<string, int> permissions;
    using (var s = sessions.OpenSession())
    using (Transaction tx = s.BeginTransaction())
    {
        User u = s.CreateQuery("from User u where u.Name=:userName")
            .SetString("userName", userName).UniqueResult<User>();
        permissions = u.Permissions;
    
        tx.Commit();
    }
    
    int accessLevel = permissions["accounts"];  // Error!
```

Since the `permissions` collection was not initialized when the
`ISession` was closed, the collection will not be able to load its
state. *NHibernate does not support lazy initialization for detached
objects*. The fix is to move the code that reads from the collection to
just before the transaction is committed.

Alternatively, we could use a non-lazy collection or association, by
specifying `lazy="false"` for the association mapping. However, it is
intended that lazy initialization be used for almost all collections and
associations. If you define too many non-lazy associations in your
object model, NHibernate will end up needing to fetch the entire
database into memory in every transaction\!

On the other hand, we often want to choose join fetching (which is
non-lazy by nature) instead of select fetching in a particular
transaction. We'll now see how to customize the fetching strategy. In
NHibernate, the mechanisms for choosing a fetch strategy are identical
for single-valued associations and collections.

## Tuning fetch strategies <a name="performance-fetching-custom"></a>

Select fetching (the default) is extremely vulnerable to N+1 selects
problems, so we might want to enable join fetching in the mapping
document:

```xml
    <set name="Permissions" 
                fetch="join">
        <key column="userId"/>
        <one-to-many class="Permission"/>
    </set

    <many-to-one name="Mother" class="Cat" fetch="join"/>
```

The `fetch` strategy defined in the mapping document affects:

  - retrieval via `Get()` or `Load()`

  - retrieval that happens implicitly when an association is navigated

  - `ICriteria` queries

  - HQL queries if `subselect` fetching is used

No matter what fetching strategy you use, the defined non-lazy graph is
guaranteed to be loaded into memory. Note that this might result in
several immediate selects being used to execute a particular HQL query.

Usually, we don't use the mapping document to customize fetching.
Instead, we keep the default behavior, and override it for a particular
transaction, using `left join fetch` in HQL. This tells NHibernate to
fetch the association eagerly in the first select, using an outer join.
In the `ICriteria` query API, you would use
`SetFetchMode(FetchMode.Join)`.

If you ever feel like you wish you could change the fetching strategy
used by `Get()` or `Load()`, simply use a `ICriteria` query, for
example:

```csharp
    User user = session.CreateCriteria(typeof(User))
        .SetFetchMode("Permissions", FetchMode.Join)
        .Add( Expression.Eq("Id", userId) )
        .UniqueResult<User>();
```

(This is NHibernate's equivalent of what some *ORM* solutions call a
"fetch plan".)

A completely different way to avoid problems with N+1 selects is to use
the [second-level cache](#the-second-level-cache), or to enable [batch fetching](#using-batch-fetching).

## Single-ended association proxies <a name="performance-fetching-proxies"></a>

Lazy fetching for collections is implemented using NHibernate's own
implementation of persistent collections. However, a different mechanism
is needed for lazy behavior in single-ended associations. The target
entity of the association must be proxied. NHibernate implements lazy
initializing proxies for persistent objects using runtime bytecode
enhancement.

By default, NHibernate generates proxies (at startup) for all persistent
classes and uses them to enable lazy fetching of `many-to-one` and
`one-to-one` associations.

The mapping file may declare an interface to use as the proxy interface
for that class, with the `proxy` attribute. By default, NHibernate uses
a subclass of the class. *Note that the proxied class must implement a
non-private default constructor. We recommend this constructor for all
persistent classes\!*

There are some gotchas to be aware of when extending this approach to
polymorphic classes, eg.

```xml
    <class name="Cat" proxy="Cat">
        ......
        <subclass name="DomesticCat">
            .....
        </subclass>
    </class>
```

Firstly, instances of `Cat` will never be castable to `DomesticCat`,
even if the underlying instance is an instance of `DomesticCat`:

```csharp
    // instantiate a proxy (does not hit the db)
    Cat cat = session.Load<Cat>(id);
    // hit the db to initialize the proxy
    if ( cat.IsDomesticCat ) {
        DomesticCat dc = (DomesticCat) cat; // Error!
        ....
    }
```

Secondly, it is possible to break proxy `==`.

```csharp
    // instantiate a Cat proxy
    Cat cat = session.Load<Cat>(id);

    // acquire new DomesticCat proxy!
    DomesticCat dc = session.Load<DomesticCat>(id);
    Console.WriteLine(cat == dc); // false
```

However, the situation is not quite as bad as it looks. Even though we
now have two references to different proxy objects, the underlying
instance will still be the same object:

```csharp
    cat.Weight = 11.0;  // hit the db to initialize the proxy
    Console.WriteLine( dc.Weight );  // 11.0
```

Third, you may not use a proxy for a `sealed` class or a class with any
non-overridable public members.

Finally, if your persistent object acquires any resources upon
instantiation (eg. in initializers or default constructor), then those
resources will also be acquired by the proxy. The proxy class is an
actual subclass of the persistent class.

These problems are all due to fundamental limitations in .NET's single
inheritance model. If you wish to avoid these problems your persistent
classes must each implement an interface that declares its business
methods. You should specify these interfaces in the mapping file. eg.

```xml
    <class name="CatImpl" proxy="ICat">
        ......
        <subclass name="DomesticCatImpl" proxy="IDomesticCat">
            .....
        </subclass>
    </class>
```

where `CatImpl` implements the interface `ICat` and `DomesticCatImpl`
implements the interface `IDomesticCat`. Then proxies for instances of
`ICat` and `IDomesticCat` may be returned by `Load()` or `Enumerable()`.
(Note that `List()` does not usually return proxies.)

```csharp
    ICat cat = session.Load<CatImpl>(catid);
    using(var iter = session
        .CreateQuery("from CatImpl as cat where cat.Name='fritz'")
        .Enumerable<CatImpl>()
        .GetEnumerator())
    {
        iter.MoveNext();
        ICat fritz = iter.Current;
    }
```

Relationships are also lazily initialized. This means you must declare
any properties to be of type `ICat`, not `CatImpl`.

Certain operations do *not* require proxy initialization

  - `Equals()`, if the persistent class does not override `Equals()`

  - `GetHashCode()`, if the persistent class does not override
    `GetHashCode()`

  - The identifier getter method

NHibernate will detect persistent classes that override `Equals()` or
`GetHashCode()`.

## Initializing collections and proxies <a name="performance-fetching-initialization"></a>

A `LazyInitializationException` will be thrown by NHibernate if an
uninitialized collection or proxy is accessed outside of the scope of
the `ISession`, ie. when the entity owning the collection or having the
reference to the proxy is in the detached state.

Sometimes we need to ensure that a proxy or collection is initialized
before closing the `ISession`. Of course, we can alway force
initialization by calling `cat.Sex` or `cat.Kittens.Count`, for example.
But that is confusing to readers of the code and is not convenient for
generic code.

The static methods `NHibernateUtil.Initialize()` and
`NHibernateUtil.IsInitialized()` provide the application with a
convenient way of working with lazily initialized collections or
proxies. `NHibernateUtil.Initialize(cat)` will force the initialization
of a proxy, `cat`, as long as its `ISession` is still open.
`NHibernateUtil.Initialize( cat.Kittens )` has a similar effect for the
collection of kittens.

Another option is to keep the `ISession` open until all needed
collections and proxies have been loaded. In some application
architectures, particularly where the code that accesses data using
NHibernate, and the code that uses it are in different application
layers or different physical processes, it can be a problem to ensure
that the `ISession` is open when a collection is initialized. There are
two basic ways to deal with this issue:

  - In a web-based application, a `HttpModule` can be used to close the
    `ISession` only at the very end of a user request, once the
    rendering of the view is complete (the *Open Session in View*
    pattern). Of course, this places heavy demands on the correctness of
    the exception handling of your application infrastructure. It is
    vitally important that the `ISession` is closed and the transaction
    ended before returning to the user, even when an exception occurs
    during rendering of the view. See the NHibernate Wiki for examples
    of this "Open Session in View" pattern.

  - In an application with a separate business tier, the business logic
    must "prepare" all collections that will be needed by the web tier
    before returning. This means that the business tier should load all
    the data and return all the data already initialized to the
    presentation/web tier that is required for a particular use case.
    Usually, the application calls `NHibernateUtil.Initialize()` for
    each collection that will be needed in the web tier (this call must
    occur before the session is closed) or retrieves the collection
    eagerly using a NHibernate query with a `FETCH` clause or a
    `FetchMode.Join` in `ICriteria`. This is usually easier if you adopt
    the *Command* pattern instead of a *Session Facade*.

  - You may also attach a previously loaded object to a new `ISession`
    with `Merge()` or `Lock()` before accessing uninitialized
    collections (or other proxies). No, NHibernate does not, and
    certainly *should* not do this automatically, since it would
    introduce ad hoc transaction semantics\!

Sometimes you don't want to initialize a large collection, but still
need some information about it (like its size) or a subset of the data.

You can use a collection filter to get the size of a collection without
initializing it:

```csharp
    s.CreateFilter(collection, "select count(*)").UniqueResult<long>()
```

The `CreateFilter()` method is also used to efficiently retrieve subsets
of a collection without needing to initialize the whole
    collection:

```csharp
    s.CreateFilter(lazyCollection, "").SetFirstResult(0).SetMaxResults(10).List<Entity>();
```

## Using batch fetching <a name="performance-fetching-batch"></a>

NHibernate can make efficient use of batch fetching, that is, NHibernate
can load several uninitialized proxies if one proxy is accessed (or
collections). Batch fetching is an optimization of the lazy select
fetching strategy. There are two ways you can tune batch fetching: on
the class and the collection level.

Batch fetching for classes/entities is easier to understand. Imagine you
have the following situation at runtime: You have 25 `Cat` instances
loaded in an `ISession`, each `Cat` has a reference to its `Owner`, a
`Person`. The `Person` class is mapped with a proxy, `lazy="true"`. If
you now iterate through all cats and call `cat.Owner` on each,
NHibernate will by default execute 25 `SELECT` statements, to retrieve
the proxied owners. You can tune this behavior by specifying a
`batch-size` in the mapping of `Person`:

```xml
    <class name="Person" batch-size="10">...</class>
```

NHibernate will now execute only three queries, the pattern is 10, 10,
5.

You may also enable batch fetching of collections. For example, if each
`Person` has a lazy collection of `Cat`s, and 10 persons are currently
loaded in the `ISesssion`, iterating through all persons will generate
10 `SELECT`s, one for every call to `person.Cats`. If you enable batch
fetching for the `Cats` collection in the mapping of `Person`,
NHibernate can pre-fetch collections:

```xml
    <class name="Person">
        <set name="Cats" batch-size="3">
            ...
        </set>
    </class>
```

With a `batch-size` of 3, NHibernate will load 3, 3, 3, 1 collections in
four `SELECT`s. Again, the value of the attribute depends on the
expected number of uninitialized collections in a particular `Session`.

Batch fetching of collections is particularly useful if you have a
nested tree of items, ie. the typical bill-of-materials pattern.
(Although a *nested set* or a *materialized path* might be a better
option for read-mostly trees.)

*Note:* if you set `default_batch_fetch_size` in configuration,
NHibernate will configure the batch fetch optimization for lazy fetching
globally. Batch sizes specified at more granular level take precedence.

## Using subselect fetching <a name="performance-fetching-subselect"></a>

If one lazy collection or single-valued proxy has to be fetched,
NHibernate loads all of them, re-running the original query in a
subselect. This works in the same way as batch-fetching, without the
piecemeal loading.

# The Second Level Cache <a name="performance-cache"></a>

A NHibernate `ISession` is a transaction-level cache of persistent data.
It is possible to configure a cluster or process-level
(`ISessionFactory`-level) cache on a class-by-class and
collection-by-collection basis. You may even plug in a clustered cache.
Be careful. Caches are never aware of changes made to the persistent
store by another application (though they may be configured to regularly
expire cached data). *In NHibernate 1.x the second level cache does not
work correctly in combination with distributed transactions.*

The second level cache requires the use of transactions, be it through
transaction scopes or NHibernate transactions. Interacting with the data
store without an explicit transaction is discouraged, and will not allow
the second level cache to work as intended.

By default, NHibernate uses HashtableCache for process-level caching.
You may choose a different implementation by specifying the name of a
class that implements `NHibernate.Cache.ICacheProvider` using the
property `cache.provider_class`.

| Cache                                       | Provider class                                                                       | Type         | Cluster Safe | Query Cache Supported |
|---------------------------------------------|--------------------------------------------------------------------------------------|--------------|--------------|-----------------------|
| Hashtable (not intended for production use) | `NHibernate.Cache.HashtableCacheProvider`                                            | memory       |              | yes                   |
| ASP.NET Cache (System.Web.Cache)            | `NHibernate.Caches.SysCache.SysCacheProvider, NHibernate.Caches.SysCache`            | memory       |              | yes                   |
| Prevalence Cache                            | `NHibernate.Caches.Prevalence.PrevalenceCacheProvider, NHibernate.Caches.Prevalence` | memory, disk |              | yes                   |

## Cache mappings <a name="performance-cache-mapping"></a>

The `<cache>` element of a class or collection mapping has the following
form:

```xml
    <cache 
        usage="read-write|nonstrict-read-write|read-only"
        region="RegionName"
    />
```

  - `usage` specifies the caching strategy: `read-write`,
    `nonstrict-read-write` or `read-only`

  - `region` (optional, defaults to the class or collection role name)
    specifies the name of the second level cache region

Alternatively (preferably?), you may specify `<class-cache>` and
`<collection-cache>` elements in `hibernate.cfg.xml`.

The `usage` attribute specifies a *cache concurrency strategy*.

## Strategy: read only <a name="performance-cache-readonly"></a>

If your application needs to read but never modify instances of a
persistent class, a `read-only` cache may be used. This is the simplest
and best performing strategy. Its even perfectly safe for use in a
cluster.

```xml
    <class name="Eg.Immutable" mutable="false">
        <cache usage="read-only"/>
        ....
    </class>
```

## Strategy: read/write <a name="performance-cache-readwrite"></a>

If the application needs to update data, a `read-write` cache might be
appropriate. This cache strategy should never be used if serializable
transaction isolation level is required. You should ensure that the
transaction is completed when `ISession.Close()` or
`ISession.Disconnect()` is called. If you wish to use this strategy in a
cluster, you should ensure that the underlying cache implementation
supports locking. The built-in cache providers do *not*.

```xml
    <class name="eg.Cat" .... >
        <cache usage="read-write"/>
        ....
        <set name="Kittens" ... >
            <cache usage="read-write"/>
            ....
        </set>
    </class>
```

## Strategy: nonstrict read/write <a name="performance-cache-nonstrict"></a>

If the application only occasionally needs to update data (ie. if it is
extremely unlikely that two transactions would try to update the same
item simultaneously) and strict transaction isolation is not required, a
`nonstrict-read-write` cache might be appropriate. When using this
strategy you should ensure that the transaction is completed when
`ISession.Close()` or `ISession.Disconnect()` is called.

The following table shows which providers are compatible with which
concurrency
strategies.

| Cache                                       | read-only | nonstrict-read-write | read-write |
|---------------------------------------------|-----------|----------------------|------------|
| Hashtable (not intended for production use) | yes       | yes                  | yes        |
| SysCache                                    | yes       | yes                  | yes        |
| PrevalenceCache                             | yes       | yes                  | yes        |

Cache Concurrency Strategy Support

Refer to [NHibernate.Caches](caches.md) for more details.

# Managing the caches <a name="performance-sessioncache"></a>

Whenever you pass an object to `Save()`, `Update()` or `SaveOrUpdate()`
and whenever you retrieve an object using `Load()`, `Get()`, `List()`,
or `Enumerable()`, that object is added to the internal cache of the
`ISession`.

When `Flush()` is subsequently called, the state of that object will be
synchronized with the database. If you do not want this synchronization
to occur or if you are processing a huge number of objects and need to
manage memory efficiently, the `Evict()` method may be used to remove
the object and its collections from the first-level cache.

```csharp
    IEnumerable<Cat> cats = sess
        .CreateQuery("from Eg.Cat as cat")
        .List<Cat>(); //a huge result set
    foreach (Cat cat in cats)
    {
        DoSomethingWithACat(cat);
        sess.Evict(cat);
    }
```

NHibernate will evict associated entities automatically if the
association is mapped with `cascade="all"` or
`cascade="all-delete-orphan"`.

The `ISession` also provides a `Contains()` method to determine if an
instance belongs to the session cache.

To completely evict all objects from the session cache, call
`ISession.Clear()`

For the second-level cache, there are methods defined on
`ISessionFactory` for evicting the cached state of an instance, entire
class, collection instance or entire collection role.

```csharp
    //evict a particular Cat
    sessionFactory.Evict(typeof(Cat), catId);
    //evict all Cats
    sessionFactory.Evict(typeof(Cat));
    //evict a particular collection of kittens
    sessionFactory.EvictCollection("Eg.Cat.Kittens", catId);
    //evict all kitten collections
    sessionFactory.EvictCollection("Eg.Cat.Kittens");
```

# The Query Cache <a name="performance-querycache"></a>

Query result sets may also be cached. This is only useful for queries
that are run frequently with the same parameters. To use the query cache
you must first enable it:

```xml
    <property name="cache.use_query_cache">true</property>>
```

This setting causes the creation of two new cache regions - one holding
cached query result sets (`NHibernate.Cache.StandardQueryCache`), the
other holding timestamps of the most recent updates to queryable tables
(`UpdateTimestampsCache`). Those region names will be prefixed by the
cache region prefix if `cache.region_prefix` setting is configured.

If you use a cache provider handling an expiration for cached entries,
you should set the `UpdateTimestampsCache` region expiration to a value
greater than the expiration of query cache regions. (Or disable its
expiration.) Otherwise the query cache may yield stale data.

Note that the query cache does not cache the state of any entities in
the result set; it caches only identifier values and results of value
type. So the query cache should always be used in conjunction with the
second-level cache.

Most queries do not benefit from caching, so by default queries are not
cached. To enable caching, call `IQuery.SetCacheable(true)`. This call
allows the query to look for existing cache results or add its results
to the cache when it is executed.

If you require fine-grained control over query cache expiration
policies, you may specify a named cache region for a particular query by
calling `IQuery.SetCacheRegion()`.

```csharp
    var blogs = sess.CreateQuery("from Blog blog where blog.Blogger = :blogger")
        .SetEntity("blogger", blogger)
        .SetMaxResults(15)
        .SetCacheable(true)
        .SetCacheRegion("frontpages")
        .List<Blog>();
```

If the query should force a refresh of its query cache region, you may
call `IQuery.SetForceCacheRefresh()` to `true`. This is particularly
useful in cases where underlying data may have been updated via a
separate process (i.e., not modified through NHibernate) and allows the
application to selectively refresh the query cache regions based on its
knowledge of those events. This is a more efficient alternative to
eviction of a query cache region via `ISessionFactory.EvictQueries()`.

# Understanding Collection performance <a name="performance-collections"></a>

We've already spent quite some time talking about collections. In this
section we will highlight a couple more issues about how collections
behave at runtime.

## Taxonomy <a name="performance-collections-taxonomy"></a>

NHibernate defines three basic kinds of collections:

  - collections of values

  - one to many associations

  - many to many associations

This classification distinguishes the various table and foreign key
relationships but does not tell us quite everything we need to know
about the relational model. To fully understand the relational structure
and performance characteristics, we must also consider the structure of
the primary key that is used by NHibernate to update or delete
collection rows. This suggests the following classification:

  - indexed collections

  - sets

  - bags

All indexed collections (maps, lists, arrays) have a primary key
consisting of the `<key>` and `<index>` columns. In this case collection
updates are usually extremely efficient - the primary key may be
efficiently indexed and a particular row may be efficiently located when
NHibernate tries to update or delete it.

Sets have a primary key consisting of `<key>` and element columns. This
may be less efficient for some types of collection element, particularly
composite elements or large text or binary fields; the database may not
be able to index a complex primary key as efficiently. On the other
hand, for one to many or many to many associations, particularly in the
case of synthetic identifiers, it is likely to be just as efficient.
(Side-note: if you want `SchemaExport` to actually create the primary
key of a `<set>` for you, you must declare all columns as
`not-null="true"`.)

`<idbag>` mappings define a surrogate key, so they are always very
efficient to update. In fact, they are the best case.

Bags are the worst case. Since a bag permits duplicate element values
and has no index column, no primary key may be defined. NHibernate has
no way of distinguishing between duplicate rows. NHibernate resolves
this problem by completely removing (in a single `DELETE`) and
recreating the collection whenever it changes. This might be very
inefficient.

Note that for a one-to-many association, the "primary key" may not be
the physical primary key of the database table - but even in this case,
the above classification is still useful. (It still reflects how
NHibernate "locates" individual rows of the
collection.)

## Lists, maps, idbags and sets are the most efficient collections to update <a name="performance-collections-mostefficientupdate"></a>

From the discussion above, it should be clear that indexed collections
and (usually) sets allow the most efficient operation in terms of
adding, removing and updating elements.

There is, arguably, one more advantage that indexed collections have
over sets for many to many associations or collections of values.
Because of the structure of an `ISet`, NHibernate doesn't ever `UPDATE`
a row when an element is "changed". Changes to an `ISet` always work via
`INSERT` and `DELETE` (of individual rows). Once again, this
consideration does not apply to one to many associations.

After observing that arrays cannot be lazy, we would conclude that
lists, maps and idbags are the most performant (non-inverse) collection
types, with sets not far behind. Sets are expected to be the most common
kind of collection in NHibernate applications. This is because the "set"
semantics are most natural in the relational model.

However, in well-designed NHibernate domain models, we usually see that
most collections are in fact one-to-many associations with
`inverse="true"`. For these associations, the update is handled by the
many-to-one end of the association, and so considerations of collection
update performance simply do not apply.

## Bags and lists are the most efficient inverse collections <a name="performance-collections-mostefficentinverse"></a>

Just before you ditch bags forever, there is a particular case in which
bags (and also lists) are much more performant than sets. For a
collection with `inverse="true"` (the standard bidirectional one-to-many
relationship idiom, for example) we can add elements to a bag or list
without needing to initialize (fetch) the bag elements\! This is because
`IList.Add()` must always succeed for a bag or `IList` (unlike an
`ISet`). This can make the following common code much faster.

```csharp
    Parent p = sess.Load<Parent>(id);
        Child c = new Child();
        c.Parent = p;
        p.Children.Add(c);  //no need to fetch the collection!
        sess.Flush();
```

## One shot delete <a name="performance-collections-oneshotdelete"></a>

Occasionally, deleting collection elements one by one can be extremely
inefficient. NHibernate isn't completely stupid, so it knows not to do
that in the case of an newly-empty collection (if you called
`list.Clear()`, for example). In this case, NHibernate will issue a
single `DELETE` and we are done\!

Suppose we add a single element to a collection of size twenty and then
remove two elements. NHibernate will issue one `INSERT` statement and
two `DELETE` statements (unless the collection is a bag). This is
certainly desirable.

However, suppose that we remove eighteen elements, leaving two and then
add thee new elements. There are two possible ways to proceed:

  - Delete eighteen rows one by one and then insert three rows

  - Remove the whole collection (in one SQL `DELETE`) and insert all
    five current elements (one by one)

NHibernate isn't smart enough to know that the second option is probably
quicker in this case. (And it would probably be undesirable for
NHibernate to be that smart; such behaviour might confuse database
triggers, etc.)

Fortunately, you can force this behaviour (ie. the second strategy) at
any time by discarding (ie. dereferencing) the original collection and
returning a newly instantiated collection with all the current elements.
This can be very useful and powerful from time to time.

Of course, one-shot-delete does not apply to collections mapped
`inverse="true"`.

# Batch updates <a name="performance-batch-updates"></a>

NHibernate supports batching SQL update commands (`INSERT`, `UPDATE`,
`DELETE`) with the following limitations:

  - the NHibernate's drive used for your RDBMS may not supports
    batching,

  - since the implementation uses reflection to access members and types
    in System.Data assembly which are not normally visible, it may not
    function in environments where necessary permissions are not
    granted,

  - optimistic concurrency checking may be impaired since `ADO.NET` 2.0
    does not return the number of rows affected by each statement in the
    batch, only the total number of rows affected by the batch.

Update batching is enabled by setting `adonet.batch_size` to a non-zero
value.

# Multi Query <a name="performance-multi-query"></a>

This functionality allows you to execute several HQL queries in one
round-trip against the database server. A simple use case is executing a
paged query while also getting the total count of results, in a single
round-trip. Here is a simple example:

```csharp
    IMultiQuery multiQuery = s.CreateMultiQuery()
        .Add(s.CreateQuery("from Item i where i.Id > ?")
              .SetInt32(0, 50).SetFirstResult(10))
        .Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
              .SetInt32(0, 50));
    IList results = multiQuery.List();
    IList items = (IList)results[0];
    long count = (long)((IList)results[1])[0];
```

The result is a list of query results, ordered according to the order of
queries added to the multi query. Named parameters can be set on the
multi query, and are shared among all the queries contained in the multi
query, like this:

```csharp
    IList results = s.CreateMultiQuery()
        .Add(s.CreateQuery("from Item i where i.Id > :id")
              .SetFirstResult(10))
        .Add("select count(*) from Item i where i.Id > :id")
        .SetInt32("id", 50)
        .List();
    IList items = (IList)results[0];
    long count = (long)((IList)results[1])[0];
```

Positional parameters are not supported on the multi query, only on the
individual queries.

As shown above, if you do not need to configure the query separately,
you can simply pass the HQL directly to the `IMultiQuery.Add()` method.

Multi query is executed by concatenating the queries and sending the
query to the database as a single string. This means that the database
should support returning several result sets in a single query. At the
moment this functionality is only enabled for Microsoft SQL Server and
SQLite.

Note that the database server is likely to impose a limit on the maximum
number of parameters in a query, in which case the limit applies to the
multi query as a whole. Queries using `in` with a large number of
arguments passed as parameters may easily exceed this limit. For
example, SQL Server has a limit of 2,100 parameters per round-trip, and
will throw an exception executing this query:

```csharp
    IList allEmployeesId  = ...; //1,500 items
    IMultiQuery multiQuery = s.CreateMultiQuery()
        .Add(s.CreateQuery("from Employee e where e.Id in :empIds")
              .SetParameter("empIds", allEmployeesId).SetFirstResult(10))
        .Add(s.CreateQuery("select count(*) from Employee e where e.Id in :empIds")
              .SetParameter("empIds", allEmployeesId));
    IList results = multiQuery.List(); // will throw an exception from SQL Server
```

An interesting usage of this feature is to load several collections of
an object in one round-trip, without an expensive cartesian product
(blog \* users \* posts).

```csharp
    Blog blog = s.CreateMultiQuery()
        .Add("select b from Blog b left join fetch b.Users where b.Id = :id")
        .Add("select b from Blog b left join fetch b.Posts where b.Id = :id")
        .SetInt32("id", 123)
        .UniqueResult<Blog>();
```

# Multi Criteria <a name="performance-multi-criteria"></a>

This is the counter-part to Multi Query, and allows you to perform
several criteria queries in a single round trip. A simple use case is
executing a paged query while also getting the total count of results,
in a single round-trip. Here is a simple example:

```csharp
    IMultiCriteria multiCrit = s.CreateMultiCriteria()
        .Add(s.CreateCriteria(typeof(Item))
              .Add(Expression.Gt("Id", 50))
              .SetFirstResult(10))
        .Add(s.CreateCriteria(typeof(Item))
              .Add(Expression.Gt("Id", 50))
              .SetProject(Projections.RowCount()));
    IList results = multiCrit.List();
    IList items = (IList)results[0];
    long count = (long)((IList)results[1])[0];
```

The result is a list of query results, ordered according to the order of
queries added to the multi criteria.

You can add `ICriteria` or `DetachedCriteria` to the Multi Criteria
query. In fact, using DetachedCriteria in this fashion has some
interesting
    implications.

```csharp
    DetachedCriteria customersCriteria = AuthorizationService.GetAssociatedCustomersQuery();
    IList results = session.CreateMultiCriteria()
        .Add(customersCriteria)
        .Add(DetachedCriteria.For<Policy>()
            .Add(Subqueries.PropertyIn("id",
                CriteriaTransformer.Clone(customersCriteria)
                    .SetProjection(Projections.Id())
                )))
        .List();
    
    ICollection<Customer> customers = CollectionHelper.ToArray<Customer>(results[0]);
    ICollection<Policy> policies = CollectionHelper.ToArray<Policy>(results[1]);
```

As you see, we get a query that represents the customers we can access,
and then we can utilize this query further in order to perform
additional logic (getting the policies of the customers we are
associated with), all in a single database round-trip.
