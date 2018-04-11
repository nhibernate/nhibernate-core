using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.CompositeElement
{
	public class Parent
	{
		public virtual int ParentId { get; set; }
		public virtual IList<Child> Children { get; set; }
	}

	public class Child
	{
		public virtual Parent ParentLink { get; set; }
	}

	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(rc =>
			{
				rc.Id(x => x.ParentId, m => m.Generator(Generators.HighLow));
				rc.List(
					x => x.Children,
					listMap =>
					{
						listMap.Table("Children");
						listMap.Index(index => index.Column("Position"));

						listMap.Key(keyMap =>
						{
							keyMap.Column(clm =>
							{
								clm.Name("ParentId");
							});
						});
						listMap.Lazy(CollectionLazy.Lazy);
						listMap.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.All);
						listMap.Inverse(true);
					},
					rel => { rel.Component(cmp => { cmp.Parent(x => x.ParentLink); }); }
				);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		[KnownBug("GH-1583")]
		public void QueryForPropertyOfParentInComponent()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from p in session.Query<Parent>().SelectMany(x => x.Children)
							  select p.ParentLink.ParentId).ToList();

				Assert.That(result, Is.Empty);
			}
		}
	}
}
