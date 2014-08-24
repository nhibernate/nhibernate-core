using NUnit.Framework;
using System.Collections;

namespace NHibernate.Test.UnionsubclassPolymorphicFormula
{
	[TestFixture, Explicit]
	public class UnionSubclassFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "UnionsubclassPolymorphicFormula.Party.hbm.xml" }; }
		}

		[Test, KnownBug("NH-2354")]
		public void QueryOverPersonTest()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					var person = new Person
					{
						FirstName = "Mark",
						LastName = "Mannson"
					};

					s.Save(person);

					var result = s.QueryOver<Party>().Where(p => p.Name == "Mark Mannson").SingleOrDefault();
					
					Assert.NotNull(result);
					s.Delete(result);
					t.Commit();
				}
				
			}
		}

		[Test, KnownBug("NH-2354")]
		public void QueryOverCompanyTest()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					var company = new Company
					{
						CompanyName = "Limited",
					};

					s.Save(company);

					var result = s.QueryOver<Party>().Where(p => p.Name == "Limited").SingleOrDefault();
					Assert.NotNull(result);
				}

			}
		}
	}
}
