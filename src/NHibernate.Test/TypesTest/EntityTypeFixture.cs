using NHibernate.Engine;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	// http://jira.nhibernate.org/browse/NH-1473
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

			Assert.AreEqual(-1, type.Compare(a, cb, EntityMode.Poco));
			Assert.AreEqual(-1, type.Compare(ca, b, EntityMode.Poco));
			Assert.AreEqual(-1, type.Compare(ca, cb, EntityMode.Poco));

			Assert.AreEqual(1, type.Compare(b, ca, EntityMode.Poco));
			Assert.AreEqual(1, type.Compare(cb, a, EntityMode.Poco));
			Assert.AreEqual(1, type.Compare(cb, ca, EntityMode.Poco));

			Assert.AreEqual(0, type.Compare(ca, a, EntityMode.Poco));
			Assert.AreEqual(0, type.Compare(a, ca, EntityMode.Poco));
		}

		[Test]
		public void Equals()
		{
			EntityType type = (EntityType) NHibernateUtil.Entity(typeof (EntityClass));

			EntityClass a = new EntityClass(1);
			EntityClass b = new EntityClass(2);
			EntityClass c = new EntityClass(1);

			Assert.IsTrue(type.IsEqual(a, a, EntityMode.Poco, (ISessionFactoryImplementor) sessions));
			Assert.IsFalse(type.IsEqual(a, b, EntityMode.Poco, (ISessionFactoryImplementor) sessions));
			Assert.IsTrue(type.IsEqual(a, c, EntityMode.Poco, (ISessionFactoryImplementor) sessions));
		}
	}
}
