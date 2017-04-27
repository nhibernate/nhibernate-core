using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3992
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(BaseEntityMapping));
			mapper.AddMapping(typeof(MappedEntityMapping));

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{

				var e1 = new MappedEntity()
				{
					BaseField = "BaseField",
					TopLevelField = "TopLevelField",
					ExtendedField = "ExtendedField"
				};
				session.Save(e1);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void Test_InhertiedClasses_AllMapsAtTopLevel()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<MappedEntity>()
							 where e.BaseField == "BaseField"
							 select e;

				Assert.AreEqual(1, result.ToList().Count);
				Assert.AreEqual("BaseField", result.First().BaseField);
				Assert.AreEqual("TopLevelField", result.First().TopLevelField);
				Assert.AreEqual("ExtendedField", result.First().ExtendedField);
			}
		}
	}
}