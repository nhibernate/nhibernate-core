# Quick-start with IIS and Microsoft SQL Server

# Getting started with NHibernate

This tutorial explains a setup of NHibernate 5.0.0 within a Microsoft
environment. The tools used in this tutorial are:

1.  Microsoft Internet Information Services (IIS) - web server
    supporting ASP.NET.
2.  Microsoft SQL Server 2012 - the database server. This tutorial uses
    the desktop edition (SQL Express), a free download from Microsoft.
    Support for other databases is only a matter of changing the
    NHibernate SQL dialect and driver configuration.
3.  Microsoft Visual Studio .NET (at least 2013) - the development
    environment.

First, we have to create a new Web project. We use the name
`QuickStart`. In the project, add a NuGet reference to `NHibernate`.
Visual Studio will automatically copy the library and its dependencies
to the project output directory. If you are using a database other than
SQL Server, add a reference to its driver assembly to your project.

We now set up the database connection information for NHibernate. To do
this, open the file `Web.config` automatically generated for your
project and add configuration elements according to the listing below:

```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <!-- Add this element -->
      <configSections>
        <section
            name="hibernate-configuration"
            type="NHibernate.Cfg.ConfigurationSectionHandler, NHibernate" />
      </configSections>
    
      <!-- Add this element -->
      <hibernate-configuration xmlns="urn:nhibernate-configuration-2.2">
        <session-factory>
          <property name="dialect">NHibernate.Dialect.MsSql2012Dialect</property>
          <property name="connection.connection_string">
            Server=localhost\SQLEXPRESS;initial catalog=quickstart;Integrated Security=True
          </property>
    
          <mapping assembly="QuickStart" />
        </session-factory>
      </hibernate-configuration>
    
      <!-- Leave the other sections unchanged -->
      <system.web>
        ...
      </system.web>
    </configuration>
```

The `<configSections>` element contains definitions of sections that
follow and handlers to use to process their content. We declare the
handler for the configuration section here. The `<hibernate-configuration>` section contains the configuration itself,
telling NHibernate that we will use a Microsoft SQL Server 2012 database
and connect to it through the specified connection string. The dialect
is a required setting, databases differ in their interpretation of the
SQL "standard". NHibernate will take care of the differences and comes
bundled with dialects for several major commercial and open source
databases.

An `ISessionFactory` is NHibernate's concept of a single datastore,
multiple databases can be used by creating multiple XML configuration
files and creating multiple `Configuration` and `ISessionFactory`
objects in your application.

The last element of the `<hibernate-configuration>` section declares
`QuickStart` as the name of an assembly containing class declarations
and mapping files. The mapping files contain the metadata for the
mapping of the POCO class to a database table (or multiple tables).
We'll come back to mapping files soon. Let's write the POCO class first
and then declare the mapping metadata for it.

# First persistent class

NHibernate works best with the Plain Old CLR Objects (POCOs, sometimes
called Plain Ordinary CLR Objects) programming model for persistent
classes. A POCO has its data accessible through the standard .NET
property mechanisms, shielding the internal representation from the
publicly visible interface:

```csharp
    namespace QuickStart
    {
        public class Cat
        {
            public virtual string Id { get; set; }
    
            public virtual string Name { get; set; }
    
            public virtual char Sex { get; set; }
    
            public virtual float Weight { get; set; }
        }
    }
```

NHibernate is not restricted in its usage of property types, all .NET
types and primitives (like `string`, `char` and `DateTime`) can be
mapped, including classes from the `System.Collections.Generic`
namespace. You can map them as values, collections of values, or
associations to other entities. The `Id` is a special property that
represents the database identifier (primary key) of that class, it is
highly recommended for entities like a `Cat`. NHibernate can use
identifiers only internally, without having to declare them on the
class, but we would lose some of the flexibility in our application
architecture.

No special interface has to be implemented for persistent classes nor do
we have to subclass from a special root persistent class. NHibernate
also doesn't use any build time processing, such as IL manipulation, it
relies solely on .NET reflection and runtime class enhancement. So,
without any dependency in the POCO class on NHibernate, we can map it to
a database table.

For the above mentioned runtime class enhancement to work, NHibernate
requires that all public properties of an entity class are declared as
`virtual`. It also requires a parameter-less constructor: if you add a
constructor having parameters, make sure to add a parameter-less
constructor too.

# Mapping the cat

The `Cat.hbm.xml` mapping file contains the metadata required for the
object/relational mapping. The metadata includes declaration of
persistent classes and the mapping of properties (to columns and foreign
key relationships to other entities) to database tables.

Please note that the `Cat.hbm.xml` file should be set to an embedded
resource.

```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2"
        namespace="QuickStart" assembly="QuickStart">
    
        <class name="Cat" table="Cat">
    
            <!-- A 32 hex character is our surrogate key. It's automatically
                generated by NHibernate with the UUID pattern. -->
            <id name="Id">
                <column name="CatId" sql-type="char(32)" not-null="true"/>
                <generator class="uuid.hex" />
            </id>
    
            <!-- A cat has to have a name, but it shouldn't be too long. -->
            <property name="Name">
                <column name="Name" length="16" not-null="true" />
            </property>
            <property name="Sex" />
            <property name="Weight" />
        </class>
    
    </hibernate-mapping>
```

Every persistent class should have an identifier attribute (actually,
only classes representing entities, not dependent value objects, which
are mapped as components of an entity). This property is used to
distinguish persistent objects: Two cats are equal if
`catA.Id.Equals(catB.Id)` is true, this concept is called *database
identity*. NHibernate comes bundled with various identifier generators
for different scenarios (including native generators for database
sequences, hi/lo identifier tables, and application assigned
identifiers). We use the UUID generator (only recommended for testing,
as integer surrogate keys generated by the database should be preferred)
and also specify the column `CatId` of the table `Cat` for the
NHibernate generated identifier value (as a primary key of the table).

All other properties of `Cat` are mapped to the same table. In the case
of the `Name` property, we mapped it with an explicit database column
declaration. This is especially useful when the database schema is
automatically generated (as SQL DDL statements) from the mapping
declaration with NHibernate's *SchemaExport* tool. All other properties
are mapped using NHibernate's default settings, which is what you need
most of the time. Here the specification of the table name with the
attribute `table` is redundant, it default to the class name when not
specified. The table `Cat` in the database looks like this:

| Column | Type         | Modifiers             |
|--------|--------------|-----------------------|
| CatId  | char(32)     | not null, primary key |
| Name   | nvarchar(16) | not null              |
| Sex    | nchar(1)     |                       |
| Weight | real         |                       |

You should now create the database and this table manually, and later
read [Toolset Guide](toolset_guide.md) if you want to automate this step with the
SchemaExport tool. This tool can create a full SQL DDL, including table
definition, custom column type constraints, unique constraints and
indexes. If you are using SQL Server, you should also make sure the
`ASPNET` user has permissions to use the database.

# Playing with cats

We're now ready to start NHibernate's `ISession`. It is the *persistence
manager* interface, we use it to store and retrieve `Cat`s to and from
the database. But first, we've to get an `ISession` (NHibernate's
unit-of-work) from the `ISessionFactory`:

```csharp
    ISessionFactory sessionFactory =
                new Configuration().Configure().BuildSessionFactory();
```

An `ISessionFactory` is responsible for one database and may only use
one XML configuration file (`Web.config` or `hibernate.cfg.xml`). You
can set other properties (and even change the mapping metadata) by
accessing the `Configuration` *before* you build the `ISessionFactory`
(it is immutable). Where do we create the `ISessionFactory` and how can
we access it in our application?

An `ISessionFactory` is usually only built once, e.g. at start-up inside
`Application_Start` event handler. This also means you should not keep
it in an instance variable in your ASP.NET pages or MVC controllers, but
in some other location. Furthermore, we need some kind of *Singleton*,
so we can access the `ISessionFactory` easily in application code. The
approach shown next solves both problems: configuration and easy access
to a `ISessionFactory`.

We implement a `NHibernateHelper` helper class:

```csharp
    using System;
    using System.Web;
    using NHibernate;
    using NHibernate.Cfg;
    
    namespace QuickStart
    {
        public sealed class NHibernateHelper
        {
            private const string CurrentSessionKey = "nhibernate.current_session";
            private static readonly ISessionFactory _sessionFactory;
    
            static NHibernateHelper()
            {
                _sessionFactory = new Configuration().Configure().BuildSessionFactory();
            }
    
            public static ISession GetCurrentSession()
            {
                var context = HttpContext.Current;
                var currentSession = context.Items[CurrentSessionKey] as ISession;
    
                if (currentSession == null)
                {
                    currentSession = _sessionFactory.OpenSession();
                    context.Items[CurrentSessionKey] = currentSession;
                }
    
                return currentSession;
            }
    
            public static void CloseSession()
            {
                var context = HttpContext.Current;
                var currentSession = context.Items[CurrentSessionKey] as ISession;
    
                if (currentSession == null)
                {
                    // No current session
                    return;
                }
    
                currentSession.Close();
                context.Items.Remove(CurrentSessionKey);
            }
    
            public static void CloseSessionFactory()
            {
                if (_sessionFactory != null)
                {
                    _sessionFactory.Close();
                }
            }
        }
    }
```

This class does not only take care of the `ISessionFactory` with its
static attribute, but also has code to remember the `ISession` for the
current HTTP request.

An `ISessionFactory` is threadsafe, many threads can access it
concurrently and request `ISession`s. An `ISession` is a non-threadsafe
object that represents a single unit-of-work with the database.
`ISession`s are opened by an `ISessionFactory` and are closed when all
work is completed:

```csharp
    ISession session = NHibernateHelper.GetCurrentSession();
    try
    {
        using (ITransaction tx = session.BeginTransaction())
        {
            var princess = new Cat
            {
                Name = "Princess",
                Sex = 'F',
                Weight = 7.4f
            };
    
            session.Save(princess);
            tx.Commit();
        }
    }
    finally
    {
        NHibernateHelper.CloseSession();
    }
```

In an `ISession`, every database operation occurs inside a transaction
that isolates the database operations (even read-only operations). We
use NHibernate's `ITransaction` API to abstract from the underlying
transaction strategy (in our case, `ADO.NET` transactions). Please note
that the example above does not handle any exceptions.

Also note that you may call `NHibernateHelper.GetCurrentSession();` as
many times as you like, you will always get the current `ISession` of
this HTTP request. You have to make sure the `ISession` is closed after
your unit-of-work completes, either in `Application_EndRequest` event
handler in your application class, or with a MVC action filter, or in a
`HttpModule` before the HTTP response is sent. The nice side effect of
the latter is easy lazy initialization: the `ISession` is still open
when the view is rendered, so NHibernate can load uninitialized objects
while you navigate the graph.

NHibernate has various methods that can be used to retrieve objects from
the database. Nowadays the most standard way is using Linq:

```csharp
    using(var tx = session.BeginTransaction())
    {
        var females = session
            .Query<Cat>()
            .Where(c => c.Sex == 'F')
            .ToList();
        foreach (var cat in females)
        {
            Console.Out.WriteLine("Female Cat: " + cat.Name);
        }
    
        tx.Commit();
    }
```

If you use an older NHibernate, you may have to import the
`NHibernate.Linq` namespace.

NHibernate also offers an object-oriented *query by criteria* API that
can be used to formulate type-safe queries, the Hibernate Query Language
(HQL), which is an easy to learn and powerful object-oriented extension
to SQL, as well as a strongly-typed LINQ API which translates internally
to HQL. NHibernate of course uses `DbCommand`s and parameter binding for
all SQL communication with the database. You may also use NHibernate's
direct SQL query feature or get a plain `ADO.NET` connection from an
`ISession` in rare cases.

Since NHibernate 5.0, the session and its queries IO bound methods have
async counterparts. Each call to an async method must be awaited before
further interacting with the session or its queries.

# Finally

We only scratched the surface of NHibernate in this small tutorial.
Please note that we don't include any ASP.NET specific code in our
examples. You have to create an ASP.NET page yourself and insert the
NHibernate code as you see fit.

Keep in mind that NHibernate, as a data access layer, is tightly
integrated into your application. Usually, all other layers depend on
the persistence mechanism. Make sure you understand the implications of
this design.
