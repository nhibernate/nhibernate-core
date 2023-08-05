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

		protected override void OnSetUp()
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				Device dv = new Device("Device");
				Drive dr = new Drive("Drive");
				s.Save(dr);
				dv.DrivesNotIgnored.Add(dr);

				_id = (int) s.Save(dv);
				s.Flush();

				s.Clear();
				s.Delete(dr);
				t.Commit();
			}
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
			using var log = new SqlLogSpy();
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
			Assert.Throws<ObjectNotFoundException>(() => NHibernateUtil.Initialize(queryOver.SingleOrDefault()));
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
	}
}
