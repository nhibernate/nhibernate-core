
using System;
using NUnit.Framework;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH3046
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test, Explicit]
		public void MemoryLeak()
		{

			long initialMemory = GC.GetTotalMemory(true);
			long nextId = 1;
			long nextIdChild = 1;

			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				// We try to insert 100.000 entities, cleaning the sessions
				// every 1000.
				// We keep track of memory, that should be increasing forever.
				// We took a maximum of 250000 that can change to greater number but
				// not increase forever.
				for (int i = 0; i < 100; i++)
				{
					GC.Collect();
					long currentMemory = GC.GetTotalMemory(true);
					long memoryIncrease = currentMemory - initialMemory;

					Assert.Less(memoryIncrease, 400000);
					// Console.WriteLine(memoryIncrease);
					for (int j = 0; j < 1000; j++)
					{
						Parent a = new Parent();
						a.Id = nextId++;

						Child c = new Child();
						c.Id = nextIdChild++;

						a.Childs.Add(c);

						session.Save(c);
						session.Save(a);
					}
					session.Flush();
					session.Clear();
				}
			}
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}
	}
}
