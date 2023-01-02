using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3204
{
	[TestFixture(true)]
	[TestFixture(false)]
	[TestFixture(null)]
	public class OneToOneOptimisticLockFixture : TestCaseMappingByCode
	{
		private readonly bool? _optimisticLock;
		private Guid _id;

		public OneToOneOptimisticLockFixture(bool? optimisticLock)
		{
			_optimisticLock = optimisticLock;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.Version(x => x.Version, m => { });
					rc.OneToOne(
						x => x.OneToOne,
						m =>
						{
							m.PropertyReference(x => x.Parent);
							m.ForeignKey("none");
							m.Cascade(Mapping.ByCode.Cascade.All);
							if (_optimisticLock.HasValue)
								m.OptimisticLock(_optimisticLock.Value);
						});
				});

			mapper.Class<Child>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.ManyToOne(
						x => x.Parent,
						m =>
						{
							m.Unique(true);
							m.ForeignKey("none");
						});
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);
				transaction.Commit();
				_id = e1.Id;
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
		public void Get()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = session.Get<Entity>(_id);

				var child = new Child() { Parent = result };
				result.OneToOne = child;
				var oldVersion = result.Version;
				transaction.Commit();
				session.Clear();

				Assert.That(
					session.Get<Entity>(_id).Version,
					Is.EqualTo(_optimisticLock.GetValueOrDefault(false) ? oldVersion + 1 : oldVersion));
			}
		}
	}
}
