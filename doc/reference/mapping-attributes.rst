*****************************
NHibernate.Mapping.Attributes
*****************************

What is NHibernate.Mapping.Attributes?
=======================================

**NHibernate.Mapping.Attributes is an add-in for
`NHibernate <http://nhibernate.info/>`_ contributed by Pierre
Henri Kuat” (aka *KPixel*); the former implementation was made by John Morris.**

NHibernate require mapping streams to bind your domain model to your database.
Usually, they are written (and maintained) in separated hbm.xml files.

With NHibernate.Mapping.Attributes, you can use .NET attributes to decorate your
entities and these attributes will be used to generate these mapping .hbm.xml
(as files or streams). So you will no longer have to bother with these *nasty*
xml files ;).

**Content of this library.**

1. **NHibernate.Mapping.Attributes**: That the only project you need (as end-user)

2. **Test**: a working sample using attributes and HbmSerializer as NUnit
   TestFixture

3. **Generator**: The program used to generate attributes and HbmWriter

4. `**Refly** <http://mbunit.tigris.org/>`_: Thanks to
   `Jonathan de Halleux <http://www.dotnetwiki.org/>`_ for this library which
   make it so easy to generate code

..

    **Important**

    This library is generated using the file
    ``/src/NHibernate.Mapping.Attributes/nhibernate-mapping.xsd`` (which is
    embedded in the assembly to be able to validate generated XML streams). As
    this file can change at each new release of NHibernate, you should
    regenerate it before using it with a different version (open the Generator
    solution, compile and run the Generator project). But, no test has been done
    with versions prior to 0.8.

What”s new?
============

**NHibernate.**

introduces many new features, improvements and changes:

1. It is possible to import classes by simply decorating them with ``[Import]
class ImportedClass1 {}``. Note that you must use
``HbmSerializer.Serialize(assembly)``; The ``<import/>`` mapping will be added
before the classes mapping. If you prefer to keep these imports in the class
using them, you can specify them all on the class:
``[Import(ClassType=typeof(ImportedClass1))] class Query {}``.

2. ``[RawXmlAttribute]`` is a new attribute allowing to insert xml as-is in the
mapping. This feature can be very useful to do complex mapping (eg: components).
It may also be used to quickly move the mapping from xml files to attributes.
Usage: ``[RawXml(After=typeof(ComponentAttribute), Content="<component
name="...">...</component>")]``. After tells after which kind of mapping the xml
should be inserted (generally, it is the type of the mapping you are inserting);
it is optional (in which case the xml is inserted on the top of the mapping).
Note: At the moment, all raw xmls are prefixed by a ``<!---->`` (in the
generated stream); this is a known side-effect.

3. ``[AttributeIdentifierAttribute]`` is a new attribute allowing to provide the
value of a defined ”place holder”. Eg:

.. code:: csharp

    public class Base {
      [Id(..., Column="{{Id.Column}}")]
      [AttributeIdentifier(Name="Id.Column", Value="ID")] // Default value
      public int Id { ... }
    }
    [AttributeIdentifier(Name="Id.Column", Value="SUB_ID")]
    [Class] public class MappedSubClass : Base { }

::

    The idea is that, when you have a mapping which is shared by many
    subclasses but which has minor differences (like different column
    names), you can put the mapping in the base class with place holders
    on these fields and give their values in subclasses. Note that this
    is possible for any mapping field taking a string (column, name,
    type, access, etc.). And, instead of Value, you can use ValueType or
    ValueObject (if you use an enum, you can control its formatting with
    ValueObject).

    The "place holder" is defined like this: `{{XXX}}`. If you don't
    want to use these double curly brackets, you can change them using
    the properties StartQuote and EndQuote of the class `HbmWriter`.

4.  It is possible to register patterns (using Regular Expressions) to
    automatically transform fully qualified names of properties types into something
    else. Eg: ``HbmSerializer.Default.HbmWriter.Patterns.Add(@"Namespace.(\S+),
    Assembly", "$1");`` will map all properties with a not-qualified type name.

5.  Two methods have been added to allow writing:
    ``cfg.AddInputStream(HbmSerializer.Default.Serialize(typeof(XXX)) )`` and
    ``cfg.AddInputStream(HbmSerializer.Default.Serialize(typeof(XXX).Assembly) )``.
    So it is no longer required to create a MemoryStream for these simple cases.

6.  Two ``WriteUserDefinedContent()`` methods have been added to ``HbmWriter``.
    They improve the extensibility of this library; it is now very easy to create a
    .NET attribute and integrate it in the mapping.

7.  Attributes ``[(Jcs)Cache]``, ``[Discriminator]`` and ``[Key]`` can be
    specified at class-level.

8.  Interfaces can be mapped (just like classes and structs).

9.  A notable ”bug” fix is the re-ordering of (joined-)subclasses; This
    operation may be required when a subclass extends another subclass. In this
    case, the extended class mapping must come before the extending class mapping.
    Note that the re-ordering takes place only for ”top-level” classes (that is not
    nested in other mapped classes). Anyway, it is quite unusual to put a
    interdependent mapped subclasses in a mapped class.

10. There are also many other little changes; refer to the release notes for
    more details.

How to use it?
===============

**The end-user class is NHibernate.Mapping.Attributes.HbmSerializer.**

This class *serialize* your domain model to mapping streams. You can either
serialize classes one by one or an assembly. Look at
``NHibernate.Mapping.Attributes.Test`` project for a working sample.

The first step is to decorate your entities with attributes; you can use:
``[Class]``, ``[Subclass]``, ``[JoinedSubclass]`` or ``[Component]``. Then, you
decorate your members (fields/properties); they can take as many attributes as
required by your mapping. Eg:

.. code:: csharp

  [NHibernate.Mapping.Attributes.Class]
  public class Example
  {
      [NHibernate.Mapping.Attributes.Property]
      public string Name;
  }

After this step, you use ``NHibernate.Mapping.Attributes.HbmSerializer``: (here,
we use Default which is an instance you can use if you don”t need/want to create
it yourself).

.. code:: csharp

  NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
  cfg.Configure();
  // Enable validation (optional)
  NHibernate.Mapping.Attributes.HbmSerializer.Default.Validate = true;
  // Here, we serialize all decorated classes
  // (but you can also do it class by class)
  System.Reflection.Assembly assembly =
      System.Reflection.Assembly.GetExecutingAssembly();

  cfg.AddInputStream(
    NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize(assembly)
  );
  // Now you can use this configuration to build your SessionFactory...

..

    **Note**

    As you can see here: NHibernate.Mapping.Attributes is **not** (really)
    intrusive. Setting attributes on your objects doesn”t force you to use them
    with NHibernate and doesn”t break any constraint on your architecture.
    Attributes are purely informative (like documentation)!

.. _mapping-attributes-tips:

Tips
=====

1.  In production, it is recommended to generate a XML mapping file from
    NHibernate.Mapping.Attributes and use this file each time the SessionFactory
    need to be built. Use: ``HbmSerializer.Default.Serialize(typeof(XXX).Assembly,
    "DomainModel.hbm.xml");`` It is slightly faster.

2.  Use HbmSerializer.Validate to enable/disable the validation of generated xml
    streams (against NHibernate mapping schema); this is useful to quickly find
    errors (they are written in StringBuilder HbmSerializer.Error). If the error is
    due to this library then see if it is a know issue and report it; you can
    contribute a solution if you solve the problem :)

3.  Your classes, fields and properties (members) can be private; just make sure
    that you have the permission to access private members using reflection
    (ReflectionPermissionFlag.MemberAccess).

4.  Members of a mapped classes are also seek in its base classes (until we
    reach *mapped* base class). So you can decorate some members of a (not mapped)
    base class and use it in its (mapped) sub class(es).

5.  For a Name taking a ``System.Type``, set the type with Name\ ``="xxx"`` (as
    ``string``) or Name\ ``Type=typeof(xxx)``; (add ``Type`` to ”Name”)

6.  By default, .NET attributes don”t keep the order of attributes; so you need
    to set it yourself when the order matter (using the first parameter of each
    attribute); it is *highly* recommended to set it when you have more than one
    attribute on the same member.

7.  As long as there is no ambiguity, you can decorate a member with many
    unrelated attributes. A good example is to put class-related attributes (like
    ``<discriminator>``) on the identifier member. But don”t forget that the order
    matters (the ``<discriminator>`` must be after the ``<id>``). The order used
    comes from the order of elements in the NHibernate mapping schema. Personally, I
    prefer using negative numbers for these attributes (if they come before!).

8.  You can add ``[HibernateMapping]`` on your classes to specify
    ``<hibernate-mapping>`` attributes (used when serializing the class in its
    stream). You can also use HbmSerializer.Hbm\* properties (used when serializing
    an assembly or a type that is not decorated with ``[HibernateMapping]``).

9.  Instead of using a string for DiscriminatorValue (in ``[Class]`` and
    ``[Subclass]``), you can use any object you want. Example:

    .. code:: csharp

      [Subclass(DiscriminatorValueEnumFormat="d", DiscriminatorValueObject=DiscEnum.Val1)]

    Here, the object is an Enum, and you can set the format you want (the default
    value is ”g”). Note that you must put it **before**! For others types, It
    simply use the ToString() method of the object.

10. Each stream generated by NHibernate.Mapping.Attributes can contain a comment
    with the date of the generation; You may enable/disable this by using the
    property HbmSerializer.WriteDateComment.

11. If you forget to provide a required xml attribute, it will obviously throw
    an exception while generating the mapping.

12. The recommended and easiest way to map ``[Component]`` is to use
    ``[ComponentProperty]``. The first step is to put ``[Component]`` on the
    component class and map its fields/properties. Note that you shouldn”t set the
    Name in ``[Component]``. Then, on each member in your classes, add
    ``[ComponentProperty]``. But you can”t override Access, Update or Insert for
    each member.

    There is a working example in *NHibernate.Mapping.Attributes.Test* (look for
    the class ``CompAddress`` and its usage in others classes).

13. Another way to map ``[Component]`` is to use the way this library works: If
    a mapped class contains a mapped component, then this component will be include
    in the class. *NHibernate.Mapping.Attributes.Test* contains the classes
    ``JoinedBaz`` and ``Stuff`` which both use the component ``Address``.

    Basically, it is done by adding

    .. code:: csharp

      [Component(Name = "MyComp")] private class SubComp : Comp {}

    in each class. One of the advantages is that you can override Access, Update
    or Insert for each member. But you have to add the component subclass in
    **each** class (and it can not be inherited). Another advantage is that you
    can use ``[AttributeIdentifier]``.

14. Finally, whenever you think that it is easier to write the mapping in XML
    (this is often the case for ``[Component]``), you can use ``[RawXml]``.

15. **About customization.**

    ``HbmSerializer`` uses ``HbmWriter`` to serialize each kind of attributes.
    Their methods are virtual; so you can create a subclass and override any
    method you want (to change its default behavior).

    Use the property HbmSerializer.HbmWriter to change the writer used (you may
    set a subclass of ``HbmWriter``).

Example using some this tips: (0, 1 and 2 are position indexes)

.. code:: csharp

  // Don't put it after [ManyToOne] !!!
  [NHibernate.Mapping.Attributes.Id(0, TypeType=typeof(int))]
  [NHibernate.Mapping.Attributes.Generator(1, Class="uuid.hex")]
  [NHibernate.Mapping.Attributes.ManyToOne(2, ClassType=typeof(Foo),  OuterJoin=OuterJoinStrategy.True)]
  private Foo Entity;

Generates:

.. code:: xml

  <id type="Int32">
      <generator class="uuid.hex" />
  </id>
  <many-to-one name="Entity" class="Namespaces.Foo, SampleAssembly" outer-join="true" />

Known issues and TODOs
=======================

First, read TODOs in the source code ;)

A Position property has been added to all attributes to order them. But there is
still a problem:

When a parent element ”p” has a child element ”x” that is also the child element
of another child element ”c” of ”p” (preceding ”x”) :D Illustration:

.. code:: xml

  <p>
    <c>
      <x />
    </c>
    <x />
  </p>

In this case, when writing:

.. code:: csharp

  [Attributes.P(0)]
  [Attributes.C(1)]
  [Attributes.X(2)]
  [Attributes.X(3)]
  public MyType MyProperty;

X(3) will always belong to C(1) ! (as X(2)).

It is the case for ``<dynamic-component>`` and ``<nested-composite-element>``.

Another bad news is that, currently, XML elements coming after this elements can
not be included in them. Eg: There is no way put a collection in
``<dynamic-component>``. The reason is that the file ``nhibernate-mapping.xsd``
tells how elements are built and in which order, and
NHibernate.Mapping.Attributes use this order.

Anyway, the solution would be to add a int ParentNode property to BaseAttribute
so that you can create a real graph...

For now, you can fallback on ``[RawXml]``.

Actually, there is no other know issue nor planned modification. This library
should be stable and complete; but if you find a bug or think of an useful
improvement, contact us!

On side note, it would be nice to write a better TestFixture than
*NHibernate.Mapping.Attributes.Test* :D

Developer Notes
===============

Any change to the schema (``nhibernate-mapping.xsd``) implies:

1. Checking if there is any change to do in the Generator (like updating
   KnowEnums / AllowMultipleValue / IsRoot / IsSystemType / IsSystemEnum /
   CanContainItself)

2. Updating ``/src/NHibernate.Mapping.Attributes/nhibernate-mapping.xsd``
   (copy/paste) and running the Generator again (even if it wasn”t modified)

3. Running the Test project and make sure that no exception is thrown. A
   class/property should be modified/added in this project to be sure that any new
   breaking change will be caught (=> update the reference hbm.xml files and/or the
   project ``NHibernate.Mapping.Attributes.csproj``)

This implementation is based on NHibernate mapping schema; so there is probably
lot of ”standard schema features” that are not supported...

The version of NHibernate.Mapping.Attributes should be the version of the
NHibernate schema used to generate it (=> the version of NHibernate library).

In the design of this project, performance is a (*very*) minor goal :) Easier
implementation and maintenance are far more important because you can (and
should) avoid to use this library in production (see the first tip in
:ref:`mapping-attributes-tips`).
