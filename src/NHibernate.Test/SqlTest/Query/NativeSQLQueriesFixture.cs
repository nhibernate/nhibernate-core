using System.Collections;
using NHibernate.Transform;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.SqlTest.Query
{
	[TestFixture]
	public class GeneralTest : TestCase
	{
		protected const string OrganizationFetchJoinEmploymentSQL =
			"SELECT org.ORGID as {org.id}, " +
			"        org.NAME as {org.name}, " +
			"        emp.EMPLOYER as {emp.key}, " +
			"        emp.EMPID as {emp.element}, " +
			"        {emp.element.*}  " +
			"FROM ORGANIZATION org " +
			"    LEFT OUTER JOIN EMPLOYMENT emp ON org.ORGID = emp.EMPLOYER";

		protected const string OrganizationJoinEmploymentSQL =
			"SELECT org.ORGID as {org.id}, " +
			"        org.NAME as {org.name}, " +
			"        {emp.*}  " +
			"FROM ORGANIZATION org " +
			"    LEFT OUTER JOIN EMPLOYMENT emp ON org.ORGID = emp.EMPLOYER";

		protected const string EmploymentSQL = "SELECT * FROM EMPLOYMENT";

		protected string EmploymentSQLMixedScalarEntity =
			"SELECT e.*, e.employer as employerid  FROM EMPLOYMENT e";

		protected const string OrgEmpRegionSQL =
			"select {org.*}, {emp.*}, emp.REGIONCODE " +
			"from ORGANIZATION org " +
			"     left outer join EMPLOYMENT emp on org.ORGID = emp.EMPLOYER";

		protected string OrgEmpPersonSQL =
			"select {org.*}, {emp.*}, {pers.*} " +
			"from ORGANIZATION org " +
			"    join EMPLOYMENT emp on org.ORGID = emp.EMPLOYER " +
			"    join PERSON pers on pers.PERID = emp.EMPLOYEE ";

		protected override IList Mappings
		{
			get { return new[] { "SqlTest.Query.NativeSQLQueries.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void FailOnNoAddEntityOrScalar()
		{
			// Note: this passes, but for the wrong reason.
			//      there is actually an exception thrown, but it is the database
			//      throwing a sql exception because the SQL gets passed
			//      "un-processed"...
			ISession s = OpenSession();
			try
			{
				string sql = "select {org.*} " +
				             "from organization org";
				s.CreateSQLQuery(sql).List();
				Assert.Fail("Should throw an exception since no AddEntity nor AddScalar has been performed.");
			}
			catch (HibernateException)
			{
				// expected behavior
			}
			finally
			{
				s.Close();
			}
		}

		[Test]
		public void SQLQueryInterface()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");

			s.Save(ifa);
			s.Save(jboss);
			s.Save(gavin);
			s.Save(emp);

			IList l = s.CreateSQLQuery(OrgEmpRegionSQL)
				.AddEntity("org", typeof(Organization))
				.AddJoin("emp", "org.employments")
				.AddScalar("regionCode", NHibernateUtil.String)
				.List();
			Assert.AreEqual(2, l.Count);

			l = s.CreateSQLQuery(OrgEmpPersonSQL)
				.AddEntity("org", typeof(Organization))
				.AddJoin("emp", "org.employments")
				.AddJoin("pers", "emp.employee")
				.List();
			Assert.AreEqual(l.Count, 1);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			l = s.CreateSQLQuery("select {org.*}, {emp.*} " +
			                     "from ORGANIZATION org " +
			                     "     left outer join EMPLOYMENT emp on org.ORGID = emp.EMPLOYER, ORGANIZATION org2")
				.AddEntity("org", typeof(Organization))
				.AddJoin("emp", "org.employments")
				.SetResultTransformer(new DistinctRootEntityResultTransformer())
				.List();
			Assert.AreEqual(l.Count, 2);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			s.Delete(emp);
			s.Delete(gavin);
			s.Delete(ifa);
			s.Delete(jboss);

			t.Commit();
			s.Close();
		}

		[Test]
		public void ResultSetMappingDefinition()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");

			s.Save(ifa);
			s.Save(jboss);
			s.Save(gavin);
			s.Save(emp);

			IList l = s.CreateSQLQuery(OrgEmpRegionSQL)
				.SetResultSetMapping("org-emp-regionCode")
				.List();
			Assert.AreEqual(l.Count, 2);

			l = s.CreateSQLQuery(OrgEmpPersonSQL)
				.SetResultSetMapping("org-emp-person")
				.List();
			Assert.AreEqual(l.Count, 1);

			s.Delete(emp);
			s.Delete(gavin);
			s.Delete(ifa);
			s.Delete(jboss);

			t.Commit();
			s.Close();
		}

		[Test]
		public void ScalarValues()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");

			object idIfa = s.Save(ifa);
			object idJBoss = s.Save(jboss);

			s.Flush();

			IList result = s.GetNamedQuery("orgNamesOnly").List();
			Assert.IsTrue(result.Contains("IFA"));
			Assert.IsTrue(result.Contains("JBoss"));

			result = s.GetNamedQuery("orgNamesOnly").SetResultTransformer(CriteriaSpecification.AliasToEntityMap).List();
			IDictionary m = (IDictionary) result[0];
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(1, m.Count);
			Assert.IsTrue(m.Contains("NAME"));

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			IEnumerator iter = s.GetNamedQuery("orgNamesAndOrgs").List().GetEnumerator();
			iter.MoveNext();
			object[] o = (object[]) iter.Current;
			Assert.AreEqual(o[0], "IFA");
			Assert.AreEqual(((Organization) o[1]).Name, "IFA");
			iter.MoveNext();
			o = (object[]) iter.Current;
			Assert.AreEqual(o[0], "JBoss");
			Assert.AreEqual(((Organization) o[1]).Name, "JBoss");

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			// test that the ordering of the results is truly based on the order in which they were defined
			iter = s.GetNamedQuery("orgsAndOrgNames").List().GetEnumerator();
			iter.MoveNext();
			object[] row = (object[]) iter.Current;
			Assert.AreEqual(typeof(Organization), row[0].GetType(), "expecting non-scalar result first");
			Assert.AreEqual(typeof(string), row[1].GetType(), "expecting scalar result second");
			Assert.AreEqual("IFA", ((Organization) row[0]).Name);
			Assert.AreEqual(row[1], "IFA");
			iter.MoveNext();
			row = (object[]) iter.Current;
			Assert.AreEqual(typeof(Organization), row[0].GetType(), "expecting non-scalar result first");
			Assert.AreEqual(typeof(string), row[1].GetType(), "expecting scalar result second");
			Assert.AreEqual(((Organization) row[0]).Name, "JBoss");
			Assert.AreEqual(row[1], "JBoss");
			Assert.IsFalse(iter.MoveNext());

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			iter = s.GetNamedQuery("orgIdsAndOrgNames").List().GetEnumerator();
			iter.MoveNext();
			o = (object[]) iter.Current;
			Assert.AreEqual(o[1], "IFA");
			Assert.AreEqual(o[0], idIfa);
			iter.MoveNext();
			o = (object[]) iter.Current;
			Assert.AreEqual(o[1], "JBoss");
			Assert.AreEqual(o[0], idJBoss);

			s.Delete(ifa);
			s.Delete(jboss);
			t.Commit();
			s.Close();
		}

		[Test]
		public void MappedAliasStrategy()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");
			s.Save(jboss);
			s.Save(ifa);
			s.Save(gavin);
			s.Save(emp);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IQuery namedQuery = s.GetNamedQuery("AllEmploymentAsMapped");
			IList list = namedQuery.List();
			Assert.AreEqual(1, list.Count);
			Employment emp2 = (Employment) list[0];
			Assert.AreEqual(emp2.EmploymentId, emp.EmploymentId);
			Assert.AreEqual(emp2.StartDate.Date, emp.StartDate.Date);
			Assert.AreEqual(emp2.EndDate, emp.EndDate);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IQuery sqlQuery = s.GetNamedQuery("EmploymentAndPerson");
			sqlQuery.SetResultTransformer(CriteriaSpecification.AliasToEntityMap);
			list = sqlQuery.List();
			Assert.AreEqual(1, list.Count);
			object res = list[0];
			AssertClassAssignability(res.GetType(), typeof(IDictionary));
			IDictionary m = (IDictionary) res;
			Assert.AreEqual(2, m.Count);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			sqlQuery = s.GetNamedQuery("organizationreturnproperty");
			sqlQuery.SetResultTransformer(CriteriaSpecification.AliasToEntityMap);
			list = sqlQuery.List();
			Assert.AreEqual(2, list.Count);
			m = (IDictionary) list[0];
			Assert.IsTrue(m.Contains("org"));
			AssertClassAssignability(m["org"].GetType(), typeof(Organization));
			Assert.IsTrue(m.Contains("emp"));
			AssertClassAssignability(m["emp"].GetType(), typeof(Employment));
			Assert.AreEqual(2, m.Count);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			namedQuery = s.GetNamedQuery("EmploymentAndPerson");
			list = namedQuery.List();
			Assert.AreEqual(1, list.Count);
			object[] objs = (object[]) list[0];
			Assert.AreEqual(2, objs.Length);
			emp2 = (Employment) objs[0];
			gavin = (Person) objs[1];
			s.Delete(emp2);
			s.Delete(jboss);
			s.Delete(gavin);
			s.Delete(ifa);
			t.Commit();
			s.Close();
		}

		/* test for native sql composite id joins which has never been implemented */

		[Test, Ignore("Failure expected")]
		public void CompositeIdJoinsFailureExpected()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Person person = new Person();
			person.Name = "Noob";

			Product product = new Product();
			product.ProductId = new Product.ProductIdType();
			product.ProductId.Orgid = "x";
			product.ProductId.Productnumber = "1234";
			product.Name = "Hibernate 3";

			Order order = new Order();
			order.OrderId = new Order.OrderIdType();
			order.OrderId.Ordernumber = "1";
			order.OrderId.Orgid = "y";

			product.Orders.Add(order);
			order.Product = product;
			order.Person = person;

			s.Save(product);
			s.Save(order);
			s.Save(person);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Product p = (Product) s.CreateQuery("from Product p join fetch p.orders").List()[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Orders));
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			object[] o = (object[]) s.CreateSQLQuery("select\r\n" +
			                                         "        product.orgid as {product.id.orgid}," +
			                                         "        product.productnumber as {product.id.productnumber}," +
			                                         "        {prod_orders}.orgid as orgid3_1_,\r\n" +
			                                         "        {prod_orders}.ordernumber as ordernum2_3_1_,\r\n" +
			                                         "        product.name as {product.name}," +
			                                         "        {prod_orders.element.*}," +
			                                         /*"        orders.PROD_NO as PROD4_3_1_,\r\n" +
				"        orders.person as person3_1_,\r\n" +
				"        orders.PROD_ORGID as PROD3_0__,\r\n" +
				"        orders.PROD_NO as PROD4_0__,\r\n" +
				"        orders.orgid as orgid0__,\r\n" +
				"        orders.ordernumber as ordernum2_0__ \r\n" +*/
			                                         "    from\r\n" +
			                                         "        Product product \r\n" +
			                                         "    inner join\r\n" +
			                                         "        TBL_ORDER {prod_orders} \r\n" +
			                                         "            on product.orgid={prod_orders}.PROD_ORGID \r\n" +
			                                         "            and product.productnumber={prod_orders}.PROD_NO")
			                        	.AddEntity("product", typeof(Product))
			                        	.AddJoin("prod_orders", "product.orders")
			                        	.List()[0];

			p = (Product) o[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(p.Orders));
			IEnumerator en = p.Orders.GetEnumerator();
			Assert.IsTrue(en.MoveNext());
			Assert.IsNotNull(en.Current);
			t.Commit();
			s.Close();
		}

		[Test]
		public void AutoDetectAliasing()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");
			s.Save(jboss);
			s.Save(ifa);
			s.Save(gavin);
			s.Save(emp);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IList list = s.CreateSQLQuery(EmploymentSQL)
				.AddEntity(typeof(Employment).FullName)
				.List();
			Assert.AreEqual(1, list.Count);

			Employment emp2 = (Employment) list[0];
			Assert.AreEqual(emp2.EmploymentId, emp.EmploymentId);
			Assert.AreEqual(emp2.StartDate.Date, emp.StartDate.Date);
			Assert.AreEqual(emp2.EndDate, emp.EndDate);

			s.Clear();

			list = s.CreateSQLQuery(EmploymentSQL)
				.AddEntity(typeof(Employment).FullName)
				.SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
				.List();
			Assert.AreEqual(1, list.Count);
			IDictionary m = (IDictionary) list[0];
			Assert.IsTrue(m.Contains("Employment"));
			Assert.AreEqual(1, m.Count);

			list = s.CreateSQLQuery(EmploymentSQL).List();
			Assert.AreEqual(1, list.Count);
			object[] o = (object[]) list[0];
			Assert.AreEqual(8, o.Length);

			list = s.CreateSQLQuery(EmploymentSQL).SetResultTransformer(CriteriaSpecification.AliasToEntityMap).List();
			Assert.AreEqual(1, list.Count);
			m = (IDictionary) list[0];
			Assert.IsTrue(m.Contains("EMPID") || m.Contains("empid"));
			Assert.IsTrue(m.Contains("AVALUE") || m.Contains("avalue"));
			Assert.IsTrue(m.Contains("ENDDATE") || m.Contains("enddate"));
			Assert.AreEqual(8, m.Count);

			// TODO H3: H3.2 can guess the return column type so they can use just addScalar("employerid"),
			// but NHibernate currently can't do it.
			list =
				s.CreateSQLQuery(EmploymentSQLMixedScalarEntity).AddScalar("employerid", NHibernateUtil.Int64).AddEntity(
					typeof(Employment)).List();
			Assert.AreEqual(1, list.Count);
			o = (object[]) list[0];
			Assert.AreEqual(2, o.Length);
			AssertClassAssignability(o[0].GetType(), typeof(long));
			AssertClassAssignability(o[1].GetType(), typeof(Employment));


			IQuery queryWithCollection = s.GetNamedQuery("organizationEmploymentsExplicitAliases");
			queryWithCollection.SetInt64("id", jboss.Id);
			list = queryWithCollection.List();
			Assert.AreEqual(list.Count, 1);

			s.Clear();

			list = s.CreateSQLQuery(OrganizationJoinEmploymentSQL)
				.AddEntity("org", typeof(Organization))
				.AddJoin("emp", "org.employments")
				.List();
			Assert.AreEqual(2, list.Count);

			s.Clear();

			list = s.CreateSQLQuery(OrganizationFetchJoinEmploymentSQL)
				.AddEntity("org", typeof(Organization))
				.AddJoin("emp", "org.employments")
				.List();
			Assert.AreEqual(2, list.Count);

			s.Clear();

			// TODO : why twice?
			s.GetNamedQuery("organizationreturnproperty").List();
			list = s.GetNamedQuery("organizationreturnproperty").List();
			Assert.AreEqual(2, list.Count);

			s.Clear();

			list = s.GetNamedQuery("organizationautodetect").List();
			Assert.AreEqual(2, list.Count);

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(emp2);

			s.Delete(jboss);
			s.Delete(gavin);
			s.Delete(ifa);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Dimension dim = new Dimension(3, int.MaxValue);
			s.Save(dim);
			//		s.Flush();
			s.CreateSQLQuery("select d_len * d_width as surface, d_len * d_width * 10 as volume from Dimension").List();
			s.Delete(dim);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			SpaceShip enterprise = new SpaceShip();
			enterprise.Model = "USS";
			enterprise.Name = "Entreprise";
			enterprise.Speed = 50d;
			Dimension d = new Dimension(45, 10);
			enterprise.Dimensions = d;
			s.Save(enterprise);
			//		s.Flush();
			object[] result = (object[]) s.GetNamedQuery("spaceship").UniqueResult();
			enterprise = (SpaceShip) result[0];
			Assert.IsTrue(50d == enterprise.Speed);
			Assert.IsTrue(450d == ExtractDoubleValue(result[1]));
			Assert.IsTrue(4500d == ExtractDoubleValue(result[2]));
			s.Delete(enterprise);
			t.Commit();
			s.Close();
		}

		[Test]
		public void MixAndMatchEntityScalar()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Speech speech = new Speech();
			speech.Length = 23d;
			speech.Name = "Mine";
			s.Save(speech);
			s.Flush();
			s.Clear();

			IList l = s.CreateSQLQuery("select name, id, flength, name as scalarName from Speech")
				.SetResultSetMapping("speech")
				.List();
			Assert.AreEqual(l.Count, 1);

			t.Rollback();
			s.Close();
		}

		[Test]
		public void ParameterList()
		{
			using (ISession s = OpenSession())
			{
				IList l = s.CreateSQLQuery("select id from Speech where id in (:idList)")
					.AddScalar("id", NHibernateUtil.Int32)
					.SetParameterList("idList", new int[] {0, 1, 2, 3}, NHibernateUtil.Int32)
					.List();
			}
		}

		private double ExtractDoubleValue(object value)
		{
			if (value is double)
			{
				return (double) value;
			}
			else if (value is decimal)
			{
				return (double) (decimal) value;
			}
			else
			{
				return double.Parse(value.ToString());
			}
		}

		public static void AssertClassAssignability(System.Type source, System.Type target)
		{
			Assert.IsTrue(target.IsAssignableFrom(source),
			              "Classes were not assignment-compatible : source<" +
			              source.FullName +
			              "> target<" +
			              target.FullName + ">"
				);
		}
	}
}