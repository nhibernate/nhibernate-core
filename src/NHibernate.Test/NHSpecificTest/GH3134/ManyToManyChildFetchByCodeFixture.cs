using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3134
{
	[TestFixture]
	public class ManyToManyChildFetchByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<AMap>();
			mapper.AddMapping<BMap>();

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var a1 = new A();
				a1.Bs.Add(new B());
				a1.Bs.Add(new B());
				session.Save(a1);

				var a2 = new A();
				a2.Bs.Add(new B());
				a2.Bs.Add(new B());
				session.Save(a2);

				transaction.Commit();
			}
		}
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				transaction.Commit();
			}
		}

		[Test]
		public void ChildFetchQueryOver()
		{
			using (var session = OpenSession())
			{
				session.QueryOver<B>().Future();

				session.QueryOver<B>()
					.Fetch(SelectMode.ChildFetch, b => b)
					.Fetch(SelectMode.Fetch, b => b.As)
					.Future();

				var result = session.QueryOver<B>()
					.Fetch(SelectMode.ChildFetch, b => b, b => b.As)
					.Fetch(SelectMode.Fetch, b => b.As.First().Bs)
					.TransformUsing(Transformers.DistinctRootEntity)
					.Future()
					.GetEnumerable()
					.ToList();

				Assert.That(result.Count, Is.EqualTo(4));
				Assert.That(NHibernateUtil.IsInitialized(result[0].As), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(result[0].As.First().Bs), Is.True);
				Assert.That(result[0].As.Count, Is.EqualTo(1));
				Assert.That(result[0].As.First().Bs.Count, Is.EqualTo(2));
			}
		}
	}
}
