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
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Entity entity = new Entity("tester");
			entity.Values.Add("value-1");
			s.Persist(entity);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			entity = s.Get<Entity>("tester");
			Assert.IsTrue(NHibernateUtil.IsInitialized(entity.Values));
			Assert.AreEqual(1, entity.Values.Count);
			Assert.AreEqual("Hello", ((IDefaultableList)entity.Values).DefaultValue);
			s.Delete(entity);
			t.Commit();
			s.Close();
		}
	}
}