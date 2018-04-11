# HQL: The Hibernate Query Language

NHibernate is equipped with an extremely powerful query language that
(quite intentionally) looks very much like SQL. But don't be fooled by
the syntax; HQL is fully object-oriented, understanding notions like
inheritance, polymorphism and association.

# Case Sensitivity

Queries are case-insensitive, except for names of .NET classes and
properties. So `SeLeCT` is the same as `sELEct` is the same as `SELECT`
but `Eg.FOO` is not `Eg.Foo` and `foo.barSet` is not `foo.BARSET`.

This manual uses lowercase HQL keywords. Some users find queries with
uppercase keywords more readable, but we find this convention ugly when
embedded in C\# code.

# The from clause

The simplest possible NHibernate query is of the form:

    from Eg.Cat

which simply returns all instances of the class `Eg.Cat`.

Most of the time, you will need to assign an *alias*, since you will
want to refer to the `Cat` in other parts of the query.

    from Eg.Cat as cat

This query assigns the alias `cat` to `Cat` instances, so we could use
that alias later in the query. The `as` keyword is optional; we could
also write:

    from Eg.Cat cat

Multiple classes may appear, resulting in a cartesian product or "cross"
join.

    from Formula, Parameter

    from Formula as form, Parameter as param

It is considered good practice to name query aliases using an initial
lowercase, consistent with naming standards for local variables (eg.
`domesticCat`).

# Associations and joins

We may also assign aliases to associated entities, or even to elements
of a collection of values, using a `join`.

    from Eg.Cat as cat 
        inner join cat.Mate as mate
        left outer join cat.Kittens as kitten
    
    from Eg.Cat as cat left join cat.Mate.Kittens as kittens
    
    from Formula form full join form.Parameter param

The supported join types are borrowed from ANSI SQL

  - `inner join`

  - `left outer join`

  - `right outer join`

  - `full join` (not usually useful)

The `inner join`, `left outer join` and `right outer join` constructs
may be abbreviated.

    from Eg.Cat as cat 
        join cat.Mate as mate
        left join cat.Kittens as kitten

In addition, a "fetch" join allows associations or collections of values
to be initialized along with their parent objects, using a single
select. This is particularly useful in the case of a collection. It
effectively overrides the outer join and lazy declarations of the
mapping file for associations and collections. See
[???](#performance-fetching) for more information.

    from Eg.Cat as cat 
        inner join fetch cat.Mate
        left join fetch cat.Kittens

The associated objects are not returned directly in the query results.
Instead, they may be accessed via the parent object.

It is possible to create a cartesian product by join fetching more than
one collection in a query, so take care in this case. Join fetching
multiple collection roles is also disabled for bag mappings. Note also
that the `fetch` construct may not be used in queries called using
`Enumerable()`. Finally, note that `full join fetch` and `right join
fetch` are not meaningful.

# The select clause

The `select` clause picks which objects and properties to return in the
query result set. Consider:

    select mate 
    from Eg.Cat as cat 
        inner join cat.Mate as mate

The query will select `Mate`s of other `Cat`s. Actually, you may express
this query more compactly as:

    select cat.Mate from Eg.Cat cat

You may even select collection elements, using the special `elements`
function. The following query returns all kittens of any cat.

    select elements(cat.Kittens) from Eg.Cat cat

Queries may return properties of any value type including properties of
component type:

    select cat.Name from Eg.DomesticCat cat
    where cat.Name like 'fri%'
    
    select cust.Name.FirstName from Customer as cust

Queries may return multiple objects and/or properties as an array of
type `object[]`

    select mother, offspr, mate.Name 
    from Eg.DomesticCat as mother
        inner join mother.Mate as mate
        left outer join mother.Kittens as offspr

or as an actual type-safe object

    select new Family(mother, mate, offspr)
    from Eg.DomesticCat as mother
        join mother.Mate as mate
        left join mother.Kittens as offspr

assuming that the class `Family` has an appropriate constructor.

# Aggregate functions

HQL queries may even return the results of aggregate functions on
properties:

    select avg(cat.Weight), sum(cat.Weight), max(cat.Weight), count(cat)
    from Eg.Cat cat

Collections may also appear inside aggregate functions in the `select`
clause.

    select cat, count( elements(cat.Kittens) ) 
    from Eg.Cat cat group by cat.Id, cat.Weight, ...

The supported aggregate functions are

  - `avg(...), sum(...), min(...), max(...)`

  - `count(*)`

  - `count(...), count(distinct ...), count(all...)`

The `distinct` and `all` keywords may be used and have the same
semantics as in SQL.

    select distinct cat.Name from Eg.Cat cat
    
    select count(distinct cat.Name), count(cat) from Eg.Cat cat

# Polymorphic queries

A query like:

    from Eg.Cat as cat

returns instances not only of `Cat`, but also of subclasses like
`DomesticCat`. NHibernate queries may name *any* .NET class or interface
in the `from` clause. The query will return instances of all persistent
classes that extend that class or implement the interface. The following
query would return all persistent objects:

    from System.Object o

The interface `INamed` might be implemented by various persistent
classes:

    from Eg.Named n, Eg.Named m where n.Name = m.Name

Note that these last two queries will require more than one SQL
`SELECT`. This means that the `order by` clause does not correctly order
the whole result set.

In order to use non-mapped base classes or interfaces in HQL queries,
they have to be imported. See [???](#mapping-declaration-import) for
more information.

# The where clause

The `where` clause allows you to narrow the list of instances returned.

    from Eg.Cat as cat where cat.Name='Fritz'

returns instances of `Cat` named 'Fritz'.

    select foo 
    from Eg.Foo foo, Eg.Bar bar
    where foo.StartDate = bar.Date

will return all instances of `Foo` for which there exists an instance of
`Bar` with a `Date` property equal to the `StartDate` property of the
`Foo`. Compound path expressions make the `where` clause extremely
powerful. Consider:

    from Eg.Cat cat where cat.Mate.Name is not null

This query translates to an SQL query with a table (inner) join. If you
were to write something like

    from Eg.Foo foo  
    where foo.Bar.Baz.Customer.Address.City is not null

you would end up with a query that would require four table joins in
SQL.

The `=` operator may be used to compare not only properties, but also
instances:

    from Eg.Cat cat, Eg.Cat rival where cat.Mate = rival.Mate
    
    select cat, mate 
    from Eg.Cat cat, Eg.Cat mate
    where cat.Mate = mate

The special property (lowercase) `id` may be used to reference the
unique identifier of an object. (You may also use its property name.)

    from Eg.Cat as cat where cat.id = 123
    
    from Eg.Cat as cat where cat.Mate.id = 69

The second query is efficient. No table join is required\!

Properties of composite identifiers may also be used. Suppose `Person`
has a composite identifier consisting of `Country` and `MedicareNumber`.

    from Bank.Person person
    where person.id.Country = 'AU' 
        and person.id.MedicareNumber = 123456
    
    from Bank.Account account
    where account.Owner.id.Country = 'AU' 
        and account.Owner.id.MedicareNumber = 123456

Once again, the second query requires no table join.

Likewise, the special property `class` accesses the discriminator value
of an instance in the case of polymorphic persistence. A .Net class name
embedded in the where clause will be translated to its discriminator
value.

    from Eg.Cat cat where cat.class = Eg.DomesticCat

You may also specify properties of components or composite user types
(and of components of components, etc). Never try to use a
path-expression that ends in a property of component type (as opposed to
a property of a component). For example, if `store.Owner` is an entity
with a component `Address`

    store.Owner.Address.City    // okay
    store.Owner.Address         // error!

An "any" type has the special properties `id` and `class`, allowing us
to express a join in the following way (where `AuditLog.Item` is a
property mapped with `<any>`).

    from Eg.AuditLog log, Eg.Payment payment 
    where log.Item.class = 'Eg.Payment, Eg, Version=...' and log.Item.id = payment.id

Notice that `log.Item.class` and `payment.class` would refer to the
values of completely different database columns in the above query.

# Expressions

Expressions allowed in the `where` clause include most of the kind of
things you could write in SQL:

  - mathematical operators `+, -, *, /`

  - binary comparison operators `=, >=, <=, <>, !=, like`

  - logical operations `and, or, not`

  - string concatenation ||

  - SQL scalar functions like `upper()` and `lower()`

  - Parentheses `( )` indicate grouping

  - `in`, `between`, `is null`

  - positional parameters `?`

  - named parameters `:name`, `:start_date`, `:x1`

  - SQL literals `'foo'`, `69`, `'1970-01-01 10:00:01.0'`

  - Enumeration values and constants `Eg.Color.Tabby`

`in` and `between` may be used as follows:

    from Eg.DomesticCat cat where cat.Name between 'A' and 'B'
    
    from Eg.DomesticCat cat where cat.Name in ( 'Foo', 'Bar', 'Baz' )

and the negated forms may be written

    from Eg.DomesticCat cat where cat.Name not between 'A' and 'B'
    
    from Eg.DomesticCat cat where cat.Name not in ( 'Foo', 'Bar', 'Baz' )

Likewise, `is null` and `is not null` may be used to test for null
values.

Booleans may be easily used in expressions by declaring HQL query
substitutions in NHibernate configuration:

```xml
    <property name="query.substitutions">true 1, false 0</property>
```

This will replace the keywords `true` and `false` with the literals `1`
and `0` in the translated SQL from this HQL:

    from Eg.Cat cat where cat.Alive = true

You may test the size of a collection with the special property `size`,
or the special `size()` function.

    from Eg.Cat cat where cat.Kittens.size > 0
    
    from Eg.Cat cat where size(cat.Kittens) > 0

For indexed collections, you may refer to the minimum and maximum
indices using `minIndex` and `maxIndex`. Similarly, you may refer to the
minimum and maximum elements of a collection of basic type using
`minElement` and `maxElement`.

    from Calendar cal where cal.Holidays.maxElement > current date

There are also functional forms (which, unlike the constructs above, are
not case sensitive):

    from Order order where maxindex(order.Items) > 100
    
    from Order order where minelement(order.Items) > 10000

The SQL functions `any, some, all, exists, in` are supported when passed
the element or index set of a collection (`elements` and `indices`
functions) or the result of a sub-query (see below).

    select mother from Eg.Cat as mother, Eg.Cat as kit
    where kit in elements(mother.Kittens)
    
    select p from Eg.NameList list, Eg.Person p
    where p.Name = some elements(list.Names)
    
    from Eg.Cat cat where exists elements(cat.Kittens)
    
    from Eg.Player p where 3 > all elements(p.Scores)
    
    from Eg.Show show where 'fizard' in indices(show.Acts)

Note that these constructs - `size`, `elements`, `indices`, `minIndex`,
`maxIndex`, `minElement`, `maxElement` - have certain usage
restrictions:

  - in a `where` clause: only for databases with sub-selects

  - in a `select` clause: only `elements` and `indices` make sense

Elements of indexed collections (arrays, lists, maps) may be referred to
by index (in a where clause only):

    from Order order where order.Items[0].id = 1234
    
    select person from Person person, Calendar calendar
    where calendar.Holidays['national day'] = person.BirthDay
        and person.Nationality.Calendar = calendar
    
    select item from Item item, Order order
    where order.Items[ order.DeliveredItemIndices[0] ] = item and order.id = 11
    
    select item from Item item, Order order
    where order.Items[ maxindex(order.items) ] = item and order.id = 11

The expression inside `[]` may even be an arithmetic expression.

    select item from Item item, Order order
    where order.Items[ size(order.Items) - 1 ] = item

HQL also provides the built-in `index()` function, for elements of a
one-to-many association or collection of values.

    select item, index(item) from Order order 
        join order.Items item
    where index(item) < 5

Scalar SQL functions supported by the underlying database may be used

    from Eg.DomesticCat cat where upper(cat.Name) like 'FRI%'

If you are not yet convinced by all this, think how much longer and less
readable the following query would be in SQL:

    select cust
    from Product prod,
        Store store
        inner join store.Customers cust
    where prod.Name = 'widget'
        and store.Location.Name in ( 'Melbourne', 'Sydney' )
        and prod = all elements(cust.CurrentOrder.LineItems)

*Hint:* something
    like

```sql
    SELECT cust.name, cust.address, cust.phone, cust.id, cust.current_order
    FROM customers cust,
        stores store,
        locations loc,
        store_customers sc,
        product prod
    WHERE prod.name = 'widget'
        AND store.loc_id = loc.id
        AND loc.name IN ( 'Melbourne', 'Sydney' )
        AND sc.store_id = store.id
        AND sc.cust_id = cust.id
        AND prod.id = ALL(
            SELECT item.prod_id
            FROM line_items item, orders o
            WHERE item.order_id = o.id
                AND cust.current_order = o.id
        )
```

# The order by clause

The list returned by a query may be ordered by any property of a
returned class or components:

    from Eg.DomesticCat cat
    order by cat.Name asc, cat.Weight desc, cat.Birthdate

The optional `asc` or `desc` indicate ascending or descending order
respectively.

# The group by clause

A query that returns aggregate values may be grouped by any property of
a returned class or components:

    select cat.Color, sum(cat.Weight), count(cat) 
    from Eg.Cat cat
    group by cat.Color
    
    select foo.id, avg( elements(foo.Names) ), max( indices(foo.Names) ) 
    from Eg.Foo foo
    group by foo.id

Note: You may use the `elements` and `indices` constructs inside a
select clause, even on databases with no sub-selects.

A `having` clause is also allowed.

    select cat.color, sum(cat.Weight), count(cat) 
    from Eg.Cat cat
    group by cat.Color 
    having cat.Color in (Eg.Color.Tabby, Eg.Color.Black)

SQL functions and aggregate functions are allowed in the `having` and
`order by` clauses, if supported by the underlying database (ie. not in
MySQL).

    select cat
    from Eg.Cat cat
        join cat.Kittens kitten
    group by cat.Id, cat.Name, cat.Other, cat.Properties
    having avg(kitten.Weight) > 100
    order by count(kitten) asc, sum(kitten.Weight) desc

Note that neither the `group by` clause nor the `order by` clause may
contain arithmetic expressions. Also note that NHibernate currently does
not expand a grouped entity, so you can't write `group by cat` if all
properties of `cat` are non-aggregated. You have to list all
non-aggregated properties explicitly.

# Sub-queries

For databases that support sub-selects, NHibernate supports sub-queries
within queries. A sub-query must be surrounded by parentheses (often by
an SQL aggregate function call). Even correlated sub-queries
(sub-queries that refer to an alias in the outer query) are allowed.

    from Eg.Cat as fatcat 
    where fatcat.Weight > ( 
        select avg(cat.Weight) from Eg.DomesticCat cat 
    )
    
    from Eg.DomesticCat as cat 
    where cat.Name = some ( 
        select name.NickName from Eg.Name as name 
    )
        
    from Eg.Cat as cat 
    where not exists ( 
        from eg.Cat as mate where mate.Mate = cat 
    )
    
    from Eg.DomesticCat as cat 
    where cat.Name not in ( 
        select name.NickName from Eg.Name as name 
    )

# HQL examples

NHibernate queries can be quite powerful and complex. In fact, the power
of the query language is one of NHibernate's main selling points. Here
are some example queries very similar to queries that I used on a recent
project. Note that most queries you will write are much simpler than
these\!

The following query returns the order id, number of items and total
value of the order for all unpaid orders for a particular customer and
given minimum total value, ordering the results by total value. In
determining the prices, it uses the current catalog. The resulting SQL
query, against the `ORDER`, `ORDER_LINE`, `PRODUCT`, `CATALOG` and
`PRICE` tables has four inner joins and an (uncorrelated) subselect.

    select order.id, sum(price.Amount), count(item)
    from Order as order
        join order.LineItems as item
        join item.Product as product,
        Catalog as catalog
        join catalog.Prices as price
    where order.Paid = false
        and order.Customer = :customer
        and price.Product = product
        and catalog.EffectiveDate < sysdate
        and catalog.EffectiveDate >= all (
            select cat.EffectiveDate 
            from Catalog as cat
            where cat.EffectiveDate < sysdate
        )
    group by order
    having sum(price.Amount) > :minAmount
    order by sum(price.Amount) desc

What a monster\! Actually, in real life, I'm not very keen on
sub-queries, so my query was really more like this:

    select order.id, sum(price.amount), count(item)
    from Order as order
        join order.LineItems as item
        join item.Product as product,
        Catalog as catalog
        join catalog.Prices as price
    where order.Paid = false
        and order.Customer = :customer
        and price.Product = product
        and catalog = :currentCatalog
    group by order
    having sum(price.Amount) > :minAmount
    order by sum(price.Amount) desc

The next query counts the number of payments in each status, excluding
all payments in the `AwaitingApproval` status where the most recent
status change was made by the current user. It translates to an SQL
query with two inner joins and a correlated subselect against the
`PAYMENT`, `PAYMENT_STATUS` and `PAYMENT_STATUS_CHANGE` tables.

    select count(payment), status.Name 
    from Payment as payment 
        join payment.CurrentStatus as status
        join payment.StatusChanges as statusChange
    where payment.Status.Name <> PaymentStatus.AwaitingApproval
        or (
            statusChange.TimeStamp = ( 
                select max(change.TimeStamp) 
                from PaymentStatusChange change 
                where change.Payment = payment
            )
            and statusChange.User <> :currentUser
        )
    group by status.Name, status.SortOrder
    order by status.SortOrder

If I would have mapped the `StatusChanges` collection as a list, instead
of a set, the query would have been much simpler to write.

    select count(payment), status.Name 
    from Payment as payment
        join payment.CurrentStatus as status
    where payment.Status.Name <> PaymentStatus.AwaitingApproval
        or payment.StatusChanges[ maxIndex(payment.StatusChanges) ].User <> :currentUser
    group by status.Name, status.SortOrder
    order by status.SortOrder

The next query uses the MS SQL Server `isNull()` function to return all
the accounts and unpaid payments for the organization to which the
current user belongs. It translates to an SQL query with three inner
joins, an outer join and a subselect against the `ACCOUNT`, `PAYMENT`,
`PAYMENT_STATUS`, `ACCOUNT_TYPE`, `ORGANIZATION` and `ORG_USER` tables.

    select account, payment
    from Account as account
        left outer join account.Payments as payment
    where :currentUser in elements(account.Holder.Users)
        and PaymentStatus.Unpaid = isNull(payment.CurrentStatus.Name, PaymentStatus.Unpaid)
    order by account.Type.SortOrder, account.AccountNumber, payment.DueDate

For some databases, we would need to do away with the (correlated)
subselect.

    select account, payment
    from Account as account
        join account.Holder.Users as user
        left outer join account.Payments as payment
    where :currentUser = user
        and PaymentStatus.Unpaid = isNull(payment.CurrentStatus.Name, PaymentStatus.Unpaid)
    order by account.Type.SortOrder, account.AccountNumber, payment.DueDate

# Tips & Tricks

You can count the number of query results without actually returning
them:

    var count = session.CreateQuery("select count(*) from ....").UniqueResult<long>();

To order a result by the size of a collection, use the following query:

    select usr.id, usr.Name
    from User as usr 
        left join usr.Messages as msg
    group by usr.id, usr.Name
    order by count(msg)

If your database supports sub-selects, you can place a condition upon
selection size in the where clause of your query:

    from User usr where size(usr.Messages) >= 1

If your database doesn't support sub-selects, use the following query:

    select usr.id, usr.Name
    from User usr
        join usr.Messages msg
    group by usr.id, usr.Name
    having count(msg) >= 1

As this solution can't return a `User` with zero messages because of the
inner join, the following form is also useful:

    select usr.id, usr.Name
    from User as usr
        left join usr.Messages as msg
    group by usr.id, usr.Name
    having count(msg) = 0

Properties of an object can be bound to named query parameters:

```csharp
    IQuery q =
        s.CreateQuery("from foo in class Foo where foo.Name=:Name and foo.Size=:Size");
    q.SetProperties(fooBean); // fooBean has properties Name and Size
    var foos = q.List<Foo>();
```

Collections are pageable by using the `IQuery` interface with a filter:

```csharp
    IQuery q = s.CreateFilter( collection, "" ); // the trivial filter
    q.setMaxResults(PageSize);
    q.setFirstResult(PageSize * pageNumber);
    var page = q.List<Cat>();
```

Collection elements may be ordered or grouped using a query filter:

```csharp
    var orderedCollection = s
        .CreateFilter(collection, "order by this.Amount")
        .List<Cat>();
    var counts = s
        .CreateFilter(collection,
            "select this.Type, count(this) group by this.Type")
        .List<object[]>();
```
