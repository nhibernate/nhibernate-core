using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1907
{
	public class Something
	{
		public virtual string Name { get; set; }
		public virtual MyType Relation { get; set; }
	}
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void CanSetParameterQueryByName()
		{
			using (ISession s = OpenSession())
			{
				var q = s.CreateQuery("from Something s where s.Relation = :aParam");
				Assert.DoesNotThrow(()=>q.SetParameter("aParam", new MyType{ ToPersist = 1}));
			}
		}

		[Test]
		public void CanSetParameterQueryByPosition()
		{
			using (ISession s = OpenSession())
			{
				var q = s.CreateQuery("from Something s where s.Relation = ?");
				q.SetParameter(0, new MyType {ToPersist = 1});
				Assert.DoesNotThrow(() => q.SetParameter(0, new MyType { ToPersist = 1 }));
			}
		}
	}
}
