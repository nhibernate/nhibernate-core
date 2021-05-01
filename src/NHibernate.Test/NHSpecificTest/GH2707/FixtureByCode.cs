using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2707
{
	[TestFixture]
	public class ConditionalFixture : TestCaseMappingByCode
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
				var e1 = new Entity1() {Id = "id1", IsChiusa = true};
				e1.CustomType = new MyType() {ToPersist = 1};
				session.Save(e1);
				var e2 = new Entity1() {Id = "id2", IsChiusa = false};
				session.Save(e2);
				e1.Parent = e1;
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
		public void EntityAndCustomTypeInConditionalResult()
		{
			using (var s = OpenSession())
				(from x in s.Query<Entity1>()
				let parent = x.Parent
				//NH-3005 - Conditional with custom type
				where (parent.IsChiusa ? x.CustomType : parent.CustomType) == x.CustomType
				select new
				{
					ParentIsChiusa = (((x == null) ? null : x.Parent) == null)
						? (bool?) null
						: x.Parent.IsChiusa,
				}).ToList();
		}
	}
}
