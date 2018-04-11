# Read-only entities

> **Important**
> 
> NHibernate's treatment of *read-only* entities may differ from what
> you may have encountered elsewhere. Incorrect usage may cause
> unexpected results.

When an entity is read-only:

  - NHibernate does not dirty-check the entity's simple properties or
    single-ended associations;

  - NHibernate will not update simple properties or updatable
    single-ended associations;

  - NHibernate will not update the version of the read-only entity if
    only simple properties or single-ended updatable associations are
    changed;

In some ways, NHibernate treats read-only entities the same as entities
that are not read-only:

  - NHibernate cascades operations to associations as defined in the
    entity mapping.

  - NHibernate updates the version if the entity has a collection with
    changes that dirties the entity;

  - A read-only entity can be deleted.

Even if an entity is not read-only, its collection association can be
affected if it contains a read-only entity.

For details about the affect of read-only entities on different property
and association types, see [section\_title](#readonly-proptypes).

For details about how to make entities read-only, see
[section\_title](#readonly-api)

NHibernate does some optimizing for read-only entities:

  - It saves execution time by not dirty-checking simple properties or
    single-ended associations.

  - It saves memory by deleting database snapshots.

# Making persistent entities read-only

Only persistent entities can be made read-only. Transient and detached
entities must be put in persistent state before they can be made
read-only.

NHibernate provides the following ways to make persistent entities
read-only:

  - you can map an entity class as *immutable*; when an entity of an
    immutable class is made persistent, NHibernate automatically makes
    it read-only. see [section\_title](#readonly-api-immutable) for
    details

  - you can change a default so that entities loaded into the session by
    NHibernate are automatically made read-only; see
    [section\_title](#readonly-api-loaddefault) for details

  - you can make an HQL query or criteria read-only so that entities
    loaded when the query or criteria executes, or iterates, are
    automatically made read-only; see
    [section\_title](#readonly-api-querycriteria) for details

  - you can make a persistent entity that is already in the in the
    session read-only; see [section\_title](#readonly-api-entity) for
    details

## Entities of immutable classes

When an entity instance of an immutable class is made persistent,
NHibernate automatically makes it read-only.

An entity of an immutable class can created and deleted the same as an
entity of a mutable class.

NHibernate treats a persistent entity of an immutable class the same way
as a read-only persistent entity of a mutable class. The only exception
is that NHibernate will not allow an entity of an immutable class to be
changed so it is not read-only.

## Loading persistent entities as read-only

> **Note**
> 
> Entities of immutable classes are automatically loaded as read-only.

To change the default behavior so NHibernate loads entity instances of
mutable classes into the session and automatically makes them read-only,
call:

    Session.DefaultReadOnly = true;

To change the default back so entities loaded by NHibernate are not made
read-only, call:

    Session.DefaultReadOnly = false;

You can determine the current setting by using the property:

    Session.DefaultReadOnly;

If `Session.DefaultReadOnly` property returns true, entities loaded by
the following are automatically made read-only:

  - Session.Load() and Session.Load\<T\>

  - Session.Get() and Session.Get\<T\>

  - Session.Merge()

  - executing, or iterating HQL queries and criteria; to override this
    setting for a particular HQL query or criteria see
    [section\_title](#readonly-api-querycriteria)

Changing this default has no effect on:

  - persistent entities already in the session when the default was
    changed

  - persistent entities that are refreshed via `Session.Refresh()`; a
    refreshed persistent entity will only be read-only if it was
    read-only before refreshing

  - persistent entities added by the application via
    `Session.Persist()`, `Session.Save()`, `Session.Update()` and
    `Session.SaveOrUpdate()`

## Loading read-only entities from an HQL query/criteria

> **Note**
> 
> Entities of immutable classes are automatically loaded as read-only.

If `Session.DefaultReadOnly` returns false (the default) when an HQL
query or criteria executes, then entities and proxies of mutable classes
loaded by the query will not be read-only.

You can override this behavior so that entities and proxies loaded by an
HQL query or criteria are automatically made read-only.

For an HQL query, call:

    Query.SetReadOnly(true);

`Query.SetReadOnly(true)` must be called before `Query.List()`,
`Query.UniqueResult()`, or `Query.Enumerable()`

For an HQL criteria, call:

    Criteria.SetReadOnly(true);

`Criteria.SetReadOnly(true)` must be called before `Criteria.List()`, or
`Criteria.UniqueResult()`

Entities and proxies that exist in the session before being returned by
an HQL query or criteria are not affected.

Uninitialized persistent collections returned by the query are not
affected. Later, when the collection is initialized, entities loaded
into the session will be read-only if `Session.DefaultReadOnly` returns
true.

Using `Query.SetReadOnly(true)` or `Criteria.SetReadOnly(true)` works
well when a single HQL query or criteria loads all the entities and
initializes all the proxies and collections that the application needs
to be read-only.

When it is not possible to load and initialize all necessary entities in
a single query or criteria, you can temporarily change the session
default to load entities as read-only before the query is executed. Then
you can explicitly initialize proxies and collections before restoring
the session default.

    using (ISession session = factory.OpenSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        session.DefaultReadOnly = true;
        Contract contract = session
            .CreateQuery("from Contract where CustomerName = 'Sherman'")
            .UniqueResult<Contract>();
        NHibernate.Initialize(contract.Plan);
        NHibernate.Initialize(contract.Variations);
        NHibernate.Initialize(contract.Notes);
        session.DefaultReadOnly = false;
        ...
        tx.Commit();
    }

If `Session.DefaultReadOnly` returns true, then you can use
Query.SetReadOnly(false) and Criteria.SetReadOnly(false) to override
this session setting and load entities that are not read-only.

## Making a persistent entity read-only

> **Note**
> 
> Persistent entities of immutable classes are automatically made
> read-only.

To make a persistent entity or proxy read-only, call:

    Session.SetReadOnly(entityOrProxy, true)

To change a read-only entity or proxy of a mutable class so it is no
longer read-only, call:

    Session.SetReadOnly(entityOrProxy, false)

> **Important**
> 
> When a read-only entity or proxy is changed so it is no longer
> read-only, NHibernate assumes that the current state of the read-only
> entity is consistent with its database representation. If this is not
> true, then any non-flushed changes made before or while the entity was
> read-only, will be ignored.

To throw away non-flushed changes and make the persistent entity
consistent with its database representation, call:

    Session.Refresh(entity);

To flush changes made before or while the entity was read-only and make
the database representation consistent with the current state of the
persistent entity:

    // evict the read-only entity so it is detached
    session.Evict(entity);
    
    // make the detached entity (with the non-flushed changes) persistent
    session.Update(entity);
    
    // now entity is no longer read-only and its changes can be flushed
    s.Flush();

# Read-only affect on property type

The following table summarizes how different property types are affected
by making an entity read-only.

<table>
<caption>Affect of read-only entity on property types</caption>
<colgroup>
<col style="width: 50%" />
<col style="width: 50%" />
</colgroup>
<thead>
<tr class="header">
<th>Property/Association Type</th>
<th>Changes flushed to DB?</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>Simple
<p>(<a href="#readonly-proptypes-simple">section_title</a>)</p></td>
<td>no*</td>
</tr>
<tr class="even">
<td><p>Unidirectional one-to-one</p>
<p>Unidirectional many-to-one</p>
<p>(<a href="#readonly-proptypes-singleended-unidir">section_title</a>)</p></td>
<td><p>no*</p>
<p>no*</p></td>
</tr>
<tr class="odd">
<td><p>Unidirectional one-to-many</p>
<p>Unidirectional many-to-many</p>
<p>(<a href="#readonly-proptypes-manyended-unidir">section_title</a>)</p></td>
<td><p>yes</p>
<p>yes</p></td>
</tr>
<tr class="even">
<td><p>Bidirectional one-to-one</p>
<p>(<a href="#readonly-proptypes-onetoone-bidir">section_title</a>)</p></td>
<td>only if the owning entity is not read-only*</td>
</tr>
<tr class="odd">
<td><p>Bidirectional one-to-many/many-to-one</p>
<p>inverse collection</p>
<p>non-inverse collection</p>
<p>(<a href="#readonly-proptypes-onetomany-manytoone">section_title</a>)</p></td>
<td><p>only added/removed entities that are not read-only*</p>
<p>yes</p></td>
</tr>
<tr class="even">
<td><p>Bidirectional many-to-many</p>
<p>(<a href="#readonly-proptypes-manytomany-bidir">section_title</a>)</p></td>
<td>yes</td>
</tr>
</tbody>
</table>

\* Behavior is different when the entity having the property/association
is read-only, compared to when it is not read-only.

## Simple properties

When a persistent object is read-only, NHibernate does not dirty-check
simple properties.

NHibernate will not synchronize simple property state changes to the
database. If you have automatic versioning, NHibernate will not
increment the version if any simple properties change.

    using (ISession session = factory.OpenSession())
    using (ITransaction tx = session.BeginTransaction())
    {
        // get a contract and make it read-only
        Contract contract = session.Get<Contract>(contractId);
        session.SetReadOnly(contract, true);
    
        // contract.CustomerName is "Sherman"
        contract.CustomerName = "Yogi";
        tx.Commit();
    
        tx = session.BeginTransaction();
    
        contract = session.Get<Contract>(contractId);
        // contract.CustomerName is still "Sherman"
        ...
        tx.Commit();
    }

## Unidirectional associations

### Unidirectional one-to-one and many-to-one

NHibernate treats unidirectional one-to-one and many-to-one associations
in the same way when the owning entity is read-only.

We use the term *unidirectional single-ended association* when referring
to functionality that is common to unidirectional one-to-one and
many-to-one associations.

NHibernate does not dirty-check unidirectional single-ended associations
when the owning entity is read-only.

If you change a read-only entity's reference to a unidirectional
single-ended association to null, or to refer to a different entity,
that change will not be flushed to the database.

> **Note**
> 
> If an entity is of an immutable class, then its references to
> unidirectional single-ended associations must be assigned when that
> entity is first created. Because the entity is automatically made
> read-only, these references can not be updated.

If automatic versioning is used, NHibernate will not increment the
version due to local changes to unidirectional single-ended
associations.

In the following examples, Contract has a unidirectional many-to-one
association with Plan. Contract cascades save and update operations to
the association.

The following shows that changing a read-only entity's many-to-one
association reference to null has no effect on the entity's database
representation.

    // get a contract with an existing plan;
    // make the contract read-only and set its plan to null
    using (var tx = session.BeginTransaction())
    {
        Contract contract = session.Get<Contract>(contractId);
        session.SetReadOnly(contract, true);
        contract.Plan = null;
        tx.Commit();
    }
    
    // get the same contract
    using (var tx = session.BeginTransaction())
    {
        Contract contract = session.Get<Contract>(contractId);
    
        // contract.Plan still refers to the original plan;
    
        tx.Commit();
    }
    session.Close();

The following shows that, even though an update to a read-only entity's
many-to-one association has no affect on the entity's database
representation, flush still cascades the save-update operation to the
locally changed association.

    // get a contract with an existing plan;
    // make the contract read-only and change to a new plan
    Contract contract;
    Plan newPlan;
    using (var tx = session.BeginTransaction())
    {
        contract = session.Get<Contract>(contractId);
        session.SetReadOnly(contract, true);
        newPlan = new Plan("new plan");
        contract.Plan = newPlan;
        tx.Commit();
    }
    
    // get the same contract
    using (var tx = session.BeginTransaction())
    {
        contract = session.Get<Contract>(contractId);
        newPlan = session.Get<Plan>(newPlan.Id);
    
        // contract.Plan still refers to the original plan;
        // newPlan is non-null because it was persisted when
        // the previous transaction was committed;
    
        tx.Commit();
    }
    session.Close();

### Unidirectional one-to-many and many-to-many

NHibernate treats unidirectional one-to-many and many-to-many
associations owned by a read-only entity the same as when owned by an
entity that is not read-only.

NHibernate dirty-checks unidirectional one-to-many and many-to-many
associations;

The collection can contain entities that are read-only, as well as
entities that are not read-only.

Entities can be added and removed from the collection; changes are
flushed to the database.

If automatic versioning is used, NHibernate will update the version due
to changes in the collection if they dirty the owning entity.

## Bidirectional associations

### Bidirectional one-to-one

If a read-only entity owns a bidirectional one-to-one association:

  - NHibernate does not dirty-check the association.

  - updates that change the association reference to null or to refer to
    a different entity will not be flushed to the database.

  - If automatic versioning is used, NHibernate will not increment the
    version due to local changes to the association.

> **Note**
> 
> If an entity is of an immutable class, and it owns a bidirectional
> one-to-one association, then its reference must be assigned when that
> entity is first created. Because the entity is automatically made
> read-only, these references cannot be updated.

When the owner is not read-only, NHibernate treats an association with a
read-only entity the same as when the association is with an entity that
is not read-only.

### Bidirectional one-to-many/many-to-one

A read-only entity has no impact on a bidirectional
one-to-many/many-to-one association if:

  - the read-only entity is on the one-to-many side using an inverse
    collection;

  - the read-only entity is on the one-to-many side using a non-inverse
    collection;

  - the one-to-many side uses a non-inverse collection that contains the
    read-only entity

When the one-to-many side uses an inverse collection:

  - a read-only entity can only be added to the collection when it is
    created;

  - a read-only entity can only be removed from the collection by an
    orphan delete or by explicitly deleting the entity.

### Bidirectional many-to-many

NHibernate treats bidirectional many-to-many associations owned by a
read-only entity the same as when owned by an entity that is not
read-only.

NHibernate dirty-checks bidirectional many-to-many associations.

The collection on either side of the association can contain entities
that are read-only, as well as entities that are not read-only.

Entities are added and removed from both sides of the collection;
changes are flushed to the database.

If automatic versioning is used, NHibernate will update the version due
to changes in both sides of the collection if they dirty the entity
owning the respective collections.
