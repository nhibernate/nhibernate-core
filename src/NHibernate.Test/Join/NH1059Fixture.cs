using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Join
{
	[TestFixture]
	[Ignore("NH-1059 not fixed yet")]
	public class NH1059Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[] { 
					"Join.Worker.hbm.xml",
				};
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Worker");

				tx.Commit();
			}
		}

		[Test]
		public void FetchJoin_ForNH1059()
		{
			long id;

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				PaidWorker p = new PaidWorker();
				p.Name = "Joe";
				p.Wage = 10m;

				s.Save(p);
				id = p.Id;

				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Get<PaidWorker>(id);
			}
		}
	}
}
