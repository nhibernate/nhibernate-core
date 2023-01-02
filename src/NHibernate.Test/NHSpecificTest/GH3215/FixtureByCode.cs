using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3215
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Member>(rc =>
			{
				rc.Id(x => x.Code);
				rc.Property(x => x.Name);
				rc.Property(x => x.Date);
			});

			mapper.Class<Request>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.ManyToOne(x => x.Member);
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void CountDistinctWithReservedWords()
		{
			using (var session = OpenSession())
			{
				var hql = "select  Count(DISTINCT r.Member.id), Count(DISTINCT Date(r.Member.Date)) from Request r";
				var result = session.CreateQuery(hql).List();

				Assert.That(result, Has.Count.EqualTo(1));
			}
		}
	}
}
