using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3290
{
	[TestFixture(true)]
	[TestFixture(false)]
	public class Fixture : TestCaseMappingByCode
	{
		private readonly bool _detectFetchLoops;

		public Fixture(bool detectFetchLoops)
		{
			_detectFetchLoops = detectFetchLoops;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, map => map.Generator(Generators.GuidComb));

				rc.Property(
					x => x.Name
				);

				rc.Set(
					x => x.Children,
					v =>
					{
						v.Table("EntityToEntity");
						v.Cascade(Mapping.ByCode.Cascade.None);
						v.Inverse(true);
						v.Key(x =>
						{
							x.Column("ParentId");
							x.NotNullable(true);
						});
						v.Lazy(CollectionLazy.Lazy);
						v.Fetch(CollectionFetchMode.Join);
					},
					h => h.ManyToMany(m => m.Column("ChildId"))
				);

				rc.Set(
					x => x.Parents,
					v =>
					{
						v.Table("EntityToEntity");
						v.Cascade(Mapping.ByCode.Cascade.All);

						v.Key(x =>
						{
							x.Column("ChildId");
							x.NotNullable(true);
						});
						v.Lazy(CollectionLazy.Lazy);
						v.Fetch(CollectionFetchMode.Join);
					},
					h => h.ManyToMany(m => m.Column("ParentId"))
				);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			if (_detectFetchLoops)
				return;

			configuration.SetProperty(Environment.DetectFetchLoops, "false");
			configuration.SetProperty(Environment.MaxFetchDepth, "2");
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var person = new Entity
			{
				Name = "pers",
				Parents = new HashSet<Entity>()
			};
			session.Save(person);
			var job = new Entity
			{
				Name = "job",
				Children = new HashSet<Entity>()
			};
			session.Save(job);

			job.Children.Add(person);
			person.Parents.Add(job);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateSQLQuery("delete from EntityToEntity").ExecuteUpdate();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void QueryWithFetch()
		{
			using var session = OpenSession();
			using var _ = session.BeginTransaction();

			var all = session
				.QueryOver<Entity>()
				.Fetch(SelectMode.Fetch, x => x.Children)
				.Fetch(SelectMode.Fetch, x => x.Parents)
				.TransformUsing(Transformers.DistinctRootEntity)
				.List();

			foreach (var entity in all)
			{
				var isPerson = entity.Name == "pers";
				if (isPerson)
					Assert.That(entity.Parents, Has.Count.EqualTo(1), "Person's job not found or non-unique.");
				else
					Assert.That(entity.Children, Has.Count.EqualTo(1), "Job's employee not found or non-unique.");
			}
		}
	}
}
