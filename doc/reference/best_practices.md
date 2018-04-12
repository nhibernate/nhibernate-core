# Best Practices

  - Write fine-grained classes and map them using `<component>`.  
    Use an `Address` class to encapsulate `street`, `suburb`, `state`,
    `postcode`. This encourages code reuse and simplifies refactoring.

  - Declare identifier properties on persistent classes.  
    NHibernate makes identifier properties optional. There are all sorts
    of reasons why you should use them. We recommend that identifiers be
    'synthetic' (generated, with no business meaning) and of a
    non-primitive type. For maximum flexibility, use `Int64` or
    `String`.

  - Place each class mapping in its own file.  
    Don't use a single monolithic mapping document. Map `Eg.Foo` in the
    file `Eg/Foo.hbm.xml`. This makes particularly good sense in a team
    environment.

  - Embed mappings in assemblies.  
    Place mapping files along with the classes they map and declare them
    as `Embedded Resource`s in Visual Studio.

  - Consider externalising query strings.  
    This is a good practice if your queries call non-ANSI-standard SQL
    functions. Externalising the query strings to mapping files will
    make the application more portable.

  - Use parameters.  
    As in `ADO.NET` , always replace non-constant values by "?". Never use
    string manipulation to bind a non-constant value in a query\! Even
    better, consider using named parameters in queries.

  - Don't manage your own `ADO.NET` connections.  
    NHibernate lets the application manage `ADO.NET` connections. This
    approach should be considered a last-resort. If you can't use the
    built-in connections providers, consider providing your own
    implementation of `NHibernate.Connection.IConnectionProvider`.

  - Consider using a custom type.  
    Suppose you have a type, say from some library, that needs to be
    persisted but doesn't provide the accessors needed to map it as a
    component. You should consider implementing
    `NHibernate.UserTypes.IUserType`. This approach frees the
    application code from implementing transformations to / from an
    NHibernate type.

  - Use hand-coded `ADO.NET` in bottlenecks.  
    In performance-critical areas of the system, some kinds of
    operations (eg. mass update / delete) might benefit from direct
    `ADO.NET` . But please, wait until you *know* something is a
    bottleneck. And don't assume that direct `ADO.NET` is necessarily
    faster. If need to use direct `ADO.NET` , it might be worth opening a
    NHibernate `ISession` and using that SQL connection. That way you
    can still use the same transaction strategy and underlying
    connection provider.

  - Understand `ISession` flushing.  
    From time to time the ISession synchronizes its persistent state
    with the database. Performance will be affected if this process
    occurs too often. You may sometimes minimize unnecessary flushing by
    disabling automatic flushing or even by changing the order of
    queries and other operations within a particular transaction.

  - In a three tiered architecture, consider using `SaveOrUpdate()`.  
    When using a distributed architecture, you could pass persistent
    objects loaded in the middle tier to and from the user interface
    tier. Use a new session to service each request. Use
    `ISession.Update()` or `ISession.SaveOrUpdate()` to update the
    persistent state of an object.

  - In a two tiered architecture, consider using session
    disconnection.  
    Database Transactions have to be as short as possible for best
    scalability. However, it is often necessary to implement long
    running Application Transactions, a single unit-of-work from the
    point of view of a user. This Application Transaction might span
    several client requests and response cycles. Either use Detached
    Objects or, in two tiered architectures, simply disconnect the
    NHibernate Session from the `ADO.NET` connection and reconnect it for
    each subsequent request. Never use a single Session for more than
    one Application Transaction use-case, otherwise, you will run into
    stale data.

  - Don't treat exceptions as recoverable.  
    This is more of a necessary practice than a "best" practice. When an
    exception occurs, roll back the `ITransaction` and close the
    `ISession`. If you don't, NHibernate can't guarantee that in-memory
    state accurately represents persistent state. As a special case of
    this, do not use `ISession.Load()` to determine if an instance with
    the given identifier exists on the database; use `Get()` or a query
    instead.

  - Prefer lazy fetching for associations.  
    Use eager (outer-join) fetching sparingly. Use proxies and/or lazy
    collections for most associations to classes that are not cached in
    the second-level cache. For associations to cached classes, where
    there is a high probability of a cache hit, explicitly disable eager
    fetching using `fetch="select"`. When an outer-join fetch is
    appropriate to a particular use case, use a query with a `left join
    fetch`.

  - Consider abstracting your business logic from NHibernate.  
    Hide (NHibernate) data-access code behind an interface. Combine the
    *DAO* and *Thread Local Session* patterns. You can even have some
    classes persisted by hand-coded `ADO.NET` , associated to NHibernate
    via an `IUserType`. (This advice is intended for "sufficiently
    large" applications; it is not appropriate for an application with
    five tables\!)

  - Implement `Equals()` and `GetHashCode()` using a unique business
    key.  
    If you compare objects outside of the ISession scope, you have to
    implement `Equals()` and `GetHashCode()`. Inside the ISession scope,
    object identity is guaranteed. If you implement these methods, never
    ever use the database identifier\! A transient object doesn't have
    an identifier value and NHibernate would assign a value when the
    object is saved. If the object is in an ISet while being saved, the
    hash code changes, breaking the contract. To implement `Equals()`
    and `GetHashCode()`, use a unique business key, that is, compare a
    unique combination of class properties. Remember that this key has
    to be stable and unique only while the object is in an ISet, not for
    the whole lifetime (not as stable as a database primary key). Never
    use collections in the `Equals()` comparison (lazy loading) and be
    careful with other associated classes that might be proxied.

  - Don't use exotic association mappings.  
    Good use-cases for a real many-to-many associations are rare. Most
    of the time you need additional information stored in the "link
    table". In this case, it is much better to use two one-to-many
    associations to an intermediate link class. In fact, we think that
    most associations are one-to-many and many-to-one, you should be
    careful when using any other association style and ask yourself if
    it is really necessary.
