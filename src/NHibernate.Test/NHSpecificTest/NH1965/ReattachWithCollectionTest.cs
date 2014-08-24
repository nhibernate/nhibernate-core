using System;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH1965
{
	public class Cat
	{
		public Cat()
		{
			Children = new List<Cat>();
		}
		public virtual int Id { get; set; }
		public virtual IList<Cat> Children { get; set; }
	}

	public class ReattachWithCollectionTest: TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			// Note: DeleteOrphans has no sense, only added to match the case reported.
			mapper.Class<Cat>(cm =>
			                  {
			                  	cm.Id(x => x.Id, map => map.Generator(Generators.Identity));
													cm.Bag(x => x.Children, map => map.Cascade(Mapping.ByCode.Cascade.All.Include(Mapping.ByCode.Cascade.DeleteOrphans)), rel => rel.OneToMany());
			                  });
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mappings;
		}

		[Test]
		public void WhenReattachThenNotThrows()
		{
			var cat = new Cat();
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				session.Save(cat);
				session.Transaction.Commit();
			}

			using (var session = OpenSession())
			{
				session.Executing(x => x.Lock(cat, LockMode.None)).NotThrows();
			}

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				session.Delete(cat);
				session.Transaction.Commit();
			}
		}
	}
}