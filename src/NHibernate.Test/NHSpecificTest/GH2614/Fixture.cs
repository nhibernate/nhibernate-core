using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2614
{
	public class Fixture : BugTestCase
	{
		[Test]
		public async Task TestOk()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						{
							s.Save(new ConcreteClass1 {Name = "C1"});
							s.Save(new ConcreteClass2 {Name = "C2"});
						}
						{
							var query = s.CreateQuery(@"
							SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT");
							query.SetMaxResults(5);
							IList listAsync = await query.ListAsync();
							Assert.AreEqual(2, listAsync.Count);
							Assert.AreEqual(2, query.List().Count);
						}
					}
					finally
					{
						s.Delete("from ConcreteClass1");
						s.Delete("from ConcreteClass2");
						t.Commit();
					}
				}
			}
		}
	}
}
