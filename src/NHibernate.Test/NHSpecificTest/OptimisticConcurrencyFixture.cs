using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.DomainModel;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class OptimisticConcurrencyFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"Multi.hbm.xml", "NHSpecific.Optimistic.hbm.xml"}; }
		}

		// NH-768
		[Test]
		public void DeleteOptimistic()
		{
			using (ISession s = OpenSession())
			{
				Optimistic op = new Optimistic();
				op.Bag = new List<string> {"xyz"};

				s.Save(op);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Optimistic");
				s.Flush();
			}
		}

		[Test]
		public void StaleObjectStateCheckWithNormalizedEntityPersister()
		{
			Top top = new Top();
			top.Name = "original name";

			try
			{
				using (ISession session = OpenSession())
				{
					session.Save(top);
					session.Flush();

					using (ISession concurrentSession = OpenSession())
					{
						Top sameTop = (Top) concurrentSession.Get(typeof(Top), top.Id);
						sameTop.Name = "another name";
						concurrentSession.Flush();
					}

					top.Name = "new name";

					var expectedException = Sfi.Settings.IsBatchVersionedDataEnabled
						? Throws.InstanceOf<StaleStateException>()
						: Throws.InstanceOf<StaleObjectStateException>();

					Assert.That(() => session.Flush(), expectedException);
				}
			}
			finally
			{
				using (ISession session = OpenSession())
				{
					session.Delete("from Top");
					session.Flush();
				}
			}
		}

		[Test]
		public void StaleObjectStateCheckWithEntityPersisterAndOptimisticLock()
		{
			Optimistic optimistic = new Optimistic();
			optimistic.String = "original string";

			try
			{
				using (ISession session = OpenSession())
				{
					session.Save(optimistic);
					session.Flush();

					using (ISession concurrentSession = OpenSession())
					{
						Optimistic sameOptimistic = (Optimistic) concurrentSession.Get(typeof(Optimistic), optimistic.Id);
						sameOptimistic.String = "another string";
						concurrentSession.Flush();
					}

					optimistic.String = "new string";

					var expectedException = Sfi.Settings.IsBatchVersionedDataEnabled
						? Throws.InstanceOf<StaleStateException>()
						: Throws.InstanceOf<StaleObjectStateException>();

					Assert.That(() => session.Flush(), expectedException);
				}
			}
			finally
			{
				using (ISession session = OpenSession())
				{
					session.Delete("from Optimistic");
					session.Flush();
				}
			}
		}
	}
}