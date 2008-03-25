/******************************************************************************\
 *
 * NHibernateEg
 * Copyright © 2006, Pierre Henri Kuaté. All rights reserved.
 * Contact: kpixel@users.sourceforge.net
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/


Open the file "doc/index.html" to read the tutorials.

For Tutorial1A and Tutorial1B:
- The source code of Tutorial1X is in "src/Tutorial1X/"
- The binaries are in "src/Tutorial1X/bin/".

The VS .NET 2003 solution of the tutorials is the file "src/NHibernateEg.sln".
The SharpDevelop combine of the tutorials is the file "src/NHibernateEg.cmbx".
You can also build the source code by running the file "src/Build-in-bin.bat".
(make sure that you correctly set up NAnt)

The up-to-date version is available online:
http://nhibernate.sourceforge.net/NHibernateEg/


--------------------------------------------------------------------------------
Tutorial1A:
-----------
- Introduction to Object / Relational Mapping with NHibernate
- Order + Entity Mapping
- NHibernate Configuration
- Basic CRUD operations


--------------------------------------------------------------------------------
Tutorial1B:
-----------
- Use hibernate.cfg.xml
- Relationship: Order - OrderDetail - Product
- Lazy loading for Order.Details and Product
- Product.<version>
- Windows listing, editing & reporting using untyped Dataset & Databinding


--------------------------------------------------------------------------------
Tutorial1C:
-----------
- Put NHibernate configuration in Web.config
- ProductEx as <joined-subclass> with Supplier as <component> + Nullables
- "Advanced" HQL query (+ Count(order) & Sum(price)) and Criteria usage
- Web App: 2nd level caching, list/edit/report using typed Dataset & Databinding


--------------------------------------------------------------------------------
Tutorial2A:
-----------
- .NET 2.0 specific features (Generics, ...)


--------------------------------------------------------------------------------
TutorialMonoA:
--------------
- Mono 1.0 / 2.0 specific (Tut1A on Linux)


--------------------------------------------------------------------------------
