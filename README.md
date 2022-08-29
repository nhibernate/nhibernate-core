Welcome to NHibernate
=====================

NHibernate is a mature, open source object-relational mapper for the .NET framework. It is actively developed,
fully featured and used in thousands of successful projects.

The NHibernate community website - <https://nhibernate.info> - has a range of resources to help you get started,
including [howtos][A1], [blogs][A2] and [reference documentation][A3].

[A1]: https://nhibernate.info/doc/
[A2]: https://nhibernate.info/blog/
[A3]: https://nhibernate.info/doc/nh/en/index.html

Latest Release Version
--------------

The quickest way to get the latest release of NHibernate is to add it to your project using 
NuGet (<https://nuget.org/List/Packages/NHibernate>).

Alternatively binaries are available from SourceForge at <http://sourceforge.net/projects/nhibernate>.

You are encouraged to review the release notes ([releasenotes.txt](releasenotes.txt)), particularly when upgrading to a 
later version. The release notes will generally document any breaking changes.

Nightly Development Builds
--------------------------

The quickest way to get the latest development build of NHibernate is to add it to your project using 
NuGet from MyGet feed (<https://www.myget.org/gallery/nhibernate>).

In order to make life a little bit easier you can register the package source in the NuGet.Config
file in the top folder of your project, similar to the following.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="NHibernateDevBuilds" value="https://www.myget.org/F/nhibernate/api/v3/index.json" />
  </packageSources>
</configuration>
```

Community Forums
----------------

There are two official NHibernate community forums:

* [NHibernate Users][B1] - a forum for users to find help using NHibernate
* [NHibernate Development][B2] - a forum for the developers of NHibernate

[B1]: http://groups.google.com/group/nhusers
[B2]: http://groups.google.com/group/nhibernate-development

Bug Reports
-----------

If you find any bugs, please report them using the [GitHub issue tracker][C1]. A
test-case that demonstrates the issue is usually required. Instructions on providing a test-case
can be found in [contributing guidelines][C3] or [here][C2].

[C1]: https://github.com/nhibernate/nhibernate-core/issues
[C2]: https://nhibernate.info/blog/2008/10/04/the-best-way-to-solve-nhibernate-bugs-submit-good-unit-test.html
[C3]: CONTRIBUTING.md

Licenses
--------

- This software is distributed under the terms of the Free Software Foundation [Lesser GNU Public License (LGPL), version 2.1][D1] (see [LICENSE.txt][D2]).
- The documentation for this software is distributed under the terms of the Free Software Foundation [GNU Free Documentation License (GNU FDL), version 1.1][D3] (see [doc/LICENSE.txt][D4]).

[D1]: http://www.gnu.org/licenses/lgpl-2.1-standalone.html
[D2]: LICENSE.txt
[D3]: http://www.gnu.org/licenses/old-licenses/fdl-1.1-standalone.html
[D4]: doc/LICENSE.txt

Credits
-------

Many thanks to the following individuals, organisations and projects whose work is so important to the success
of NHibernate (in no particular order):

* [NUnit][] - unit-testing
* [Nant][] - build automation
* [CodeBetter][] - [TeamCity][] continuous integration and build management server hosting
* [GitHub][] and [SourceForge][] - source code hosting
* [Atlassian][] - JIRA bug tracker licence and hosting
* [Log4net][] - logging, by the [Apache Software Foundation][]
* [JetBrains][] - [ReSharper][] licences for NHibernate developers 
* [LinFu][] - proxy implementation (Philip Laureano)
* Iesi.Collections - source code taken from an [article][] written by Jason Smith
* [Relinq][] - Linq provider for NHibernate
* [AsyncGenerator][] - Roslyn based async C# code generator by @maca88


[NUnit]: http://www.nunit.org
[Nant]: http://nant.sourceforge.net
[CodeBetter]: http://www.codebetter.com
[TeamCity]: http://www.jetbrains.com/teamcity
[GitHub]: http://www.github.com
[SourceForge]: http://www.sourceforge.net
[Atlassian]: http://www.atlassian.com
[Log4net]: http://logging.apache.org/log4net
[Apache Software Foundation]: http://www.apache.org
[JetBrains]: http://www.jetbrains.com
[ReSharper]: http://www.jetbrains.com/resharper
[LinFu]: https://github.com/philiplaureano/LinFu
[article]: http://www.codeproject.com/KB/recipes/sets.aspx
[Relinq]: https://github.com/re-motion/Relinq
[AsyncGenerator]: http://github.com/maca88/AsyncGenerator
