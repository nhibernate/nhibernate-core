# Collection Mapping

# Persistent Collections

NHibernate requires that persistent collection-valued fields be declared
as a generic interface type, for example:

    public class Product
    {
        public ISet<Part> Parts { get; set; } = new HashSet<Part>();
    
        public string SerialNumber { get; set; }
    }

The actual interface might be
`System.Collections.Generic.ICollection<T>`,
`System.Collections.Generic.IList<T>`,
`System.Collections.Generic.IDictionary<K, V>`,
`System.Collections.Generic.ISet<T>` or ... anything you like\! (Where
"anything you like" means you will have to write an implementation of
`NHibernate.UserType.IUserCollectionType`.)

Notice how we initialized the instance variable with an instance of
`HashSet<T>`. This is the best way to initialize collection valued
properties of newly instantiated (non-persistent) instances. When you
make the instance persistent - by calling `Save()`, for example -
NHibernate will actually replace the `HashSet<T>` with an instance of
NHibernate's own implementation of `ISet<T>`. Watch out for errors like
this:

    Cat cat = new DomesticCat();
    Cat kitten = new DomesticCat();
    ....
    ISet<Cat> kittens = new HashSet<Cat>();
    kittens.Add(kitten);
    cat.Kittens = kittens;
    session.Save(cat);
    kittens = cat.Kittens; //Okay, kittens collection is an ISet
    HashSet<Cat> hs = (HashSet<Cat>) cat.Kittens; //Error!

Collection instances have the usual behavior of value types. They are
automatically persisted when referenced by a persistent object and
automatically deleted when unreferenced. If a collection is passed from
one persistent object to another, its elements might be moved from one
table to another. Two entities may not share a reference to the same
collection instance. Due to the underlying relational model,
collection-valued properties do not support null value semantics;
NHibernate does not distinguish between a null collection reference and
an empty collection.

You shouldn't have to worry much about any of this. Just use
NHibernate's collections the same way you use ordinary .NET collections,
but make sure you understand the semantics of bidirectional associations
(discussed later) before using them.

Collection instances are distinguished in the database by a foreign key
to the owning entity. This foreign key is referred to as the *collection
key*. The collection key is mapped by the `<key>` element.

Collections may contain almost any other NHibernate type, including all
basic types, custom types, entity types and components. This is an
important definition: An object in a collection can either be handled
with "pass by value" semantics (it therefore fully depends on the
collection owner) or it can be a reference to another entity with an own
lifecycle. Collections may not contain other collections. The contained
type is referred to as the *collection element type*. Collection
elements are mapped by `<element>`, `<composite-element>`,
`<one-to-many>`, `<many-to-many>` or `<many-to-any>`. The first two map
elements with value semantics, the other three are used to map entity
associations.

All collection types except `ISet` and bag have an *index* column - a
column that maps to an array or `IList` index or `IDictionary` key. The
index of an `IDictionary` may be of any basic type, an entity type or
even a composite type (it may not be a collection). The index of an
array or list is always of type `Int32`. Indexes are mapped using
`<index>`, `<index-many-to-many>`, `<composite-index>` or
`<index-many-to-any>`.

There are quite a range of mappings that can be generated for
collections, covering many common relational models. We suggest you
experiment with the schema generation tool to get a feeling for how
various mapping declarations translate to database tables.

# Mapping a Collection

Collections are declared by the `<set>`, `<list>`, `<map>`, `<bag>`,
`<array>` and `<primitive-array>` elements. `<map>` is representative:

    <map
        name="propertyName"
        table="table_name"
        schema="schema_name"
        lazy="true|false|extra"
        inverse="true|false"
        cascade="all|none|save-update|delete|all-delete-orphan"
        sort="unsorted|natural|comparatorClass"
        order-by="column_name asc|desc"
        where="arbitrary sql where condition"
        fetch="select|join"
        batch-size="N"
        access="field|property|ClassName"
        optimistic-lock="true|false"
        generic="true|false"
    >
    
        <key .... />
        <index .... />
        <element .... />
    </map>

  - `name` the collection property name

  - `table` (optional - defaults to property name) the name of the
    collection table (not used for one-to-many associations)

  - `schema` (optional) the name of a table schema to override the
    schema declared on the root element

  - `lazy` (optional - defaults to `true`) may be used to disable lazy
    fetching and specify that the association is always eagerly fetched.
    Using `extra` fetches only the elements that are needed - see
    [???](#performance-fetching) for more information.

  - `inverse` (optional - defaults to `false`) mark this collection as
    the "inverse" end of a bidirectional association

  - `cascade` (optional - defaults to `none`) enable operations to
    cascade to child entities

  - `sort` (optional) specify a sorted collection with `natural` sort
    order, or a given comparator class

  - `order-by` (optional) specify a table column (or columns) that
    define the iteration order of the `IDictionary`, `ISet` or bag,
    together with an optional `asc` or `desc`

  - `where` (optional) specify an arbitrary SQL `WHERE` condition to be
    used when retrieving or removing the collection (useful if the
    collection should contain only a subset of the available data)

  - `fetch` (optional) Choose between outer-join fetching and fetching
    by sequential select.

  - `batch-size` (optional, defaults to `1`) specify a "batch size" for
    lazily fetching instances of this collection.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `optimistic-lock` (optional - defaults to `true`): Species that
    changes to the state of the collection results in increment of the
    owning entity's version. (For one to many associations, it is often
    reasonable to disable this setting.)

  - `generic` (optional, obsolete): Choose between generic and
    non-generic collection interfaces. But currently NHibernate only
    supports generic collections.

The mapping of an `IList` or array requires a separate table column
holding the array or list index (the `i` in `foo[i]`). If your
relational model doesn't have an index column, e.g. if you're working
with legacy data, use an unordered `ISet` instead. This seems to put
people off who assume that `IList` should just be a more convenient way
of accessing an unordered collection. NHibernate collections strictly
obey the actual semantics attached to the `ISet`, `IList` and
`IDictionary` interfaces. `IList` elements don't just spontaneously
rearrange themselves\!

On the other hand, people who planned to use the `IList` to emulate
*bag* semantics have a legitimate grievance here. A bag is an unordered,
unindexed collection which may contain the same element multiple times.
The .NET collections framework lacks an `IBag` interface, hence you have
to emulate it with an `IList`. NHibernate lets you map properties of
type `IList` or `ICollection` with the `<bag>` element. Note that bag
semantics are not really part of the `ICollection` contract and they
actually conflict with the semantics of the `IList` contract (however,
you can sort the bag arbitrarily, discussed later in this chapter).

Note: Large NHibernate bags mapped with `inverse="false"` are
inefficient and should be avoided; NHibernate can't create, delete or
update rows individually, because there is no key that may be used to
identify an individual row.

# Collections of Values and Many-To-Many Associations

A collection table is required for any collection of values and any
collection of references to other entities mapped as a many-to-many
association (the natural semantics for a .NET collection). The table
requires (foreign) key column(s), element column(s) and possibly index
column(s).

The foreign key from the collection table to the table of the owning
class is declared using a `<key>` element.

    <key column="column_name"/>

  - `column` (required): The name of the foreign key column.

For indexed collections like maps and lists, we require an `<index>`
element. For lists, this column contains sequential integers numbered
from zero. Make sure that your index really starts from zero if you have
to deal with legacy data. For maps, the column may contain any values of
any NHibernate type.

    <index
            column="column_name"
            type="typename"
    />

  - `column` (required): The name of the column holding the collection
    index values.

  - `type` (optional, defaults to `Int32`): The type of the collection
    index.

Alternatively, a map may be indexed by objects of entity type. We use
the `<index-many-to-many>` element.

    <index-many-to-many
            column="column_name"
            class="ClassName"
    />

  - `column` (required): The name of the foreign key column for the
    collection index values.

  - `class` (required): The entity class used as the collection index.

For a collection of values, we use the `<element>` tag.

    <element
            column="column_name"
            type="typename"
    />

  - `column` (required): The name of the column holding the collection
    element values.

  - `type` (required): The type of the collection element.

A collection of entities with its own table corresponds to the
relational notion of *many-to-many association*. A many to many
association is the most natural mapping of a .NET collection but is not
usually the best relational model.

    <many-to-many
            column="column_name"
            class="ClassName"
            fetch="join|select"
            not-found="ignore|exception"
        />

  - `column` (required): The name of the element foreign key column.

  - `class` (required): The name of the associated class.

  - `fetch` (optional, defaults to `join`): enables outer-join or
    sequential select fetching for this association. This is a special
    case; for full eager fetching (in a single SELECT) of an entity and
    its many-to-many relationships to other entities, you would enable
    join fetching not only of the collection itself, but also with this
    attribute on the `
                                                                                                    <many-to-many>` nested element.

  - `not-found` (optional - defaults to `exception`): Specifies how
    foreign keys that reference missing rows will be handled: `ignore`
    will treat a missing row as a null association.

Some examples, first, a set of strings:

    <set name="Names" table="NAMES">
        <key column="GROUPID"/>
        <element column="NAME" type="String"/>
    </set>

A bag containing integers (with an iteration order determined by the
`order-by` attribute):

    <bag name="Sizes" table="SIZES" order-by="SIZE ASC">
        <key column="OWNER"/>
        <element column="SIZE" type="Int32"/>
    </bag>

An array of entities - in this case, a many to many association (note
that the entities are lifecycle objects, `cascade="all"`):

    <array name="Foos" table="BAR_FOOS" cascade="all">
        <key column="BAR_ID"/>
        <index column="I"/>
        <many-to-many column="FOO_ID" class="Eg.Foo, Eg"/>
    </array>

A map from string indices to
    dates:

    <map name="Holidays" table="holidays" schema="dbo" order-by="hol_name asc">
        <key column="id"/>
        <index column="hol_name" type="String"/>
        <element column="hol_date" type="Date"/>
    </map>

A list of components (discussed in the next chapter):

    <list name="CarComponents" table="car_components">
        <key column="car_id"/>
        <index column="posn"/>
        <composite-element class="Eg.Car.CarComponent">
                <property name="Price" type="float"/>
                <property name="Type" type="Eg.Car.ComponentType, Eg"/>
                <property name="SerialNumber" column="serial_no" type="String"/>
        </composite-element>
    </list>

# One-To-Many Associations

A *one to many association* links the tables of two classes *directly*,
with no intervening collection table. (This implements a *one-to-many*
relational model.) This relational model loses some of the semantics of
.NET collections:

  - No null values may be contained in a dictionary, set or list

  - An instance of the contained entity class may not belong to more
    than one instance of the collection

  - An instance of the contained entity class may not appear at more
    than one value of the collection index

An association from `Foo` to `Bar` requires the addition of a key column
and possibly an index column to the table of the contained entity class,
`Bar`. These columns are mapped using the `<key>` and `<index>` elements
described above.

The `<one-to-many>` tag indicates a one to many association.

    <one-to-many
            class="ClassName"
            not-found="ignore|exception"
        />

  - `class` (required): The name of the associated class.

  - `not-found` (optional - defaults to `exception`): Specifies how
    foreign keys that reference missing rows will be handled: `ignore`
    will treat a missing row as a null association.

Example:

    <set name="Bars">
        <key column="foo_id"/>
        <one-to-many class="Eg.Bar, Eg"/>
    </set>

Notice that the `<one-to-many>` element does not need to declare any
columns. Nor is it necessary to specify the `table` name anywhere.

*Very Important Note:* If the `<key>` column of a `<one-to-many>`
association is declared `NOT NULL`, NHibernate may cause constraint
violations when it creates or updates the association. To prevent this
problem, *you must use a bidirectional association* with the many valued
end (the set or bag) marked as `inverse="true"`. See the discussion of
bidirectional associations later in this chapter.

# Lazy Initialization

Collections (other than arrays) may be lazily initialized, meaning they
load their state from the database only when the application needs to
access it. Initialization happens transparently to the user so the
application would not normally need to worry about this (in fact,
transparent lazy initialization is the main reason why NHibernate needs
its own collection implementations). However, if the application tries
something like this:

    IDictionary<string, int> permissions;
    using (s = sessions.OpenSession())
    using (ITransaction tx = sessions.BeginTransaction())
    {
        var u = s.Load<User>(userId);
        permissions = u.Permissions;
        tx.Commit();
    }
    
    int accessLevel = permissions["accounts"];  // Error!

It could be in for a nasty surprise. Since the permissions collection
was not initialized when the `ISession` was committed, the collection
will never be able to load its state. The fix is to move the line that
reads from the collection to just before the commit. (There are other
more advanced ways to solve this problem, however.)

Alternatively, use a non-lazy collection. Since lazy initialization can
lead to bugs like that above, non-laziness is the default. However, it
is intended that lazy initialization be used for almost all collections,
especially for collections of entities (for reasons of efficiency).

Exceptions that occur while lazily initializing a collection are wrapped
in a `LazyInitializationException`.

Declare a lazy collection using the optional `lazy` attribute:

    <set name="Names" table="NAMES" lazy="true">
        <key column="group_id"/>
        <element column="NAME" type="String"/>
    </set>

In some application architectures, particularly where the code that
accesses data using NHibernate, and the code that uses it are in
different application layers, it can be a problem to ensure that the
`ISession` is open when a collection is initialized. There are two basic
ways to deal with this issue:

  - In a web-based application, an event handler can be used to close
    the `ISession` only at the very end of a user request, once the
    rendering of the view is complete. Of course, this places heavy
    demands upon the correctness of the exception handling of your
    application infrastructure. It is vitally important that the
    `ISession` is closed and the transaction ended before returning to
    the user, even when an exception occurs during rendering of the
    view. The event handler has to be able to access the `ISession` for
    this approach. We recommend that the current `ISession` is stored in
    the `HttpContext.Items` collection (see chapter 1,
    [???](#quickstart-playingwithcats), for an example implementation).

  - In an application with a separate business tier, the business logic
    must "prepare" all collections that will be needed by the web tier
    before returning. This means that the business tier should load all
    the data and return all the data already initialized to the
    presentation/web tier that is required for a particular use case.
    Usually, the application calls `NHibernateUtil.Initialize()` for
    each collection that will be needed in the web tier (this call must
    occur before the session is closed) or retrieves the collection
    eagerly using a NHibernate query with a `FETCH` clause.

  - You may also attach a previously loaded object to a new `ISession`
    with `Update()` or `Lock()` before accessing uninitialized
    collections (or other proxies). NHibernate can not do this
    automatically, as it would introduce ad hoc transaction semantics\!

You can use the `CreateFilter()` method of the NHibernate ISession API
to get the size of a collection without initializing it:

    var count = s
        .CreateFilter(collection, "select count(*)")
        .UniqueResult<long>();

`CreateFilter()` is also used to efficiently retrieve subsets of a
collection without needing to initialize the whole collection.

# Sorted Collections

NHibernate supports collections implemented by
`System.Collections.Generic.SortedList<T>` and
`System.Collections.Generic.SortedSet<T>`. You must specify a comparer
in the mapping file:

    <set name="Aliases" table="person_aliases" sort="natural">
        <key column="person"/>
        <element column="name" type="String"/>
    </set>
    
    <map name="Holidays" sort="My.Custom.HolidayComparer, MyAssembly" lazy="true">
        <key column="year_id"/>
        <index column="hol_name" type="String"/>
        <element column="hol_date" type="Date"/>
    </map>

Allowed values of the `sort` attribute are `unsorted`, `natural` and the
name of a class implementing `System.Collections.Generic.IComparer<T>`.

If you want the database itself to order the collection elements use the
`order-by` attribute of `set`, `bag` or `map` mappings. This performs
the ordering in the SQL query, not in memory.

Setting the `order-by` attribute tells NHibernate to use
`Iesi.Collections.Generic.LinkedHashSet` class internally for sets,
maintaining the order of the elements. It is not supported on maps.

    <set name="Aliases" table="person_aliases" order-by="name asc">
        <key column="person"/>
        <element column="name" type="String"/>
    </set>
    
    <map name="Holidays" order-by="hol_date, hol_name" lazy="true">
        <key column="year_id"/>
        <index column="hol_name" type="String"/>
        <element column="hol_date type="Date"/>
    </map>

Note that the value of the `order-by` attribute is an SQL ordering, not
a HQL ordering\!

Associations may even be sorted by some arbitrary criteria at runtime
using a `CreateFilter()`.

    sortedUsers = s
        .CreateFilter(group.Users, "order by this.Name")
        .List<User>();

# Using an `<idbag>`

If you've fully embraced our view that composite keys are a bad thing
and that entities should have synthetic identifiers (surrogate keys),
then you might find it a bit odd that the many to many associations and
collections of values that we've shown so far all map to tables with
composite keys\! Now, this point is quite arguable; a pure association
table doesn't seem to benefit much from a surrogate key (though a
collection of composite values *might*). Nevertheless, NHibernate
provides a feature that allows you to map many to many associations and
collections of values to a table with a surrogate key.

The `<idbag>` element lets you map a `List` (or `Collection`) with bag
semantics.

    <idbag name="Lovers" table="LOVERS" lazy="true">
        <collection-id column="ID" type="Int64">
            <generator class="hilo"/>
        </collection-id>
        <key column="PERSON1"/>
        <many-to-many column="PERSON2" class="Eg.Person" fetch="join"/>
    </idbag>

As you can see, an `<idbag>` has a synthetic id generator, just like an
entity class\! A different surrogate key is assigned to each collection
row. NHibernate does not provide any mechanism to discover the surrogate
key value of a particular row, however.

Note that the update performance of an `<idbag>` is *much* better than a
regular `<bag>`\! NHibernate can locate individual rows efficiently and
update or delete them individually, just like a list, map or set.

As of version 2.0, the `native` identifier generation strategy is
supported for `<idbag>` collection identifiers.

# Bidirectional Associations

A *bidirectional association* allows navigation from both "ends" of the
association. Two kinds of bidirectional association are supported:

  - one-to-many  
    set or bag valued at one end, single-valued at the other

  - many-to-many  
    set or bag valued at both ends

You may specify a bidirectional many-to-many association simply by
mapping two many-to-many associations to the same database table and
declaring one end as *inverse* (which one is your choice). Here's an
example of a bidirectional many-to-many association from a class back to
*itself* (each category can have many items and each item can be in many
categories):

    <class name="NHibernate.Auction.Category, NHibernate.Auction">
      <id name="Id" column="ID"/>
      ...
      <bag name="Items" table="CATEGORY_ITEM" lazy="true">
        <key column="CATEGORY_ID"/>
        <many-to-many class="NHibernate.Auction.Item, NHibernate.Auction" column="ITEM_ID"/>
      </bag>
    </class>
    
    <class name="NHibernate.Auction.Item, NHibernate.Auction">
      <id name="id" column="ID"/>
      ...
    
      <!-- inverse end -->
      <bag name="categories" table="CATEGORY_ITEM" inverse="true" lazy="true">
        <key column="ITEM_ID"/>
        <many-to-many class="NHibernate.Auction.Category, NHibernate.Auction"
            column="CATEGORY_ID"/>
      </bag>
    </class>

Changes made only to the inverse end of the association are *not*
persisted. This means that NHibernate has two representations in memory
for every bidirectional association, one link from A to B and another
link from B to A. This is easier to understand if you think about the
.NET object model and how we create a many-to-many relationship in
    C\#:

    category.Items.Add(item);          // The category now "knows" about the relationship
    item.Categories.Add(category);     // The item now "knows" about the relationship
    
    session.Update(item);                     // No effect, nothing will be saved!
    session.Update(category);                 // The relationship will be saved

The non-inverse side is used to save the in-memory representation to the
database. We would get an unnecessary INSERT/UPDATE and probably even a
foreign key violation if both would trigger changes\! The same is of
course also true for bidirectional one-to-many associations.

You may map a bidirectional one-to-many association by mapping a
one-to-many association to the same table column(s) as a many-to-one
association and declaring the many-valued end `inverse="true"`.

    <class name="Eg.Parent, Eg">
        <id name="Id" column="id"/>
        ....
        <set name="Children" inverse="true" lazy="true">
            <key column="parent_id"/>
            <one-to-many class="Eg.Child, Eg"/>
        </set>
    </class>
    
    <class name="Eg.Child, Eg">
        <id name="Id" column="id"/>
        ....
        <many-to-one name="Parent" class="Eg.Parent, Eg" column="parent_id"/>
    </class>

Mapping one end of an association with `inverse="true"` doesn't affect
the operation of cascades, both are different concepts\!

# Bidirectional associations with indexed collections

There are some additional considerations for bidirectional mappings with
indexed collections (where one end is represented as a `<list>` or
`<map>`) when using NHibernate mapping files. If there is a property of
the child class that maps to the index column you can use
`inverse="true"` on the collection mapping:

    <class name="Parent">
        <id name="Id" column="parent_id"/>
        ....
        <map name="Children" inverse="true">
            <key column="parent_id"/>
            <map-key column="name"
                type="string"/>
            <one-to-many class="Child"/>
        </map>
    </class>
    
    <class name="Child">
        <id name="Id" column="child_id"/>
        ....
        <property name="Name" column="name"
            not-null="true"/>
        <many-to-one name="Parent"
            class="Parent"
            column="parent_id"
            not-null="true"/>
    </class>

If there is no such property on the child class, the association cannot
be considered truly bidirectional. That is, there is information
available at one end of the association that is not available at the
other end. In this case, you cannot map the collection `inverse="true"`.
Instead, you could use the following mapping:

    <class name="Parent">
        <id name="Id" column="parent_id"/>
        ....
        <map name="Children">
            <key column="parent_id"
                not-null="true"/>
            <map-key column="name"
                type="string"/>
            <one-to-many class="Child"/>
        </map>
    </class>
    
    <class name="Child">
        <id name="Id" column="child_id"/>
        ....
        <many-to-one name="Parent"
            class="Parent"
            column="parent_id"
            insert="false"
            update="false"
            not-null="true"/>
    </class>

Note that in this mapping, the collection-valued end of the association
is responsible for updates to the foreign key.

# Ternary Associations

There are two possible approaches to mapping a ternary association. One
approach is to use composite elements (discussed below). Another is to
use an `IDictionary` with an association as its index:

    <map name="Contracts" lazy="true">
        <key column="employer_id"/>
        <index-many-to-many column="employee_id" class="Employee"/>
        <one-to-many class="Contract"/>
    </map>

    <map name="Connections" lazy="true">
        <key column="node1_id"/>
        <index-many-to-many column="node2_id" class="Node"/>
        <many-to-many column="connection_id" class="Connection"/>
    </map>

# Heterogeneous Associations

The `<many-to-any>` and `<index-many-to-any>` elements provide for true
heterogeneous associations. These mapping elements work in the same way
as the `<any>` element - and should also be used rarely, if ever.

# Collection examples

The previous sections are pretty confusing. So lets look at an example.
This class:

    using System;
    using System.Collections.Generic;
    
    namespace Eg
    
        public class Parent
        {
            public long Id { get; set; }
    
            private ISet<Child> Children { get; set; }
    
            ....
            ....
        }
    }

has a collection of `Eg.Child` instances. If each child has at most one
parent, the most natural mapping is a one-to-many association:

    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2"
        assembly="Eg" namespace="Eg">
    
        <class name="Parent">
            <id name="Id">
                <generator class="sequence"/>
            </id>
            <set name="Children" lazy="true">
                <key column="parent_id"/>
                <one-to-many class="Child"/>
            </set>
        </class>
    
        <class name="Child">
            <id name="Id">
                <generator class="sequence"/>
            </id>
            <property name="Name"/>
        </class>
    
    </hibernate-mapping>

This maps to the following table definitions:

    create table parent (Id bigint not null primary key)
    create table child (Id bigint not null primary key, Name varchar(255), parent_id bigint)
    alter table child add constraint childfk0 (parent_id) references parent

If the parent is *required*, use a bidirectional one-to-many
association:

    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2"
        assembly="Eg" namespace="Eg">
    
        <class name="Parent">
            <id name="Id">
                <generator class="sequence"/>
            </id>
            <set name="Children" inverse="true" lazy="true">
                <key column="parent_id"/>
                <one-to-many class="Child"/>
            </set>
        </class>
    
        <class name="Child">
            <id name="Id">
                <generator class="sequence"/>
            </id>
            <property name="Name"/>
            <many-to-one name="parent" class="Parent" column="parent_id" not-null="true"/>
        </class>
    
    </hibernate-mapping>

Notice the `NOT NULL` constraint:

    create table parent ( Id bigint not null primary key )
    create table child ( Id bigint not null
                         primary key,
                         Name varchar(255),
                         parent_id bigint not null )
    alter table child add constraint childfk0 (parent_id) references parent

On the other hand, if a child might have multiple parents, a
many-to-many association is appropriate:

    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2"
        assembly="Eg" namespace="Eg">
    
        <class name="Parent">
            <id name="Id">
                <generator class="sequence"/>
            </id>
            <set name="Children" lazy="true" table="childset">
                <key column="parent_id"/>
                <many-to-many class="Child" column="child_id"/>
            </set>
        </class>
    
        <class name="eg.Child">
            <id name="Id">
                <generator class="sequence"/>
            </id>
            <property name="Name"/>
        </class>
    
    </hibernate-mapping>

Table definitions:

    create table parent ( Id bigint not null primary key )
    create table child ( Id bigint not null primary key, name varchar(255) )
    create table childset ( parent_id bigint not null,
                            child_id bigint not null,
                            primary key ( parent_id, child_id ) )
    alter table childset add constraint childsetfk0 (parent_id) references parent
    alter table childset add constraint childsetfk1 (child_id) references child

See also [???](#example-parentchild).
