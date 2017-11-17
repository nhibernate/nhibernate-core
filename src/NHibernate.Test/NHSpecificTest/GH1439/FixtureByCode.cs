using NHibernate.Test.NHSpecificTest.GH0000;

namespace NHibernate.Test.NHSpecificTest.GH1439
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
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.ShowSql, "true");
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<CompositeEntity>(rc =>
			{
				rc.ComposedId(
					c =>
					{
						c.Property(t => t.Id);
						c.Property(t => t.Name);
					});

				rc.Property(x => x.LazyProperty, x => x.Lazy(true));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new CompositeEntity { Id = 1, Name = "Bob", LazyProperty = "LazyProperty"};
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
		public void YourTestName()
		{
			using (ISession session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var result = (from e in session.Query<CompositeEntity>()
							 where e.Name == "Bob"
							 select e).ToList();
				session.Flush();
				tran.Commit();
				Assert.AreEqual(1, result.ToList().Count);
			}
		}
	}
}
