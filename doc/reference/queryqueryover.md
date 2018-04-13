# QueryOver Queries

The ICriteria API is NHibernate's implementation of Query Object.
NHibernate 3.0 introduces the QueryOver API, which combines the use of
Extension Methods and Lambda Expressions (both new in .Net 3.5) to
provide a statically type-safe wrapper round the ICriteria API.

QueryOver uses Lambda Expressions to provide some extra syntax to remove
the 'magic strings' from your ICriteria queries.

So, for example:

```csharp
    .Add(Expression.Eq("Name", "Smith"))
```

becomes:

```csharp
    .Where<Person>(p => p.Name == "Smith")
```

With this kind of syntax there are no 'magic strings', and refactoring
tools like 'Find All References', and 'Refactor-\>Rename' work
perfectly.

Note: QueryOver is intended to remove the references to 'magic strings'
from the ICriteria API while maintaining it's opaqueness. It is *not* a
LINQ provider; NHibernate has a built-in [Linq provider](querysql.md) for
this.

# Structure of a Query

Queries are created from an ISession using the syntax:

```csharp
    IList<Cat> cats =
        session.QueryOver<Cat>()
            .Where(c => c.Name == "Max")
            .List();
```

Detached QueryOver (analogous to DetachedCriteria) can be created, and
then used with an ISession using:

```csharp
    QueryOver<Cat> query =
        QueryOver.Of<Cat>()
            .Where(c => c.Name == "Paddy");
            
    IList<Cat> cats =
        query.GetExecutableQueryOver(session)
            .List();
```

Queries can be built up to use restrictions, projections, and ordering
using a fluent inline syntax:

```csharp
    var catNames =
        session.QueryOver<Cat>()
            .WhereRestrictionOn(c => c.Age).IsBetween(2).And(8)
            .Select(c => c.Name)
            .OrderBy(c => c.Name).Asc
            .List<string>();
```

# Simple Expressions

The Restrictions class (used by ICriteria) has been extended to include
overloads that allow Lambda Expression syntax. The Where() method works
for simple expressions (\<, \<=, ==, \!=, \>, \>=) so instead of:

```csharp
    ICriterion equalCriterion = Restrictions.Eq("Name", "Max")
```

You can
    write:

```csharp
    ICriterion equalCriterion = Restrictions.Where<Cat>(c => c.Name == "Max")
```

Since the QueryOver class (and IQueryOver interface) is generic and
knows the type of the query, there is an inline syntax for restrictions
that does not require the additional qualification of class name. So you
can also write:

```csharp
    var cats =
        session.QueryOver<Cat>()
            .Where(c => c.Name == "Max")
            .And(c => c.Age > 4)
            .List();
```

Note, the methods Where() and And() are semantically identical; the
And() method is purely to allow QueryOver to look similar to HQL/SQL.

Boolean comparisons can be made directly instead of comparing to
true/false:

```csharp
        .Where(p => p.IsParent)
        .And(p => !p.IsRetired)
```

 

Simple expressions can also be combined using the || and && operators.
So ICriteria like:

```csharp
        .Add(Restrictions.And(
                Restrictions.Eq("Name", "test name"),
                Restrictions.Or(
                    Restrictions.Gt("Age", 21),
                    Restrictions.Eq("HasCar", true))))
```

Can be written in QueryOver as:

```csharp
        .Where(p => p.Name == "test name" && (p.Age > 21 || p.HasCar))
```

 

Each of the corresponding overloads in the QueryOver API allows the use
of regular ICriterion to allow access to private properties.

```csharp
        .Where(Restrictions.Eq("Name", "Max"))
```

It is worth noting that the QueryOver API is built on top of the
ICriteria API. Internally the structures are the same, so at runtime the
statement below, and the statement above, are stored as exactly the same
ICriterion. The actual Lambda Expression is not stored in the query.

```csharp
        .Where(c => c.Name == "Max")
```

# Additional Restrictions

Some SQL operators/functions do not have a direct equivalent in C\#.
(e.g., the SQL `where name like '%anna%'`). These operators have
overloads for QueryOver in the Restrictions class, so you can write:

```csharp
        .Where(Restrictions.On<Cat>(c => c.Name).IsLike("%anna%"))
```

There is also an inline syntax to avoid the qualification of the type:

```csharp
        .WhereRestrictionOn(c => c.Name).IsLike("%anna%")
```

While simple expressions (see above) can be combined using the || and &&
operators, this is not possible with the other restrictions. So this
ICriteria:

```csharp
        .Add(Restrictions.Or(
            Restrictions.Gt("Age", 5)
            Restrictions.In("Name", new string[] { "Max", "Paddy" })))
```

Would have to be written as:

```csharp
        .Add(Restrictions.Or(
            Restrictions.Where<Cat>(c => c.Age > 5)
            Restrictions.On<Cat>(c => c.Name).IsIn(new string[] { "Max", "Paddy" })))
```

However, in addition to the additional restrictions factory methods,
there are extension methods to allow a more concise inline syntax for
some of the operators. So this:

```csharp
        .WhereRestrictionOn(c => c.Name).IsLike("%anna%")
```

May also be written as:

```csharp
        .Where(c => c..Name.IsLike("%anna%"))
```

# Associations

QueryOver can navigate association paths using JoinQueryOver()
(analogous to ICriteria.CreateCriteria() to create sub-criteria).

The factory method QuerOver\<T\>() on ISession returns an
IQueryOver\<T\>. More accurately, it returns an IQueryOver\<T,T\> (which
inherits from IQueryOver\<T\>).

An IQueryOver has two types of interest; the root type (the type of
entity that the query returns), and the type of the 'current' entity
being queried. For example, the following query uses a join to create a
sub-QueryOver (analogous to creating sub-criteria in the ICriteria API):

```csharp
    IQueryOver<Cat,Kitten> catQuery =
        session.QueryOver<Cat>()
            .JoinQueryOver(c => c.Kittens)
                .Where(k => k.Name == "Tiddles");
```

The JoinQueryOver returns a new instance of the IQueryOver than has its
root at the Kittens collection. The default type for restrictions is now
Kitten (restricting on the name 'Tiddles' in the above example), while
calling .List() will return an IList\<Cat\>. The type
IQueryOver\<Cat,Kitten\> inherits from IQueryOver\<Cat\>.

Note, the overload for JoinQueryOver takes an IEnumerable\<T\>, and the
C\# compiler infers the type from that. If your collection type is not
IEnumerable\<T\>, then you need to qualify the type of the sub-criteria:

```csharp
    IQueryOver<Cat,Kitten> catQuery =
        session.QueryOver<Cat>()
            .JoinQueryOver<Kitten>(c => c.Kittens)
                .Where(k => k.Name == "Tiddles");
```

The default join is an inner-join. Each of the additional join types can
be specified using the methods `.Inner, .Left, .Right,` or `.Full`. For
example, to left outer-join on Kittens use:

```csharp
    IQueryOver<Cat,Kitten> catQuery =
        session.QueryOver<Cat>()
            .Left.JoinQueryOver(c => c.Kittens)
                .Where(k => k.Name == "Tiddles");
```

# Join entities without association (Entity joins or ad hoc joins)

In QueryOver you have the ability to define a join to any entity, not
just through a mapped association. To achieve it, use `JoinEntityAlias`
and `JoinEntityQueryOver`. By example:

```csharp
    Cat cat = null;
    Cat joinedCat = null;
    
    var uniquelyNamedCats = sess.QueryOver<Cat>(() => cat)
        .JoinEntityAlias(
            () => joinedCat,
            () => cat.Name == joinedCat.Name && cat.Id != joinedCat.Id,
            JoinType.LeftOuterJoin)
        .Where(() => joinedCat.Id == null)
        .List();
```

# Aliases

In the traditional ICriteria interface aliases are assigned using 'magic
strings', however their value does not correspond to a name in the
object domain. For example, when an alias is assigned using
`.CreateAlias("Kitten", "kittenAlias")`, the string "kittenAlias" does
not correspond to a property or class in the domain.

In QueryOver, aliases are assigned using an empty variable. The variable
can be declared anywhere (but should be `null` at runtime). The compiler
can then check the syntax against the variable is used correctly, but at
runtime the variable is not evaluated (it's just used as a placeholder
for the alias).

Each Lambda Expression function in QueryOver has a corresponding
overload to allow use of aliases, and a .JoinAlias function to traverse
associations using aliases without creating a sub-QueryOver.

```csharp
    Cat catAlias = null;
    Kitten kittenAlias = null;
    
    IQueryOver<Cat,Cat> catQuery =
        session.QueryOver<Cat>(() => catAlias)
            .JoinAlias(() => catAlias.Kittens, () => kittenAlias)
            .Where(() => catAlias.Age > 5)
            .And(() => kittenAlias.Name == "Tiddles");
```

# Projections

Simple projections of the properties of the root type can be added using
the `.Select` method which can take multiple Lambda Expression
arguments:

```csharp
    var selection =
        session.QueryOver<Cat>()
            .Select(
                c => c.Name,
                c => c.Age)
            .List<object[]>();
```

Because this query no longer returns a Cat, the return type must be
explicitly specified. If a single property is projected, the return type
can be specified using:

```csharp
    IList<int> ages =
        session.QueryOver<Cat>()
            .Select(c => c.Age)
            .List<int>();
```

However, if multiple properties are projected, then the returned list
will contain object arrays, as per a projection in ICriteria. This could
be fed into an anonymous type using:

```csharp
    var catDetails =
        session.QueryOver<Cat>()
            .Select(
                c => c.Name,
                c => c.Age)
            .List<object[]>()
            .Select(properties => new {
                CatName = (string)properties[0],
                CatAge = (int)properties[1],
                });
                
    Console.WriteLine(catDetails[0].CatName);
    Console.WriteLine(catDetails[0].CatAge);
```

Note that the second `.Select` call in this example is an extension
method on IEnumerable\<T\> supplied in System.Linq; it is not part of
NHibernate.

QueryOver allows arbitrary IProjection to be added (allowing private
properties to be projected). The Projections factory class also has
overloads to allow Lambda Expressions to be used:

```csharp
    var selection =
        session.QueryOver<Cat>()
            .Select(Projections.ProjectionList()
                .Add(Projections.Property<Cat>(c => c.Name))
                .Add(Projections.Avg<Cat>(c => c.Age)))
            .List<object[]>();
```

In addition there is an inline syntax for creating projection lists that
does not require the explicit class qualification:

```csharp
    var selection =
        session.QueryOver<Cat>()
            .SelectList(list => list
                .Select(c => c.Name)
                .SelectAvg(c => c.Age))
            .List<object[]>();
```

Projections can also have arbitrary aliases assigned to them to allow
result transformation. If there is a CatSummary DTO class defined as:

```csharp
    public class CatSummary
    {
        public string Name { get; set; }
        public int AverageAge { get; set; }
    }
```

... then aliased projections can be used with the AliasToBean\<T\>
transformer:

```csharp
    CatSummary summaryDto = null;
    IList<CatSummary> catReport =
        session.QueryOver<Cat>()
            .SelectList(list => list
                .SelectGroup(c => c.Name).WithAlias(() => summaryDto.Name)
                .SelectAvg(c => c.Age).WithAlias(() => summaryDto.AverageAge))
            .TransformUsing(Transformers.AliasToBean<CatSummary>())
            .List<CatSummary>();
```

# Projection Functions

In addition to projecting properties, there are extension methods to
allow certain common dialect-registered functions to be applied. For
example you can write the following to get 3 letters named people.

```csharp
        .Where(p => p.FirstName.StrLength() == 3)
```

The functions can also be used inside projections:

```csharp
        .Select(
            p => Projections.Concat(p.LastName, ", ", p.FirstName),
            p => p.Height.Abs())
```

# Entities Projection

You can add entity projections via the `AsEntity()` extension.

```csharp
    Cat mate = null;
    
    var catAndMateNameList = sess.QueryOver<Cat>()
        .JoinAlias(c => c.Mate, () => mate)
        .Select(c => c.AsEntity(), c => mate.Name)
        .List<object[]>();
```

Or it can be done via the `Projections.RootEntity` and
`Projections.Entity` methods if more control over loaded entities is
required. For instance, entity projections can be lazy loaded or fetched
with lazy properties:

```csharp
    .Select(
        Projections.Entity(() => alias1).SetLazy(true),
        Projections.Entity(() => alias2).SetFetchLazyProperties(true),
        Projections.RootEntity()
    )
```

# Sub-queries

The Sub-queries factory class has overloads to allow Lambda Expressions
to express sub-query restrictions. For example:

```csharp
    QueryOver<Cat> maximumAge =
        QueryOver.Of<Cat>()
            .SelectList(p => p.SelectMax(c => c.Age));
    
    IList<Cat> oldestCats =
        session.QueryOver<Cat>()
            .Where(Subqueries.WhereProperty<Cat>(c => c.Age).Eq(maximumAge))
            .List();
``` 

The inline syntax allows you to use sub-queries without re-qualifying
the type:

```csharp
    IList<Cat> oldestCats =
        session.QueryOver<Cat>()
            .WithSubquery.WhereProperty(c => c.Age).Eq(maximumAge)
            .List();
```

There is an extension method `As()` on (a detached) QueryOver that
allows you to cast it to any type. This is used in conjunction with the
overloads `Where(), WhereAll(),` and `WhereSome()` to allow use of the
built-in C\# operators for comparison, so the above query can be written
as:

```csharp
    IList<Cat> oldestCats =
        session.QueryOver<Cat>()
            .WithSubquery.Where(c => c.Age == maximumAge.As<int>())
            .List();
```
