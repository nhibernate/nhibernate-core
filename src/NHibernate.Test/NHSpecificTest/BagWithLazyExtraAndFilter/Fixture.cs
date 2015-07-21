using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.BagWithLazyExtraAndFilter
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void CanUseFilterForLazyExtra()
		{
			using (var s = OpenSession())
			{
				s.BeginTransaction();
				var machineRequest = new MachineRequest { EnvId = 1L, Id = 2L };
				s.Save(new Env
				{
					Id = 1L,
					RequestsFailed = new List<MachineRequest>
					{
						machineRequest
					}
				});
				s.Save(machineRequest);
				s.Transaction.Commit();
			}

			using (var s = OpenSession())
			{
				var env = s.Load<Env>(1L);
				Assert.AreEqual(1, env.RequestsFailed.Count);
			}

			using (var s = OpenSession())
			{
				s.EnableFilter("CurrentOnly");
				var env = s.Load<Env>(1L);
				Assert.AreEqual(0, env.RequestsFailed.Count);
			}

			using (var s = OpenSession())
			{
				s.BeginTransaction();
				s.Delete(s.Load<MachineRequest>(2L));
				s.Delete(s.Load<Env>(1L));
				s.Transaction.Commit();
			}
		}
	}
}
