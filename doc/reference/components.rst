*****************
Component Mapping
*****************

The notion of a *component* is re-used in several different contexts, for
different purposes, throughout NHibernate.

Dependent objects
==================

A component is a contained object that is persisted as a value type, not an
entity. The term ”component” refers to the object-oriented notion of composition
(not to architecture-level components). For example, you might model a person
like this:

.. code:: csharp

  public class Person
  {
    public string Key { get; set; }

    public DateTime Birthday { get; set; }

    public Name Name { get; set; }

    ......
    ......
  }

  public class Name
  {
    public string First { get; set; }

    public string Last { get; set; }

    public char Initial { get; set; }
  }

Now ``Name`` may be persisted as a component of ``Person``. Notice that ``Name``
defines getter and setter methods for its persistent properties, but doesn”t
need to declare any interfaces or identifier properties.

Our NHibernate mapping would look like:

.. code:: xml

  <class name="Eg.Person, Eg" table="person">
    <id name="Key" column="pid" type="string">
      <generator class="uuid.hex"/>
    </id>
    <property name="Birthday" type="date"/>
    <component name="Name" class="Eg.Name, Eg"> <!-- class attribute optional -->
      <property name="Initial"/>
      <property name="First"/>
      <property name="Last"/>
    </component>
  </class>

The person table would have the columns ``pid``, ``Birthday``, ``Initial``,
``First`` and ``Last``.

Like all value types, components do not support shared references. The null
value semantics of a component are *ad hoc*. When reloading the containing
object, NHibernate will assume that if all component columns are null, then the
entire component is null. This should be okay for most purposes.

The properties of a component may be of any NHibernate type (collections,
many-to-one associations, other components, etc). Nested components should *not*
be considered an exotic usage. NHibernate is intended to support a very
fine-grained object model.

The ``<component>`` element allows a ``<parent>`` sub-element that maps a
property of the component class as a reference back to the containing entity.

.. code:: xml

  <class name="Eg.Person, Eg" table="person">
    <id name="Key" column="pid" type="string">
      <generator class="uuid.hex"/>
    </id>
    <property name="Birthday" type="date"/>
    <component name="Name" class="Eg.Name, Eg">
      <parent name="NamedPerson"/> <!-- reference back to the Person -->
      <property name="Initial"/>
      <property name="First"/>
      <property name="Last"/>
    </component>
  </class>

Collections of dependent objects
=================================

Collections of components are supported (eg. an array of type ``Name``). Declare
your component collection by replacing the ``<element>`` tag with a
``<composite-element>`` tag.

.. code:: xml

  <set name="SomeNames" table="some_names" lazy="true">
    <key column="id"/>
    <composite-element class="Eg.Name, Eg"> <!-- class attribute required -->
      <property name="Initial"/>
      <property name="First"/>
      <property name="Last"/>
    </composite-element>
  </set>

Note: if you define an ``ISet`` of composite elements, it is very important to
implement ``Equals()`` and ``GetHashCode()`` correctly.

Composite elements may contain components but not collections. If your composite
element itself contains components, use the ``<nested-composite-element>`` tag.
This is a pretty exotic case - a collection of components which themselves have
components. By this stage you should be asking yourself if a one-to-many
association is more appropriate. Try remodelling the composite element as an
entity - but note that even though the object model is the same, the relational
model and persistence semantics are still slightly different.

Please note that a composite element mapping doesn”t support null-able
properties if you”re using a ``<set>``. NHibernate has to use each columns value
to identify a record when deleting objects (there is no separate primary key
column in the composite element table), which is not possible with null values.
You have to either use only not-null properties in a composite-element or choose
a ``<list>``, ``<map>``, ``<bag>`` or ``<idbag>``.

A special case of a composite element is a composite element with a nested
``<many-to-one>`` element. A mapping like this allows you to map extra columns
of a many-to-many association table to the composite element class. The
following is a many-to-many association from ``Order`` to ``Item`` where
``PurchaseDate``, ``Price`` and ``Quantity`` are properties of the association:

.. code:: xml

  <class name="Order" .... >
    ....
    <set name="PurchasedItems" table="purchase_items" lazy="true">
      <key column="order_id">
      <composite-element class="Purchase">
        <property name="PurchaseDate"/>
        <property name="Price"/>
        <property name="Quantity"/>
        <many-to-one name="Item" class="Item"/> <!-- class attribute is optional -->
      </composite-element>
    </set>
  </class>

Even ternary (or quaternary, etc) associations are possible:

.. code:: xml

  <class name="Order" .... >
    ....
    <set name="PurchasedItems" table="purchase_items" lazy="true">
      <key column="order_id">
      <composite-element class="OrderLine">
        <many-to-one name="PurchaseDetails" class="Purchase"/>
        <many-to-one name="Item" class="Item"/>
      </composite-element>
    </set>
  </class>

Composite elements may appear in queries using the same syntax as associations
to other entities.

Components as IDictionary indices
==================================

The ``<composite-index>`` element lets you map a component class as the key of
an ``IDictionary``. Make sure you override ``GetHashCode()`` and ``Equals()``
correctly on the component class.

.. _components-compositeid:

Components as composite identifiers
====================================

You may use a component as an identifier of an entity class. Your component
class must satisfy certain requirements:

-  It must be marked with the ``Serializable`` attribute.

-  It must re-implement ``Equals()`` and ``GetHashCode()``, consistently with
   the database”s notion of composite key equality.

-  It should re-implement ``ToString()`` if you consider using the second level
   cache. See :ref:`NHibernate.Caches-howto`.

You can”t use an ``IIdentifierGenerator`` to generate composite keys. Instead
the application must assign its own identifiers.

Since a composite identifier must be assigned to the object before saving it, we
can”t use ``unsaved-value`` of the identifier to distinguish between newly
instantiated instances and instances saved in a previous session.

You may instead implement ``IInterceptor.IsTransient()`` if you wish to use
``SaveOrUpdate()`` or cascading save / update. As an alternative, you may also
set the ``unsaved-value`` attribute on a ``<version>`` (or ``<timestamp>``)
element to specify a value that indicates a new transient instance. In this
case, the version of the entity is used instead of the (assigned) identifier and
you don”t have to implement ``IInterceptor.IsTransient()`` yourself.

Use the ``<composite-id>`` tag (same attributes and elements as ``<component>``)
in place of ``<id>`` for the declaration of a composite identifier class:

.. code:: xml

  <class name="Foo" table="FOOS">
    <composite-id name="CompId" class="FooCompositeID">
      <key-property name="String"/>
      <key-property name="Short"/>
      <key-property name="Date" column="date_" type="Date"/>
    </composite-id>
    <property name="Name"/>
    ....
  </class>

Now, any foreign keys into the table ``FOOS`` are also composite. You must
declare this in your mappings for other classes. An association to ``Foo`` would
be declared like this:

.. code:: xml

  <many-to-one name="Foo" class="Foo">
    <!-- the "class" attribute is optional, as usual -->
    <column name="foo_string"/>
    <column name="foo_short"/>
    <column name="foo_date"/>
  </many-to-one>

This new ``<column>`` tag is also used by multi-column custom types. Actually it
is an alternative to the ``column`` attribute everywhere. A collection with
elements of type ``Foo`` would use:

.. code:: xml

  <set name="Foos">
    <key column="owner_id"/>
    <many-to-many class="Foo">
      <column name="foo_string"/>
      <column name="foo_short"/>
      <column name="foo_date"/>
    </many-to-many>
  </set>

On the other hand, ``<one-to-many>``, as usual, declares no columns.

If ``Foo`` itself contains collections, they will also need a composite foreign
key.

.. code:: xml

  <class name="Foo">
    ....
    ....
    <set name="Dates" lazy="true">
      <key>   <!-- a collection inherits the composite key type -->
        <column name="foo_string"/>
        <column name="foo_short"/>
        <column name="foo_date"/>
      </key>
      <element column="foo_date" type="Date"/>
    </set>
  </class>

Dynamic components
===================

You may even map a property of type ``IDictionary``:

.. code:: xml

  <dynamic-component name="UserAttributes">
    <property name="Foo" column="FOO"/>
    <property name="Bar" column="BAR"/>
    <many-to-one name="Baz" class="Baz" column="BAZ"/>
  </dynamic-component>

The semantics of a ``<dynamic-component>`` mapping are identical to
``<component>``. The advantage of this kind of mapping is the ability to
determine the actual properties of the component at deployment time, just by
editing the mapping document. (Runtime manipulation of the mapping document is
also possible, using a DOM parser.)
