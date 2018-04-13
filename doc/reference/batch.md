# Batch processing

A naive approach to inserting 100 000 rows in the database using
NHibernate might look like this:

```csharp
    using (ISession session = sessionFactory.OpenSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        for (int i = 0; i < 100000; i++)
        {
            Customer customer = new Customer(.....);
            session.Save(customer);
        }
        tx.Commit();
    }
```

This would fall over with an `OutOfMemoryException` somewhere around the
50 000th row. That's because NHibernate caches all the newly inserted
`Customer` instances in the session-level cache.

In this chapter we'll show you how to avoid this problem. First,
however, if you are doing batch processing, it is absolutely critical
that you enable the use of ADO batching, if you intend to achieve
reasonable performance. Set the ADO batch size to a reasonable number
(say, 10-50):

    adonet.batch_size 20

Note that NHibernate disables insert batching at the ADO level
transparently if you use an `identity` identifier generator.

You also might like to do this kind of work in a process where
interaction with the second-level cache is completely disabled:

    cache.use_second_level_cache false

However, this is not absolutely necessary, since we can explicitly set
the `CacheMode` to disable interaction with the second-level cache.

# Batch inserts <a name="batch-inserts"></a>

When making new objects persistent, you must `Flush()` and then
`Clear()` the session regularly, to control the size of the first-level
cache.

```csharp
    using (ISession session = sessionFactory.openSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        for (int i = 0; i < 100000; i++)
        {
            Customer customer = new Customer(.....);
            session.Save(customer);
            // 20, same as the ADO batch size
            if (i % 20 == 0)
            {
                // flush a batch of inserts and release memory:
                session.Flush();
                session.Clear();
            }
        }
    
        tx.Commit();
    }
```

# The StatelessSession interface <a name="batch-statelesssession"></a>

Alternatively, NHibernate provides a command-oriented API that may be
used for streaming data to and from the database in the form of detached
objects. A `IStatelessSession` has no persistence context associated
with it and does not provide many of the higher-level life cycle
semantics. In particular, a stateless session does not implement a
first-level cache nor interact with any second-level or query cache. It
does not implement transactional write-behind or automatic dirty
checking. Operations performed using a stateless session do not ever
cascade to associated instances. Collections are ignored by a stateless
session. Operations performed via a stateless session bypass
NHibernate's event model and interceptors. Stateless sessions are
vulnerable to data aliasing effects, due to the lack of a first-level
cache. A stateless session is a lower-level abstraction, much closer to
the underlying ADO.

 ```csharp
    using (IStatelessSession session = sessionFactory.OpenStatelessSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        var customers = session.GetNamedQuery("GetCustomers")
            .Enumerable<Customer>();
        while (customers.MoveNext())
        {
            Customer customer = customers.Current;
            customer.updateStuff(...);
            session.Update(customer);
        }
    
        tx.Commit();
    }
```

Note that in this code example, the `Customer` instances returned by the
query are immediately detached. They are never associated with any
persistence context.

The `insert(), update()` and `delete()` operations defined by the
`StatelessSession` interface are considered to be direct database
row-level operations, which result in immediate execution of a SQL
`INSERT, UPDATE` or `DELETE` respectively. Thus, they have very
different semantics to the `Save(), SaveOrUpdate()` and `Delete()`
operations defined by the `ISession` interface.

# DML-style operations <a name="batch-direct"></a>

As already discussed, automatic and transparent object/relational
mapping is concerned with the management of object state. This implies
that the object state is available in memory, hence manipulating (using
the SQL `Data Manipulation Language` (DML) statements: `INSERT`,
`UPDATE`, `DELETE`) data directly in the database will not affect
in-memory state. However, NHibernate provides methods for bulk SQL-style
DML statement execution which are performed through the Hibernate Query
Language ([HQL](queryhql.md)). A [Linq implementation](querylinq.md#modifying-entities-inside-the-database) is available too.

The pseudo-syntax for `UPDATE` and `DELETE` statements is: `( UPDATE |
DELETE ) FROM? EntityName (WHERE where_conditions)?`. Some points to
note:

  - In the from-clause, the FROM keyword is optional

  - There can only be a single entity named in the from-clause; it can
    optionally be aliased. If the entity name is aliased, then any
    property references must be qualified using that alias; if the
    entity name is not aliased, then it is illegal for any property
    references to be qualified.

  - No [joins](queryhql.md#associations-and-joins) (either implicit or explicit) can be
    specified in a bulk HQL query. Sub-queries may be used in the
    where-clause; the sub-queries, themselves, may contain joins.

  - The where-clause is also optional.

As an example, to execute an HQL `UPDATE`, use the
`IQuery.ExecuteUpdate()` method:

```csharp
    using (ISession session = sessionFactory.OpenSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        string hqlUpdate = "update Customer c set c.name = :newName where c.name = :oldName";
        // or string hqlUpdate = "update Customer set name = :newName where name = :oldName";
        int updatedEntities = s.CreateQuery(hqlUpdate)
            .SetString("newName", newName)
            .SetString("oldName", oldName)
            .ExecuteUpdate();
        tx.Commit();
    }
```

HQL `UPDATE` statements, by default do not effect the
[version](mapping.md#version-optional) or the
[timestamp](mapping.md#timestamp-optional) property values for the
affected entities. However, you can force NHibernate to properly reset
the `version` or `timestamp` property values through the use of a
`versioned update`. This is achieved by adding the `VERSIONED` keyword
after the `UPDATE` keyword.

```csharp
    using (ISession session = sessionFactory.OpenSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        string hqlVersionedUpdate =
            "update versioned Customer set name = :newName where name = :oldName";
        int updatedEntities = s.CreateQuery(hqlUpdate)
            .SetString("newName", newName)
            .SetString("oldName", oldName)
            .ExecuteUpdate();
        tx.Commit();
    }
```

Note that custom version types (`NHibernate.Usertype.IUserVersionType`)
are not allowed in conjunction with a `update versioned` statement.

To execute an HQL `DELETE`, use the same `IQuery.ExecuteUpdate()`
method:

```csharp
    using (ISession session = sessionFactory.OpenSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        string hqlDelete = "delete Customer c where c.name = :oldName";
        // or String hqlDelete = "delete Customer where name = :oldName";
        int deletedEntities = s.CreateQuery(hqlDelete)
            .SetString("oldName", oldName)
            .ExecuteUpdate();
        tx.Commit();
    }
```

The `int` value returned by the `IQuery.ExecuteUpdate()` method indicate
the number of entities effected by the operation. Consider this may or
may not correlate to the number of rows effected in the database. An HQL
bulk operation might result in multiple actual SQL statements being
executed, for joined-subclass, for example. The returned number
indicates the number of actual entities affected by the statement. Going
back to the example of joined-subclass, a delete against one of the
subclasses may actually result in deletes against not just the table to
which that subclass is mapped, but also the "root" table and potentially
joined-subclass tables further down the inheritance hierarchy.

The pseudo-syntax for `INSERT` statements is: `INSERT INTO EntityName
properties_list select_statement`. Some points to note:

  - Only the `INSERT INTO ... SELECT ...` form is supported; not the
    `INSERT INTO ... VALUES ...` form.
    
    The `properties_list` is analogous to the `column specification` in
    the SQL `INSERT` statement. For entities involved in mapped
    inheritance, only properties directly defined on that given
    class-level can be used in the properties\_list. Superclass
    properties are not allowed; and subclass properties do not make
    sense. In other words, `INSERT` statements are inherently
    non-polymorphic.

  - `select_statement` can be any valid HQL select query, with the caveat
    that the return types must match the types expected by the insert.
    Currently, this is checked during query compilation rather than
    allowing the check to relegate to the database. Note however that
    this might cause problems between NHibernate `Type`s which are
    *equivalent* as opposed to *equal*. This might cause issues with
    mismatches between a property defined as a
    `NHibernate.Type.DateType` and a property defined as a
    `NHibernate.Type.TimestampType`, even though the database might not
    make a distinction or might be able to handle the conversion.

  - For the id property, the insert statement gives you two options. You
    can either explicitly specify the id property in the
    `properties_list` (in which case its value is taken from the
    corresponding select expression) or omit it from the
    `properties_list` (in which case a generated value is used). This
    later option is only available when using id generators that operate
    in the database; attempting to use this option with any "in memory"
    type generators will cause an exception during parsing. Note that
    for the purposes of this discussion, in-database generators are
    considered to be `NHibernate.Id.SequenceGenerator` (and its
    subclasses) and any implementors of
    `NHibernate.Id.IPostInsertIdentifierGenerator`. The most notable
    exception here is `NHibernate.Id.TableHiLoGenerator`, which cannot
    be used because it does not expose a selectable way to get its
    values.

  - For properties mapped as either `version` or `timestamp`, the insert
    statement gives you two options. You can either specify the property
    in the `properties_list` (in which case its value is taken from the
    corresponding select expressions) or omit it from the
    `properties_list` (in which case the `seed value` defined by the
    `NHibernate.Type.IVersionType` is used).

An example HQL `INSERT` statement execution:

```csharp
    using (ISession session = sessionFactory.OpenSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        var hqlInsert =
            "insert into DelinquentAccount (id, name) " +
            "select c.id, c.name from Customer c where ...";
        int createdEntities = s.CreateQuery(hqlInsert)
            .ExecuteUpdate();
        tx.Commit();
    }
```
