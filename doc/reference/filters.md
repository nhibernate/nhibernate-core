# Filtering data

NHibernate provides an innovative new approach to handling data with
"visibility" rules. A *NHibernate filter* is a global, named,
parameterized filter that may be enabled or disabled for a particular
NHibernate session.

# NHibernate filters

NHibernate adds the ability to pre-define filter criteria and attach
those filters at both a class and a collection level. A filter criteria
is the ability to define a restriction clause very similar to the
existing "where" attribute available on the class and various collection
elements. Except these filter conditions can be parameterized. The
application can then make the decision at runtime whether given filters
should be enabled and what their parameter values should be. Filters can
be used like database views, but parameterized inside the application.

In order to use filters, they must first be defined and then attached to
the appropriate mapping elements. To define a filter, use the
`<filter-def/>` element within a `<hibernate-mapping/>` element:

```xml
    <filter-def name="myFilter">
        <filter-param name="myFilterParam" type="String"/>
    </filter-def>
```

Then, this filter can be attached to a class:

```xml
    <class name="MyClass" ...>
        ...
        <filter name="myFilter" condition=":myFilterParam = MY_FILTERED_COLUMN"/>
    </class>
```

or, to a collection:

```xml
    <set ...>
        <filter name="myFilter" condition=":myFilterParam = MY_FILTERED_COLUMN"/>
    </set>
```

or, even to both (or multiples of each) at the same time.

The methods on `ISession` are: `EnableFilter(string filterName)`,
`GetEnabledFilter(string filterName)`, and `DisableFilter(string
filterName)`. By default, filters are *not* enabled for a given session;
they must be explicitly enabled through use of the
`ISession.EnableFilter()` method, which returns an instance of the
`IFilter` interface. Using the simple filter defined above, this would
look
    like:

```csharp
    session.EnableFilter("myFilter").SetParameter("myFilterParam", "some-value");
```

Note that methods on the `NHibernate.IFilter` interface do allow the
method-chaining common to much of NHibernate.

A full example, using temporal data with an effective record date
pattern:

```xml
    <filter-def name="effectiveDate">
        <filter-param name="asOfDate" type="date"/>
    </filter-def>
    
    <class name="Employee" ...>
    ...
        <many-to-one name="Department" column="dept_id" class="Department"/>
        <property name="EffectiveStartDate" type="date" column="eff_start_dt"/>
        <property name="EffectiveEndDate" type="date" column="eff_end_dt"/>
    ...
        <!--
            Note that this assumes non-terminal records have an eff_end_dt set to
            a max db date for simplicity-sake
        -->
        <filter name="effectiveDate"
                condition=":asOfDate BETWEEN eff_start_dt and eff_end_dt"/>
    </class>
    
    <class name="Department" ...>
    ...
        <set name="Employees" lazy="true">
            <key column="dept_id"/>
            <one-to-many class="Employee"/>
            <filter name="effectiveDate"
                    condition=":asOfDate BETWEEN eff_start_dt and eff_end_dt"/>
        </set>
    </class>
```

Then, in order to ensure that you always get back currently effective
records, simply enable the filter on the session prior to retrieving
employee data:

```csharp
    ISession session = ...;
    session.EnableFilter("effectiveDate").SetParameter("asOfDate", DateTime.Today);
    var results = session.CreateQuery("from Employee as e where e.Salary > :targetSalary")
             .SetInt64("targetSalary", 1000000L)
             .List<Employee>();
```

In the HQL above, even though we only explicitly mentioned a salary
constraint on the results, because of the enabled filter the query will
return only currently active employees who have a salary greater than a
million dollars.

Note: if you plan on using filters with outer joining (either through
HQL or load fetching) be careful of the direction of the condition
expression. It's safest to set this up for left outer joining; in
general, place the parameter first followed by the column name(s) after
the operator.

Default all filter definitions are applied to `<many-to-one/>` and
`<one-to-one/>` elements. You can turn off this behaviour by using
`use-many-to-one` attribute on `<filter-def/>` element.

```xml
    <filter-def name="effectiveDate" use-many-to-one="false"/>
```
