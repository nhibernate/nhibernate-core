# Transactions And Concurrency

NHibernate is not itself a database. It is a lightweight
object-relational mapping tool. Transaction management is delegated to
the underlying database connection. If the connection is enlisted with a
distributed transaction, operations performed by the `ISession` are
atomically part of the wider distributed transaction. NHibernate can be
seen as a thin adapter to `ADO.NET` , adding object-oriented semantics.

# Configurations, Sessions and Factories

An `ISessionFactory` is an expensive-to-create, threadsafe object
intended to be shared by all application threads. An `ISession` is an
inexpensive, non-threadsafe object that should be used once, for a
single business process, and then discarded. For example, when using
NHibernate in an ASP.NET application, pages could obtain an
`ISessionFactory` using:

```csharp
    ISessionFactory sf = Global.SessionFactory;
```

Each call to a service method could create a new `ISession`, `Flush()`
it, `Commit()` its transaction, `Close()` it and finally discard it.
(The `ISessionFactory` may also be kept in a static *Singleton* helper
variable.)

We use the NHibernate `ITransaction` API as discussed previously, a
single `Commit()` of a NHibernate `ITransaction` flushes the state and
commits any underlying database connection. Transaction scopes can be
used instead, which allows the transaction to be distributed. See
[Transaction scopes (System.Transactions)](#transaction-scopes-systemtransactions).

NHibernate may be used without managing transactions neither with the
`ITransaction` API nor with transaction scopes. We advise against such a
pattern, because this causes each single interaction of the session with
the database to be a transaction on its own. This causes overhead,
breaks the unit of work in case of errors during flushing, and causes
some features to be disabled like auto-flush and second level cache. If
the application encounters locks contention or deadlocks, investigate
lowering the isolation level (like enabling read committed snapshot with
SQL Server) or shortening your transactions rather than removing them.

Ensure you understand the semantics of `Flush()`. Flushing synchronizes
the persistent store with in-memory changes but *not* vice-versa. Note
that for all NHibernate `ADO.NET` connections/transactions, the
transaction isolation level for that connection applies to all
operations executed by NHibernate\!

The next few sections will discuss alternative approaches that utilize
versioning to ensure transaction atomicity. These are considered
"advanced" approaches to be used with care.

# Threads and connections

You should observe the following practices when creating NHibernate
Sessions:

  - Never create more than one concurrent `ISession` or `ITransaction`
    instance per database connection.

  - Be extremely careful when creating more than one `ISession` per
    database per transaction. The `ISession` itself keeps track of
    updates made to loaded objects, so a different `ISession` might see
    stale data.

  - The `ISession` is *not* threadsafe\! Never access the same
    `ISession` in two concurrent threads. An `ISession` is usually only
    a single unit-of-work\!
    
    Since NHibernate 5.0, the session and its queries IO bound methods
    have async counterparts. Each call to an async method must be
    awaited before further interacting with the session or its queries.

# Considering object identity

The application may concurrently access the same persistent state in two
different units-of-work. However, an instance of a persistent class is
never shared between two `ISession` instances. Hence there are two
different notions of identity:

  - Database Identity  
    `foo.Id.Equals( bar.Id )`

  - CLR Identity  
    `foo == bar`

Then for objects attached to a *particular* `Session`, the two notions
are equivalent. However, while the application might concurrently access
the "same" (persistent identity) business object in two different
sessions, the two instances will actually be "different" (CLR identity).

This approach leaves NHibernate and the database to worry about
concurrency. The application never needs to synchronize on any business
object, as long as it sticks to a single thread per `ISession` or object
identity (within an `ISession` the application may safely use `==` to
compare objects).

# Optimistic concurrency control

Many business processes require a whole series of interactions with the
user interleaved with database accesses. In web and enterprise
applications it is not acceptable for a database transaction to span a
user interaction.

Maintaining isolation of business processes becomes the partial
responsibility of the application tier, hence we call this process a
long running *application transaction*. A single application transaction
usually spans several database transactions. It will be atomic if only
one of these database transactions (the last one) stores the updated
data, all others simply read data.

The only approach that is consistent with high concurrency and high
scalability is optimistic concurrency control with versioning.
NHibernate provides for three possible approaches to writing application
code that uses optimistic concurrency.

## Long session with automatic versioning

A single `ISession` instance and its persistent instances are used for
the whole application transaction.

The `ISession` uses optimistic locking with versioning to ensure that
many database transactions appear to the application as a single logical
application transaction. The `ISession` is disconnected from any
underlying `ADO.NET` connection when waiting for user interaction. This
approach is the most efficient in terms of database access. The
application need not concern itself with version checking or with
reattaching detached instances.

```csharp
    // foo is an instance loaded earlier by the Session
    session.Reconnect();
    using (var transaction = session.BeginTransaction())
    {
        foo.Property = "bar";
        session.Flush();
        transaction.Commit();
    }
    session.Disconnect();
```

The `foo` object still knows which `ISession` it was loaded it. As soon
as the `ISession` has an `ADO.NET` connection, we commit the changes to
the object.

This pattern is problematic if our `ISession` is too big to be stored
during user think time, e.g. an `HttpSession` should be kept as small as
possible. As the `ISession` is also the (mandatory) first-level cache
and contains all loaded objects, we can probably use this strategy only
for a few request/response cycles. This is indeed recommended, as the
`ISession` will soon also have stale data.

## Many sessions with automatic versioning

Each interaction with the persistent store occurs in a new `ISession`.
However, the same persistent instances are reused for each interaction
with the database. The application manipulates the state of detached
instances originally loaded in another `ISession` and then
"re-associates" them using `ISession.Update()` or
`ISession.SaveOrUpdate()`.

```csharp
    // foo is an instance loaded by a previous Session
    foo.Property = "bar";
    using (var session = factory.OpenSession())
    using (var transaction = session.BeginTransaction())
    {
        session.SaveOrUpdate(foo);
        session.Flush();
        transaction.Commit();
    }
```

You may also call `Lock()` instead of `Update()` and use `LockMode.Read`
(performing a version check, bypassing all caches) if you are sure that
the object has not been modified.

## Customizing automatic versioning

You may disable NHibernate's automatic version increment for particular
properties and collections by setting the `optimistic-lock` mapping
attribute to `false`. NHibernate will then no longer increment versions
if the property is dirty.

Legacy database schemas are often static and can't be modified. Or,
other applications might also access the same database and don't know
how to handle version numbers or even timestamps. In both cases,
versioning can't rely on a particular column in a table. To force a
version check without a version or timestamp property mapping, with a
comparison of the state of all fields in a row, turn on
`optimistic-lock="all"` in the `<class>` mapping. Note that this
conceptually only works if NHibernate can compare the old and new state,
i.e. if you use a single long `ISession` and not
session-per-request-with-detached-objects.

Sometimes concurrent modification can be permitted as long as the
changes that have been made don't overlap. If you set
`optimistic-lock="dirty"` when mapping the `<class>`, NHibernate will
only compare dirty fields during flush.

In both cases, with dedicated version/timestamp columns or with
full/dirty field comparison, NHibernate uses a single `UPDATE` statement
(with an appropriate `WHERE` clause) per entity to execute the version
check and update the information. If you use transitive persistence to
cascade reattachment to associated entities, NHibernate might execute
unnecessary updates. This is usually not a problem, but *on update*
triggers in the database might be executed even when no changes have
been made to detached instances. You can customize this behavior by
setting `select-before-update="true"` in the `<class>` mapping, forcing
NHibernate to `SELECT` the instance to ensure that changes did actually
occur, before updating the row.

## Application version checking

Each interaction with the database occurs in a new `ISession` that
reloads all persistent instances from the database before manipulating
them. This approach forces the application to carry out its own version
checking to ensure application transaction isolation. (Of course,
NHibernate will still *update* version numbers for you.) This approach
is the least efficient in terms of database access.

```csharp
    // foo is an instance loaded by a previous Session
    using (var session = factory.OpenSession())
    using (var transaction = session.BeginTransaction())
    {
        int oldVersion = foo.Version;
        session.Load( foo, foo.Key );
        if ( oldVersion != foo.Version ) throw new StaleObjectStateException();
        foo.Property = "bar";
        session.Flush();
        transaction.Commit();
    }
```

Of course, if you are operating in a low-data-concurrency environment
and don't require version checking, you may use this approach and just
skip the version check.

# Session disconnection

The first approach described above is to maintain a single `ISession`
for a whole business process that spans user think time. (For example, a
servlet might keep an `ISession` in the user's `HttpSession`.) For
performance reasons you should

1.  commit the `ITransaction` and then

2.  disconnect the `ISession` from the `ADO.NET` connection

before waiting for user activity. The method `ISession.Disconnect()`
will disconnect the session from the `ADO.NET` connection and return the
connection to the pool (unless you provided the connection).

`ISession.Reconnect()` obtains a new connection (or you may supply one)
and restarts the session. After reconnection, to force a version check
on data you aren't updating, you may call `ISession.Lock()` on any
objects that might have been updated by another transaction. You don't
need to lock any data that you *are* updating.

Here is an example:

```csharp
    ISessionFactory sessions;
    IList<Foo> fooList;
    Bar bar;
    ....
    ISession s = sessions.OpenSession();
    ITransaction tx = null;
    
    try
    {
        tx = s.BeginTransaction())
    
        fooList = s
            .CreateQuery(
                "select foo from Eg.Foo foo where foo.Date = current date"
                // uses db2 date function
            ).List<Foo>();
    
        bar = new Bar();
        s.Save(bar);
    
        tx.Commit();
    }
    catch (Exception)
    {
        if (tx != null) tx.Rollback();
        s.Close();
        throw;
    }
    s.Disconnect();
```

Later on:

```csharp
    s.Reconnect();
    
    try
    {
        tx = s.BeginTransaction();
    
        bar.FooTable = new HashMap();
        foreach (Foo foo in fooList)
        {
            s.Lock(foo, LockMode.Read);    //check that foo isn't stale
            bar.FooTable.Put( foo.Name, foo );
        }
    
        tx.Commit();
    }
    catch (Exception)
    {
        if (tx != null) tx.Rollback();
        throw;
    }
    finally
    {
        s.Close();
    }
```

You can see from this how the relationship between `ITransaction`s and
`ISession`s is many-to-one, An `ISession` represents a conversation
between the application and the database. The `ITransaction` breaks that
conversation up into atomic units of work at the database level.

# Pessimistic Locking

It is not intended that users spend much time worrying about locking
strategies. It's usually enough to specify an isolation level for the
`ADO.NET` connections and then simply let the database do all the work.
However, advanced users may sometimes wish to obtain exclusive
pessimistic locks, or re-obtain locks at the start of a new transaction.

NHibernate will always use the locking mechanism of the database, never
lock objects in memory\!

The `LockMode` class defines the different lock levels that may be
acquired by NHibernate. A lock is obtained by the following mechanisms:

  - `LockMode.Write` is acquired automatically when NHibernate updates
    or inserts a row.

  - `LockMode.Upgrade` may be acquired upon explicit user request using
    `SELECT ... FOR UPDATE` on databases which support that syntax.

  - `LockMode.UpgradeNoWait` may be acquired upon explicit user request
    using a `SELECT ... FOR UPDATE NOWAIT` under Oracle.

  - `LockMode.Read` is acquired automatically when NHibernate reads data
    under Repeatable Read or Serializable isolation level. May be
    re-acquired by explicit user request.

  - `LockMode.None` represents the absence of a lock. All objects switch
    to this lock mode at the end of an `ITransaction`. Objects
    associated with the session via a call to `Update()` or
    `SaveOrUpdate()` also start out in this lock mode.

The "explicit user request" is expressed in one of the following ways:

  - A call to `ISession.Load()`, specifying a `LockMode`.

  - A call to `ISession.Lock()`.

  - A call to `IQuery.SetLockMode()`.

If `ISession.Load()` is called with `Upgrade` or `UpgradeNoWait`, and
the requested object was not yet loaded by the session, the object is
loaded using `SELECT ... FOR UPDATE`. If `Load()` is called for an
object that is already loaded with a less restrictive lock than the one
requested, NHibernate calls `Lock()` for that object.

`ISession.Lock()` performs a version number check if the specified lock
mode is `Read`, `Upgrade` or `UpgradeNoWait`. (In the case of `Upgrade`
or `UpgradeNoWait`, `SELECT ... FOR UPDATE` is used.)

If the database does not support the requested lock mode, NHibernate
will use an appropriate alternate mode (instead of throwing an
exception). This ensures that applications will be portable.

# Connection Release Modes

The legacy (1.0.x) behavior of NHibernate in regards to `ADO.NET` 
connection management was that a `ISession` would obtain a connection
when it was first needed and then hold unto that connection until the
session was closed. NHibernate introduced the notion of connection
release modes to tell a session how to handle its `ADO.NET` connections.
Note that the following discussion is pertinent only to connections
provided through a configured `IConnectionProvider`; user-supplied
connections are outside the breadth of this discussion. The different
release modes are identified by the enumerated values of
`NHibernate.ConnectionReleaseMode`:

  - `OnClose` - is essentially the legacy behavior described above. The
    NHibernate session obtains a connection when it first needs to
    perform some database access and holds unto that connection until
    the session is closed.

  - `AfterTransaction` - says to release connections after a
    `NHibernate.ITransaction` has completed.

The configuration parameter `connection.release_mode` is used to specify
which release mode to use. The possible values:

  - `auto` (the default) - equivalent to `after_transaction` in the
    current release. It is rarely a good idea to change this default
    behavior as failures due to the value of this setting tend to
    indicate bugs and/or invalid assumptions in user code.

  - `on_close` - says to use `ConnectionReleaseMode.OnClose`. This
    setting is left for backwards compatibility, but its use is highly
    discouraged.

  - `after_transaction` - says to use
    `ConnectionReleaseMode.AfterTransaction`. Note that with
    `ConnectionReleaseMode.AfterTransaction`, if a session is considered
    to be in auto-commit mode (i.e. no transaction was started)
    connections will be released after every operation.

If your application manages transactions through .NET APIs such as
`System.Transactions` library while not using a compatible transaction
factory (see `transaction.factory_class` in
[Optional configuration properties](configuration.md#optional-configuration-properties)),
`ConnectionReleaseMode.AfterTransaction` may cause NHibernate to open
and close several connections during one transaction, leading to
unnecessary overhead and transaction promotion from local to
distributed. Specifying `ConnectionReleaseMode.OnClose` will revert to
the legacy behavior and prevent this problem from occurring.

# Transaction scopes (System.Transactions)

Instead of using NHibernate `ITransaction`, `TransactionScope` can be
used. Please do not use both simultaneously. Using `TransactionScope`
requires using a compatible transaction factory (see
`transaction.factory_class` in [Optional configuration properties](configuration.md#optional-configuration-properties)). The
default transaction factory supports scopes.

When using `TransactionScope` with NHibernate, you need to be aware of
following points:

  - The session will enlist with the first scope in which the session is
    used (or opened). As of NHibernate v5.0, it will enlist its
    connection in the transaction regardless of connection string
    `Enlist` setting. Prior to v5.0, it was relying on that setting
    being considered `true`, and on acquiring the connection within the
    scope.
    
    Sub-scopes are not supported. The session will be enlisted in the
    first scope within which it was used, until this scope is committed
    or rollback. If auto-enlistment is enabled on the connection and the
    session used on others scopes than the one in which it is currently
    enlisted, the connection may enlist in another scope, and the
    session will then fail to use it.
    
    As of NHibernate v5.0, session auto-enlistment can be disabled from
    the session builder obtained with `ISessionFactory.WithOptions()`,
    using the `AutoJoinTransaction` option. The connection may still
    enlist itself if connection string `Enlist` setting is not `false`.
    A session can explicitly join the current system transaction by
    calling `ISession.JoinTransaction()`.

  - As of NHibernate v5.0, `FlushMode.Commit` requires the configuration
    setting `transaction.use_connection_on_system_events` to be true for
    flushing from transaction scope commit. Otherwise, it will be your
    responsibility to flush the session before completing the scope.
    
    Using `transaction.use_connection_on_system_events` can cause
    undesired transaction promotions to distributed: it requires using a
    dedicated connection for flushing, and it delays session disposal
    (if done inside the scope) to the scope disposal. If you want to
    avoid this, set this setting to `false` and manually flush your
    sessions.

  - As of NHibernate v5.0, `ConnectionReleaseMode.AfterTransaction` has
    no more by default an "immediate" effect with transaction scopes.
    Previously, it was releasing the connection from transaction
    completion events. But this is not officially supported by Microsoft
    and this can cause issues especially with distributed transactions.
    
    Since v5.0, by default, the connection will be actually released
    after the scope disposal at the first session usage involving a
    connection, or at the session closing, whichever come first.
    Alternatively, you may `Disconnect()` the session. (Requires
    `Reconnect()` before re-using the session.)
    
    When using `transaction.use_connection_on_system_events`, if the
    session is disposed within the scope, the connection releasing will
    still occurs from transaction completion event.

  - As of NHibernate v5.0, using transaction scope and trying to use the
    session connection within `AfterTransactionCompletion` is forbidden
    and will raise an exception. If the setting
    `transaction.use_connection_on_system_events` is `false`, it will
    forbid any connection usage from `BeforeTransactionCompletion` event
    too, when this event is triggered by a transaction scope commit or
    rollback.
