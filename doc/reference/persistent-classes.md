# Persistent Classes

Persistent classes are classes in an application that implement the
entities of the business problem (e.g. Customer and Order in an
E-commerce application). Persistent classes have, as the name implies,
transient and also persistent instance stored in the database.

NHibernate works best if these classes follow some simple rules, also
known as the Plain Old CLR Object (POCO) programming model.

# A simple POCO example <a name="persistent-classes-poco"></a>

Most .NET applications require a persistent class representing felines.

```csharp
    using System;
    using System.Collections.Generic;
    
    namespace Eg
    {
        public class Cat
        {
            long _id;
            // identifier
    
            public virtual long Id
            {
                get { return _id; }
                protected set { _id = value; }
            }
    
            public virtual string Name { get; set; }
            public virtual Cat Mate { get; set; }
            public virtual DateTime Birthdate { get; set; }
            public virtual float Weight { get; set; }
            public virtual Color Color { get; set; }
            public virtual ISet<Cat> Kittens { get; set; }
            public virtual char Sex { get; set; }
    
            // AddKitten not needed by NHibernate
            public virtual void AddKitten(Cat kitten)
            {
                kittens.Add(kitten);
            }
        }
    }
```

There are four main rules to follow here:

## Declare properties for persistent fields <a name="persistent-classes-poco-accessors"></a>

`Cat` declares properties for all the persistent fields. Many other *ORM
tools* directly persist instance variables. We believe it is far better
to decouple this implementation detail from the persistence mechanism.
NHibernate persists properties, using their getter and setter methods.

Properties need *not* be declared public - NHibernate can persist a
property with an `internal`, `protected`, `protected internal` or
`private` visibility.

As shown in the example, both automatic properties and properties with a
backing field are supported.

## Implement a default constructor <a name="persistent-classes-poco-constructor"></a>

`Cat` has an implicit default (no-argument) constructor. All persistent
classes must have a default constructor (which may be non-public) so
NHibernate can instantiate them using `Activator.CreateInstance()`.

## Provide an identifier property (optional) <a name="persistent-classes-poco-identifier"></a>

`Cat` has a property called `Id`. This property holds the primary key
column of a database table. The property might have been called
anything, and its type might have been any primitive type, `string` or
`System.DateTime`. (If your legacy database table has composite keys,
you can even use a user-defined class with properties of these types -
see the section on composite identifiers below.)

The identifier property is optional. You can leave it off and let
NHibernate keep track of object identifiers internally. However, for
many applications it is still a good (and very popular) design decision.

What's more, some functionality is available only to classes which
declare an identifier property:

  - Cascaded updates (see "Lifecycle Objects")

  - `ISession.SaveOrUpdate()`

We recommend you declare consistently-named identifier properties on
persistent classes.

## Prefer non-sealed classes and virtual methods (optional) <a name="persistent-classes-poco-sealed"></a>

A central feature of NHibernate, *proxies*, depends upon the persistent
class being non-sealed and all its public methods, properties and events
declared as virtual. Another possibility is for the class to implement
an interface that declares all public members.

You can persist `sealed` classes that do not implement an interface and
don't have virtual members with NHibernate, but you won't be able to use
proxies - which will limit your options for performance tuning.

# Implementing inheritance <a name="persistent-classes-inheritance"></a>

A subclass must also observe the first and second rules. It inherits its
identifier property from `Cat`.

```csharp
    using System;
    namespace Eg
    {
        public class DomesticCat : Cat
        {
            public virtual string Name { get; set; }
        }
    }
```

# Implementing `Equals()` and `GetHashCode()` <a name="persistent-classes-equalshashcode"></a>

You have to override the `Equals()` and `GetHashCode()` methods if you
intend to mix objects of persistent classes (e.g. in an `ISet`).

*This only applies if these objects are loaded in two different
`ISession`s, as NHibernate only guarantees identity (` a == b `, the
default implementation of `Equals()`) inside a single `ISession`\!*

Even if both objects `a` and `b` are the same database row (they have
the same primary key value as their identifier), we can't guarantee that
they are the same object instance outside of a particular `ISession`
context.

The most obvious way is to implement `Equals()`/`GetHashCode()` by
comparing the identifier value of both objects. If the value is the
same, both must be the same database row, they are therefore equal (if
both are added to an `ISet`, we will only have one element in the
`ISet`). Unfortunately, we can't use that approach. NHibernate will only
assign identifier values to objects that are persistent, a newly created
instance will not have any identifier value\! We recommend implementing
`Equals()` and `GetHashCode()` using *Business key equality*.

Business key equality means that the `Equals()` method compares only the
properties that form the business key, a key that would identify our
instance in the real world (a *natural* candidate key):

```csharp
    public class Cat
    {
    
        ...
        public override bool Equals(object other)
        {
            if (this == other) return true;
            
            Cat cat = other as Cat;
            if (cat == null) return false; // null or not a cat
    
            if (Name != cat.Name) return false;
            if (!Birthday.Equals(cat.Birthday)) return false;
    
            return true;
        }
    
        public override int GetHashCode()
        {
            unchecked
            {
                int result;
                result = Name.GetHashCode();
                result = 29 * result + Birthday.GetHashCode();
                return result;
            }
        }
    
    }
```

Keep in mind that our candidate key (in this case a composite of name
and birthday) has to be only valid for a particular comparison operation
(maybe even only in a single use case). We don't need the stability
criteria we usually apply to a real primary key\!

# Dynamic models <a name="persistent-classes-dynamicmodels"></a>

*Note that the following features are currently considered experimental
and may change in the near future.*

Persistent entities don't necessarily have to be represented as POCO
classes at runtime. NHibernate also supports dynamic models (using
`Dictionaries` of `Dictionary`s at runtime) . With this approach, you
don't write persistent classes, only mapping files.

The following examples demonstrates the representation using `Map`s
(Dictionary). First, in the mapping file, an `entity-name` has to be
declared instead of a class name:

```xml
    <hibernate-mapping>
    
        <class entity-name="Customer">
    
            <id name="id"
                type="long"
                column="ID">
                <generator class="sequence"/>
            </id>
    
            <property name="name"
                column="NAME"
                type="string"/>
    
            <property name="address"
                column="ADDRESS"
                type="string"/>
    
            <many-to-one name="organization"
                column="ORGANIZATION_ID"
                class="Organization"/>
    
            <bag name="orders"
                inverse="true"
                lazy="false"
                cascade="all">
                <key column="CUSTOMER_ID"/>
                <one-to-many class="Order"/>
            </bag>
    
        </class>
        
    </hibernate-mapping>
```

Note that even though associations are declared using target class
names, the target type of an associations may also be a dynamic entity
instead of a POCO.

At runtime we can work with `Dictionaries` of `Dictionaries`:

```csharp
    using(ISession s = OpenSession())
    using(ITransaction tx = s.BeginTransaction())
    {
        // Create a customer
        var frank = new Dictionary<string, object>();
        frank["name"] = "Frank";
    
        // Create an organization
        var foobar = new Dictionary<string, object>();
        foobar["name"] = "Foobar Inc.";
    
        // Link both
        frank["organization"] =  foobar;
    
        // Save both
        s.Save("Customer", frank);
        s.Save("Organization", foobar);
    
        tx.Commit();
    }
```

The advantages of a dynamic mapping are quick turnaround time for
prototyping without the need for entity class implementation. However,
you lose compile-time type checking and will very likely deal with many
exceptions at runtime. Thanks to the NHibernate mapping, the database
schema can easily be normalized and sound, allowing to add a proper
domain model implementation on top later on.

# Tuplizers <a name="persistent-classes-tuplizers"></a>

`NHibernate.Tuple.Tuplizer`, and its sub-interfaces, are responsible for
managing a particular representation of a piece of data, given that
representation's `NHibernate.EntityMode`. If a given piece of data is
thought of as a data structure, then a tuplizer is the thing which knows
how to create such a data structure and how to extract values from and
inject values into such a data structure. For example, for the POCO
entity mode, the corresponding tuplizer knows how create the POCO
through its constructor and how to access the POCO properties using the
defined property accessors. There are two high-level types of Tuplizers,
represented by the `NHibernate.Tuple.Entity.IEntityTuplizer` and
`NHibernate.Tuple.Component.IComponentTuplizer` interfaces.
`IEntityTuplizer`s are responsible for managing the above mentioned
contracts in regards to entities, while `IComponentTuplizer`s do the
same for components.

Users may also plug in their own tuplizers. Perhaps you require that a
`System.Collections.IDictionary` implementation other than
`System.Collections.Hashtable` be used while in the dynamic-map
entity-mode; or perhaps you need to define a different proxy generation
strategy than the one used by default. Both would be achieved by
defining a custom tuplizer implementation. Tuplizers definitions are
attached to the entity or component mapping they are meant to manage.
Going back to the example of our customer entity:

```xml
    <hibernate-mapping>
        <class entity-name="Customer">
            <!--
                Override the dynamic-map entity-mode
                tuplizer for the customer entity
            -->
            <tuplizer entity-mode="dynamic-map"
                    class="CustomMapTuplizerImpl"/>
    
            <id name="id" type="long" column="ID">
                <generator class="sequence"/>
            </id>
    
            <!-- other properties -->
            ...
        </class>
    </hibernate-mapping>
```

```csharp    
    public class CustomMapTuplizerImpl : NHibernate.Tuple.Entity.DynamicMapEntityTuplizer
    {
        // override the BuildInstantiator() method to plug in our custom map...
        protected override IInstantiator BuildInstantiator(
            NHibernate.Mapping.PersistentClass mappingInfo)
        {
            return new CustomMapInstantiator(mappingInfo);
        }
    
        private sealed class CustomMapInstantiator : NHibernate.Tuple.DynamicMapInstantiator
        {
            // override the generateMap() method to return our custom map...
            protected override IDictionary GenerateMap()
            {
                return new CustomMap();
            }
        }
    }
```

# Lifecycle Callbacks <a name="persistent-classes-lifecycle"></a>

Optionally, a persistent class might implement the interface
`ILifecycle` which provides some callbacks that allow the persistent
object to perform necessary initialization/cleanup after save or load
and before deletion or update.

The NHibernate [`IInterceptor`](events.md#interceptors) offers a less
intrusive alternative, however.

```csharp
    public interface ILifecycle
    {
            LifecycleVeto OnSave(ISession s);
            LifecycleVeto OnUpdate(ISession s);
            LifecycleVeto OnDelete(ISession s);
            void OnLoad(ISession s, object id);
    }
```

  - `OnSave` - called just before the object is saved or inserted

  - `OnUpdate` - called just before an object is updated (when the
    object is passed to `ISession.Update()`)

  - `OnDelete` - called just before an object is deleted

  - `OnLoad` - called just after an object is loaded

`OnSave()`, `OnDelete()` and `OnUpdate()` may be used to cascade saves
and deletions of dependent objects. This is an alternative to declaring
cascaded operations in the mapping file. `OnLoad()` may be used to
initialize transient properties of the object from its persistent state.
It may not be used to load dependent objects since the `ISession`
interface may not be invoked from inside this method. A further intended
usage of `OnLoad()`, `OnSave()` and `OnUpdate()` is to store a reference
to the current `ISession` for later use.

Note that `OnUpdate()` is not called every time the object's persistent
state is updated. It is called only when a transient object is passed to
`ISession.Update()`.

If `OnSave()`, `OnUpdate()` or `OnDelete()` return `LifecycleVeto.Veto`,
the operation is silently vetoed. If a `CallbackException` is thrown,
the operation is vetoed and the exception is passed back to the
application.

Note that `OnSave()` is called after an identifier is assigned to the
object, except when native key generation is used.

# IValidatable callback <a name="persistent-classes-validatable"></a>

If the persistent class needs to check invariants before its state is
persisted, it may implement the following interface:

```csharp
    public interface IValidatable
    {
        void Validate();
    }
```

The object should throw a `ValidationFailure` if an invariant was
violated. An instance of `Validatable` should not change its state from
inside `Validate()`.

Unlike the callback methods of the `ILifecycle` interface, `Validate()`
might be called at unpredictable times. The application should not rely
upon calls to `Validate()` for business functionality.
