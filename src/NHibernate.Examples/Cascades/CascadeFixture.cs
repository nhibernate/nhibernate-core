using System;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace NHibernate.Examples.Cascades
{
	/// <summary>
	/// Summary description for CascadeFixture.
	/// </summary>
	[TestFixture]
	public class CascadeFixture
	{
		private ISessionFactory factory = null;
		private Configuration cfg = null;

		[SetUp]
		public void SetUp()
		{
			cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddAssembly("NHibernate.Examples");
			new SchemaExport(cfg).Create(true, true);

			factory = cfg.BuildSessionFactory();
		}

		[TearDown]
		public void TearDown()
		{
			//new SchemaExport(cfg).Drop(true, true);	
		}

		[Test]
		public void Insert()
		{
			ISession session = factory.OpenSession();

			Parent dad = new Parent();
			dad.Name = "the dad";

			Child boy = new Child();
			boy.Name = "the boy";
			boy.SingleParent = dad;

			Child girl = new Child();
			girl.Name = "the girl";
			girl.SingleParent = dad;

			dad.AddChild(boy);
			dad.AddChild(girl);

			session.SaveOrUpdate(dad);

			dad.Aliases.Add("a1", new Alias("Daddy", "u"));
			dad.Aliases.Add("a2", new Alias("Father", "f"));

			session.Flush();
			session.Close();

			int boyId = boy.Id;
			int girlId = girl.Id;
			int dadId = dad.Id;

			session = factory.OpenSession();

			dad = (Parent) session.Load(typeof(Parent), dadId);

			foreach (Child child in dad.Children)
			{
				if (child.Name == "the girl")
				{
					Assert.AreEqual(girlId, child.Id);
				}
				else
				{
					Assert.AreEqual(boyId, child.Id);
				}
			}
		}
	}
}