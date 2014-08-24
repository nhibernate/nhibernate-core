using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Join
{
	[TestFixture]
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
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// This line would fail before the fix
				s.CreateQuery("from Worker").List<Worker>();
			}
		}

		[Test]
		public void FetchJoinWithCriteria_ForNH1059()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				// This line would fail before the fix
				s.CreateCriteria(typeof(Worker)).List<Worker>();
			}
		}
	}
}
