using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.IdTest
{
	[TestFixture]
	[Ignore("Not supported yet")]
	public class UseIdentifierRollbackTest: TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "IdTest.Product.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.UseIdentifierRollBack, "true");
			base.Configure(configuration);
		}

		public void SimpleRollback()
		{
			ISession session = OpenSession();
			ITransaction t = session.BeginTransaction();
			Product prod = new Product();
			Assert.IsNull(prod.Name);
			session.Persist(prod);
			session.Flush();
			Assert.IsNotNull(prod.Name);
			t.Rollback();
			session.Close();
		}
	}
}
