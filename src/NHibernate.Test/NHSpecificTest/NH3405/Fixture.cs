using System;
using System.Linq;
using System.Xml.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3405
{
	public class XmlTest
	{
		public virtual Int32 Id { get; set; }
		public virtual XDocument Data { get; set; }
	}

	public class Fixture : TestCaseMappingByCode
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.WrapResultSets, Boolean.TrueString);
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<XmlTest>(rc =>
			{
				rc.Table("`XML_TEST`");
				rc.Id(x => x.Id,
					x =>
					{
						x.Column("`ID`");
						x.Generator(Generators.HighLow);
					});
				rc.Property(x => x.Data,
					x =>
					{
						x.Column("`DATA`");
						x.Type<XDocType>();
						x.NotNullable(true);
					});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var t = new XmlTest { Data = XDocument.Parse("<test>123</test>") };

				session.Save(t);
				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CanLoadEntityWithXDocument()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var test = session.Query<XmlTest>().First();

				Assert.NotNull(test);
			}
		}
	}
}
