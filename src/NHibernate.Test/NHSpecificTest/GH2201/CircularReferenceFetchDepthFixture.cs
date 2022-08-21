using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2201
{
	[TestFixture(1)]
	[TestFixture(2)]
	public class CircularReferenceFetchDepthFixture : BaseFetchFixture
	{
		private int _id2;
		private int _id3;

		public CircularReferenceFetchDepthFixture(int depth) : base(depth)
		{
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty("max_fetch_depth", _depth.ToString());
			base.Configure(configuration);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			_id2 = _id;

			//Generate another test entities
			base.OnSetUp();
			_id3 = _id;
			base.OnSetUp();
		}

		[Test]
		public void QueryOver()
		{
			using(var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				Entity e1 = null;
				Entity e2 = null;
				Entity e3 = null;
				var result = session.QueryOver<Entity>(() => e1)
					.JoinEntityAlias(() => e2, () => e2.EntityNumber == e1.EntityNumber && e2.EntityId == _id2)
					.JoinEntityAlias(() => e3, () => e3.EntityNumber == e1.EntityNumber && e3.EntityId == _id3)
					.Where(e => e.EntityId == _id).SingleOrDefault();

				Verify(result);
				
				Verify(session.Load<Entity>(_id2));
				Verify(session.Load<Entity>(_id3));
			}
		}

		[Test]
		public void Get()
		{
			using (var session = OpenSession())
			{
				var result = session.Get<Entity>(_id);
				Verify(result);
			}
		}
	}
}
