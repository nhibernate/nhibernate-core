# Manipulating Persistent Data

# Creating a persistent object <a name="manipulatingdata-creating"></a>

An object (entity instance) is either *transient* or *persistent* with
respect to a particular `ISession`. Newly instantiated objects are, of
course, transient. The session offers services for saving (ie.
persisting) transient instances:

```csharp
    DomesticCat fritz = new DomesticCat();
    fritz.Color = Color.Ginger;
    fritz.Sex = 'M';
    fritz.Name = "Fritz";
    long generatedId = (long) sess.Save(fritz);

    DomesticCat pk = new DomesticCat();
    pk.Color = Color.Tabby;
    pk.Sex = 'F';
    pk.Name = "PK";
    pk.Kittens = new HashSet<Cat>();
    pk.AddKitten(fritz);
    sess.Save( pk, 1234L );
```

The single-argument `Save()` generates and assigns a unique identifier
to `fritz`. The two-argument form attempts to persist `pk` using the
given identifier. We generally discourage the use of the two-argument
form since it may be used to create primary keys with business meaning.

Associated objects may be made persistent in any order you like unless
you have a `NOT NULL` constraint upon a foreign key column. There is
never a risk of violating foreign key constraints. However, you might
violate a `NOT NULL` constraint if you `Save()` the objects in the wrong
order.

# Loading an object <a name="manipulatingdata-loading"></a>

The `Load()` methods of `ISession` give you a way to retrieve a
persistent instance if you already know its identifier. One version
takes a class object and will load the state into a newly instantiated
object. The second version allows you to supply an instance into which
the state will be loaded. The form which takes an instance is only
useful in special circumstances (DIY instance pooling etc.)

```csharp
    Cat fritz = sess.Load<Cat>(generatedId);

    long pkId = 1234;
    DomesticCat pk = sess.Load<DomesticCat>(pkId);

    Cat cat = new DomesticCat();
    // load pk's state into cat
    sess.Load( cat, pkId );
    var kittens = cat.Kittens;
```

Note that `Load()` will throw an unrecoverable exception if there is no
matching database row. If the class is mapped with a proxy, `Load()`
returns an object that is an uninitialized proxy and does not actually
hit the database until you invoke a method of the object. This behaviour
is very useful if you wish to create an association to an object without
actually loading it from the database.

If you are not certain that a matching row exists, you should use the
`Get()` method, which hits the database immediately and returns null if
there is no matching row.

```csharp
    Cat cat = sess.Get<Cat>(id);
    if (cat==null) {
        cat = new Cat();
        sess.Save(cat, id);
    }
    return cat;
```

You may also load an objects using an SQL `SELECT ... FOR UPDATE`. See
the next section for a discussion of NHibernate `LockMode`s.

```csharp
    Cat cat = sess.Get<Cat>(id, LockMode.Upgrade);
```

Note that any associated instances or contained collections are *not*
selected `FOR UPDATE`.

It is possible to re-load an object and all its collections at any time,
using the `Refresh()` method. This is useful when database triggers are
used to initialize some of the properties of the object.

```csharp
    sess.Save(cat);
    sess.Flush(); //force the SQL INSERT
    sess.Refresh(cat); //re-read the state (after the trigger executes)
```

An important question usually appears at this point: How much does
NHibernate load from the database and how many SQL `SELECT`s will it
use? This depends on the *fetching strategy* and is explained in
[Fetching strategies](performance.md#fetching-strategies).

# Querying <a name="manipulatingdata-querying"></a>

If you don't know the identifier(s) of the object(s) you are looking
for, use the `CreateQuery()` method of `ISession`. NHibernate supports a
simple but powerful object oriented query language.

```csharp
    IList<Cat> cats = sess
        .CreateQuery("from Cat as cat where cat.Birthdate = ?")
        .SetDateTime(0, date)
        .List<Cat>();
    
    var mates = sess
        .CreateQuery("select mate from Cat as cat join cat.Mate as mate " +
            "where cat.name = ?")
        .SetString(0, name)
        .List<Cat>();
    
    var cats = sess
        .CreateQuery("from Cat as cat where cat.Mate.Birthdate is null")
        .List<Cat>();
    
    var moreCats = sess
        .CreateQuery("from Cat as cat where " +
            "cat.Name = 'Fritz' or cat.id = ? or cat.id = ?")
        .SetInt64(0, id1)
        .SetParameter(1, id2, NHibernateUtil.Int64)
        .List<Cat>();
    
    var mates = sess
        .CreateQuery("from Cat as cat where cat.Mate = ?")
        .SetEntity(0, izi)
        .List<Cat>();
    );
    
    var problems = sess
        .CreateQuery("from GoldFish as fish " +
            "where fish.Birthday > fish.Deceased or fish.Birthday is null")
        .List<GoldFish>();
```

These given `Set` parameters are used to bind the given values to the
`?` query placeholders (which map to input parameters of an `ADO.NET` 
`DbCommand`). Just as in `ADO.NET` , you should use this binding mechanism
in preference to string manipulation.

The `NHibernateUtil` class defines a number of static methods and
constants, providing access to most of the built-in types, as instances
of `NHibernate.Type.IType`.

If you expect your query to return a very large number of objects, but
you don't expect to use them all, you might get better performance from
the `Enumerable()` method, which return a `IEnumerable`. The iterator
will load objects on demand, using the identifiers returned by an
initial SQL query (n+1 selects total).

```csharp
    // fetch ids
    IEnumerable<Qux> en = sess
        .CreateQuery("from eg.Qux q order by q.Likeliness")
        .Enumerable<Qux>();
    foreach (Qux qux in en)
    {
        // something we couldnt express in the query
        if ( qux.CalculateComplicatedAlgorithm() ) {
            // dont need to process the rest
            break;
        }
    }
```

The `Enumerable()` method also performs better if you expect that many
of the objects are already loaded and cached by the session, or if the
query results contain the same objects many times. (When no data is
cached or repeated, `CreateQuery()` is almost always faster.) Here is an
example of a query that should be called using `Enumerable()`:

```csharp
    var en = sess
        .CreateQuery(
            "select customer, product " +
            "from Customer customer, " +
            "Product product " +
            "join customer.Purchases purchase " +
            "where product = purchase.Product")
        .Enumerable<object[]>();
```

Calling the previous query using `CreateQuery()` would return a very
large `ADO.NET` result set containing the same data many times.

NHibernate queries sometimes return tuples of objects, in which case
each tuple is returned as an array:

```csharp
    var foosAndBars = sess
        .CreateQuery(
            "select foo, bar from Foo foo, Bar bar " +
            "where bar.Date = foo.Date")
        .Enumerable<object[]>();
    foreach (object[] tuple in foosAndBars)
    {
        Foo foo = tuple[0]; Bar bar = tuple[1];
        ....
    }
```

## Scalar queries <a name="manipulatingdata-scalarqueries"></a>

Queries may specify a property of a class in the `select` clause. They
may even call SQL aggregate functions. Properties or aggregates are
considered "scalar" results.

```csharp
    var results = sess
        .CreateQuery(
            "select cat.Color, min(cat.Birthdate), count(cat) from Cat cat " +
            "group by cat.Color")
        .Enumerable<object[]>();
    foreach (object[] row in results)
    {
        Color type = (Color) row[0];
        DateTime oldest = (DateTime) row[1];
        int count = (int) row[2];
        .....
    }

    var en = sess
        .CreateQuery(
            "select cat.Type, cat.Birthdate, cat.Name from DomesticCat cat")
        .Enumerable<object[]>();

    IList<object[]> list = sess
        .CreateQuery("select cat, cat.Mate.Name from DomesticCat cat")
        .List<object[]>();
```

## The IQuery interface <a name="manipulatingdata-queryinterface"></a>

If you need to specify bounds upon your result set (the maximum number
of rows you want to retrieve and / or the first row you want to
retrieve) you should obtain an instance of `NHibernate.IQuery`:

```csharp
    IQuery q = sess.CreateQuery("from DomesticCat cat");
    q.SetFirstResult(20);
    q.SetMaxResults(10);
    var cats = q.List<Cat>();
```

You may even define a named query in the mapping document. (Remember to
use a `CDATA` section if your query contains characters that could be
interpreted as markup.)

```xml
    <query name="Eg.DomesticCat.by.name.and.minimum.weight"><![CDATA[
        from Eg.DomesticCat as cat
            where cat.Name = ?
            and cat.Weight > ?
    ] ]></query>
```
```csharp
    IQuery q = sess.GetNamedQuery("Eg.DomesticCat.by.name.and.minimum.weight");
    q.SetString(0, name);
    q.SetInt32(1, minWeight);
    var cats = q.List<Cat>();
```

Named queries are by default validated at startup time, allowing to
catch errors more easily than having to test all the application
features using HQL queries. In case of validation errors, the details of
failing queries are logged and a validation error is raised.

Named queries accepts a number of attributes matching settings available
on the `IQuery` interface.

  - `flush-mode` - override the session flush mode just for this query.

  - `cacheable` - allow the query results to be cached by the second
    level cache. See [NHibernate.Caches](caches.md).

  - `cache-region` - specify the cache region of the query.

  - `cache-mode` - specify the cache mode of the query.

  - `fetch-size` - set a fetch size for the underlying ADO query.

  - `timeout` - set the query timeout in seconds.

  - `read-only` - `true` switches yielded entities to read-only. See
    [Read-only entities](readonly.md).

  - `comment` - add a custom comment to the generated SQL.

The query interface supports the use of named parameters. Named
parameters are identifiers of the form `:name` in the query string.
There are methods on `IQuery` for binding values to named or positional
parameters. NHibernate numbers parameters from zero. The advantages of
named parameters are:

  - named parameters are insensitive to the order they occur in the
    query string

  - they may occur multiple times in the same query

  - they are self-documenting

```csharp
    //named parameter (preferred)
    IQuery q = sess.CreateQuery("from DomesticCat cat where cat.Name = :name");
    q.SetString("name", "Fritz");
    var cats = q.Enumerable<DomesticCat>();

    //positional parameter
    IQuery q = sess.CreateQuery("from DomesticCat cat where cat.Name = ?");
    q.SetString(0, "Izi");
    var cats = q.Enumerable<DomesticCat>();

    //named parameter list
    var names = new List<string>();
    names.Add("Izi");
    names.Add("Fritz");
    IQuery q = sess.CreateQuery("from DomesticCat cat where cat.Name in (:namesList)");
    q.SetParameterList("namesList", names);
    var cats = q.List<DomesticCat>();
```

## Filtering collections <a name="manipulatingdata-filtering"></a>

A collection *filter* is a special type of query that may be applied to
a persistent collection or array. The query string may refer to `this`,
meaning the current collection element.

```csharp
    var blackKittens = session
        .CreateFilter(pk.Kittens, "where this.Color = ?")
        .SetEnum(0, Color.Black)
        .List<Cat>();
```

The returned collection is considered a bag.

Observe that filters do not require a `from` clause (though they may
have one if required). Filters are not limited to returning the
collection elements themselves.

```csharp
    var blackKittenMates = session
        .CreateFilter(pk.Kittens,
            "select this.Mate where this.Color = Eg.Color.Black")
        .List<Cat>();
```

## Criteria queries <a name="manipulatingdata-criteria"></a>

HQL is extremely powerful but some people prefer to build queries
dynamically, using an object oriented API, rather than embedding strings
in their .NET code. For these people, NHibernate provides an intuitive
`ICriteria` query API.

```csharp
    ICriteria crit = session.CreateCriteria<Cat>();
    crit.Add(Expression.Eq("color", Eg.Color.Black));
    crit.SetMaxResults(10);
    var cats = crit.List<Cat>();
```

If you are uncomfortable with SQL-like syntax, this is perhaps the
easiest way to get started with NHibernate. This API is also more
extensible than HQL. Applications might provide their own
implementations of the `ICriterion` interface.

## Queries in native SQL <a name="manipulatingdata-nativesql"></a>

You may express a query in SQL, using `CreateSQLQuery()`. You must
enclose SQL aliases in braces.

```csharp
    var cats = session
        .CreateSQLQuery("SELECT {cat.*} FROM CAT {cat} WHERE ROWNUM<10")
        .AddEntity("cat", typeof(Cat))
        .List<Cat>();

    var cats = session
        .CreateSQLQuery(
            "SELECT {cat}.ID AS {cat.Id}, {cat}.SEX AS {cat.Sex}, " +
               "{cat}.MATE AS {cat.Mate}, {cat}.SUBCLASS AS {cat.class}, ... " +
            "FROM CAT {cat} WHERE ROWNUM<10")
        .AddEntity("cat", typeof(Cat))
        .List<Cat>()
```

SQL queries may contain named and positional parameters, just like
NHibernate queries.

# Updating objects <a name="manipulatingdata-updating"></a>

## Updating in the same ISession <a name="manipulatingdata-updating-insession"></a>

*Transactional persistent instances* (ie. objects loaded, saved, created
or queried by the `ISession`) may be manipulated by the application and
any changes to persistent state will be persisted when the `ISession` is
*flushed* (discussed later in this chapter). So the most straightforward
way to update the state of an object is to `Load()` it, and then
manipulate it directly, while the `ISession` is open:

```csharp
    DomesticCat cat = sess.Load<DomesticCat>(69L);
    cat.Name = "PK";
    sess.Flush();  // changes to cat are automatically detected and persisted
```

Sometimes this programming model is inefficient since it would require
both an SQL `SELECT` (to load an object) and an SQL `UPDATE` (to persist
its updated state) in the same session. Therefore NHibernate offers an
alternate approach.

## Updating detached objects <a name="manipulatingdata-updating-detached"></a>

Many applications need to retrieve an object in one transaction, send it
to the UI layer for manipulation, then save the changes in a new
transaction. (Applications that use this kind of approach in a
high-concurrency environment usually use versioned data to ensure
transaction isolation.) This approach requires a slightly different
programming model to the one described in the last section. NHibernate
supports this model by providing the method `ISession.Update()`.

```csharp
    // in the first session
    Cat cat = firstSession.Load<Cat>(catId);
    Cat potentialMate = new Cat();
    firstSession.Save(potentialMate);
    
    // in a higher tier of the application
    cat.Mate = potentialMate;
    
    // later, in a new session
    secondSession.Update(cat);  // update cat
    secondSession.Update(mate); // update mate
```

If the `Cat` with identifier `catId` had already been loaded by
`secondSession` when the application tried to update it, an exception
would have been thrown.

The application should individually `Update()` transient instances
reachable from the given transient instance if and *only* if it wants
their state also updated. (Except for lifecycle objects, discussed
later.)

NHibernate users have requested a general purpose method that either
saves a transient instance by generating a new identifier or update the
persistent state associated with its current identifier. The
`SaveOrUpdate()` method now implements this functionality.

NHibernate distinguishes "new" (unsaved) instances from "existing"
(saved or loaded in a previous session) instances by the value of their
identifier (or version, or timestamp) property. The `unsaved-value`
attribute of the `<id>` (or `<version>`, or `<timestamp>`) mapping
specifies which values should be interpreted as representing a "new"
instance.

```xml
    <id name="Id" type="Int64" column="uid" unsaved-value="0">
        <generator class="hilo"/>
    </id>
```

The allowed values of `unsaved-value` are:

  - `any` - always save

  - `none` - always update

  - `null` - save when identifier is null

  - valid identifier value - save when identifier is null or the given
    value

  - `undefined` - if set for `version` or `timestamp`, then identifier
    check is used

If `unsaved-value` is not specified for a class, NHibernate will attempt
to guess it by creating an instance of the class using the no-argument
constructor and reading the property value from the instance.

```csharp
    // in the first session
    Cat cat = firstSession.Load<Cat>(catID);
    
    // in a higher tier of the application
    Cat mate = new Cat();
    cat.Mate = mate;
    
    // later, in a new session
    secondSession.SaveOrUpdate(cat);   // update existing state (cat has a non-null id)
    secondSession.SaveOrUpdate(mate);  // save the new instance (mate has a null id)
```

The usage and semantics of `SaveOrUpdate()` seems to be confusing for
new users. Firstly, so long as you are not trying to use instances from
one session in another new session, you should not need to use
`Update()` or `SaveOrUpdate()`. Some whole applications will never use
either of these methods.

Usually `Update()` or `SaveOrUpdate()` are used in the following
scenario:

  - the application loads an object in the first session

  - the object is passed up to the UI tier

  - some modifications are made to the object

  - the object is passed back down to the business logic tier

  - the application persists these modifications by calling `Update()`
    in a second session

`SaveOrUpdate()` does the following:

  - if the object is already persistent in this session, do nothing

  - if the object has no identifier property, `Save()` it

  - if the object's identifier matches the criteria specified by
    `unsaved-value`, `Save()` it

  - if the object is versioned (`version` or `timestamp`), then the
    version will take precedence to identifier check, unless the
    versions `unsaved-value="undefined"` (default value)

  - if another object associated with the session has the same
    identifier, throw an exception

The last case can be avoided by using `Merge(Object o)`. This method
copies the state of the given object onto the persistent object with the
same identifier. If there is no persistent instance currently associated
with the session, it will be loaded. The method returns the persistent
instance. If the given instance is unsaved or does not exist in the
database, NHibernate will save it and return it as a newly persistent
instance. Otherwise, the given instance does not become associated with
the session. In most applications with detached objects, you need both
methods, `SaveOrUpdate()` and `Merge()`.

## Reattaching detached objects <a name="manipulatingdata-update-lock"></a>

The `Lock()` method allows the application to re-associate an unmodified
object with a new session.

```csharp
    //just reassociate:
    sess.Lock(fritz, LockMode.None);
    //do a version check, then reassociate:
    sess.Lock(izi, LockMode.Read);
    //do a version check, using SELECT ... FOR UPDATE, then reassociate:
    sess.Lock(pk, LockMode.Upgrade);
```

# Deleting persistent objects <a name="manipulatingdata-deleting"></a>

`ISession.Delete()` will remove an object's state from the database. Of
course, your application might still hold a reference to it. So it's
best to think of `Delete()` as making a persistent instance transient.

```csharp
    sess.Delete(cat);
```

You may also delete many objects at once by passing a NHibernate query
string to `Delete()`.

```csharp
    sess.Delete("from Cat");
```

You may now delete objects in any order you like, without risk of
foreign key constraint violations. Of course, it is still possible to
violate a `NOT
NULL` constraint on a foreign key column by deleting objects in the
wrong order.

# Flush <a name="manipulatingdata-flushing"></a>

From time to time the `ISession` will execute the SQL statements needed
to synchronize the `ADO.NET` connection's state with the state of objects
held in memory. This process, *flush*, occurs by default at the
following points

  - from some invocations of `IQuery` methods such as `List` or
    `Enumerable`, and from similar methods of other querying API.

  - from `NHibernate.ITransaction.Commit()`

  - from `ISession.Flush()`

The SQL statements are issued in the following order

1.  all entity insertions, in the same order the corresponding objects
    were saved using `ISession.Save()`

2.  all entity updates

3.  all collection deletions

4.  all collection element deletions, updates and insertions

5.  all collection insertions

6.  all entity deletions, in the same order the corresponding objects
    were deleted using `ISession.Delete()`

(An exception is that objects using `identity` ID generation are
inserted when they are saved.)

Except when you explicitly `Flush()`, there are absolutely no guarantees
about *when* the `Session` executes the `ADO.NET` calls, only the *order*
in which they are executed. However, NHibernate does guarantee that the
queries methods will never return stale data; nor will they return the
wrong data.

It is possible to change the default behavior so that flush occurs less
frequently. The `FlushMode` class defines three different modes: only
flush at commit time (and only when the NHibernate `ITransaction` API is
used, or inside a transaction scope), flush automatically using the
explained routine (will only work inside an explicit NHibernate
`ITransaction` or inside a transaction scope), or never flush unless
`Flush()` is called explicitly. The last mode is useful for long running
units of work, where an ISession is kept open and disconnected for a
long time (see [Optimistic concurrency control](transactions.md#optimistic-concurrency-control)).

```csharp
    sess = sf.OpenSession();
    using (ITransaction tx = sess.BeginTransaction())
    {
        // allow queries to return stale state
        sess.FlushMode = FlushMode.Commit;
        Cat izi = sess.Load<Cat>(id);
        izi.Name = "iznizi";
        // execute some queries....
        sess.CreateQuery("from Cat as cat left outer join cat.Kittens kitten")
            .List<object[]>();
        // change to izi is not flushed!
        ...
        tx.Commit(); // flush occurs
    }
```

# Checking dirtiness <a name="manipulatingdata-dirtiness"></a>

`ISession.IsDirty()` will return whether the session hold any pending
change to flush or not. Be cautious when using this method, its default
implementation may have the following effects:

  - Dirty checks all the loaded entities. NHibernate does not instrument
    the entities for being notified of changes done on loaded ones.
    Instead, it stores their initial state and compare them to it. If
    session has loaded a lot of entities, the dirty checking will have a
    significant impact.

  - Triggers pending cascade operations. This includes any pending
    `Save` of, by example, children added to a collection having the
    `Save` cascade enabled. Depending on the entities ID generators (see
    [generator](mapping.md#generator)), this may trigger calls to
    the database, or even entity insertions if they are using the
    `identity` generator.

# Ending a Session <a name="manipulatingdata-endingsession"></a>

Ending a session involves four distinct phases:

  - flush the session

  - commit the transaction

  - close the session

  - handle exceptions

## Flushing the Session <a name="manipulatingdata-endingsession-flushing"></a>

If you happen to be using the `ITransaction` API, you don't need to
worry about this step. It will be performed implicitly when the
transaction is committed. Otherwise you should call `ISession.Flush()`
to ensure that all changes are synchronized with the database.

## Committing the database transaction <a name="manipulatingdata-endingsession-commit"></a>

If you are using the NHibernate `ITransaction` API, this looks like:

```csharp
    tx.Commit(); // flush the session and commit the transaction
```

If you are managing `ADO.NET` transactions yourself you should manually
`Commit()` the `ADO.NET` transaction.

```csharp
    sess.Flush();
    currentTransaction.Commit();
```

If you decide *not* to commit your changes:

```csharp
    tx.Rollback();  // rollback the transaction
```

or:

```csharp
    currentTransaction.Rollback();
```

If you rollback the transaction you should immediately close and discard
the current session to ensure that NHibernate's internal state is
consistent.

## Closing the ISession <a name="manipulatingdata-endingsession-close"></a>

A call to `ISession.Close()` marks the end of a session. The main
implication of `Close()` is that the `ADO.NET` connection will be
relinquished by the session.

```csharp
    tx.Commit();
    sess.Close();

    sess.Flush();
    currentTransaction.Commit();
    sess.Close();
```

If you provided your own connection, `Close()` returns a reference to
it, so you can manually close it or return it to the pool. Otherwise
`Close()` returns it to the pool.

# Exception handling <a name="manipulatingdata-exceptions"></a>

NHibernate use might lead to exceptions, usually `HibernateException`.
This exception can have a nested inner exception (the root cause), use
the `InnerException` property to access it.

If the `ISession` throws an exception you should immediately rollback
the transaction, call `ISession.Close()` and discard the `ISession`
instance. Certain methods of `ISession` will *not* leave the session in
a consistent state.

For exceptions thrown by the data provider while interacting with the
database, NHibernate will wrap the error in an instance of
`ADOException`. The underlying exception is accessible by calling
`ADOException.InnerException`. NHibernate converts the `DbException`
into an appropriate `ADOException` subclass using the
`ISQLExceptionConverter` attached to the SessionFactory. By default, the
`ISQLExceptionConverter` is defined by the configured dialect; however,
it is also possible to plug in a custom implementation (see the api-docs
for the `ISQLExceptionConverter` class for details).

The following exception handling idiom shows the typical case in
NHibernate applications:

```csharp
    using (ISession sess = factory.OpenSession())
    using (ITransaction tx = sess.BeginTransaction())
    {
        // do some work
        ...
        tx.Commit();
    }

```
Or, when manually managing `ADO.NET` transactions:

```csharp
    ISession sess = factory.openSession();
    try
    {
        // do some work
        ...
        sess.Flush();
        currentTransaction.Commit();
    }
    catch (Exception e)
    {
        currentTransaction.Rollback();
        throw;
    }
    finally
    {
        sess.Close();
    }
```

# Lifecycles and object graphs <a name="manipulatingdata-graphs"></a>

To save or update all objects in a graph of associated objects, you must
either

  - `Save()`, `SaveOrUpdate()` or `Update()` each individual object OR

  - map associated objects using `cascade="all"` or
    `cascade="save-update"`.

Likewise, to delete all objects in a graph, either

  - `Delete()` each individual object OR

  - map associated objects using `cascade="all"`,
    `cascade="all-delete-orphan"` or `cascade="delete"`.

Recommendation:

  - If the child object's lifespan is bounded by the lifespan of the of
    the parent object make it a *lifecycle object* by specifying
    `cascade="all"`.

  - Otherwise, `Save()` and `Delete()` it explicitly from application
    code. If you really want to save yourself some extra typing, use
    `cascade="save-update"` and explicit `Delete()`.

Mapping an association (many-to-one, one-to-one or collection) with
`cascade="all"` marks the association as a *parent/child* style
relationship where save/update/deletion of the parent results in
save/update/deletion of the child(ren). Furthermore, a mere reference to
a child from a persistent parent will result in save / update of the
child. The metaphor is incomplete, however. A child which becomes
unreferenced by its parent is *not* automatically deleted, except in the
cases of `<one-to-many>` and `<one-to-one>` associations that have been
mapped with `cascade="all-delete-orphan"` or `cascade="delete-orphan"`.
The precise semantics of cascading operations are as follows:

  - If a parent is saved, all children are passed to `SaveOrUpdate()`

  - If a parent is passed to `Update()` or `SaveOrUpdate()`, all
    children are passed to `SaveOrUpdate()`

  - If a transient child becomes referenced by a persistent parent, it
    is passed to `SaveOrUpdate()`

  - If a parent is deleted, all children are passed to `Delete()`

  - If a transient child is dereferenced by a persistent parent,
    *nothing special happens* (the application should explicitly delete
    the child if necessary) unless `cascade="all-delete-orphan"` or
    `cascade="delete-orphan"`, in which case the "orphaned" child is
    deleted.

NHibernate does not fully implement "persistence by reachability", which
would imply (inefficient) persistent garbage collection. However, due to
popular demand, NHibernate does support the notion of entities becoming
persistent when referenced by another persistent object. Associations
marked `cascade="save-update"` behave in this way. If you wish to use
this approach throughout your application, it's easier to specify the
`default-cascade` attribute of the `<hibernate-mapping>` element.

# Interceptors <a name="manipulatingdata-interceptors"></a>

The `IInterceptor` interface provides callbacks from the session to the
application allowing the application to inspect and / or manipulate
properties of a persistent object before it is saved, updated, deleted
or loaded. One possible use for this is to track auditing information.
For example, the following `IInterceptor` automatically sets the
`CreateTimestamp` when an `IAuditable` is created and updates the
`LastUpdateTimestamp` property when an `IAuditable` is updated.

```csharp
    using System;
    using NHibernate.Type;
    
    namespace NHibernate.Test
    {
        [Serializable]
        public class AuditInterceptor : IInterceptor
        {
        
            private int updates;
            private int creates;
        
            public void OnDelete(object entity,
                                 object id,
                                 object[] state,
                                 string[] propertyNames,
                                 IType[] types)
            {
                // do nothing
            }
        
            public boolean OnFlushDirty(object entity, 
                                        object id, 
                                        object[] currentState,
                                        object[] previousState,
                                        string[] propertyNames,
                                        IType[] types) {
        
                if ( entity is IAuditable )
                {
                    updates++;
                    for ( int i=0; i < propertyNames.Length; i++ )
                    {
                        if ( "LastUpdateTimestamp" == propertyNames[i] )
                        {
                            currentState[i] = DateTime.Now;
                            return true;
                        }
                    }
                }
                return false;
            }
        
            public boolean OnLoad(object entity, 
                                  object id,
                                  object[] state,
                                  string[] propertyNames,
                                  IType[] types)
            {
                return false;
            }
        
            public boolean OnSave(object entity,
                                  object id,
                                  object[] state,
                                  string[] propertyNames,
                                  IType[] types)
            {
                if ( entity is IAuditable )
                {
                    creates++;
                    for ( int i=0; i<propertyNames.Length; i++ )
                    {
                        if ( "CreateTimestamp" == propertyNames[i] )
                        {
                            state[i] = DateTime.Now;
                            return true;
                        }
                    }
                }
                return false;
            }
        
            public void PostFlush(ICollection entities)
            {
                Console.Out.WriteLine("Creations: {0}, Updates: {1}", creates, updates);
            }
        
            public void PreFlush(ICollection entities) {
                updates=0;
                creates=0;
            }
            
            ......
            ......
        }
    }
```

The interceptor would be specified when a session is created.

```csharp
    ISession session = sf.OpenSession( new AuditInterceptor() );
```

You may also set an interceptor on a global level, using the
`Configuration`:

```csharp
    new Configuration().SetInterceptor( new AuditInterceptor() );
```

# Metadata API <a name="manipulatingdata-metadata"></a>

NHibernate requires a very rich meta-level model of all entity and value
types. From time to time, this model is very useful to the application
itself. For example, the application might use NHibernate's metadata to
implement a "smart" deep-copy algorithm that understands which objects
should be copied (eg. mutable value types) and which should not (eg.
immutable value types and, possibly, associated entities).

NHibernate exposes metadata via the `IClassMetadata` and
`ICollectionMetadata` interfaces and the `IType` hierarchy. Instances of
the metadata interfaces may be obtained from the `ISessionFactory`.

```csharp
    Cat fritz = ......;
    IClassMetadata catMeta = sessionfactory.GetClassMetadata(typeof(Cat));
    long id = (long) catMeta.GetIdentifier(fritz);
    object[] propertyValues = catMeta.GetPropertyValues(fritz);
    string[] propertyNames = catMeta.PropertyNames;
    IType[] propertyTypes = catMeta.PropertyTypes;
    
    // get an dictionary of all properties which are not collections or associations
    // TODO: what about components?
    
    var namedValues = new Dictionary<string, object>();
    for (int i = 0; i < propertyNames.Length; i++)
    {
        if (!propertyTypes[i].IsEntityType && !propertyTypes[i].IsCollectionType)
        {
            namedValues[propertyNames[i]] = propertyValues[i];
        }
    }
```
