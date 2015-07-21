using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Extendshbm
{
	[TestFixture]
	public class ExtendsFixture
	{
		protected static string BaseForMappings
		{
			get { return "NHibernate.Test."; }
		}

		[Test]
		public void AllInOne()
		{
			Configuration cfg = new Configuration();

			cfg.AddResource(BaseForMappings + "Extendshbm.allinone.hbm.xml", typeof(ExtendsFixture).Assembly);
			Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof(Person).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof(Employee).FullName), Is.Not.Null);
		}

		[Test]
		public void OutOfOrder()
		{
			Configuration cfg = new Configuration();

			cfg.AddResource(BaseForMappings + "Extendshbm.Customer.hbm.xml", typeof (ExtendsFixture).Assembly);

			Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Null, "cannot be in the configuration yet!");

			cfg.AddResource(BaseForMappings + "Extendshbm.Person.hbm.xml", typeof (ExtendsFixture).Assembly);
			cfg.AddResource(BaseForMappings + "Extendshbm.Employee.hbm.xml", typeof (ExtendsFixture).Assembly);

			cfg.BuildSessionFactory();

			Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof (Person).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof (Employee).FullName), Is.Not.Null);
		}

		[Test]
		public void NwaitingForSuper()
		{
			Configuration cfg = new Configuration();

			cfg.AddResource(BaseForMappings + "Extendshbm.Customer.hbm.xml", typeof (ExtendsFixture).Assembly);
			Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Null, "cannot be in the configuration yet!");

			cfg.AddResource(BaseForMappings + "Extendshbm.Employee.hbm.xml", typeof (ExtendsFixture).Assembly);
			Assert.That(cfg.GetClassMapping(typeof (Employee).FullName), Is.Null, "cannot be in the configuration yet!");

			cfg.AddResource(BaseForMappings + "Extendshbm.Person.hbm.xml", typeof (ExtendsFixture).Assembly);

			cfg.BuildMappings();
			Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof (Person).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof (Employee).FullName), Is.Not.Null);
		}

		[Test]
		public void MissingSuper()
		{
			Configuration cfg = new Configuration();

			try
			{
				cfg.AddResource(BaseForMappings + "Extendshbm.Customer.hbm.xml", typeof (ExtendsFixture).Assembly);
				Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Null, "cannot be in the configuration yet!");
				cfg.AddResource(BaseForMappings + "Extendshbm.Employee.hbm.xml", typeof (ExtendsFixture).Assembly);

				cfg.BuildSessionFactory();

				Assert.Fail("Should not be able to build sessionfactory without a Person");
			}
			catch (HibernateException) {}
		}

		[Test]
		public void AllSeparateInOne()
		{
			Configuration cfg = new Configuration();

			cfg.AddResource(BaseForMappings + "Extendshbm.allseparateinone.hbm.xml", typeof (ExtendsFixture).Assembly);

			cfg.BuildSessionFactory();
			Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof (Person).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof (Employee).FullName), Is.Not.Null);
		}

		[Test]
		public void JoinedSubclassAndEntityNamesOnly()
		{
			Configuration cfg = new Configuration();

			cfg.AddResource(BaseForMappings + "Extendshbm.entitynames.hbm.xml", typeof (ExtendsFixture).Assembly);

			cfg.BuildMappings();
			Assert.That(cfg.GetClassMapping("EntityHasName"), Is.Not.Null);
			Assert.That(cfg.GetClassMapping("EntityCompany"), Is.Not.Null);
		}

		[Test]
		public void JoinedSubclassAndEntityNamesOnlyWithCollection()
		{
			Configuration cfg = new Configuration();

			cfg.AddResource(BaseForMappings + "Extendshbm.entitynamesWithColl.hbm.xml", typeof(ExtendsFixture).Assembly);

			cfg.BuildMappings();
			Assert.That(cfg.GetClassMapping("EntityHasName"), Is.Not.Null);
			Assert.That(cfg.GetClassMapping("EntityCompany"), Is.Not.Null);
		}

		[Test]
		public void EntityNamesWithPackageFailureExpected()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource(BaseForMappings + "Extendshbm.packageentitynames.hbm.xml", typeof (ExtendsFixture).Assembly);

			cfg.BuildMappings();

			Assert.That(cfg.GetClassMapping("EntityHasName"), Is.Not.Null);
			Assert.That(cfg.GetClassMapping("EntityCompany"), Is.Not.Null);
		}

		[Test]
		public void EntityNamesWithPackageWithCollection()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource(BaseForMappings + "Extendshbm.packageentitynamesWithColl.hbm.xml", typeof(ExtendsFixture).Assembly);

			cfg.BuildMappings();

			Assert.That(cfg.GetClassMapping("EntityHasName"), Is.Not.Null);
			Assert.That(cfg.GetClassMapping("EntityCompany"), Is.Not.Null);
		}

		[Test]
		public void EntityNamesWithPackageFailureExpectedDiffFiles()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource(BaseForMappings + "Extendshbm.packageentitynamesf1.hbm.xml", typeof(ExtendsFixture).Assembly);
			cfg.AddResource(BaseForMappings + "Extendshbm.packageentitynamesf2.hbm.xml", typeof(ExtendsFixture).Assembly);

			cfg.BuildMappings();

			Assert.That(cfg.GetClassMapping("EntityHasName"), Is.Not.Null);
			Assert.That(cfg.GetClassMapping("EntityCompany"), Is.Not.Null);
		}

		[Test]
		public void UnionSubclass()
		{
			Configuration cfg = new Configuration();

			cfg.AddResource(BaseForMappings + "Extendshbm.unionsubclass.hbm.xml", typeof (ExtendsFixture).Assembly);

			cfg.BuildMappings();

			Assert.That(cfg.GetClassMapping(typeof (Person).FullName), Is.Not.Null);
			Assert.That(cfg.GetClassMapping(typeof (Customer).FullName), Is.Not.Null);
		}
	}
}