using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.NHSpecificTest.NH1845;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2893
{
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<User>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name, mapping => mapping.Length(256));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return (dialect is Dialect.SybaseSQLAnywhere12Dialect);
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.SetProperty("hbm2ddl.keywords", "auto-quote");
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var newUser = new User() { Id = 1000, Name = "Julian Maughan" };
				session.Save(newUser);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from User").ExecuteUpdate();
				transaction.Commit();
			}
			base.OnTearDown();
		}

		[Test]
		[Explicit("Reproduces the issue only on Sybase SQL Anywhere with the driver configured with UseNamedPrefixInSql = false")]
		public void Test()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var list = session
					.CreateCriteria<User>()
					.Add(Restrictions.InsensitiveLike("Name", "Julian", MatchMode.Anywhere))
					.List<User>();

				Assert.That(list.Count, Is.EqualTo(1));
				Assert.That(list[0].Id, Is.EqualTo(1000));

				transaction.Commit();
			}
		}
	}
}