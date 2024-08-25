using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3421
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		private SqlInterceptor _interceptor;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Component(x => x.Attributes, new {
					Sku = (string)null
				}, dc => {
					dc.Property(x => x.Sku);
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			_interceptor = new SqlInterceptor();

			configuration.SetInterceptor(_interceptor);
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally", Attributes = new Dictionary<string, object>() {
					{ "Sku", "AAA" }
				} };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Entity").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void TestFlushDoesNotTriggerAnUpdate()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var foo = session.Query<Entity>().ToList();

				session.Flush();

				var updateStatements = _interceptor.SqlStatements.Where(s => s.ToString().ToUpper().Contains("UPDATE")).ToList();

				Assert.That(updateStatements, Has.Count.EqualTo(0));
			}
		}

		public class SqlInterceptor : EmptyInterceptor
		{
			public IList<SqlString> SqlStatements = new List<SqlString>();

			public override SqlString OnPrepareStatement(SqlString sql)
			{
				SqlStatements.Add(sql);

				return base.OnPrepareStatement(sql);
			}
		}
	}
}
