using NHibernate.Engine;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	// http://nhibernate.jira.com/browse/NH-1473
	[TestFixture]
	public class EntityTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Entity"; }
		}

		[Test]
		public void Compare()
		{
			EntityType type = (EntityType) NHibernateUtil.Entity(typeof (EntityClass));

			EntityClass a = new EntityClass(1);
			EntityClass b = new EntityClass(2);

			EntityClass ca = new ComparableEntityClass(1);
			EntityClass cb = new ComparableEntityClass(2);

			Assert.AreEqual(-1, type.Compare(a, cb));
			Assert.AreEqual(-1, type.Compare(ca, b));
			Assert.AreEqual(-1, type.Compare(ca, cb));

			Assert.AreEqual(1, type.Compare(b, ca));
			Assert.AreEqual(1, type.Compare(cb, a));
			Assert.AreEqual(1, type.Compare(cb, ca));

			Assert.AreEqual(0, type.Compare(ca, a));
			Assert.AreEqual(0, type.Compare(a, ca));
		}

		[Test]
		public void Equals()
		{
			EntityType type = (EntityType) NHibernateUtil.Entity(typeof (EntityClass));

			EntityClass a = new EntityClass(1);
			EntityClass b = new EntityClass(2);
			EntityClass c = new EntityClass(1);

			Assert.IsTrue(type.IsEqual(a, a, (ISessionFactoryImplementor) Sfi));
			Assert.IsFalse(type.IsEqual(a, b, (ISessionFactoryImplementor) Sfi));
			Assert.IsTrue(type.IsEqual(a, c, (ISessionFactoryImplementor) Sfi));
		}
	}
}
