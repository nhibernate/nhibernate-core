using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1166
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var model = new ModelMapper();
			model.Class<Person>(map =>
			{
				map.Id(x => x.Id, g => g.Generator(Generators.Assigned));
				map.Property(x => x.Name, x => x.Type(TypeFactory.GetAnsiStringType(50)));
			});
			return model.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				session.Save(new Person { Id = Guid.NewGuid(), Name = "Test" });
				session.Save(new Person { Id = Guid.NewGuid(), Name = "Test 2" });
				session.Save(new Person { Id = Guid.NewGuid(), Name = "NTest" });
				session.Flush();
			}
			base.OnSetUp();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				session.Delete("from Person");
				session.Flush();
			}
			base.OnTearDown();
		}

		[Test]
		public void StartWith()
		{
			using (ISession session = OpenSession())
			{
				var result = session.Query<Person>().Where(x => x.Name.StartsWith("Tes")).ToList();

				foreach (var r in result)
				{
					Assert.NotNull(r);
				}
			}
		}
	}
}
