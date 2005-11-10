/******************************************************************************\
 *
 * NHibernateEg
 * Copyright © 2005, Pierre Henri Kuaté. All rights reserved.
 * Contact: kpixel@users.sourceforge.net
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/


TODO:
- Sort the methods in ProductsList (and others)

- Add #D combines!
- "Build-in-bin.bat" don't work!


--------------------------------------------------------------------------------
Tutorial1A:
-----------
- Order + Entity Mapping
- NHibernate Configuration
- Basic CRUD operations


--------------------------------------------------------------------------------
Tutorial1B:
-----------
- Put hibernate.cfg.xml code in App.config
- Relationship: Order - OrderDetail - Product
- Lazy loading for Order.Details and Product
- Product.<version>
- Windows listing, editing & reporting (=> untyped Dataset & Databinding)


--------------------------------------------------------------------------------
Tutorial1C:
-----------
- Put hibernate.cfg.xml code in Web.config
- ProductEx as <joined-subclass> with Supplier as <component> + Nullables
- "Advanced" HQL query (+ Count(order) & Sum(price)) and Criteria usage
- Web App: 2nd level caching, list/edit/report (=> typed Dataset & Databinding)


--------------------------------------------------------------------------------
Tutorial2A:
-----------
- .NET 2.0 specific features (Generics, ...)


--------------------------------------------------------------------------------
TutorialMonoA:
--------------
- Mono 1.0 / 2.0 specific (Tut1A on Linux)


--------------------------------------------------------------------------------
