using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.UserCollection.Parameterized
{
	[TestFixture]
	public class ParameterizedUserCollectionTypeFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "UserCollection.Parameterized.Mapping.hbm.xml" }; }
		}

		[Test]
		public void BasicOperation()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var entity = new Entity("tester");
				entity.Values.Add("value-1");
				s.Persist(entity);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var entity = s.Get<Entity>("tester");
				Assert.IsTrue(NHibernateUtil.IsInitialized(entity.Values));
				Assert.AreEqual(1, entity.Values.Count);
				Assert.AreEqual("Hello", ((IDefaultableList) entity.Values).DefaultValue);

				s.Delete(entity);
				t.Commit();
			}
		}
	}
}