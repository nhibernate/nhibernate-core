# Toolset Guide

Roundtrip engineering with NHibernate is possible using a set of
commandline tools maintained as part of the NHibernate project, along
with NHibernate support built into various code generation tools
(MyGeneration, CodeSmith, ObjectMapper, AndroMDA).

The NHibernate main package comes bundled with the most important tool
(it can even be used from "inside" NHibernate on-the-fly):

  - DDL schema generation from a mapping file (aka `SchemaExport`,
    `hbm2ddl`)

Other tools directly provided by the NHibernate project are delivered
with a separate package, *NHibernateContrib*. This package includes
tools for the following tasks:

  - mapping file generation from .NET classes marked with attributes
    (`NHibernate.Mapping.Attributes`, or NHMA for short)

Third party tools with NHibernate support are:

  - CodeSmith, MyGeneration, and ObjectMapper (mapping file generation
    from an existing database schema)

  - AndroMDA (MDA (Model-Driven Architecture) approach generating code
    for persistent classes from UML diagrams and their XML/XMI
    representation)

These 3rd party tools are not documented in this reference. Please refer
to the NHibernate website for up-to-date information.

# Schema Generation

The generated schema includes referential integrity constraints (primary
and foreign keys) for entity and collection tables. Tables and sequences
are also created for mapped identifier generators.

You *must* specify a SQL `Dialect` via the `hibernate.dialect` property
when using this tool.

## Customizing the schema

Many NHibernate mapping elements define an optional attribute named
`length`. You may set the length of a column with this attribute. (Or,
for numeric/decimal data types, the precision.)

Some tags also accept a `not-null` attribute (for generating a `NOT
NULL` constraint on table columns) and a `unique` attribute (for
generating `UNIQUE` constraint on table columns).

Some tags accept an `index` attribute for specifying the name of an
index for that column. A `unique-key` attribute can be used to group
columns in a single unit key constraint. Currently, the specified value
of the `unique-key` attribute is *not* used to name the constraint, only
to group the columns in the mapping file.

Examples:

    <property name="Foo" type="String" length="64" not-null="true"/>
    
    <many-to-one name="Bar" foreign-key="fk_foo_bar" not-null="true"/>
    
    <element column="serial_number" type="Int64" not-null="true" unique="true"/>

Alternatively, these elements also accept a child `<column>` element.
This is particularly useful for multi-column types:

    <property name="Foo" type="String">
        <column name="foo" length="64" not-null="true" sql-type="text"/>
    </property>
    
    <property name="Bar" type="My.CustomTypes.MultiColumnType, My.CustomTypes"/>
        <column name="fee" not-null="true" index="bar_idx"/>
        <column name="fi" not-null="true" index="bar_idx"/>
        <column name="fo" not-null="true" index="bar_idx"/>
    </property>

The `sql-type` attribute allows the user to override the default mapping
of NHibernate type to SQL data type.

The `check` attribute allows you to specify a check constraint.

    <property name="Foo" type="Int32">
        <column name="foo" check="foo > 10"/>
    </property>
    
    <class name="Foo" table="foos" check="bar < 100.0">
        ...
        <property name="Bar" type="Single"/>
    </class>

| Attribute     | Values             | Interpretation                                                                                                                                                                                                                                       |
| ------------- | ------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `length`      | number             | column length/decimal precision                                                                                                                                                                                                                      |
| `not-null`    | `true\|false`      | specifies that the column should be non-nullable                                                                                                                                                                                                     |
| `unique`      | `true\|false`      | specifies that the column should have a unique constraint                                                                                                                                                                                            |
| `index`       | `index_name`       | specifies the name of a (multi-column) index                                                                                                                                                                                                         |
| `unique-key`  | `unique_key_name`  | specifies the name of a multi-column unique constraint                                                                                                                                                                                               |
| `foreign-key` | `foreign_key_name` | specifies the name of the foreign key constraint generated for an association, use it on \<one-to-one\>, \<many-to-one\>, \<key\>, and \<many-to-many\> mapping elements. Note that `inverse="true"` sides will not be considered by `SchemaExport`. |
| `sql-type`    | `column_type`      | overrides the default column type (attribute of `<column>` element only)                                                                                                                                                                             |
| `check`       | SQL expression     | create an SQL check constraint on either column or table                                                                                                                                                                                             |

Summary

## Running the tool

The `SchemaExport` tool writes a DDL script to standard out and/or
executes the DDL statements.

You may embed `SchemaExport` in your application:

    Configuration cfg = ....;
    new SchemaExport(cfg).Create(false, true);
