using System;
using NUnit.Framework;
using NHibernate.Dialect.Function;
using System.Collections;

namespace NHibernate.Test.HQLFunctionTest
{
	/// <summary>
	/// This test run each HQL function separatelly so is easy to know wich function need
	/// an override in the specific dialect implementation.
	/// </summary>
	[TestFixture, Ignore("HQLFunctions not yet completely implemented (HQL parser don't parse HQLFunction)")]
	public class HQLFunctions : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{ return new string[] { "HQLFunctionTest.Animal.hbm.xml" }; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Animal");
				s.Flush();
			}
		}

		[Test]
		public void AggregateCount()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select count(*) from Animal").UniqueResult();
				Assert.AreEqual(typeof(int), result.GetType());
				Assert.AreEqual(2, result);
			}
		}

		[Test]
		public void AggregateAvg()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1",20);
				Animal a2 = new Animal("a2",10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select avg(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(double), result.GetType());
				Assert.AreEqual(15D, result);
			}
		}

		[Test]
		public void AggregateMax()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select max(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(float), result.GetType()); //use column type
				Assert.AreEqual(20F, result);
			}
		}

		[Test]
		public void AggregateMin()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select min(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(float), result.GetType()); //use column type
				Assert.AreEqual(10F, result);
			}
		}

		[Test]
		public void AggregateSum()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("a1", 20);
				Animal a2 = new Animal("a2", 10);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				object result = s.CreateQuery("select sum(a.BodyWeight) from Animal a").UniqueResult();
				Assert.AreEqual(typeof(double), result.GetType());
				Assert.AreEqual(30D, result);
			}
		}

		[Test]
		public void SubString()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "select substring(a.description) as pep from Animal a";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(1, lresult.Count);

				hql = "from Animal a where substring(a.description, 2, 3) = 'bcd'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				hql = "from Animal a where substring(a.description, 4) = 'def'";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}			
		}

		[Test]
		public void Locate()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where locate('bc', a.Description, 2) = 2";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				hql = "from Animal a where locate('bc', a.Description) = 2";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Trim()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abc   ", 20);
				Animal a2 = new Animal("   abc", 20);
				Animal a3 = new Animal("___abc__", 20);
				s.Save(a1);
				s.Save(a2);
				s.Save(a3);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where trim(a.Description) = 'abc'";
				IList lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(2, lresult.Count);

				hql = "from Animal a where trim('_' from a.Description) = 'abc'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("___abc__", result.Description);

				hql = "from Animal a where trim(trailing from a.Description) = 'abc'";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abc   ", result.Description);

				hql = "from Animal a where trim(leading from a.Description) = 'abc'";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("   abc", result.Description);

				hql = "from Animal a where trim(both from a.Description) = 'abc'";
				lresult = s.CreateQuery(hql).List();
				Assert.AreEqual(2, lresult.Count);
			}
		}

		[Test]
		public void Length()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("12345", 20);
				Animal a2 = new Animal("1234", 20);
				s.Save(a1);
				s.Save(a2);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where length(a.Description) = 5";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("12345", result.Description);
			}
		}

		[Test]
		public void Bit_length()
		{
			// test only the parser
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where bit_length(a.BodyWeight) = 24";
				IList result = s.CreateQuery(hql).List();
			}
		}

		[Test]
		public void Coalesce()
		{
			//session.createQuery("from Human h where coalesce(h.nickName, h.name.first, h.name.last) = 'max'").list();

		}

		[Test]
		public void Nullif()
		{
			//session.createQuery("select nullif(nickName, '1e1') from Human").list();
		}

		[Test]
		public void Abs()
		{
			//results = session.createQuery("select abs(bodyWeight*-1) from Human").list();
		}

		[Test]
		public void Mod()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where mod(16, 4) = 4";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Sqrt()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1024f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal an where sqrt(an.bodyWeight)/2 > 10";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Upper()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal an where upper(an.Description)='ABCDEF'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Lower()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("ABCDEF", 1f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal an where lower(an.Description)='abcdef'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("ABCDEF", result.Description);
			}
		}

		[Test]
		public void Cast()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1.3f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where cast(a.bodyWeight as string) like '1.%'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);

				hql = "from Animal a where cast(a.bodyWeight as integer) = 1";
				result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Extract()
		{
		//Session session = openSession();
		//Transaction txn = session.beginTransaction();
		//session.createQuery("select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Mammal m").list();
		//session.createQuery("select day(m.birthdate), month(m.birthdate), year(m.birthdate) from Mammal m").list();
		//if ( !(getDialect() instanceof DB2Dialect) ) { //no ANSI extract
		//  session.createQuery("select extract(second from current_timestamp()), extract(minute from current_timestamp()), extract(hour from current_timestamp()) from Mammal m").list();
		//  session.createQuery("select extract(day from m.birthdate), extract(month from m.birthdate), extract(year from m.birthdate) from Mammal m").list();
		//}
		//txn.commit();
		//session.close();					
		}

		[Test]
		public void Concat()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 1f);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where a.description = concat('a', concat('b','c'), 'd'||'e')||'f'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}

		[Test]
		public void Second()
		{
			//session.createQuery("select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Mammal m").list();
			//session.createQuery("select day(m.birthdate), month(m.birthdate), year(m.birthdate) from Mammal m").list();
		}

		[Test]
		public void Minute()
		{
			//session.createQuery("select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Mammal m").list();
			//session.createQuery("select day(m.birthdate), month(m.birthdate), year(m.birthdate) from Mammal m").list();
		}

		[Test]
		public void Hour()
		{
			//session.createQuery("select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Mammal m").list();
			//session.createQuery("select day(m.birthdate), month(m.birthdate), year(m.birthdate) from Mammal m").list();
		}

		[Test]
		public void Day()
		{
			//session.createQuery("select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Mammal m").list();
			//session.createQuery("select day(m.birthdate), month(m.birthdate), year(m.birthdate) from Mammal m").list();
		}

		[Test]
		public void Month()
		{
			//session.createQuery("select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Mammal m").list();
			//session.createQuery("select day(m.birthdate), month(m.birthdate), year(m.birthdate) from Mammal m").list();
		}

		[Test]
		public void Year()
		{
			//session.createQuery("select second(current_timestamp()), minute(current_timestamp()), hour(current_timestamp()) from Mammal m").list();
			//session.createQuery("select day(m.birthdate), month(m.birthdate), year(m.birthdate) from Mammal m").list();
		}

		[Test]
		public void Str()
		{
			using (ISession s = OpenSession())
			{
				Animal a1 = new Animal("abcdef", 20);
				s.Save(a1);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				string hql = "from Animal a where str(123) = '123'";
				Animal result = (Animal)s.CreateQuery(hql).UniqueResult();
				Assert.AreEqual("abcdef", result.Description);
			}
		}
	}
}
