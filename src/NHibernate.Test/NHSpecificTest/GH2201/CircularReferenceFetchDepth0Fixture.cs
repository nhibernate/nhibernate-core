using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2201
{
	[TestFixture]
	public class CircularReferenceFetchDepth0Fixture : BaseFetchFixture
	{
		private int _id2;

		public CircularReferenceFetchDepth0Fixture() : base(0)
		{
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty("max_fetch_depth", "0");
			base.Configure(configuration);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			_id2 = _id;
			//Generate another test entity
			base.OnSetUp();
		}

		[Test]
		public void QueryOver()
		{
			using (var session = OpenSession())
			{
				Entity e1 = null;
				Entity e2 = null;
				var result = session.QueryOver<Entity>(() => e1)
					.JoinEntityAlias(() => e2, () => e2.EntityNumber == e1.EntityNumber && e2.EntityId != _id)
					.Where(e => e.EntityId == _id).SingleOrDefault();

				VerifyChildrenNotInitialized(result);
				VerifyChildrenNotInitialized(session.Load<Entity>(_id2));
			}
		}

		[Test]
		public void Get()
		{
			using (var session = OpenSession())
			{
				var result = session.Get<Entity>(_id);
				VerifyChildrenNotInitialized(result);
			}
		}
	}
}
