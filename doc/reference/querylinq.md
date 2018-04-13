# Linq Queries

NHibernate 3.0 introduces the Linq to NHibernate provider, which allows
the use of the Linq API for querying with NHibernate.

`IQueryable` queries are obtained with the `Query` methods used on the
`ISession` or `IStatelessSession`. (Prior to NHibernate 5.0, these
methods were extensions defined in the `NHibernate.Linq` namespace.) A
number of NHibernate Linq extensions giving access to NHibernate
specific features are defined in the `NHibernate.Linq` namespace. Of
course, the Linq namespace is still needed too.

```csharp
    using System.Linq;
    using NHibernate.Linq;
```

Note: NHibernate has another querying API which uses lambda,
[QueryOver](queryqueryover.md). It should not be confused with a Linq
provider.

# Structure of a Query

Queries are created from an ISession using the syntax:

```csharp
    IList<Cat> cats =
        session.Query<Cat>()
            .Where(c => c.Color == "white")
            .ToList();
```

The `Query<TEntity>` function yields an `IQueryable<TEntity>`, with
which Linq extension methods or Linq syntax can be used. When executed,
the `IQueryable<TEntity>` will be translated to a SQL query on the
database.

It is possible to query a specific sub-class while still using a
queryable of the base class.

```csharp
    IList<Cat> cats =
        session.Query<Cat>("Eg.DomesticCat, Eg")
            .Where(c => c.Name == "Max")
            .ToList();
```

Starting with NHibernate 5.0, queries can also be created from an entity
collection, with the standard Linq extension `AsQueryable` available
from `System.Linq` namespace.

```csharp
    IList<Cat> whiteKittens =
        cat.Kittens.AsQueryable()
            .Where(k => k.Color == "white")
            .ToList();
```

This will be executed as a query on that `cat`'s kittens without loading
the entire collection.

If the collection is a map, call `AsQueryable` on its `Values` property.

```csharp
    IList<Cat> whiteKittens =
        cat.Kittens.Values.AsQueryable()
            .Where(k => k.Color == "white")
            .ToList();
```
 

A client timeout for the query can be defined. As most others NHibernate
specific features for Linq, this is available through an extension
defined in `NHibernate.Linq` namespace.

```csharp
    IList<Cat> cats =
        session.Query<Cat>()
            .Where(c => c.Color == "black")
            // Allows 10 seconds only.
            .SetOptions(o => o.SetTimeout(10))
            .ToList();
```

# Parameter types

Query parameters get extracted from the Linq expression. Their types are
selected according to [NHibernate types](mapping.md#nhibernate-types) default for
.Net types.

The `MappedAs` extension method allows to override the default type.

```csharp
    IList<Cat> cats =
        session.Query<Cat>()
            .Where(c => c.BirthDate == DateTime.Today.MappedAs(NHibernateUtil.Date))
            .ToList();

    IList<Cat> cats =
        session.Query<Cat>()
            .Where(c => c.Name == "Max".MappedAs(TypeFactory.Basic("AnsiString(200)")))
            .ToList();
```

# Supported methods and members

Many methods and members of common .Net types are supported by the Linq
to NHibernate provider. They will be translated to the appropriate SQL,
provided they are called on an entity property (or expression deriving
from) or at least one of their arguments references an entity property.
(Otherwise, their return values will be evaluated with .Net runtime
before query execution.)

## Common methods

The .Net 4 `CompareTo` method of strings and numerical types is
translated to a `case` statement yielding `-1|0|1` according to the
result of the comparison.

Many type conversions are available. For all of them, .Net overloads
with more than one argument are not supported.

Numerical types can be converted to other numerical types or parsed from
strings, using following methods:

  - `Convert.ToDecimal`

  - `Convert.ToDouble`

  - `Convert.ToInt32`

  - `Decimal.Parse`

  - `Double.Parse`

  - `Int32.Parse`

Strings can be converted to `Boolean` and `DateTime` with
`Convert.ToBoolean` or `Boolean.Parse` and `Convert.ToDateTime` or
`DateTime.Parse` respectively.

On all types supporting string conversion, `ToString` method can be
called.

```csharp
    IList<string> catBirthDates =
        session.Query<Cat>()
            .Select(c => c.BirthDate.ToString())
            .ToList();
```

`Equals` methods taking a single argument with the same type can be
used. Of course, `==` is supported too.

## `DateTime` and `DateTimeOffset`

Date and time parts properties can be called on `DateTime` and
`DateTimeOffset`. Those properties are:

  - `Date`

  - `Day`

  - `Hour`

  - `Minute`

  - `Month`

  - `Second`

  - `Year`

## `ICollection`, non generic and generic

Collections `Contains` methods are supported.

```csharp
    IList<Cat> catsWithWrongKitten =
        session.Query<Cat>()
            .Where(c => c.Kittens.Contains(c))
            .ToList();
```

## `IDictionary`, non generic and generic

Dictionaries `Item` getter are supported. This enables referencing a
dictionary item value in a `where` condition, as it can be done with
[HQL expressions](#queryhql-expressions).

Non generic dictionary method `Contains` and generic dictionary method
`ContainsKey` are translated to corresponding [`indices`](#queryhql-expressions). Supposing `Acts` in following HQL
example is generic,

```sql
    from Eg.Show show where 'fizard' in indices(show.Acts)

```

it could be written with Linq:

```csharp
    IList<Show> shows =
        session.Query<Show>()
            .Where(s => s.Acts.ContainsKey("fizard"))
            .ToList();
```

## Mathematical functions

The following list of mathematical functions from `System.Math` is
handled:

  - Trigonometric functions: `Acos`, `Asin`, `Atan`, `Atan2`, `Cos`,
    `Cosh`, `Sin`, `Sinh`, `Tan`, `Tanh`

  - `Abs` (all overloads)

  - `Ceiling` (both overloads)

  - `Floor` (both overloads)

  - `Pow`

  - `Round` (only overloads without a mode argument)

  - `Sign` (all overloads)

  - `Sqrt`

  - `Truncate` (both overloads)

## Nullables

On `Nullable<>` types, `GetValueOrDefault` methods, with or without a
provided default value, are supported.

## Strings

The following properties and methods are supported on strings:

  - `Contains`

  - `EndsWith` (without additional parameters)

  - `IndexOf` (only overloads taking a character or a string, and
    optionally a start index)

  - `Length`

  - `Replace` (both overloads)

  - `StartsWith` (without additional parameters)

  - `Substring` (both overloads)

  - `ToLower` (without additional parameters) and `ToLowerInvariant`,
    both translated to the same database lower function.

  - `ToUpper` (without additional parameters) and `ToUpperInvariant`,
    both translated to the same database upper function.

  - `Trim` (both overloads)

  - `TrimEnd`

  - `TrimStart`

 

Furthermore, a string `Like` extension methods allows expressing SQL
`like` conditions.

```csharp
    IList<DomesticCat> cats =
        session.Query<DomesticCat>()
            .Where(c => c.Name.Like("L%l%l"))
            .ToList();
```

This `Like` extension method is a Linq to NHibernate method only. Trying
to call it in another context is not supported.

If you want to avoid depending on the `NHibernate.Linq` namespace, you
can define your own replica of the `Like` methods. Any 2 or 3 arguments
method named `Like` in a class named `SqlMethods` will be translated.

# Future results

Future results are supported by the Linq provider. They are not
evaluated till one gets executed. At that point, all defined future
results are evaluated in one single round-trip to database.

```csharp
    // Define queries
    IFutureEnumerable<Cat> cats =
        session.Query<Cat>()
            .Where(c => c.Color == "black")
            .ToFuture();
    IFutureValue<int> catCount =
        session.Query<Cat>()
            .ToFutureValue(q => q.Count());
    // Execute them
    foreach(Cat cat in cats.GetEnumerable())
    {
        // Do something
    }
    if (catCount.Value > 10)
    {
        // Do something
    }
```

In above example, accessing `catCount.Value` does not trigger a
round-trip to database: it has been evaluated with
`cats.GetEnumerable()` call. If instead `catCount.Value` was accessed
first, it would have executed both future and `cats.GetEnumerable()`
would have not trigger a round-trip to database.

# Fetching associations

A Linq query may load associated entities or collection of entities.
Once the query is defined, using `Fetch` allows fetching a related
entity, and `FetchMany` allows fetching a collection. These methods are
defined as extensions in `NHibernate.Linq` namespace.

```csharp
    IList<Cat> oldCats =
        session.Query<Cat>()
            .Where(c => c.BirthDate.Year < 2010)
            .Fetch(c => c.Mate)
            .FetchMany(c => c.Kittens)
            .ToList();
```

Issuing many `FetchMany` on the same query may cause a cartesian product
over the fetched collections. This can be avoided by splitting the
fetches among [future queries](#future-results).

```csharp
    IQueryable<Cat> oldCatsQuery =
        session.Query<Cat>()
            .Where(c => c.BirthDate.Year < 2010);
    oldCatsQuery
        .Fetch(c => c.Mate)
        .FetchMany(c => c.Kittens)
        .ToFuture();
    IList<Cat> oldCats =
        oldCatsQuery
            .FetchMany(c => c.AnotherCollection)
            .ToFuture()
            .GetEnumerable()
            .ToList();
```

Use `ThenFetch` and `ThenFetchMany` for fetching associations of the
previously fetched association.

```csharp
    IList<Cat> oldCats =
        session.Query<Cat>()
            .Where(c => c.BirthDate.Year < 2010)
            .Fetch(c => c.Mate)
            .FetchMany(c => c.Kittens)
            .ThenFetch(k => k.Mate)
            .ToList();
```

# Modifying entities inside the database

Beginning with NHibernate 5.0, Linq queries can be used for inserting,
updating or deleting entities. The query defines the data to delete,
update or insert, and then `Delete`, `Update`, `UpdateBuilder`,
`InsertInto` and `InsertBuilder` queryable extension methods allow to
delete it, or instruct in which way it should be updated or inserted.
Those queries happen entirely inside the database, without extracting
corresponding entities out of the database.

These operations are a Linq implementation of [DML-style operations](batch.md#dml-style-operations), with the same abilities and limitations.

## Inserting new entities

`InsertInto` and `InsertBuilder` method extensions expect a NHibernate
queryable defining the data source of the insert. This data can be
entities or a projection. Then they allow specifying the target entity
type to insert, and how to convert source data to those target entities.
Three forms of target specification exist.

Using projection to target entity:

```csharp
    session.Query<Cat>()
        .Where(c => c.BodyWeight > 20)
        .InsertInto(c => new Dog { Name = c.Name + "dog", BodyWeight = c.BodyWeight });
```

Projections can be done with an anonymous object too, but it requires
supplying explicitly the target type, which in turn requires
re-specifying the source type:

```csharp
    session.Query<Cat>()
        .Where(c => c.BodyWeight > 20)
        .InsertInto<Cat, Dog>(c => new { Name = c.Name + "dog", BodyWeight = c.BodyWeight });
```

Or using assignments:

```csharp
    session.Query<Cat>()
        .Where(c => c.BodyWeight > 20)
        .InsertBuilder()
        .Into<Dog>()
        .Value(d => d.Name, c => c.Name + "dog")
        .Value(d => d.BodyWeight, c => c.BodyWeight)
        .Insert();
```

In all cases, unspecified properties are not included in the resulting
SQL insert. [`version`](#mapping-declaration-version) and
[`timestamp`](#mapping-declaration-timestamp) properties are exceptions.
If not specified, they are inserted with their `seed` value.

For more information on `Insert` limitations, please refer to
[DML-style operations](batch.md#dml-style-operations).

## Updating entities

`Update` and `UpdateBuilder` method extensions expect a NHibernate
queryable defining the entities to update. Then they allow specifying
which properties should be updated with which values. As for insertion,
three forms of target specification exist.

Using projection to updated entity:

```csharp
    session.Query<Cat>()
        .Where(c => c.BodyWeight > 20)
        .Update(c => new Cat { BodyWeight = c.BodyWeight / 2 });
```

Projections can be done with an anonymous object too:

```csharp
    session.Query<Cat>()
        .Where(c => c.BodyWeight > 20)
        .Update(c => new { BodyWeight = c.BodyWeight / 2 });
```

Or using assignments:

```csharp
    session.Query<Cat>()
        .Where(c => c.BodyWeight > 20)
        .UpdateBuilder()
        .Set(c => c.BodyWeight, c => c.BodyWeight / 2)
        .Update();
```

In all cases, unspecified properties are not included in the resulting
SQL update. This could be changed for
[`version`](#mapping-declaration-version) and
[`timestamp`](#mapping-declaration-timestamp) properties: using
`UpdateVersioned` instead of `Update` allows incrementing the version.
Custom version types (`NHibernate.Usertype.IUserVersionType`) are not
supported.

When using projection to updated entity, please note that the
constructed entity must have the exact same type than the underlying
queryable source type. Attempting to project to any other class
(anonymous projections excepted) will fail.

## Deleting entities

`Delete` method extension expects a queryable defining the entities to
delete. It immediately deletes them.

```csharp
    session.Query<Cat>()
        .Where(c => c.BodyWeight > 20)
        .Delete();
```

# Query cache

The Linq provider can use the query cache if it is setup. Refer to
[The Query Cache](performance.md#the-query-cache) for more details on how to set it up.


`SetOptions` extension method allows to enable the cache for the query.

```csharp
    IList<Cat> oldCats =
        session.Query<Cat>()
            .Where(c => c.BirthDate.Year < 2010)
            .SetOptions(o => o.SetCacheable(true))
            .ToList();
```


The cache mode and cache region can be specified too.

```csharp
    IList<Cat> cats =
        session.Query<Cat>()
            .Where(c => c.Name == "Max")
            .SetOptions(o => o
                .SetCacheable(true)
                .SetCacheRegion("catNames")
                .SetCacheMode(CacheMode.Put))
            .ToList();
```

# Extending the Linq to NHibernate provider

The Linq to NHibernate provider can be extended for supporting
additional SQL functions or translating additional methods or properties
to a SQL query.

## Adding SQL functions

NHibernate Linq provider feature a `LinqExtensionMethod` attribute. It
allows using an arbitrary, built-in or user defined, SQL function. It
should be applied on a method having the same arguments than the SQL
function.

```csharp
    public static class CustomLinqExtensions
    {
        [LinqExtensionMethod()]
        public static string Checksum(this double input)
        {
            // No need to implement it in .Net, unless you wish to call it
            // outside IQueryable context too.
            throw new NotImplementedException("This call should be translated " +
                "to SQL and run db side, but it has been run with .Net runtime");
        }
    }
```

Then it can be used in a Linq to NHibernate query.

```csharp
    var rnd = (new Random()).NextDouble();
    IList<Cat> cats =
        session.Query<Cat>()
            // Pseudo random order
            .OrderBy(c => (c.Id * rnd).Checksum())
            .ToList();
```

The function name is inferred from the method name. If needed, another
name can be provided.

```csharp
    public static class CustomLinqExtensions
    {
        [LinqExtensionMethod("dbo.aCustomFunction")]
        public static string ACustomFunction(this string input, string otherInput)
        {
            throw new NotImplementedException();
        }
    }
```

Since NHibernate v5.0, the Linq provider will no more evaluate in-memory
the method call even when it does not depend on the queried data. If you
wish to have the method call evaluated before querying whenever
possible, and then replaced in the query by its resulting value, specify
`LinqExtensionPreEvaluation.AllowPreEvaluation` on the attribute.

```csharp
    public static class CustomLinqExtensions
    {
        [LinqExtensionMethod("dbo.aCustomFunction",
            LinqExtensionPreEvaluation.AllowPreEvaluation)]
        public static string ACustomFunction(this string input, string otherInput)
        {
            // In-memory evaluation implementation.
            return input.Replace(otherInput, "blah");
        }
    }
```

## Adding a custom generator

Generators are responsible for translating .Net method calls found in
lambdas to the proper HQL constructs. Adding support for a new method
call can be achieved by registering an additional generator in the Linq
to NHibernate provider.

If the purpose of the added method is to simply call some SQL function,
using [Adding SQL functions](#adding-sql-functions) will be
easier.

 
As an example, here is how to add support for an `AsNullable` method
which would allow to call aggregates which may yield `null` without to
explicitly cast to the nullable type of the aggregate.

```csharp
    public static class NullableExtensions
    {
        public static T? AsNullable<T>(this T value) where T : struct
        {
            // Allow runtime use.
            // Not useful for linq-to-nhibernate, could be:
            // throw NotSupportedException();
            return value;
        }
    }
```

Adding support in Linq to NHibernate for a custom method requires a
generator. For this `AsNullable` method, we need a method generator,
declaring statically its supported method.

```csharp
    public class AsNullableGenerator : BaseHqlGeneratorForMethod
    {
        public AsNullableGenerator()
        {
            SupportedMethods = new[]
            {
                 ReflectHelper.GetMethodDefinition(() => NullableExtensions.AsNullable(0))
            };
        }
    
        public override HqlTreeNode BuildHql(MethodInfo method,
            Expression targetObject,
            ReadOnlyCollection<Expression> arguments,
            HqlTreeBuilder treeBuilder,
            IHqlExpressionVisitor visitor)
        {
            // This has just to transmit the argument "as is", HQL does not need
            // a specific call for null conversion.
            return visitor.Visit(arguments[0]).AsExpression();
        }
    }
```

There are property generators too, and the supported methods or
properties can be dynamically declared. Check NHibernate
`NHibernate.Linq.Functions` namespace classes's sources for more
examples. `CompareGenerator` and `DateTimePropertiesHqlGenerator` are
examples of those other cases.

For adding `AsNullableGenerator` in Linq to NHibernate provider, a new
generators registry should be used. Derive from the default one and
merge it. (Here we have a static declaration of method support case.)

```csharp
    public class ExtendedLinqToHqlGeneratorsRegistry :  DefaultLinqToHqlGeneratorsRegistry
    {
        public ExtendedLinqToHqlGeneratorsRegistry()
            : base()
        {
            this.Merge(new AsNullableGenerator());
        }
    }
```

In the case of dynamic declaration of method support, another call is
required instead of the merge: `RegisterGenerator`. `CompareGenerator`
illustrates this.

The last step is to instruct NHibernate to use this extended registry.
It can be achieved through [xml configuration](session-configuration.md#xml-configuration-file)
under `session-factory` node, or by [code](session-configuration.md#programmatic-configuration)
before building the session factory. Use one of them.

```xml
    <property name="linqtohql.generatorsregistry">
        YourNameSpace.ExtendedLinqToHqlGeneratorsRegistry, YourAssemblyName
    </property>
```

```csharp
    using NHibernate.Cfg;
    // ...
    
    var cfg = new Configuration();
    cfg.LinqToHqlGeneratorsRegistry<ExtendedLinqToHqlGeneratorsRegistry>();
    // And build the session factory with this configuration.
```

Now the following query could be executed, without failing if no `Max`
cat exists.

```csharp
    var oldestMaxBirthDate =
        session.Query<Cat>()
            .Where(c => c.Name == "Max")
            .Select(c => c.BirthDate.AsNullable())
            .Min();
```

(Of course, the same result could be obtained with
`(DateTime?)(c.BirthDate)`.)

By default, the Linq provider will try to evaluate the method call with
.Net runtime whenever possible, instead of translating it to SQL. It
will not do it if at least one of the parameters of the method call has
its value originating from an entity, or if the method is marked with
the `NoPreEvaluation` attribute (available since NHibernate 5.0).
