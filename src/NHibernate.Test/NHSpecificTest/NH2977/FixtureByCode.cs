using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2977
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="Entity" /> mapping is performed 
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			return new HbmMapping();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.MsSql2000Dialect;
		}

		[Test]
		public void CanGetUniqueStoredProcedureResult()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.CreateSQLQuery("EXEC sp_stored_procedures ?")
					.SetString(0, "sp_help")
					.UniqueResult();
				Assert.That(result, Is.Not.Null);
			}
		}

		[Test]
		public void CanLimitStoredProcedureResults()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.CreateSQLQuery("EXEC sp_stored_procedures")
					.SetMaxResults(5)
					.List();
				Assert.That(result, Has.Count.EqualTo(5));
			}
		}
	}
}