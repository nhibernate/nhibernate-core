using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2923
{
	public class ExtraLazyFixture : TestCaseMappingByCode
	{
		private object bobId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.List(x => x.Children,
					m =>
					{
						m.Lazy(CollectionLazy.Extra);
						m.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
						m.Inverse(true);
					},
					relation => relation.OneToMany());
			});
			mapper.Class<Child>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Parent { Name = "Bob" };
				bobId = session.Save(e1);

				var e2 = new Parent { Name = "Sally" };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void ShouldNotThrowException()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var bob = session.Get<Parent>(bobId);

				int? count = null;
				Assert.DoesNotThrow(() =>
				{
					count = bob.Children.Count;
				});
				Assert.NotNull(count);
				Assert.That(count.Value, Is.EqualTo(0));
			}
		}
	}
}
