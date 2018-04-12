# Preface

Working with object-oriented software and a relational database can be
cumbersome and time consuming in today's enterprise environments.
NHibernate is an object/relational mapping tool for .NET environments.
The term object/relational mapping (*ORM*) refers to the technique of
mapping a data representation from an object model to a relational data
model with a SQL-based schema.

NHibernate not only takes care of the mapping from .NET classes to
database tables (and from .NET data types to SQL data types), but also
provides data query and retrieval facilities and can significantly
reduce development time otherwise spent with manual data handling in SQL
and `ADO.NET`.

NHibernate's goal is to relieve the developer from 95 percent of common
data persistence related programming tasks. NHibernate may not be the
best solution for data-centric applications that only use
stored-procedures to implement the business logic in the database, it is
most useful with object-oriented domain models and business logic in the
.NET-based middle-tier. However, NHibernate can certainly help you to
remove or encapsulate vendor-specific SQL code and will help with the
common task of result set translation from a tabular representation to a
graph of objects.

If you are new to NHibernate and Object/Relational Mapping or even .NET
Framework, please follow these steps:

1.  Read [Quick-start with IIS and Microsoft SQL Server](quickstart.md) for a 30 minute tutorial, using Internet
    Information Services (IIS) web server.

2.  Read [Architecture](architecture.md) to understand the environments where
    NHibernate can be used.

3.  Use this reference documentation as your primary source of
    information. Consider reading *[Hibernate in Action](https://www.manning.com/books/hibernate-in-action)* (java)
    or *[NHibernate in Action](https://www.manning.com/books/nhibernate-in-action)* or
    *[NHibernate 4.x Cookbook - Second Edition](https://www.packtpub.com/application-development/nhibernate-40-cookbook)*
    or *[NHibernate 2 Beginner's Guide](https://www.packtpub.com/application-development/nhibernate-2-beginners-guide)*
    if you need more help with application design or if you prefer a
    step-by-step tutorial. Also visit http://nhibernate.sourceforge.net/NHibernateEg/ for NHibernate
    tutorial with examples.

4.  FAQs are answered on the [NHibernate users group](https://groups.google.com/forum/#!forum/nhusers).

5.  The Community Area on the [NHibernate website](http://nhibernate.info/) is a good source for design
    patterns and various integration solutions (ASP.NET, Windows Forms).

If you have questions, use the [NHibernate user forum](https://groups.google.com/forum/#!forum/nhusers). We also provide
a [GitHub issue tracking system](https://github.com/nhibernate/nhibernate-core/issues) for bug
reports and feature requests. If you are interested in the development
of NHibernate, join the developer mailing list. If you are interested in
translating this documentation into your language, contact us on the
[developer mailing list](https://groups.google.com/forum/#!forum/nhibernate-development).
