# Basic O/R Mapping

# Mapping declaration

Object/relational mappings are defined in an XML document. The mapping
document is designed to be readable and hand-editable. The mapping
language is object-centric, meaning that mappings are constructed around
persistent class declarations, not table declarations.

Note that, even though many NHibernate users choose to define XML
mappings by hand, a number of tools exist to generate the mapping
document, including NHibernate.Mapping.Attributes library and various
template-based code generators (CodeSmith, MyGeneration). You may also
use `NHibernate.Mapping.ByCode` available since NHibernate 3.2, or
[Fluent NHibernate](https://github.com/jagregory/fluent-nhibernate).

Let's kick off with an example mapping:

```xml
    <?xml version="1.0"?>
    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2" assembly="Eg"
        namespace="Eg">
    
            <class name="Cat" table="CATS" discriminator-value="C">
                    <id name="Id" column="uid" type="Int64">
                            <generator class="hilo"/>
                    </id>
                    <discriminator column="subclass" type="Char"/>
                    <property name="BirthDate" type="Date"/>
                    <property name="Color" not-null="true"/>
                    <property name="Sex" not-null="true" update="false"/>
                    <property name="Weight"/>
                    <many-to-one name="Mate" column="mate_id"/>
                    <set name="Kittens">
                            <key column="mother_id"/>
                            <one-to-many class="Cat"/>
                    </set>
                    <subclass name="DomesticCat" discriminator-value="D">
                            <property name="Name" type="String"/>
                    </subclass>
            </class>
    
            <class name="Dog">
                    <!-- mapping for Dog could go here -->
            </class>
    
    </hibernate-mapping>
```

We will now discuss the content of the mapping document. We will only
describe the document elements and attributes that are used by
NHibernate at runtime. The mapping document also contains some extra
optional attributes and elements that affect the database schemas
exported by the schema export tool. (For example the `
not-null` attribute.)

## XML Namespace

All XML mappings should declare the XML namespace shown. The actual
schema definition may be found in the `src\nhibernate-mapping.xsd` file
in the NHibernate distribution.

*Tip: to enable IntelliSense for mapping and configuration files, copy
the appropriate `.xsd` files as part of any project in your solution,
(`Build Action` can be "None") or as "Solution Files" or in your `"Lib"`
folder and then add it to the `Schemas` property of the xml file. You
can copy it in `<VS installation directory>\Xml\Schemas`, take care
because you will have to deal with different version of the xsd for
different versions of NHibernate.*

## hibernate-mapping

This element has several optional attributes. The `schema` attribute
specifies that tables referred to by this mapping belong to the named
schema. If specified, table names will be qualified by the given schema
name. If missing, table names will be unqualified. The `default-cascade`
attribute specifies what cascade style should be assumed for properties
and collections which do not specify a `cascade` attribute. The
`auto-import` attribute lets us use unqualified class names in the query
language, by default. The `assembly` and `namespace` attributes specify
the assembly where persistent classes are located and the namespace they
are declared in.

```xml
    <hibernate-mapping
             schema="schemaName"
             default-cascade="none|save-update"
             auto-import="true|false"
             assembly="Eg"
             namespace="Eg"
             default-access="field|property|field.camecase..."
             default-lazy="true|false"
     />
```

  - `schema` (optional): The name of a database schema.

  - `default-cascade` (optional - defaults to `none`): A default cascade
    style.

  - `auto-import` (optional - defaults to `true`): Specifies whether we
    can use unqualified class names (of classes in this mapping) in the
    query language.

  - `assembly` and `namespace`(optional): Specify assembly and namespace
    to assume for unqualified class names in the mapping document.

  - `default-access` (optional - defaults to property): The strategy
    NHibernate should use for accessing a property value

  - `default-lazy` (optional - defaults to `true`): Lazy fetching may be
    completely disabled by setting default-lazy="false".

If you are not using `assembly` and `namespace` attributes, you have to
specify fully-qualified class names, including the name of the assembly
that classes are declared in.

If you have two persistent classes with the same (unqualified) name, you
should set `auto-import="false"`. NHibernate will throw an exception if
you attempt to assign two classes to the same "imported" name.

## class

You may declare a persistent class using the `class` element:

```xml
    <class
            name="ClassName"
            table="tableName"
            discriminator-value="discriminator_value"
            mutable="true|false"
            schema="owner"
            proxy="ProxyInterface"
            dynamic-update="true|false"
            dynamic-insert="true|false"
            select-before-update="true|false"
            polymorphism="implicit|explicit"
            where="arbitrary sql where condition"
            persister="PersisterClass"
            batch-size="N"
            optimistic-lock="none|version|dirty|all"
            lazy="true|false"
            abstract="true|false"
    />
```

  - `name`: The fully qualified .NET class name of the persistent class
    (or interface), including its assembly name.

  - `table`(optional - defaults to the unqualified class name): The name
    of its database table.

  - `discriminator-value` (optional - defaults to the class name): A
    value that distinguishes individual subclasses, used for polymorphic
    behaviour. Acceptable values include `null` and `not null`.

  - `mutable` (optional, defaults to `true`): Specifies that instances
    of the class are (not) mutable.

  - `schema` (optional): Override the schema name specified by the root
    `<hibernate-mapping>` element.

  - `proxy` (optional): Specifies an interface to use for lazy
    initializing proxies. You may specify the name of the class itself.

  - `dynamic-update` (optional, defaults to `false`): Specifies that
    `UPDATE` SQL should be generated at runtime and contain only those
    columns whose values have changed.

  - `dynamic-insert` (optional, defaults to `false`): Specifies that
    `INSERT` SQL should be generated at runtime and contain only the
    columns whose values are not null.

  - `select-before-update` (optional, defaults to `false`): Specifies
    that NHibernate should *never* perform an SQL `UPDATE` unless it is
    certain that an object is actually modified. In certain cases
    (actually, only when a transient object has been associated with a
    new session using `update()`), this means that NHibernate will
    perform an extra SQL `SELECT` to determine if an `UPDATE` is
    actually required.

  - `polymorphism` (optional, defaults to `implicit`): Determines
    whether implicit or explicit query polymorphism is used.

  - `where` (optional) specify an arbitrary SQL `WHERE` condition to be
    used when retrieving objects of this class

  - `persister` (optional): Specifies a custom `IClassPersister`.

  - `batch-size` (optional, defaults to `1`) specify a "batch size" for
    fetching instances of this class by identifier.

  - `optimistic-lock` (optional, defaults to `version`): Determines the
    optimistic locking strategy.

  - `lazy` (optional): Lazy fetching may be completely disabled by
    setting `lazy="false"`.

  - `abstract` (optional): Used to mark abstract superclasses in
    `<union-subclass>` hierarchies.

It is perfectly acceptable for the named persistent class to be an
interface. You would then declare implementing classes of that interface
using the `<subclass>` element. You may persist any inner class. You
should specify the class name using the standard form ie. `Eg.Foo+Bar,
Eg`. Due to an HQL parser limitation inner classes can not be used in
queries in NHibernate 1.0.

Changes to immutable classes, `mutable="false"`, will not be persisted.
This allows NHibernate to make some minor performance optimizations.

The optional `proxy` attribute enables lazy initialization of persistent
instances of the class. NHibernate will initially return proxies which
implement the named interface. The actual persistent object will be
loaded when a method of the proxy is invoked. See "Proxies for Lazy
Initialization" below.

*Implicit* polymorphism means that instances of the class will be
returned by a query that names any superclass or implemented interface
or the class and that instances of any subclass of the class will be
returned by a query that names the class itself. *Explicit* polymorphism
means that class instances will be returned only be queries that
explicitly name that class and that queries that name the class will
return only instances of subclasses mapped inside this `<class>`
declaration as a `<subclass>` or `<joined-subclass>`. For most purposes
the default, `polymorphism="implicit"`, is appropriate. Explicit
polymorphism is useful when two different classes are mapped to the same
table (this allows a "lightweight" class that contains a subset of the
table columns).

The `persister` attribute lets you customize the persistence strategy
used for the class. You may, for example, specify your own subclass of
`NHibernate.Persister.EntityPersister` or you might even provide a
completely new implementation of the interface
`NHibernate.Persister.IClassPersister` that implements persistence via,
for example, stored procedure calls, serialization to flat files or
LDAP. See `NHibernate.DomainModel.CustomPersister` for a simple example
(of "persistence" to a `Hashtable`).

Note that the `dynamic-update` and `dynamic-insert` settings are not
inherited by subclasses and so may also be specified on the `<subclass>`
or `<joined-subclass>` elements. These settings may increase performance
in some cases, but might actually decrease performance in others. Use
judiciously.

Use of `select-before-update` will usually decrease performance. It is
very useful to prevent a database update trigger being called
unnecessarily.

If you enable `dynamic-update`, you will have a choice of optimistic
locking strategies:

  - `version` check the version/timestamp columns

  - `all` check all columns

  - `dirty` check the changed columns

  - `none` do not use optimistic locking

We *very* strongly recommend that you use version/timestamp columns for
optimistic locking with NHibernate. This is the optimal strategy with
respect to performance and is the only strategy that correctly handles
modifications made outside of the session (ie. when `ISession.Update()`
is used). Keep in mind that a version or timestamp property should never
be null, no matter what `unsaved-value` strategy, or an instance will be
detected as transient.

Beginning with NHibernate 1.2.0, version numbers start with 1, not 0 as
in previous versions. This was done to allow using 0 as `unsaved-value`
for the version property.

## subselect

An alternative to mapping a class to table or view columns is to map a
*query*. For that, we use the `<subselect>` element, which is mutually
exclusive with `<subclass>`, `<joined-subclass>` and `<union-subclass>`.
The content of the `subselect` element is a SQL query:

```xml
    <subselect>
        <![CDATA[
        SELECT cat.ID, cat.NAME, cat.SEX, cat.MATE FROM cat
        ]]>
    </subselect>
```

Usually, when mapping a query using `subselect` you will want to mark
the class as not mutable (`mutable="false"`), unless you specify custom
SQL for performing the UPDATE, DELETE and INSERT operations.

Also, it makes sense to force synchronization of the tables affected by
the query, using one or more `<synchronize>` entries:

```xml
    <subselect>
        <![CDATA[
        SELECT cat.ID, cat.NAME, cat.SEX, cat.MATE FROM cat
        ]]>
    </subselect>
    <syncronize table="cat"/>
```

You then still have to declare the class id and properties.

## id

Mapped classes *must* declare the primary key column of the database
table. Most classes will also have a property holding the unique
identifier of an instance. The `<id>` element defines the mapping from
that property to the primary key column.

```xml
    <id
            name="PropertyName"
            type="typename"
            column="column_name"
            unsaved-value="any|none|null|id_value"
            access="field|property|nosetter|ClassName">
    
            <generator class="generatorClass"/>
    </id>
```

  - `name` (optional): The name of the identifier property.

  - `type` (optional): A name that indicates the NHibernate type.

  - `column` (optional - defaults to the property name): The name of the
    primary key column.

  - `unsaved-value` (optional - defaults to a "sensible" value): An
    identifier property value that indicates that an instance is newly
    instantiated (unsaved), distinguishing it from transient instances
    that were saved or loaded in a previous session.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

If the `name` attribute is missing, it is assumed that the class has no
identifier property.

The `unsaved-value` attribute is almost never needed in NHibernate 1.0.

There is an alternative `<composite-id>` declaration to allow access to
legacy data with composite keys. We strongly discourage its use for
anything else.

### generator

The required `generator` names a .NET class used to generate unique
identifiers for instances of the persistent class.

The generator can be declared using the `<generator>` child element. If
any parameters are required to configure or initialize the generator
instance, they are passed using `<param>` elements.

```xml
    <id name="Id" type="Int64" column="uid" unsaved-value="0">
            <generator class="NHibernate.Id.TableHiLoGenerator">
                    <param name="table">uid_table</param>
                    <param name="column">next_hi_value_column</param>
            </generator>
    </id>
```

If no parameters are required, the generator can be declared using a
`generator` attribute directly on the `<id>` element, as
    follows:

```xml
    <id name="Id" type="Int64" column="uid" unsaved-value="0" generator="native" />
```

All generators implement the interface
`NHibernate.Id.IIdentifierGenerator`. This is a very simple interface;
some applications may choose to provide their own specialized
implementations. However, NHibernate provides a range of built-in
implementations. There are shortcut names for the built-in generators:

  - `increment`  
    generates identifiers of any integral type that are unique only when
    no other process is inserting data into the same table. *Do not use
    in a cluster.*

  - `identity`  
    supports identity columns in DB2, MySQL, MS SQL Server and Sybase.
    The identifier returned by the database is converted to the property
    type using `Convert.ChangeType`. Any integral property type is thus supported.

  - `sequence`  
    uses a sequence in DB2, PostgreSQL, Oracle or a generator in
    Firebird. The identifier returned by the database is converted to
    the property type using `Convert.ChangeType`. Any integral property
    type is thus supported.

  - `hilo`  
    uses a hi/lo algorithm to efficiently generate identifiers of any
    integral type, given a table and column (by default
    `hibernate_unique_key` and `next_hi` respectively) as a source of hi
    values. The hi/lo algorithm generates identifiers that are unique
    only for a particular database. *Do not use this generator with a
    user-supplied connection.*
    
    You can use the "where" parameter to specify the row to use in a
    table. This is useful if you want to use a single table for your
    identifiers, with different rows for each table.

  - `seqhilo`  
    uses a hi/lo algorithm to efficiently generate identifiers of any
    integral type, given a named database sequence.

  - `uuid.hex`  
    uses `System.Guid` and its `ToString(string format)` method to
    generate identifiers of type string. The length of the string
    returned depends on the configured `format`.

  - `uuid.string`  
    uses a new `System.Guid` to create a `byte[]` that is converted to a
    string.

  - `guid`  
    uses a new `System.Guid` as the identifier.

  - `guid.comb`  
    uses the algorithm to generate a new `System.Guid` described by
    Jimmy Nilsson in [this article](https://www.informit.com/articles/article.aspx?p=25862).

  - `native`  
    picks `identity`, `sequence` or `hilo` depending upon the
    capabilities of the underlying database.

  - `assigned`  
    lets the application to assign an identifier to the object before
    `Save()` is called.

  - `foreign`  
    uses the identifier of another associated object. Usually used in
    conjunction with a `<one-to-one>` primary key association.

### Hi/Lo Algorithm

The `hilo` and `seqhilo` generators provide two alternate
implementations of the hi/lo algorithm, a favorite approach to
identifier generation. The first implementation requires a "special"
database table to hold the next available "hi" value. The second uses an
Oracle-style sequence (where supported).

```xml
    <id name="Id" type="Int64" column="cat_id">
            <generator class="hilo">
                    <param name="table">hi_value</param>
                    <param name="column">next_value</param>
                    <param name="max_lo">100</param>
            </generator>
    </id>

    <id name="Id" type="Int64" column="cat_id">
            <generator class="seqhilo">
                    <param name="sequence">hi_value</param>
                    <param name="max_lo">100</param>
            </generator>
    </id>
```

Unfortunately, you can't use `hilo` when supplying your own
`DbConnection` to NHibernate. NHibernate must be able to fetch the "hi"
value in a new transaction.

### UUID Hex Algorithm

```xml
    <id name="Id" type="String" column="cat_id">
            <generator class="uuid.hex">
                <param name="format">format_value</param>
                <param name="separator">separator_value</param>
            </generator>
    </id>
```

The UUID is generated by calling `Guid.NewGuid().ToString(format)`. The
valid values for `format` are described in the MSDN documentation. The
default `separator` is `-` and should rarely be modified. The `format`
determines if the configured `separator` can replace the default
separator used by the `format`.

### UUID String Algorithm

The UUID is generated by calling `Guid.NewGuid().ToByteArray()` and then
converting the `byte[]` into a `char[]`. The `char[]` is returned as a
`String` consisting of 16 characters.

### GUID Algorithms

The `guid` identifier is generated by calling `Guid.NewGuid()`. To
address some of the performance concerns with using Guids as primary
keys, foreign keys, and as part of indexes with MS SQL the `guid.comb`
can be used. The benefit of using the `guid.comb` with other databases
that support GUIDs has not been measured.

### Identity columns and Sequences

For databases which support identity columns (DB2, MySQL, Sybase, MS
SQL), you may use `identity` key generation. For databases that support
sequences (DB2, Oracle, PostgreSQL, Interbase, McKoi, SAP DB) you may
use `sequence` style key generation. Both these strategies require two
SQL queries to insert a new object.

```xml
    <id name="Id" type="Int64" column="uid">
            <generator class="sequence">
                    <param name="sequence">uid_sequence</param>
            </generator>
    </id>

    <id name="Id" type="Int64" column="uid" unsaved-value="0">
            <generator class="identity"/>
    </id>
```

For cross-platform development, the `native` strategy will choose from
the `identity`, `sequence` and `hilo` strategies, dependent upon the
capabilities of the underlying database.

### Assigned Identifiers

If you want the application to assign identifiers (as opposed to having
NHibernate generate them), you may use the `assigned` generator. This
special generator will use the identifier value already assigned to the
object's identifier property. Be very careful when using this feature to
assign keys with business meaning (almost always a terrible design
decision).

Due to its inherent nature, entities that use this generator cannot be
saved via the ISession's SaveOrUpdate() method. Instead you have to
explicitly specify to NHibernate if the object should be saved or
updated by calling either the `Save()` or `Update()` method of the
ISession.

### Enhanced identifier generators

Starting with NHibernate release 3.3.0, there are 2 new generators which
represent a re-thinking of 2 different aspects of identifier generation.
The first aspect is database portability; the second is optimization.
Optimization means that you do not have to query the database for every
request for a new identifier value. These two new generators are
intended to take the place of some of the named generators described
above, starting in 3.3.x. However, they are included in the current
releases and can be referenced by FQN.

The first of these new generators is
`NHibernate.Id.Enhanced.SequenceStyleGenerator` (short name
`enhanced-sequence`) which is intended, firstly, as a replacement for
the `sequence` generator and, secondly, as a better portability
generator than `native`. This is because `native` generally chooses
between `identity` and `sequence` which have largely different semantics
that can cause subtle issues in applications eyeing portability.
`NHibernate.Id.Enhanced.SequenceStyleGenerator`, however, achieves
portability in a different manner. It chooses between a table or a
sequence in the database to store its incrementing values, depending on
the capabilities of the dialect being used. The difference between this
and `native` is that table-based and sequence-based storage have the
same exact semantic. In fact, sequences are exactly what NHibernate
tries to emulate with its table-based generators. This generator has a
number of configuration parameters:

  - `sequence_name` (optional, defaults to `hibernate_sequence`): the
    name of the sequence or table to be used.

  - `initial_value` (optional, defaults to `1`): the initial value to be
    retrieved from the sequence/table. In sequence creation terms, this
    is analogous to the clause typically named "STARTS WITH".

  - `increment_size` (optional - defaults to `1`): the value by which
    subsequent calls to the sequence/table should differ. In sequence
    creation terms, this is analogous to the clause typically named
    "INCREMENT BY".

  - `force_table_use` (optional - defaults to `false`): should we force
    the use of a table as the backing structure even though the dialect
    might support sequence?

  - `value_column` (optional - defaults to `next_val`): only relevant
    for table structures, it is the name of the column on the table
    which is used to hold the value.

  - `prefer_sequence_per_entity` (optional - defaults to `false`):
    should we create separate sequence for each entity that share
    current generator based on its name?

  - `sequence_per_entity_suffix` (optional - defaults to `_SEQ`): suffix
    added to the name of a dedicated sequence.

  - `optimizer` (optional - defaults to `none`): See [Identifier generator optimization](#identifier-generator-optimization)

The second of these new generators is
`NHibernate.Id.Enhanced.TableGenerator` (short name `enhanced-table`),
which is intended, firstly, as a replacement for the `table` generator,
even though it actually functions much more like
`org.hibernate.id.MultipleHiLoPerTableGenerator` (not available in
NHibernate), and secondly, as a re-implementation of
`org.hibernate.id.MultipleHiLoPerTableGenerator` (not available in
NHibernate) that utilizes the notion of pluggable optimizers.
Essentially this generator defines a table capable of holding a number
of different increment values simultaneously by using multiple
distinctly keyed rows. This generator has a number of configuration
parameters:

  - `table_name` (optional - defaults to `hibernate_sequences`): the
    name of the table to be used.

  - `value_column_name` (optional - defaults to `next_val`): the name of
    the column on the table that is used to hold the value.

  - `segment_column_name` (optional - defaults to `sequence_name`): the
    name of the column on the table that is used to hold the "segment
    key". This is the value which identifies which increment value to
    use.

  - `segment_value` (optional - defaults to `default`): The "segment
    key" value for the segment from which we want to pull increment
    values for this generator.

  - `segment_value_length` (optional - defaults to `255`): Used for
    schema generation; the column size to create this segment key
    column.

  - `initial_value` (optional - defaults to `1`): The initial value to
    be retrieved from the table.

  - `increment_size` (optional - defaults to `1`): The value by which
    subsequent calls to the table should differ.

  - `optimizer` (optional - defaults to `none`): See [Identifier generator optimization](#identifier-generator-optimization).

#### Identifier generator optimization

For identifier generators that store values in the database, it is
inefficient for them to hit the database on each and every call to
generate a new identifier value. Instead, you can group a bunch of them
in memory and only hit the database when you have exhausted your
in-memory value group. This is the role of the pluggable optimizers.
Currently only the two enhanced generators ([Enhanced identifier
generators](#enhanced-identifier-generators) support this operation.

  - `none` (generally this is the default if no optimizer was
    specified): this will not perform any optimizations and hit the
    database for each and every request.

  - `hilo`: applies a hi/lo algorithm around the database retrieved
    values. The values from the database for this optimizer are expected
    to be sequential. The values retrieved from the database structure
    for this optimizer indicates the "group number". The
    `increment_size` is multiplied by that value in memory to define a
    group "hi value".

  - `pooled`: as with the case of `hilo`, this optimizer attempts to
    minimize the number of hits to the database. Here, however, we
    simply store the starting value for the "next group" into the
    database structure rather than a sequential value in combination
    with an in-memory grouping algorithm. Here, `increment_size` refers
    to the values coming from the database.

  - `pooled-lo`: similar to `pooled`, except that it's the starting
    value of the "current group" that is stored into the database
    structure. Here, `increment_size` refers to the values coming from
    the database.

## composite-id

```xml
    <composite-id
            name="PropertyName"
            class="ClassName"
            unsaved-value="any|none"
            access="field|property|nosetter|ClassName">
    
            <key-property name="PropertyName" type="typename" column="column_name"/>
            <key-many-to-one name="PropertyName class="ClassName" column="column_name"/>
            ......
    </composite-id>
```

For a table with a composite key, you may map multiple properties of the
class as identifier properties. The `<composite-id>` element accepts
`<key-property>` property mappings and `<key-many-to-one>` mappings as
child elements.

```xml
    <composite-id>
            <key-property name="MedicareNumber"/>
            <key-property name="Dependent"/>
    </composite-id>
```

Your persistent class *must* override `Equals()` and `GetHashCode()` to
implement composite identifier equality. It must also be marked with the
`Serializable` attribute.

Unfortunately, this approach to composite identifiers means that a
persistent object is its own identifier. There is no convenient "handle"
other than the object itself. You must instantiate an instance of the
persistent class itself and populate its identifier properties before
you can `Load()` the persistent state associated with a composite key.
We will describe a much more convenient approach where the composite
identifier is implemented as a separate class in
[Components as composite identifiers](component_mapping.md#components-as-composite-identifiers). The attributes described below apply
only to this alternative approach:

  - `name` (optional, required for this approach): A property of
    component type that holds the composite identifier (see next
    section).

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `class` (optional - defaults to the property type determined by
    reflection): The component class used as a composite identifier (see
    next section).

## discriminator

The `<discriminator>` element is required for polymorphic persistence
using the table-per-class-hierarchy mapping strategy and declares a
discriminator column of the table. The discriminator column contains
marker values that tell the persistence layer what subclass to
instantiate for a particular row. A restricted set of types may be used:
`String`, `Char`, `Int32`, `Byte`, `Short`, `Boolean`, `YesNo`,
`TrueFalse`.

```xml
    <discriminator
            column="discriminator_column"
            type="discriminator_type"
            force="true|false"
            insert="true|false"
            formula="arbitrary SQL expression"
    />
```

  - `column` (optional - defaults to `class`) the name of the
    discriminator column.

  - `type` (optional - defaults to `String`) a name that indicates the
    NHibernate type

  - `force` (optional - defaults to `false`) "force" NHibernate to
    specify allowed discriminator values even when retrieving all
    instances of the root class.

  - `insert` (optional - defaults to `true`) set this to `false` if your
    discriminator column is also part of a mapped composite identifier.

  - `formula` (optional) an arbitrary SQL expression that is executed
    when a type has to be evaluated. Allows content-based
    discrimination.

Actual values of the discriminator column are specified by the
`discriminator-value` attribute of the `<class>` and `<subclass>`
elements.

The `force` attribute is (only) useful if the table contains rows with
"extra" discriminator values that are not mapped to a persistent class.
This will not usually be the case.

Using the `formula` attribute you can declare an arbitrary SQL
expression that will be used to evaluate the type of a row:

```xml
    <discriminator
        formula="case when CLASS_TYPE in ('a', 'b', 'c') then 0 else 1 end"
        type="Int32"/>
```

## version (optional)

The `<version>` element is optional and indicates that the table
contains versioned data. This is particularly useful if you plan to use
*long transactions* (see below).

```xml
    <version
            column="version_column"
            name="PropertyName"
            type="typename"
            access="field|property|nosetter|ClassName"
            unsaved-value="null|negative|undefined|value"
            generated="never|always"
    />
```

  - `column` (optional - defaults to the property name): The name of the
    column holding the version number.

  - `name`: The name of a property of the persistent class.

  - `type` (optional - defaults to `Int32`): The type of the version
    number.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `unsaved-value` (optional - defaults to a "sensible" value): A
    version property value that indicates that an instance is newly
    instantiated (unsaved), distinguishing it from transient instances
    that were saved or loaded in a previous session. (`undefined`
    specifies that the identifier property value should be used.)

  - `generated` (optional - defaults to `never`): Specifies that this
    version property value is actually generated by the database. See
    the discussion of [Generated Properties](#generated-properties).

Version may be of type `Int64`, `Int32`, `Int16`, `Ticks`, `Timestamp`,
`TimeSpan`, `datetimeoffset`, ... (or their nullable counterparts in
.NET 2.0). Any type implementing `IVersionType` is usable as a version.

## timestamp (optional)

The optional `<timestamp>` element indicates that the table contains
timestamped data. This is intended as an alternative to versioning.
Timestamps are by nature a less safe implementation of optimistic
locking. However, sometimes the application might use the timestamps in
other ways.

```xml
    <timestamp
            column="timestamp_column"
            name="PropertyName"
            access="field|property|nosetter|ClassName"
            unsaved-value="null|undefined|value"
            generated="never|always"
    />
```

  - `column` (optional - defaults to the property name): The name of a
    column holding the timestamp.

  - `name`: The name of a property of .NET type `DateTime` of the
    persistent class.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `unsaved-value` (optional - defaults to `null`): A timestamp
    property value that indicates that an instance is newly instantiated
    (unsaved), distinguishing it from transient instances that were
    saved or loaded in a previous session. (`undefined` specifies that
    the identifier property value should be used.)

  - `generated` (optional - defaults to `never`): Specifies that this
    timestamp property value is actually generated by the database. See
    the discussion of [Generated Properties](#generated-properties).

Note that `<timestamp>` is equivalent to `<version type="timestamp">`.

## property

The `<property>` element declares a persistent property of the class.

```xml
    <property
            name="propertyName"
            column="column_name"
            type="typename"
            update="true|false"
            insert="true|false"
            formula="arbitrary SQL expression"
            access="field|property|ClassName"
            optimistic-lock="true|false"
            generated="never|insert|always"
            lazy="true|false"
    />
```

  - `name`: the name of the property of your class.

  - `column` (optional - defaults to the property name): the name of the
    mapped database table column.

  - `type` (optional): a name that indicates the NHibernate type.

  - `update, insert` (optional - defaults to `true`) : specifies that
    the mapped columns should be included in SQL `UPDATE` and/or
    `INSERT` statements. Setting both to `false` allows a pure "derived"
    property whose value is initialized from some other property that
    maps to the same column(s) or by a trigger or other application.

  - `formula` (optional): an SQL expression that defines the value for a
    *computed* property. Computed properties do not have a column
    mapping of their own.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `optimistic-lock` (optional - defaults to `true`): Specifies that
    updates to this property do or do not require acquisition of the
    optimistic lock. In other words, determines if a version increment
    should occur when this property is dirty.

  - `generated` (optional - defaults to `never`): Specifies that this
    property value is actually generated by the database. See the
    discussion of [Generated Properties](#generated-properties).

  - `lazy` (optional - defaults to `false`): Specifies that this
    property is lazy. A lazy property is not loaded when the object is
    initially loaded, unless the fetch mode has been overridden in a
    specific query. Values for lazy properties are loaded when any lazy
    property of the object is accessed.

*typename* could be:

1.  The name of a NHibernate basic type (eg. `Int32, String, Char, DateTime, Timestamp, Single, Byte[], Object, ...`).

2.  The name of a .NET type with a default basic type (eg.
    `System.Int16, System.Single, System.Char, System.String, System.DateTime, System.Byte[], ...`).

3.  The name of an enumeration type (eg. `Eg.Color, Eg`).

4.  The name of a serializable .NET type.

5.  The class name of a custom type (eg. `Illflow.Type.MyCustomType`).

Note that you have to specify full *assembly-qualified* names for all
except basic NHibernate types (unless you set `assembly` and/or
`namespace` attributes of the `<hibernate-mapping>` element).

NHibernate supports .NET 2.0 `Nullable` types. These types are mostly
treated the same as plain non-`Nullable` types internally. For example,
a property of type `Nullable<Int32>` can be mapped using `type="Int32"`
or `type="System.Int32"`.

If you do not specify a type, NHibernate will use reflection upon the
named property to take a guess at the correct NHibernate type.
NHibernate will try to interpret the name of the return class of the
property getter using rules 2, 3, 4 in that order. However, this is not
always enough. In certain cases you will still need the `type`
attribute. (For example, to distinguish between
`NHibernateUtil.DateTime` and `NHibernateUtil.Timestamp`, or to specify
a custom type.)

See also [NHibernate Types](#nhibernate-types).

The `access` attribute lets you control how NHibernate will access the
value of the property at runtime. The value of the `access` attribute
should be text formatted as `access-strategy.naming-strategy`. The
`.naming-strategy` is not always
required.

| Access Strategy Name | Description                                                                                                                                                                                                                                                                                                                                                                                                                     |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `property`           | The default implementation. NHibernate uses the get/set accessors of the property. No naming strategy should be used with this access strategy because the value of the `name` attribute is the name of the property.                                                                                                                                                                                                           |
| `field`              | NHibernate will access the field directly. NHibernate uses the value of the `name` attribute as the name of the field. This can be used when a property's getter and setter contain extra actions that you don't want to occur when NHibernate is populating or reading the object. If you want the name of the property and not the field to be what the consumers of your API use with HQL, then a naming strategy is needed. |
| `nosetter`           | NHibernate will access the field directly when setting the value and will use the Property when getting the value. This can be used when a property only exposes a get accessor because the consumers of your API can't change the value directly. A naming strategy is required because NHibernate uses the value of the `name` attribute as the property name and needs to be told what the name of the field is.             |
| `ClassName`          | If NHibernate's built in access strategies are not what is needed for your situation then you can build your own by implementing the interface `NHibernate.Property.IPropertyAccessor`. The value of the `access` attribute should be an assembly-qualified name that can be loaded with `Activator.CreateInstance(string assemblyQualifiedName)`.                                                                              |

Access Strategies

| Naming Strategy Name      | Description                                                                                                                                                                         |
| ------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `camelcase`               | The `name` attribute is converted to camel case to find the field. `<property name="FooBar" ... >` uses the field `fooBar`.                                                         |
| `camelcase-underscore`    | The `name` attribute is converted to camel case and prefixed with an underscore to find the field. `<property name="FooBar" ... >` uses the field `_fooBar`.                        |
| `camelcase-m-underscore`  | The `name` attribute is converted to camel case and prefixed with the character `m` and an underscore to find the field. `<property name="FooBar" ... >` uses the field `m_fooBar`. |
| `lowercase`               | The `name` attribute is converted to lower case to find the Field. `<property name="FooBar" ... >` uses the field `foobar`.                                                         |
| `lowercase-underscore`    | The `name` attribute is converted to lower case and prefixed with an underscore to find the Field. `<property name="FooBar" ... >` uses the field `_foobar`.                        |
| `pascalcase-underscore`   | The `name` attribute is prefixed with an underscore to find the field. `<property name="FooBar" ... >` uses the field `_FooBar`.                                                    |
| `pascalcase-m`            | The `name` attribute is prefixed with the character `m` to find the field. `<property name="FooBar" ... >` uses the field `mFooBar`.                                                |
| `pascalcase-m-underscore` | The `name` attribute is prefixed with the character `m` and an underscore to find the field. `<property name="FooBar" ... >` uses the field `m_FooBar`.                             |

Naming Strategies

## many-to-one

An ordinary association to another persistent class is declared using a
`many-to-one` element. The relational model is a many-to-one
association. (It's really just an object reference.)

```xml
    <many-to-one
            name="PropertyName"
            column="column_name"
            class="ClassName"
            cascade="all|none|save-update|delete|delete-orphan|all-delete-orphan"
            fetch="join|select"
            update="true|false"
            insert="true|false"
            property-ref="PropertyNameFromAssociatedClass"
            access="field|property|nosetter|ClassName"
            unique="true|false"
            optimistic-lock="true|false"
            not-found="ignore|exception"
    />
```

  - `name`: The name of the property.

  - `column` (optional): The name of the column.

  - `class` (optional - defaults to the property type determined by
    reflection): The name of the associated class.

  - `cascade` (optional): Specifies which operations should be cascaded
    from the parent object to the associated object.

  - `fetch` (optional - defaults to `select`): Chooses between
    outer-join fetching or sequential select fetching.

  - `update, insert` (optional - defaults to `true`) specifies that the
    mapped columns should be included in SQL `UPDATE` and/or `INSERT`
    statements. Setting both to `false` allows a pure "derived"
    association whose value is initialized from some other property that
    maps to the same column(s) or by a trigger or other application.

  - `property-ref`: (optional) The name of a property of the associated
    class that is joined to this foreign key. If not specified, the
    primary key of the associated class is used.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `unique` (optional): Enable the DDL generation of a unique
    constraint for the foreign-key column.

  - `optimistic-lock` (optional - defaults to `true`): Specifies that
    updates to this property do or do not require acquisition of the
    optimistic lock. In other words, determines if a version increment
    should occur when this property is dirty.

  - `not-found` (optional - defaults to `exception`): Specifies how
    foreign keys that reference missing rows will be handled: `ignore`
    will treat a missing row as a null association.

The `cascade` attribute permits the following values: `all`,
`save-update`, `delete`, `delete-orphan`, `all-delete-orphan` and
`none`. Setting a value other than `none` will propagate certain
operations to the associated (child) object. See "Lifecycle Objects"
below.

The `fetch` attribute accepts two different values:

  - `join` Fetch the association using an outer join

  - `select` Fetch the association using a separate query

A typical `many-to-one` declaration looks as simple as

    <many-to-one name="product" class="Product" column="PRODUCT_ID"/>

The `property-ref` attribute should only be used for mapping legacy data
where a foreign key refers to a unique key of the associated table other
than the primary key. This is an ugly relational model. For example,
suppose the `Product` class had a unique serial number, that is not the
primary key. (The `unique` attribute controls NHibernate's DDL
generation with the SchemaExport
    tool.)

    <property name="serialNumber" unique="true" type="string" column="SERIAL_NUMBER"/>

Then the mapping for `OrderItem` might
    use:

    <many-to-one name="product" property-ref="serialNumber" column="PRODUCT_SERIAL_NUMBER"/>

This is certainly not encouraged, however.

## one-to-one

A one-to-one association to another persistent class is declared using a
`one-to-one` element.

```xml
    <one-to-one
            name="PropertyName"
            class="ClassName"
            cascade="all|none|save-update|delete|delete-orphan|all-delete-orphan"
            constrained="true|false"
            fetch="join|select"
            property-ref="PropertyNameFromAssociatedClass"
            access="field|property|nosetter|ClassName"
    />
```

  - `name`: The name of the property.

  - `class` (optional - defaults to the property type determined by
    reflection): The name of the associated class.

  - `cascade` (optional) specifies which operations should be cascaded
    from the parent object to the associated object.

  - `constrained` (optional) specifies that a foreign key constraint on
    the primary key of the mapped table references the table of the
    associated class. This option affects the order in which `Save()`
    and `Delete()` are cascaded (and is also used by the schema export
    tool).

  - `fetch` (optional - defaults to `select`): Chooses between
    outer-join fetching or sequential select fetching.

  - `property-ref`: (optional) The name of a property of the associated
    class that is joined to the primary key of this class. If not
    specified, the primary key of the associated class is used.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

There are two varieties of one-to-one association:

  - primary key associations

  - unique foreign key associations

Primary key associations don't need an extra table column; if two rows
are related by the association then the two table rows share the same
primary key value. So if you want two objects to be related by a primary
key association, you must make sure that they are assigned the same
identifier value\!

For a primary key association, add the following mappings to `Employee`
and `Person`, respectively.

```xml
    <one-to-one name="Person" class="Person"/>

    <one-to-one name="Employee" class="Employee" constrained="true"/>
```

Now we must ensure that the primary keys of related rows in the PERSON
and EMPLOYEE tables are equal. We use a special NHibernate identifier
generation strategy called `foreign`:

```xml
    <class name="Person" table="PERSON">
        <id name="Id" column="PERSON_ID">
            <generator class="foreign">
                <param name="property">Employee</param>
            </generator>
        </id>
        ...
        <one-to-one name="Employee"
            class="Employee"
            constrained="true"/>
    </class>
```

A newly saved instance of `Person` is then assigned the same primary key
value as the `Employee` instance referred with the `Employee` property
of that `Person`.

Alternatively, a foreign key with a unique constraint, from `Employee`
to `Person`, may be expressed
    as:

```xml
    <many-to-one name="Person" class="Person" column="PERSON_ID" unique="true"/>
```

And this association may be made bidirectional by adding the following
to the `Person` mapping:

```xml
    <one-to-one name="Employee" class="Employee" property-ref="Person"/>
```

## natural-id

```xml
    <natural-id mutable="true|false"/>
            <property ... />
            <many-to-one ... />
            ......
    </natural-id>
```

Even though we recommend the use of surrogate keys as primary keys, you
should still try to identify natural keys for all entities. A natural
key is a property or combination of properties that is unique and
non-null. If it is also immutable, even better. Map the properties of
the natural key inside the `<natural-id>` element. NHibernate will
generate the necessary unique key and nullability constraints, and your
mapping will be more self-documenting.

We strongly recommend that you implement `Equals()` and `GetHashCode()`
to compare the natural key properties of the entity.

This mapping is not intended for use with entities with natural primary
keys.

  - `mutable` (optional, defaults to `false`): By default, natural
    identifier properties as assumed to be immutable (constant).

## component, dynamic-component

The `<component>` element maps properties of a child object to columns
of the table of a parent class. Components may, in turn, declare their
own properties, components or collections. See "Components" below.

```xml
    <component 
            name="PropertyName" 
            class="ClassName"
            insert="true|false"
            upate="true|false"
            access="field|property|nosetter|ClassName"
            optimistic-lock="true|false">
    
            <property ...../>
            <many-to-one .... />
            ........
    </component>
```

  - `name`: The name of the property.

  - `class` (optional - defaults to the property type determined by
    reflection): The name of the component (child) class.

  - `insert`: Do the mapped columns appear in SQL `INSERT`s?

  - `update`: Do the mapped columns appear in SQL `UPDATE`s?

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `optimistic-lock` (optional - defaults to `true`): Specifies that
    updates to this component do or do not require acquisition of the
    optimistic lock. In other words, determines if a version increment
    should occur when this property is dirty.

The child `<property>` tags map properties of the child class to table
columns.

The `<component>` element allows a `<parent>` sub-element that maps a
property of the component class as a reference back to the containing
entity.

The `<dynamic-component>` element allows an `IDictionary` to be mapped
as a component, where the property names refer to keys of the
dictionary.

## properties

The `<properties>` element allows the definition of a named, logical
grouping of the properties of a class. The most important use of the
construct is that it allows a combination of properties to be the target
of a `property-ref`. It is also a convenient way to define a
multi-column unique constraint. For example:

```xml
    <properties
          name="logicalName"
          insert="true|false"
          update="true|false"
          optimistic-lock="true|false"
          unique="true|false">
    
          <property .../>
          <many-to-one .../>
          ........
    </properties>
```

  - `name`: the logical name of the grouping. It is *not* an actual
    property name.

  - `insert`: do the mapped columns appear in SQL `INSERTs`?

  - `update`: do the mapped columns appear in SQL `UPDATEs`?

  - `optimistic-lock` (optional - defaults to `true`): specifies that
    updates to these properties either do or do not require acquisition
    of the optimistic lock. It determines if a version increment should
    occur when these properties are dirty.

  - `unique` (optional - defaults to `false`): specifies that a unique
    constraint exists upon all mapped columns of the component.

For example, if we have the following `<properties>` mapping:

```xml
    <class name="Person">
          <id name="personNumber" />
          <properties name="name" unique="true" update="false">
              <property name="firstName" />
              <property name="lastName" />
              <property name="initial" />
          </properties>
    </class>
```

You might have some legacy data association that refers to this unique
key of the `Person` table, instead of to the primary key:

```xml
    <many-to-one name="owner" class="Person" property-ref="name">
            <column name="firstName" />
            <column name="lastName" />
            <column name="initial" />
    </many-to-one>
```

The use of this outside the context of mapping legacy data is not
recommended.

## subclass

Finally, polymorphic persistence requires the declaration of each
subclass of the root persistent class. For the (recommended)
table-per-class-hierarchy mapping strategy, the `<subclass>` declaration
is used.

```xml
    <subclass
            name="ClassName"
            discriminator-value="discriminator_value"
            proxy="ProxyInterface"
            lazy="true|false"
            dynamic-update="true|false"
            dynamic-insert="true|false">
    
            <property .... />
            <properties .... />
            .....
    </subclass>
```

  - `name`: The fully qualified .NET class name of the subclass,
    including its assembly name.

  - `discriminator-value` (optional - defaults to the class name): A
    value that distinguishes individual subclasses.

  - `proxy` (optional): Specifies a class or interface to use for lazy
    initializing proxies.

  - `lazy` (optional, defaults to `true`): Setting `lazy="false"`
    disables the use of lazy fetching.

Each subclass should declare its own persistent properties and
subclasses. `<version>` and `<id>` properties are assumed to be
inherited from the root class. Each subclass in a hierarchy must define
a unique `discriminator-value`. If none is specified, the fully
qualified .NET class name is used.

For information about inheritance mappings, see [Inheritance Mapping](inheritance_mapping.md).

## joined-subclass

Alternatively, a subclass that is persisted to its own table
(table-per-subclass mapping strategy) is declared using a
`<joined-subclass>` element.

```xml
    <joined-subclass
            name="ClassName"
            proxy="ProxyInterface"
            lazy="true|false"
            dynamic-update="true|false"
            dynamic-insert="true|false">
    
            <key .... >
    
            <property .... />
            <properties .... />
            .....
    </joined-subclass>
```

  - `name`: The fully qualified class name of the subclass.

  - `proxy` (optional): Specifies a class or interface to use for lazy
    initializing proxies.

  - `lazy` (optional): Setting `lazy="true"` is a shortcut equivalent to
    specifying the name of the class itself as the `proxy` interface.

No discriminator column is required for this mapping strategy. Each
subclass must, however, declare a table column holding the object
identifier using the `<key>` element. The mapping at the start of the
chapter would be re-written as:

```xml
    <?xml version="1.0"?>
    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2" assembly="Eg"
        namespace="Eg">
    
            <class name="Cat" table="CATS">
                    <id name="Id" column="uid" type="Int64">
                            <generator class="hilo"/>
                    </id>
                    <property name="BirthDate" type="Date"/>
                    <property name="Color" not-null="true"/>
                    <property name="Sex" not-null="true"/>
                    <property name="Weight"/>
                    <many-to-one name="Mate"/>
                    <set name="Kittens">
                            <key column="MOTHER"/>
                            <one-to-many class="Cat"/>
                    </set>
                    <joined-subclass name="DomesticCat" table="DOMESTIC_CATS">
                        <key column="CAT"/>
                            <property name="Name" type="String"/>
                    </joined-subclass>
            </class>
    
            <class name="Dog">
                    <!-- mapping for Dog could go here -->
            </class>
    
    </hibernate-mapping>
```

For information about inheritance mappings, see [Inheritance Mapping](inheritance_mapping.md).

## union-subclass

A third option is to map only the concrete classes of an inheritance
hierarchy to tables, (the table-per-concrete-class strategy) where each
table defines all persistent state of the class, including inherited
state. In NHibernate, it is not absolutely necessary to explicitly map
such inheritance hierarchies. You can simply map each class with a
separate `<class>` declaration. However, if you wish use polymorphic
associations (e.g. an association to the superclass of your hierarchy),
you need to use the `<union-subclass>` mapping.

```xml
    <union-subclass
            name="ClassName"
            table="tablename"
            proxy="ProxyInterface"
            lazy="true|false"
            dynamic-update="true|false"
            dynamic-insert="true|false"
            schema="schema"
            catalog="catalog"
            extends="SuperclassName"
            abstract="true|false"
            persister="ClassName"
            subselect="SQL expression"
            entity-name="EntityName"
            node="element-name">
    
            <property .... />
            <properties .... />
            .....
    </union-subclass>
```

  - `name`: The fully qualified class name of the subclass.

  - `table`: The name of the subclass table.

  - `proxy` (optional): Specifies a class or interface to use for lazy
    initializing proxies.

  - `lazy` (optional, defaults to `true`): Setting `lazy="false"`
    disables the use of lazy fetching.

No discriminator column or key column is required for this mapping
strategy.

For information about inheritance mappings, see [Inheritance Mapping](inheritance_mapping.md).

## join

Using the `<join>` element, it is possible to map properties of one
class to several tables, when there's a 1-to-1 relationship between the
tables.

```xml
    <join
            table="tablename"
            schema="owner"
            fetch="join|select"
            inverse="true|false"
            optional="true|false">
    
            <key ... />
    
            <property ... />
            ...
    </join>
```

  - `table`: The name of the joined table.

  - `schema` (optional): Override the schema name specified by the root
    `<hibernate-mapping>` element.

  - `fetch` (optional - defaults to `join`): If set to `join`, the
    default, NHibernate will use an inner join to retrieve a `<join>`
    defined by a class or its superclasses and an outer join for a
    `<join>` defined by a subclass. If set to `select` then NHibernate
    will use a sequential select for a `<join>` defined on a subclass,
    which will be issued only if a row turns out to represent an
    instance of the subclass. Inner joins will still be used to retrieve
    a `<join>` defined by the class and its superclasses.

  - `inverse` (optional - defaults to `false`): If enabled, NHibernate
    will not try to insert or update the properties defined by this
    join.

  - `optional` (optional - defaults to `false`): If enabled, NHibernate
    will insert a row only if the properties defined by this join are
    non-null and will always use an outer join to retrieve the
    properties.

For example, the address information for a person can be mapped to a
separate table (while preserving value type semantics for all
properties):

```xml
    <class name="Person"
        table="PERSON">
    
        <id name="id" column="PERSON_ID">...</id>
    
        <join table="ADDRESS">
            <key column="ADDRESS_ID"/>
            <property name="address"/>
            <property name="zip"/>
            <property name="country"/>
        </join>
        ...
```

This feature is often only useful for legacy data models, we recommend
fewer tables than classes and a fine-grained domain model. However, it
is useful for switching between inheritance mapping strategies in a
single hierarchy, as explained later.

## map, set, list, bag

Collections are discussed later.

## import

Suppose your application has two persistent classes with the same name,
and you don't want to specify the fully qualified name in NHibernate
queries. Classes may be "imported" explicitly, rather than relying upon
`auto-import="true"`. You may even import classes and interfaces that
are not explicitly mapped.

```xml
    <import class="System.Object" rename="Universe"/>

    <import
            class="ClassName"
            rename="ShortName"
    />
```

  - `class`: The fully qualified class name of any .NET class, including
    its assembly name.

  - `rename` (optional - defaults to the unqualified class name): A name
    that may be used in the query language.

# NHibernate Types

## Entities and values

To understand the behaviour of various .NET language-level objects with
respect to the persistence service, we need to classify them into two
groups:

An *entity* exists independently of any other objects holding references
to the entity. Contrast this with the usual .NET model where an
unreferenced object is garbage collected. Entities must be explicitly
saved and deleted (except that saves and deletions may be *cascaded*
from a parent entity to its children). This is different from the ODMG
model of object persistence by reachability - and corresponds more
closely to how application objects are usually used in large systems.
Entities support circular and shared references. They may also be
versioned.

An entity's persistent state consists of references to other entities
and instances of *value* types. Values are primitives, collections,
components and certain immutable objects. Unlike entities, values (in
particular collections and components) *are* persisted and deleted by
reachability. Since value objects (and primitives) are persisted and
deleted along with their containing entity they may not be independently
versioned. Values have no independent identity, so they cannot be shared
by two entities or collections.

All NHibernate types except collections support null semantics if the
.NET type is nullable (i.e. not derived from `System.ValueType`).

Up until now, we've been using the term "persistent class" to refer to
entities. We will continue to do that. Strictly speaking, however, not
all user-defined classes with persistent state are entities. A
*component* is a user defined class with value semantics.

## Basic value types

The *basic types* may be roughly categorized into three groups -
`System.ValueType` types, `System.Object` types, and `System.Object`
types for large objects. Just like Columns for System.ValueType types
can handle `null` values only if the entity property is properly typed
with a `Nullable<T>`. Otherwise `null` will be replaced by the default
value for the type when reading, and then will be overwritten by it when
persisting the entity, potentially leading to phantom
updates.

| NHibernate Type     | .NET Type               | Database Type                                             | Remarks                                                                                                                                                                                                                                        |
| ------------------- | ----------------------- | --------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `AnsiChar`          | `System.Char`           | `DbType.AnsiStringFixedLength - 1 char`                   | `type="AnsiChar"` must be specified.                                                                                                                                                                                                           |
| `Boolean`           | `System.Boolean`        | `DbType.Boolean`                                          | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Byte`              | `System.Byte`           | `DbType.Byte`                                             | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Char`              | `System.Char`           | `DbType.StringFixedLength - 1 char`                       | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Currency`          | `System.Decimal`        | `DbType.Currency`                                         | `type="Currency"` must be specified.                                                                                                                                                                                                           |
| `Date`              | `System.DateTime`       | `DbType.Date`                                             | `type="Date"` must be specified.                                                                                                                                                                                                               |
| `DateTime`          | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | Default when no `type` attribute specified. Does no longer ignore fractional seconds since NHibernate v5.0.                                                                                                                                    |
| `DateTimeNoMs`      | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | `type="DateTimeNoMs"` must be specified. Ignores fractional seconds. Available since NHibernate v5.0.                                                                                                                                          |
| `DateTime2`         | `System.DateTime`       | `DbType.DateTime2`                                        | `type="DateTime2"` must be specified. Obsolete since NHibernate v5.0, use `DateTime` instead.                                                                                                                                                  |
| `DateTimeOffset`    | `System.DateTimeOffset` | `DbType.DateTimeOffset`                                   | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `DbTimestamp`       | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | `type="DbTimestamp"` must be specified. When used as a `version` field, uses the database's current time retrieved in dedicated queries, rather than the client's current time.                                                                |
| `Decimal`           | `System.Decimal`        | `DbType.Decimal`                                          | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Double`            | `System.Double`         | `DbType.Double`                                           | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Guid`              | `System.Guid`           | `DbType.Guid`                                             | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Int16`             | `System.Int16`          | `DbType.Int16`                                            | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Int32`             | `System.Int32`          | `DbType.Int32`                                            | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Int64`             | `System.Int64`          | `DbType.Int64`                                            | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `LocalDateTime`     | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | `type="LocalDateTime"` must be specified. Ensures the `DateTimeKind` is set to `DateTimeKind.Local`. Throws if set with a date having another kind. Does no longer ignore fractional seconds since NHibernate v5.0.                            |
| `LocalDateTimeNoMs` | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | `type="LocalDateTimeNoMs"` must be specified. Similar to `LocalDateTime` but ignores fractional seconds. Available since NHibernate v5.0.                                                                                                      |
| `PersistentEnum`    | A `System.Enum`         | The `DbType` for the underlying value.                    | Do not specify `type="PersistentEnum"` in the mapping. Instead specify the Assembly Qualified Name of the Enum or let NHibernate use Reflection to "guess" the Type. The UnderlyingType of the Enum is used to determine the correct `DbType`. |
| `SByte`             | `System.SByte`          | `DbType.SByte`                                            | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Single`            | `System.Single`         | `DbType.Single`                                           | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Ticks`             | `System.DateTime`       | `DbType.Int64`                                            | `type="Ticks"` must be specified.                                                                                                                                                                                                              |
| `Time`              | `System.DateTime`       | `DbType.Time`                                             | `type="Time"` must be specified.                                                                                                                                                                                                               |
| `TimeAsTimeSpan`    | `System.TimeSpan`       | `DbType.Time`                                             | `type="TimeAsTimeSpan"` must be specified.                                                                                                                                                                                                     |
| `TimeSpan`          | `System.TimeSpan`       | `DbType.Int64`                                            | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `Timestamp`         | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | Obsolete, its `Timestamp` alias will be remapped to `DateTime` in a future version.                                                                                                                                                            |
| `TrueFalse`         | `System.Boolean`        | `DbType.AnsiStringFixedLength` - 1 char either 'T' or 'F' | `type="TrueFalse"` must be specified.                                                                                                                                                                                                          |
| `UInt16`            | `System.UInt16`         | `DbType.UInt16`                                           | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `UInt32`            | `System.UInt32`         | `DbType.UInt32`                                           | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `UInt64`            | `System.UInt64`         | `DbType.UInt64`                                           | Default when no `type` attribute specified.                                                                                                                                                                                                    |
| `UtcDateTime`       | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | Ensures the `DateTimeKind` is set to `DateTimeKind.Utc`. Throws if set with a date having another kind. Does no longer ignore fractional seconds since NHibernate v5.0.                                                                        |
| `UtcDateTimeNoMs`   | `System.DateTime`       | `DbType.DateTime` / `DbType.DateTime2`                    | `type="UtcDateTimeNoMs"` must be specified. Similar to `UtcDateTime` but ignores fractional seconds. Available since NHibernate v5.0.                                                                                                          |
| `YesNo`             | `System.Boolean`        | `DbType.AnsiStringFixedLength` - 1 char either 'Y' or 'N' | `type="YesNo"` must be specified.                                                                                                                                                                                                              |

System.ValueType Mapping Types

  - Since NHibernate v5.0 and if the dialect supports it,
    `DbType.DateTime2` is used instead of `DbType.DateTime`. This may be
    disabled by setting `sql_types.keep_datetime` to
`true`.

| NHibernate Type | .NET Type                          | Database Type                                    | Remarks                                     |
| --------------- | ---------------------------------- | ------------------------------------------------ | ------------------------------------------- |
| `AnsiString`    | `System.String`                    | `DbType.AnsiString`                              | `type="AnsiString"` must be specified.      |
| `CultureInfo`   | `System.Globalization.CultureInfo` | `DbType.String` - 5 chars for culture            | Default when no `type` attribute specified. |
| `Binary`        | `System.Byte[]`                    | `DbType.Binary`                                  | Default when no `type` attribute specified. |
| `Type`          | `System.Type`                      | `DbType.String` holding Assembly Qualified Name. | Default when no `type` attribute specified. |
| `String`        | `System.String`                    | `DbType.String`                                  | Default when no `type` attribute specified. |
| `Uri`           | `System.Uri`                       | `DbType.String`                                  | Default when no `type` attribute specified. |

System.Object Mapping
Types

| NHibernate Type | .NET Type                                                      | Database Type   | Remarks                                                                                                                   |
| --------------- | -------------------------------------------------------------- | --------------- | ------------------------------------------------------------------------------------------------------------------------- |
| `StringClob`    | `System.String`                                                | `DbType.String` | `type="StringClob"` must be specified. Entire field is read into memory.                                                  |
| `BinaryBlob`    | `System.Byte[]`                                                | `DbType.Binary` | `type="BinaryBlob"` must be specified. Entire field is read into memory.                                                  |
| `Serializable`  | Any `System.Object` that is marked with SerializableAttribute. | `DbType.Binary` | `type="Serializable"` should be specified. This is the fallback type if no NHibernate Type can be found for the Property. |
| `XDoc`          | `System.Xml.Linq.XDocument`                                    | `DbType.Xml`    | Default when no `type` attribute specified. Entire field is read into memory.                                             |
| `XmlDoc`        | `System.Xml.XmlDocument`                                       | `DbType.Xml`    | Default when no `type` attribute specified. Entire field is read into memory.                                             |

Large Object Mapping Types

NHibernate supports some additional type names for compatibility with
Java's Hibernate (useful for those coming over from Hibernate or using
some of the tools to generate `hbm.xml` files). A `type="integer"` or
`type="int"` will map to an `Int32` NHibernate type, `type="short"` to
an `Int16` NHibernateType. To see all of the conversions you can view
the source of static constructor of the class
`NHibernate.Type.TypeFactory`.

Default NHibernate types used when no `type` attribute is specified can
be overridden by using the `NHibernate.Type.TypeFactory.RegisterType`
static method before configuring and building session factories.

## Custom value types

It is relatively easy for developers to create their own value types.
For example, you might want to persist properties of type `Int64` to
`VARCHAR` columns. NHibernate does not provide a built-in type for this.
But custom types are not limited to mapping a property (or collection
element) to a single table column. So, for example, you might have a
property `Name { get; set; }` of type `String` that is persisted to the
columns `FIRST_NAME`, `INITIAL`, `SURNAME`.

To implement a custom type, implement either
`NHibernate.UserTypes.IUserType` or
`NHibernate.UserTypes.ICompositeUserType` and declare properties using
the fully qualified name of the type. Check out
`NHibernate.DomainModel.DoubleStringType` to see the kind of things that
are possible.

```xml
    <property name="TwoStrings"
        type="NHibernate.DomainModel.DoubleStringType, NHibernate.DomainModel">
      <column name="first_string"/>
      <column name="second_string"/>
    </property>
```

Notice the use of `<column>` tags to map a property to multiple columns.

The `ICompositeUserType`, `IEnhancedUserType`, `INullableUserType`,
`IUserCollectionType`, and `IUserVersionType` interfaces provide support
for more specialized uses.

You may even supply parameters to an `IUserType` in the mapping file. To
do this, your `IUserType` must implement the
`NHibernate.UserTypes.IParameterizedType` interface. To supply
parameters to your custom type, you can use the `<type>` element in your
mapping files.

```xml
    <property name="priority">
        <type name="MyCompany.UserTypes.DefaultValueIntegerType">
            <param name="default">0</param>
        </type>
    </property>
```

The `IUserType` can now retrieve the value for the parameter named
`default` from the `IDictionary` object passed to it.

If you use a certain `UserType` very often, it may be useful to define a
shorter name for it. You can do this using the `<typedef>` element.
Typedefs assign a name to a custom type, and may also contain a list of
default parameter values if the type is
    parameterized.

```xml
    <typedef class="MyCompany.UserTypes.DefaultValueIntegerType" name="default_zero">
        <param name="default">0</param>
    </typedef>

    <property name="priority" type="default_zero"/>
```

It is also possible to override the parameters supplied in a typedef on
a case-by-case basis by using type parameters on the property mapping.

Even though NHibernate's rich range of built-in types and support for
components means you will very rarely *need* to use a custom type, it is
nevertheless considered good form to use custom types for (non-entity)
classes that occur frequently in your application. For example, a
`MonetaryAmount` class is a good candidate for an `ICompositeUserType`,
even though it could easily be mapped as a component. One motivation for
this is abstraction. With a custom type, your mapping documents would be
future-proofed against possible changes in your way of representing
monetary values.

## Any type mappings

There is one further type of property mapping. The `<any>` mapping
element defines a polymorphic association to classes from multiple
tables. This type of mapping always requires more than one column. The
first column holds the type of the associated entity. The remaining
columns hold the identifier. It is impossible to specify a foreign key
constraint for this kind of association, so this is most certainly not
meant as the usual way of mapping (polymorphic) associations. You should
use this only in very special cases (eg. audit logs, user session data,
etc).

```xml
    <any name="AnyEntity" id-type="Int64" meta-type="Eg.Custom.Class2TablenameType">
        <column name="table_name"/>
        <column name="id"/>
    </any>
```

The `meta-type` attribute lets the application specify a custom type
that maps database column values to persistent classes which have
identifier properties of the type specified by `id-type`. If the
meta-type returns instances of `System.Type`, nothing else is required.
On the other hand, if it is a basic type like `String` or `Char`, you
must specify the mapping from values to classes.

```xml
    <any name="AnyEntity" id-type="Int64" meta-type="String">
        <meta-value value="TBL_ANIMAL" class="Animal"/>
        <meta-value value="TBL_HUMAN" class="Human"/>
        <meta-value value="TBL_ALIEN" class="Alien"/>
        <column name="table_name"/>
        <column name="id"/>
    </any>

    <any
            name="PropertyName"
            id-type="idtypename"
            meta-type="metatypename"
            cascade="none|all|save-update"
            access="field|property|nosetter|ClassName"
            optimistic-lock="true|false"
    >
            <meta-value ... />
            <meta-value ... />
            .....
            <column .... />
            <column .... />
            .....
    </any>
```

  - `name`: the property name.

  - `id-type`: the identifier type.

  - `meta-type` (optional - defaults to `Type`): a type that maps
    `System.Type` to a single database column or, alternatively, a type
    that is allowed for a discriminator mapping.

  - `cascade` (optional - defaults to `none`): the cascade style.

  - `access` (optional - defaults to `property`): The strategy
    NHibernate should use for accessing the property value.

  - `optimistic-lock` (optional - defaults to `true`): Specifies that
    updates to this property do or do not require acquisition of the
    optimistic lock. In other words, define if a version increment
    should occur if this property is dirty.

# SQL quoted identifiers

You may force NHibernate to quote an identifier in the generated SQL by
enclosing the table or column name in back-ticks in the mapping
document. NHibernate will use the correct quotation style for the SQL
`Dialect` (usually double quotes, but brackets for SQL Server and
back-ticks for MySQL).

```xml
    <class name="LineItem" table="`Line Item`">
        <id name="Id" column="`Item Id`"/><generator class="assigned"/></id>
        <property name="ItemNumber" column="`Item #`"/>
        ...
    </class>
```

Quoting column identifiers is required if a table contains two columns
differing only by case. Ensure you use consistent casing when quoting
identifiers.

# Modular mapping files

It is possible to define `subclass` and `joined-subclass` mappings in
separate mapping documents, directly beneath `hibernate-mapping`. This
allows you to extend a class hierarchy just by adding a new mapping
file. You must specify an `extends` attribute in the subclass mapping,
naming a previously mapped superclass. Use of this feature makes the
ordering of the mapping documents important\!

```xml
    <hibernate-mapping>
            <subclass name="Eg.Subclass.DomesticCat, Eg"
                extends="Eg.Cat, Eg" discriminator-value="D">
                 <property name="name" type="string"/>
            </subclass>
    </hibernate-mapping>
```

# Generated Properties

Generated properties are properties which have their values generated by
the database. Typically, NHibernate applications needed to `Refresh`
objects which contain any properties for which the database was
generating values. Marking properties as generated, however, lets the
application delegate this responsibility to NHibernate. Essentially,
whenever NHibernate issues an SQL INSERT or UPDATE for an entity which
has defined generated properties, it immediately issues a select
afterwards to retrieve the generated values.

Properties marked as generated must additionally be non-insertable and
non-updatable. Only [version (optional)](#version-optional),
[timestamp (optional)](#timestamp-optional), and
[property](#property) can be marked as generated.

`never` (the default) - means that the given property value is not
generated within the database.

`insert` - states that the given property value is generated on insert,
but is not regenerated on subsequent updates. Things like created-date
would fall into this category. Note that even though [version
(optional)](#version-optional) and [timestamp
(optional)](#timestamp-optional) properties can be marked as
generated, this option is not available there...

`always` - states that the property value is generated both on insert
and on update.

# Auxiliary Database Objects

Allows CREATE and DROP of arbitrary database objects, in conjunction
with NHibernate's schema evolution tools, to provide the ability to
fully define a user schema within the NHibernate mapping files. Although
designed specifically for creating and dropping things like triggers or
stored procedures, really any SQL command that can be run via a
`DbCommand.ExecuteNonQuery()` method is valid here (ALTERs, INSERTS,
etc). There are essentially two modes for defining auxiliary database
objects.

The first mode is to explicitly list the CREATE and DROP commands out in
the mapping file:

```xml
    <nhibernate-mapping>
        ...
        <database-object>
            <create>CREATE TRIGGER my_trigger ...</create>
            <drop>DROP TRIGGER my_trigger</drop>
        </database-object>
    </nhibernate-mapping>
```

The second mode is to supply a custom class which knows how to construct
the CREATE and DROP commands. This custom class must implement the
`NHibernate.Mapping.IAuxiliaryDatabaseObject` interface.

```xml
    <hibernate-mapping>
        ...
        <database-object>
            <definition class="MyTriggerDefinition, MyAssembly"/>
        </database-object>
    </hibernate-mapping>
```

You may also specify parameters to be passed to the database object:

```xml
    <hibernate-mapping>
        ...
        <database-object>
            <definition class="MyTriggerDefinition, MyAssembly">
                <param name="parameterName">parameterValue</param>
            </definition>
        </database-object>
    </hibernate-mapping>
```

NHibernate will call `IAuxiliaryDatabaseObject.SetParameterValues`
passing it a dictionary of parameter names and values.

Additionally, these database objects can be optionally scoped such that
they only apply when certain dialects are used.

```xml
    <hibernate-mapping>
        ...
        <database-object>
            <definition class="MyTriggerDefinition"/>
            <dialect-scope name="NHibernate.Dialect.Oracle9iDialect"/>
            <dialect-scope name="NHibernate.Dialect.Oracle8iDialect"/>
        </database-object>
    </hibernate-mapping>
```
