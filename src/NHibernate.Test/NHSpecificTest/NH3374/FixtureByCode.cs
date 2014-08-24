using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3374
{
	[Ignore("Not fixed yet.")]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Document>(rc =>
			{
				rc.Id(x => x.Id, idMapper => idMapper.Generator(Generators.Identity));
				rc.ManyToOne(x => x.Blob, m =>
					{
						m.Cascade(Mapping.ByCode.Cascade.All);
					});
				rc.Property(x => x.Name);
			});
			
			mapper.Class<Blob>(map =>
				{
					map.Id(x => x.Id, idMapper => idMapper.Generator(Generators.Identity));
					map.Property(x => x.Bytes, y =>
						{
							y.Column(x =>
							{
								x.SqlType("varbinary(max)");
								x.Length(int.MaxValue);
							});
							y.Lazy(true);
						});
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Document { Name = "Bob" };
				e1.Blob = new Blob { Bytes = new byte[] { 1, 2, 3 } };
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
		public void TestNoTargetException()
		{
			Document document = LoadDetachedEntity();
			Blob blob = LoadDetachedBlob();

			blob.Bytes = new byte[] { 4, 5, 6 };
			document.Blob = blob;

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Merge(document);
			}
		}

		private Blob LoadDetachedBlob()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var blob = session.Get<Blob>(1);
				NHibernateUtil.Initialize(blob.Bytes);
				return blob;
			}
		}

		private Document LoadDetachedEntity()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				return session.Get<Document>(1);
			}
		}
	}
}
