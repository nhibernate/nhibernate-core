using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3218
{
	[TestFixture]
	public class ContainsParameterFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Child>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Component(
					x => x.Component,
					ekm =>
					{
						ekm.Property(ek => ek.Id1);
						ekm.Property(ek => ek.Id2);
					});
			});
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Bag(x => x.List, m => { }, r => r.OneToMany());
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void ContainsOnId()
		{
			using (var session = OpenSession())
			{
				Guid clientId = Guid.NewGuid();
				session.Query<Entity>().Where(x => x.List.Select(l => l.Id).Contains(clientId)).ToList();
			}
		}

		[Test]
		public void ContainsOnNameWithInnerSubquery()
		{
			using (var session = OpenSession())
			{
				var clientId = "aa";
				session.Query<Entity>().Where(x =>
					x.List.Where(l => session.Query<Entity>().Any(s => s.Name == l.Name)).Select(l => l.Name)
						.Contains(clientId)).ToList();
			}
		}

		[Test]
		public void ContainsOnEntity()
		{
			using (var session = OpenSession())
			{
				var client = session.Load<Child>(Guid.NewGuid());
				session.Query<Entity>().Where(x => x.List.Contains(client)).ToList();
			}
		}

		[Test]
		public void ContainsOnName()
		{
			using (var session = OpenSession())
			{
				var client = "aaa";
				session.Query<Entity>().Where(x => x.List.Select(l => l.Name).Contains(client)).ToList();
			}
		}

		[Test]
		public void ContainsOnComponent()
		{
			using (var session = OpenSession())
			{
				var client = new CompositeKey() { Id1 = 1, Id2 = 2 };
				session.Query<Entity>().Where(x => x.List.Select(l => l.Component).Contains(client)).ToList();
			}
		}
	}
}
