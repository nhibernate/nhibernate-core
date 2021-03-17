using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2704
{
	[TestFixture]
	public class EnhancedUserTypeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.AddMapping<Entity1Map>();
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateSQLQuery(
					@"insert into TA (id,ischiusa) values ('id1','S');
insert into TA (id,ischiusa) values ('id2','N');").ExecuteUpdate();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void CompareWithConstant()
		{
			var yes = true;
			using (var s = OpenSession())
				Assert.IsTrue(s.Query<Entity1>().Where(x => x.IsChiusa == yes).Any());
		}

		[Test]
		public void NotOnProperty()
		{
			using (var s = OpenSession())
				Assert.IsTrue(s.Query<Entity1>().Where(x => !x.IsChiusa).All(x => !x.IsChiusa));
		}

		[Test]
		public void CompareWithInlineConstant()
		{
			using (var s = OpenSession())
				Assert.IsTrue(s.Query<Entity1>().Where(x => x.IsChiusa == false).Any());
		}

		[Test]
		public void CompareWithNotOnConstant()
		{
			var no = false;
			using (var s = OpenSession())
				Assert.IsTrue(s.Query<Entity1>().Where(x => x.IsChiusa == !no).Any());
		}
	}
}
