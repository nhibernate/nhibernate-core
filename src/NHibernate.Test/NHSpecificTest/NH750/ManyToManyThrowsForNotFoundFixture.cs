using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	[TestFixture]
	public class ManyToManyThrowsForNotFoundFixture : BugTestCase
	{
		private int _id;
		private int _withTemplateId;

		protected override void OnSetUp()
		{
			using var s = Sfi.OpenSession();
			using var t = s.BeginTransaction();
			Device dv = new Device("Device");
			Drive dr = new Drive("Drive");
			var withTemplate = new Device("Device With Device 2 template") { Template = dv };
			s.Save(dr);
			dv.DrivesNotIgnored.Add(dr);

			_id = (int) s.Save(dv);
			_withTemplateId = (int)s.Save(withTemplate);
			s.Flush();

			s.Clear();
			s.Delete(dr);
			t.Commit();
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Device");
				s.Delete("from Drive");
				t.Commit();
			}
		}

		[Test]
		public void LazyLoad()
		{
			using var s = OpenSession();
			var device = s.Get<Device>(_id);
			Assert.Throws<ObjectNotFoundException>(() => NHibernateUtil.Initialize(device.DrivesNotIgnored));
		}

		[Test]
		public void QueryOverFetch()
		{
			using var s = OpenSession();
			var queryOver = s.QueryOver<Device>()
			                 .Fetch(SelectMode.Fetch, x => x.DrivesNotIgnored)
			                 .Where(Restrictions.IdEq(_id))
			                 .TransformUsing(Transformers.DistinctRootEntity);
			Assert.Throws<ObjectNotFoundException>(() => queryOver.SingleOrDefault());
		}

		[Test]
		public void QueryOverFetch2()
		{
			using var s = OpenSession();
			var queryOver = s.QueryOver<Device>()
			                 .Fetch(SelectMode.Fetch, x=> x.Template, x => x.Template.DrivesNotIgnored)
			                 .Where(Restrictions.IdEq(_withTemplateId))
			                 .TransformUsing(Transformers.DistinctRootEntity);
			Assert.Throws<ObjectNotFoundException>(() => queryOver.SingleOrDefault());
		}

		[Test]
		public void LinqFetch()
		{
			using var s = OpenSession();
			var query = s.Query<Device>()

			             .Fetch(x => x.DrivesNotIgnored)
			             .Where(x => x.Id == _id);
			Assert.Throws<ObjectNotFoundException>(() => NHibernateUtil.Initialize(query.SingleOrDefault()));
		}

		[Test]
		public void LinqFetch2()
		{
			using var s = OpenSession();
			var query = s.Query<Device>()

			             .Fetch(x => x.Template)
			             .ThenFetchMany(x => x.DrivesNotIgnored)
			             .Where(x => x.Id == _withTemplateId);
			Assert.Throws<ObjectNotFoundException>(() => NHibernateUtil.Initialize(query.SingleOrDefault()));
		}
	}
}
