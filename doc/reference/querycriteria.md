# Criteria Queries

NHibernate features an intuitive, extensible criteria query API.

# Creating an `ICriteria` instance <a name="querycriteria-creating"></a>

The interface `NHibernate.ICriteria` represents a query against a
particular persistent class. The `ISession` is a factory for `ICriteria`
instances.

```csharp
    ICriteria crit = sess.CreateCriteria<Cat>();
    crit.SetMaxResults(50);
    var cats = crit.List<Cat>();
```

# Narrowing the result set <a name="querycriteria-narrowing"></a>

An individual query criterion is an instance of the interface
`NHibernate.Expression.ICriterion`. The class
`NHibernate.Expression.Expression` defines factory methods for obtaining
certain built-in `ICriterion` types.

```csharp
    var cats = sess.CreateCriteria<Cat>()
        .Add( Expression.Like("Name", "Fritz%") )
        .Add( Expression.Between("Weight", minWeight, maxWeight) )
        .List<Cat>();
```

Expressions may be grouped logically.

```csharp
    var cats = sess.CreateCriteria<Cat>()
        .Add( Expression.Like("Name", "Fritz%") )
        .Add( Expression.Or(
            Expression.Eq( "Age", 0 ),
            Expression.IsNull("Age")
        ) )
        .List<Cat>();

    var cats = sess.CreateCriteria<Cat>()
        .Add( Expression.In( "Name", new String[] { "Fritz", "Izi", "Pk" } ) )
        .Add( Expression.Disjunction()
            .Add( Expression.IsNull("Age") )
            .Add( Expression.Eq("Age", 0 ) )
            .Add( Expression.Eq("Age", 1 ) )
            .Add( Expression.Eq("Age", 2 ) )
        ) )
        .List<Cat>();
```

There are quite a range of built-in criterion types (`Expression`
subclasses), but one that is especially useful lets you specify SQL
directly.

```csharp
    // Create a string parameter for the SqlString below
    var cats = sess.CreateCriteria<Cat>()
        .Add(Expression.Sql("lower({alias}.Name) like lower(?)",
            "Fritz%", NHibernateUtil.String))
        .List<Cat>();
```

The `{alias}` placeholder with be replaced by the row alias of the
queried entity.

# Ordering the results <a name="querycriteria-ordering"></a>

You may order the results using `NHibernate.Expression.Order`.

```csharp
    var cats = sess.CreateCriteria<Cat>()
        .Add( Expression.Like("Name", "F%")
        .AddOrder( Order.Asc("Name") )
        .AddOrder( Order.Desc("Age") )
        .SetMaxResults(50)
        .List<Cat>();
```

# Associations <a name="querycriteria-associations"></a>

You may easily specify constraints upon related entities by navigating
associations using `CreateCriteria()`.

```csharp
    var cats = sess.CreateCriteria<Cat>()
        .Add( Expression.Like("Name", "F%")
        .CreateCriteria("Kittens")
            .Add( Expression.Like("Name", "F%") )
        .List<Cat>();
```

Note that the second `CreateCriteria()` returns a new instance of
`ICriteria`, which refers to the elements of the `Kittens` collection.

The following, alternate form is useful in certain circumstances.

```csharp
    var cats = sess.CreateCriteria<Cat>()
        .CreateAlias("Kittens", "kt")
        .CreateAlias("Mate", "mt")
        .Add( Expression.EqProperty("kt.Name", "mt.Name") )
        .List<Cat>();
```

(`CreateAlias()` does not create a new instance of `ICriteria`.)

Note that the kittens collections held by the `Cat` instances returned
by the previous two queries are *not* pre-filtered by the criteria\! If
you wish to retrieve just the kittens that match the criteria, you must
use `SetResultTransformer(Transformers.AliasToEntityMap)`.

```csharp
    var cats = sess.CreateCriteria<Cat>()
        .CreateCriteria("Kittens", "kt")
            .Add( Expression.Eq("Name", "F%") )
        .SetResultTransformer(Transformers.AliasToEntityMap)
        .List<IDictionary>();
    foreach ( IDictionary map in cats )
    {
        Cat cat = (Cat) map[CriteriaSpecification.RootAlias];
        Cat kitten = (Cat) map["kt"];
    }
```

projection. See [Projections, aggregation and grouping](#projections-aggregation-and-grouping) for more information.
Note that for retrieving just kittens you can also use an entity

# Join entities without association (Entity joins or ad hoc joins) <a name="querycriteria-querycriteria_entityjoin"></a>

In criteria you have the ability to define a join to any entity, not
just through a mapped association. To achieve it, use
`CreateEntityAlias` and `CreateEntityCriteria`. By example:

```csharp
    IList<Cat> uniquelyNamedCats = sess.CreateCriteria<Cat>("c")
        .CreateEntityAlias(
            "joinedCat",
            Restrictions.And(
                Restrictions.EqProperty("c.Name", "joinedCat.Name"),
                Restrictions.NotEqProperty("c.Id", "joinedCat.Id")),
            JoinType.LeftOuterJoin,
            typeof(Cat).FullName)
        .Add(Restrictions.IsNull("joinedCat.Id"))
        .List();
```

# Dynamic association fetching <a name="querycriteria-dynamicfetching"></a>

You may specify association fetching semantics at runtime using
`SetFetchMode()`.

```csharp
    var cats = sess.CreateCriteria<Cat>()
        .Add( Expression.Like("Name", "Fritz%") )
        .SetFetchMode("Mate", FetchMode.Eager)
        .SetFetchMode("Kittens", FetchMode.Eager)
        .List<Cat>();
```

This query will fetch both `Mate` and `Kittens` by outer join. See
[Fetching strategies](performance.md#fetching-strategies) for more information.

# Example queries <a name="querycriteria-examples"></a>

The class `NHibernate.Expression.Example` allows you to construct a
query criterion from a given instance.

```csharp
    Cat cat = new Cat();
    cat.Sex = 'F';
    cat.Color = Color.Black;
    var results = session.CreateCriteria<Cat>()
        .Add( Example.Create(cat) )
        .List<Cat>();
```

Version properties, identifiers and associations are ignored. By
default, null-valued properties and properties which return an empty
string from the call to `ToString()` are excluded.

You can adjust how the `Example` is applied.

```csharp
    Example example = Example.Create(cat)
        .ExcludeZeroes()           //exclude null- or zero-valued properties
        .ExcludeProperty("Color")  //exclude the property named "color"
        .IgnoreCase()              //perform case insensitive string comparisons
        .EnableLike();             //use like for string comparisons
    var results = session.CreateCriteria<Cat>()
        .Add(example)
        .List<Cat>();
```

You can even use examples to place criteria upon associated objects.

```csharp
    var results = session.CreateCriteria<Cat>()
        .Add( Example.Create(cat) )
        .CreateCriteria("Mate")
            .Add( Example.Create( cat.Mate ) )
        .List<Cat>();
```

# Projections, aggregation and grouping <a name="querycriteria-projection"></a>

The class `NHibernate.Expression.Projections` is a factory for
`IProjection` instances. We apply a projection to a query by calling
`SetProjection()`.

```csharp
    var results = session.CreateCriteria<Cat>()
        .SetProjection( Projections.RowCount() )
        .Add( Expression.Eq("Color", Color.BLACK) )
        .List<int>();

    var results = session.CreateCriteria<Cat>()
        .SetProjection( Projections.ProjectionList()
            .Add( Projections.RowCount() )
            .Add( Projections.Avg("Weight") )
            .Add( Projections.Max("Weight") )
            .Add( Projections.GroupProperty("Color") )
        )
        .List<object[]>();
```

There is no explicit "group by" necessary in a criteria query. Certain
projection types are defined to be *grouping projections*, which also
appear in the SQL `group by` clause.

An alias may optionally be assigned to a projection, so that the
projected value may be referred to in restrictions or orderings. Here
are two different ways to do this:

```csharp
    var results = session.CreateCriteria<Cat>()
        .SetProjection( Projections.Alias( Projections.GroupProperty("Color"), "colr" ) )
        .AddOrder( Order.Asc("colr") )
        .List<string>();

    var results = session.CreateCriteria<Cat>()
        .SetProjection( Projections.GroupProperty("Color").As("colr") )
        .AddOrder( Order.Asc("colr") )
        .List<string>();
```

The `Alias()` and `As()` methods simply wrap a projection instance in
another, aliased, instance of `IProjection`. As a shortcut, you can
assign an alias when you add the projection to a projection list:

```csharp
    var results = session.CreateCriteria<Cat>()
        .SetProjection( Projections.ProjectionList()
            .Add( Projections.RowCount(), "catCountByColor" )
            .Add( Projections.Avg("Weight"), "avgWeight" )
            .Add( Projections.Max("Weight"), "maxWeight" )
            .Add( Projections.GroupProperty("Color"), "color" )
        )
        .AddOrder( Order.Desc("catCountByColor") )
        .AddOrder( Order.Desc("avgWeight") )
        .List<object[]>();

    var results = session.CreateCriteria(typeof(DomesticCat), "cat")
        .CreateAlias("kittens", "kit")
        .SetProjection( Projections.ProjectionList()
            .Add( Projections.Property("cat.Name"), "catName" )
            .Add( Projections.Property("kit.Name"), "kitName" )
        )
        .AddOrder( Order.Asc("catName") )
        .AddOrder( Order.Asc("kitName") )
        .List<object[]>();
```

You can also add an entity projection to a criteria query:

```csharp
    var kittens = sess.CreateCriteria<Cat>()
        .CreateCriteria("Kittens", "kt")
        .Add(Expression.Eq("Name", "F%"))
        .SetProjection(Projections.Entity(typeof(Cat), "kt"))
        .List();

    var cats = sess.CreateCriteria<Cat>()
        .CreateCriteria("Kittens", "kt")
        .Add(Expression.Eq("Name", "F%"))
        .SetProjection(
            Projections.RootEntity(),
            Projections.Entity(typeof(Cat), "kt"))
        .List<object[]>();
    
    foreach (var objs in cats)
    {
        Cat cat = (Cat) objs[0];
        Cat kitten = (Cat) objs[1];
    }
```

See [Entities Projection](queryqueryover.md#entities-projection) for more information.

# Detached queries and sub-queries <a name="querycriteria-detachedqueries"></a>

The `DetachedCriteria` class lets you create a query outside the scope
of a session, and then later execute it using some arbitrary `ISession`.

```csharp
    DetachedCriteria query = DetachedCriteria.For<Cat>()
        .Add( Expression.Eq("sex", 'F') );
    
    using (ISession session = ....)
    using (ITransaction txn = session.BeginTransaction())
    {
        var results = query.GetExecutableCriteria(session).SetMaxResults(100).List<Cat>();
        txn.Commit();
    }
```

A `DetachedCriteria` may also be used to express a sub-query. ICriterion
instances involving sub-queries may be obtained via `Subqueries`.

```csharp
    DetachedCriteria avgWeight = DetachedCriteria.For<Cat>()
        .SetProjection( Projections.Avg("Weight") );
    session.CreateCriteria<Cat>()
        .Add( Subqueries.Gt("Weight", avgWeight) )
        .List<Cat>();

    DetachedCriteria weights = DetachedCriteria.For<Cat>()
        .SetProjection( Projections.Property("Weight") );
    session.CreateCriteria<Cat>()
        .Add( Subqueries.GeAll("Weight", weights) )
        .List<Cat>();
```

Even correlated sub-queries are possible:

```csharp
    DetachedCriteria avgWeightForSex = DetachedCriteria.For<Cat>("cat2")
        .SetProjection( Projections.Avg("Weight") )
        .Add( Expression.EqProperty("cat2.Sex", "cat.Sex") );
    session.CreateCriteria(typeof(Cat), "cat")
        .Add( Subqueries.Gt("weight", avgWeightForSex) )
        .List<Cat>();
```
