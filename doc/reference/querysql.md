# Native SQL

You may also express queries in the native SQL dialect of your database.
This is useful if you want to utilize database specific features such as
query hints or the `CONNECT` keyword in Oracle. It also provides a clean
migration path from a direct SQL/`ADO.NET` based application to
NHibernate.

NHibernate allows you to specify handwritten SQL (including stored
procedures) for all create, update, delete, and load operations.

# Using an `ISQLQuery` <a name="querysql-creating"></a>

Execution of native SQL queries is controlled via the `ISQLQuery`
interface, which is obtained by calling `ISession.CreateSQLQuery()`. The
following describes how to use this API for querying.

## Scalar queries <a name="querysql-scalar"></a>

The most basic SQL query is to get a list of scalars (values).

```csharp
    sess.CreateSQLQuery("SELECT * FROM CATS")
     .AddScalar("ID", NHibernateUtil.Int64)
     .AddScalar("NAME", NHibernateUtil.String)
     .AddScalar("BIRTHDATE", NHibernateUtil.Date)
```

This query specified:

  - the SQL query string

  - the columns and types to return

This will return an `IList` of `Object` arrays (`object[]`) with scalar
values for each column in the CATS table. Only these three columns will
be returned, even though the query is using `*` and could return more
than the three listed columns.

## Entity queries <a name="querysql-entity"></a>

The above query was about returning scalar values, basically returning
the "raw" values from the result set. The following shows how to get
entity objects from a native SQL query via `AddEntity()`.

```csharp
    sess.CreateSQLQuery("SELECT * FROM CATS").AddEntity(typeof(Cat));
    sess.CreateSQLQuery("SELECT ID, NAME, BIRTHDATE FROM CATS").AddEntity(typeof(Cat));
```

This query specified:

  - the SQL query string

  - the entity returned by the query

Assuming that Cat is mapped as a class with the columns ID, NAME and
BIRTHDATE the above queries will both return an IList where each element
is a Cat entity.

If the entity is mapped with a `many-to-one` to another entity it is
required to also return its identifier when performing the native query,
otherwise a database specific "column not found" error will occur. The
additional columns will automatically be returned when using the \*
notation, but we prefer to be explicit as in the following example for a
`many-to-one` to a `Dog`:

```csharp
    sess.CreateSQLQuery("SELECT ID, NAME, BIRTHDATE, DOG_ID FROM CATS")
        .AddEntity(typeof(Cat));
```

This will allow cat.Dog property access to function properly.

## Handling associations and collections <a name="querysql-associations-collections"></a>

It is possible to eagerly join in the `Dog` to avoid the possible extra
round-trip for initializing the proxy. This is done via the `AddJoin()`
method, which allows you to join in an association or collection.

```csharp
    sess
        .CreateSQLQuery(
            "SELECT cat.ID, NAME, BIRTHDATE, DOG_ID, D_ID, D_NAME " +
            "FROM CATS cat, DOGS d WHERE cat.DOG_ID = d.D_ID")
        .AddEntity("cat", typeof(Cat))
        .AddJoin("cat.Dog");
```

In this example the returned `Cat`'s will have their `Dog` property
fully initialized without any extra round-trip to the database. Notice
that we added a alias name ("cat") to be able to specify the target
property path of the join. It is possible to do the same eager joining
for collections, e.g. if the `Cat` had a one-to-many to `Dog` instead.

```csharp
    sess
        .CreateSQLQuery(
            "SELECT ID, NAME, BIRTHDATE, D_ID, D_NAME, CAT_ID " +
            "FROM CATS cat, DOGS d WHERE cat.ID = d.CAT_ID")
        .AddEntity("cat", typeof(Cat))
        .AddJoin("cat.Dogs");
```

At this stage we are reaching the limits of what is possible with native
queries without starting to enhance the SQL queries to make them usable
in NHibernate; the problems start to arise when returning multiple
entities of the same type or when the default alias/column names are not
enough.

## Returning multiple entities <a name="querysql-multiple-entities"></a>

Until now the result set column names are assumed to be the same as the
column names specified in the mapping document. This can be problematic
for SQL queries which join multiple tables, since the same column names
may appear in more than one table.

Column alias injection is needed in the following query (which most
likely will fail):

```csharp
    sess
        .CreateSQLQuery(
            "SELECT cat.*, mother.* " +
            "FROM CATS cat, CATS mother WHERE cat.MOTHER_ID = mother.ID")
        .AddEntity("cat", typeof(Cat))
        .AddEntity("mother", typeof(Cat))
```

The intention for this query is to return two Cat instances per row, a
cat and its mother. This will fail since there is a conflict of names
since they are mapped to the same column names and on some databases the
returned column aliases will most likely be on the form `"c.ID"`,
`"c.NAME"`, etc. which are not equal to the columns specified in the
mappings ("ID" and "NAME").

The following form is not vulnerable to column name duplication:

```csharp
    sess
        .CreateSQLQuery(
            "SELECT {cat.*}, {mother.*} " +
            "FROM CATS cat, CATS mother WHERE cat.MOTHER_ID = mother.ID")
        .AddEntity("cat", typeof(Cat))
        .AddEntity("mother", typeof(Cat))
```

This query specified:

  - the SQL query string, with placeholders for NHibernate to inject
    column aliases

  - the entities returned by the query

The {cat.\*} and {mother.\*} notation used above is a shorthand for "all
properties". Alternatively, you may list the columns explicitly, but
even in this case we let NHibernate inject the SQL column aliases for
each property. The placeholder for a column alias is just the property
name qualified by the table alias. In the following example, we retrieve
Cats and their mothers from a different table (cat\_log) to the one
declared in the mapping metadata. Notice that we may even use the
property aliases in the where clause if we like.

```csharp
    String sql = "SELECT c.ID as {c.Id}, c.NAME as {c.Name}, " + 
             "c.BIRTHDATE as {c.BirthDate}, c.MOTHER_ID as {c.Mother}, {mother.*} " +
             "FROM CAT_LOG c, CAT_LOG m WHERE {c.Mother} = m.ID";
    
    var loggedCats = sess.CreateSQLQuery(sql)
        .AddEntity("c", typeof(Cat))
        .AddEntity("m", typeof(Cat)).List<object[]>();
```

### Alias and property references <a name="querysql-aliasreferences"></a>

For most cases the above alias injection is needed, but for queries
relating to more complex mappings like composite properties, inheritance
discriminators, collections etc. there are some specific aliases to use
to allow NHibernate to inject the proper aliases.

The following table shows the different possibilities of using the alias
injection. Note: the alias names in the result are examples, each alias
will have a unique and probably different name when
used.

| Description                                     | Syntax                                         | Example                                                            |
|-------------------------------------------------|------------------------------------------------|--------------------------------------------------------------------|
| A simple property                               | `{[aliasname].[propertyname]}`                 | `A_NAME as {item.Name}`                                            |
| A composite property                            | `{[aliasname].[componentname].[propertyname]}` | `CURRENCY as {item.Amount.Currency}, VALUE as {item.Amount.Value}` |
| Discriminator of an entity                      | `{[aliasname].class}`                          | `DISC as {item.class}`                                             |
| All properties of an entity                     | `{[aliasname].*}`                              | `{item.*}`                                                         |
| A collection key                                | `{[aliasname].key}`                            | `ORGID as {coll.key}`                                              |
| The id of an collection                         | `{[aliasname].id}`                             | `EMPID as {coll.id}`                                               |
| The element of an collection                    | `{[aliasname].element}`                        | `XID as {coll.element}`                                            |
| property of the element in the collection       | `{[aliasname].element.[propertyname]}`         | `NAME as {coll.element.Name}`                                      |
| All properties of the element in the collection | `{[aliasname].element.*}`                      | `{coll.element.*}`                                                 |
| All properties of the collection                | `{[aliasname].*}`                              | `{coll.*}`                                                         |

Alias injection names

## Returning non-managed entities <a name="querysql-non-managed-entities"></a>

It is possible to apply an `IResultTransformer` to native sql queries.
Allowing it to e.g. return non-managed entities.

```csharp
    sess.CreateSQLQuery("SELECT NAME, BIRTHDATE FROM CATS")
            .SetResultTransformer(Transformers.AliasToBean(typeof(CatDTO)))
```

This query specified:

  - the SQL query string

  - a result transformer

The above query will return a list of `CatDTO` which has been
instantiated and injected the values of NAME and BIRTHNAME into its
corresponding properties or fields.

IMPORTANT: The custom `IResultTransformer` should override `Equals` and
`GetHashCode`, otherwise the query translation won't be cached. This
also will result in memory leak.

## Handling inheritance <a name="querysql-inheritance"></a>

Native SQL queries which query for entities that are mapped as part of
an inheritance hierarchy must include all properties for the base class
and all its subclasses.

## Parameters <a name="querysql-parameters"></a>

Native SQL queries support positional as well as named parameters:

```csharp
    var query = sess
        .CreateSQLQuery("SELECT * FROM CATS WHERE NAME like ?")
        .AddEntity(typeof(Cat));
    var pusList = query.SetString(0, "Pus%").List<Cat>();
    
    query = sess
        .createSQLQuery("SELECT * FROM CATS WHERE NAME like :name")
        .AddEntity(typeof(Cat));
    var pusList = query.SetString("name", "Pus%").List<Cat>();
```

# Named SQL queries <a name="querysql-namedqueries"></a>

Named SQL queries may be defined in the mapping document and called in
exactly the same way as a named HQL query. In this case, we do *not*
need to call `AddEntity()`.

```xml
    <sql-query name="persons">
        <return alias="person" class="eg.Person"/>
        SELECT person.NAME AS {person.Name},
               person.AGE AS {person.Age},
               person.SEX AS {person.Sex}
        FROM PERSON person
        WHERE person.NAME LIKE :namePattern
    </sql-query>
```

```csharp
    var people = sess.GetNamedQuery("persons")
        .SetString("namePattern", namePattern)
        .SetMaxResults(50)
        .List<Person>();
```

The `<return-join>` and `<load-collection>` elements are used to join
associations and define queries which initialize collections,
respectively.

```xml
    <sql-query name="personsWith">
        <return alias="person" class="eg.Person"/>
        <return-join alias="address" property="person.MailingAddress"/>
        SELECT person.NAME AS {person.Name},
               person.AGE AS {person.Age},
               person.SEX AS {person.Sex},
               adddress.STREET AS {address.Street},
               adddress.CITY AS {address.City},
               adddress.STATE AS {address.State},
               adddress.ZIP AS {address.Zip}
        FROM PERSON person
        JOIN ADDRESS adddress
            ON person.ID = address.PERSON_ID AND address.TYPE='MAILING'
        WHERE person.NAME LIKE :namePattern
    </sql-query>
```

A named SQL query may return a scalar value. You must declare the column
alias and NHibernate type using the `<return-scalar>` element:

```xml
    <sql-query name="mySqlQuery">
        <return-scalar column="name" type="String"/>
        <return-scalar column="age" type="Int64"/>
        SELECT p.NAME AS name,
               p.AGE AS age,
        FROM PERSON p WHERE p.NAME LIKE 'Hiber%'
    </sql-query>
```

You can externalize the resultset mapping information in a `<resultset>`
element to either reuse them across several named queries or through the
`SetResultSetMapping()` API.

```xml
    <resultset name="personAddress">
        <return alias="person" class="eg.Person"/>
        <return-join alias="address" property="person.MailingAddress"/>
    </resultset>
    
    <sql-query name="personsWith" resultset-ref="personAddress">
        SELECT person.NAME AS {person.Name},
               person.AGE AS {person.Age},
               person.SEX AS {person.Sex},
               adddress.STREET AS {address.Street},
               adddress.CITY AS {address.City},
               adddress.STATE AS {address.State},
               adddress.ZIP AS {address.Zip}
        FROM PERSON person
        JOIN ADDRESS adddress
            ON person.ID = address.PERSON_ID AND address.TYPE='MAILING'
        WHERE person.NAME LIKE :namePattern
    </sql-query>
```

You can alternatively use the resultset mapping information in your
.hbm.xml files directly in code.

```csharp
    var cats = sess.CreateSQLQuery(
            "select {cat.*}, {kitten.*} " +
            "from cats cat, cats kitten " +
            "where kitten.mother = cat.id")
        .SetResultSetMapping("catAndKitten")
        .List<Cat>();
```

Like HQL named queries, SQL named queries accepts a number of attributes
matching settings available on the `ISQLQuery` interface.

  - `flush-mode` - override the session flush mode just for this query.

  - `cacheable` - allow the query results to be cached by the second
    level cache. See [NHibernate.Caches](caches.md).

  - `cache-region` - specify the cache region of the query.

  - `cache-mode` - specify the cache mode of the query.

  - `fetch-size` - set a fetch size for the underlying ADO query.

  - `timeout` - set the query timeout in seconds.

  - `read-only` - `true` switches yielded entities to read-only. See
    [Read-only entities](readonly.md).

  - `comment` - add a custom comment to the SQL.

## Using return-property to explicitly specify column/alias names <a name="propertyresults"></a>

With `<return-property>` you can explicitly tell NHibernate what column
aliases to use, instead of using the `{}`-syntax to let NHibernate
inject its own aliases.

```xml
    <sql-query name="mySqlQuery">
        <return alias="person" class="eg.Person">
            <return-property name="Name" column="myName"/>
            <return-property name="Age" column="myAge"/>
            <return-property name="Sex" column="mySex"/>
        </return>
        SELECT person.NAME AS myName,
               person.AGE AS myAge,
               person.SEX AS mySex,
        FROM PERSON person WHERE person.NAME LIKE :name
    </sql-query>
```

`<return-property>` also works with multiple columns. This solves a
limitation with the `{}`-syntax which can not allow fine grained control
of multi-column properties.

```xml
    <sql-query name="organizationCurrentEmployments">
        <return alias="emp" class="Employment">
            <return-property name="Salary">
                <return-column name="VALUE"/>
                <return-column name="CURRENCY"/>
            </return-property>
            <return-property name="EndDate" column="myEndDate"/>
        </return>
            SELECT EMPLOYEE AS {emp.Employee}, EMPLOYER AS {emp.Employer},
            STARTDATE AS {emp.StartDate}, ENDDATE AS {emp.EndDate},
            REGIONCODE as {emp.RegionCode}, EID AS {emp.Id}, VALUE, CURRENCY
            FROM EMPLOYMENT
            WHERE EMPLOYER = :id AND ENDDATE IS NULL
            ORDER BY STARTDATE ASC
    </sql-query>
```

Notice that in this example we used `<return-property>` in combination
with the `{}`-syntax for injection, allowing users to choose how they
want to refer column and properties.

If your mapping has a discriminator you must use
`<return-discriminator>` to specify the discriminator column.

## Using stored procedures for querying <a name="sp_query"></a>

NHibernate introduces support for queries via stored procedures and
functions. Most of the following documentation is equivalent for both.
The stored procedure/function must return a resultset to be able to work
with NHibernate. An example of such a stored function in MS SQL Server
2000 and higher is as follows:

```sql
    CREATE PROCEDURE selectAllEmployments AS
        SELECT EMPLOYEE, EMPLOYER, STARTDATE, ENDDATE,
        REGIONCODE, EMPID, VALUE, CURRENCY
        FROM EMPLOYMENT
```

To use this query in NHibernate you need to map it via a named query.

```xml
    <sql-query name="selectAllEmployments_SP">
        <return alias="emp" class="Employment">
            <return-property name="employee" column="EMPLOYEE"/>
            <return-property name="employer" column="EMPLOYER"/>
            <return-property name="startDate" column="STARTDATE"/>
            <return-property name="endDate" column="ENDDATE"/>
            <return-property name="regionCode" column="REGIONCODE"/>
            <return-property name="id" column="EID"/>
            <return-property name="salary">
                <return-column name="VALUE"/>
                <return-column name="CURRENCY"/>
            </return-property>
        </return>
        exec selectAllEmployments
    </sql-query>
```

Notice that stored procedures currently only return scalars and
entities. `<return-join>` and `<load-collection>` are not supported.

### Rules/limitations for using stored procedures <a name="querysql-limits-storedprocedures"></a>

To use stored procedures with NHibernate the procedures/functions have
to follow some rules. If they do not follow those rules they are not
usable with NHibernate. If you still want to use these procedures you
have to execute them via `session.Connection`. The rules are different
for each database, since database vendors have different stored
procedure semantics/syntax.

Stored procedure queries can't be paged with
`SetFirstResult()/SetMaxResults()`.

Recommended call form is dependent on your database. For MS SQL Server
use `exec functionName <parameters>`.

For Oracle the following rules apply:

  - A function must return a result set. The first parameter of a
    procedure must be an `OUT` that returns a result set. This is done
    by using a `SYS_REFCURSOR` type in Oracle 9i or later. In Oracle you
    need to define a `REF CURSOR` type, see Oracle literature.

For MS SQL server the following rules apply:

  - The procedure must return a result set. NHibernate will use
    `DbCommand.ExecuteReader()` to obtain the results.

  - If you can enable `SET NOCOUNT ON` in your procedure it will
    probably be more efficient, but this is not a requirement.

# Custom SQL for create, update and delete <a name="querysql-cud"></a>

NHibernate can use custom SQL statements for create, update, and delete
operations. The class and collection persisters in NHibernate already
contain a set of configuration time generated strings (insertsql,
deletesql, updatesql etc.). The mapping tags `<sql-insert>`,
`<sql-delete>`, and `<sql-update>` override these strings:

```xml
    <class name="Person">
        <id name="id">
            <generator class="increment"/>
        </id>
        <property name="name" not-null="true"/>
        <sql-insert>INSERT INTO PERSON (NAME, ID) VALUES ( UPPER(?), ? )</sql-insert>
        <sql-update>UPDATE PERSON SET NAME=UPPER(?) WHERE ID=?</sql-update>
        <sql-delete>DELETE FROM PERSON WHERE ID=?</sql-delete>
    </class>
```

Note that the custom `sql-insert` will not be used if you use `identity`
to generate identifier values for the class.

The SQL is directly executed in your database, so you are free to use
any dialect you like. This will of course reduce the portability of your
mapping if you use database specific SQL.

Stored procedures are supported if the database-native syntax is used:

```xml
    <class name="Person">
        <id name="id">
            <generator class="increment"/>
        </id>
        <property name="name" not-null="true"/>
        <sql-insert>exec createPerson ?, ?</sql-insert>
        <sql-delete>exec deletePerson ?</sql-delete>
        <sql-update>exec updatePerson ?, ?</sql-update>
    </class>
```

The order of the positional parameters is currently vital, as they must
be in the same sequence as NHibernate expects them.

You can see the expected order by enabling debug logging for the
`NHibernate.Persister.Entity` level. With this level enabled NHibernate
will print out the static SQL that is used to create, update, delete
etc. entities. (To see the expected sequence, remember to not include
your custom SQL in the mapping files as that will override the
NHibernate generated static sql.)

The stored procedures are by default required to affect the same number
of rows as NHibernate-generated SQL would. NHibernate uses
`DbCommand.ExecuteNonQuery` to retrieve the number of rows affected.
This check can be disabled by using `check="none"` attribute in
`sql-insert` element.

# Custom SQL for loading <a name="querysql-load"></a>

You may also declare your own SQL (or HQL) queries for entity loading:

```xml
    <sql-query name="person">
        <return alias="pers" class="Person" lock-mode="upgrade"/>
        SELECT NAME AS {pers.Name}, ID AS {pers.Id}
        FROM PERSON
        WHERE ID=?
        FOR UPDATE
    </sql-query>
```

This is just a named query declaration, as discussed earlier. You may
reference this named query in a class mapping:

```xml
    <class name="Person">
        <id name="Id">
            <generator class="increment"/>
        </id>
        <property name="Name" not-null="true"/>
        <loader query-ref="person"/>
    </class>
```

This even works with stored procedures.

You may even define a query for collection loading:

```xml
    <set name="Employments" inverse="true">
        <key/>
        <one-to-many class="Employment"/>
        <loader query-ref="employments"/>
    </set>

    <sql-query name="employments">
        <load-collection alias="emp" role="Person.Employments"/>
        SELECT {emp.*}
        FROM EMPLOYMENT emp
        WHERE EMPLOYER = :id
        ORDER BY STARTDATE ASC, EMPLOYEE ASC
    </sql-query>
```

You could even define an entity loader that loads a collection by join
fetching:

```xml
    <sql-query name="person">
        <return alias="pers" class="Person"/>
        <return-join alias="emp" property="pers.Employments"/>
        SELECT NAME AS {pers.*}, {emp.*}
        FROM PERSON pers
        LEFT OUTER JOIN EMPLOYMENT emp
            ON pers.ID = emp.PERSON_ID
        WHERE ID=?
    </sql-query>
```
