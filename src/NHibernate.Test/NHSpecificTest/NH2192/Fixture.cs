using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2192
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Save(new ContentItem() { Name = "Test" });
				s.Save(new ContentItem() { Name = "Test" });
				s.Save(new ContentItem() { Name = "Test2" });
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from ContentItem");
				tx.Commit();
			}

			base.OnTearDown();
		}

		private const int _threadCount = 150;

		[Test]
		public void HqlIsThreadsafe_UsingThreads()
		{
			object sync = new object();
			List<int> results = new List<int>();
			List<Exception> exceptions = new List<Exception>();

			var threads = new List<Thread>();

			for (int i=0; i<_threadCount; i++)
			{
				var thread = new Thread(new ThreadStart(() =>
					{
						try
						{
							int result = FetchRowResults();
							lock (sync)
							{
								results.Add(result);
							}
						}
						catch (Exception e)
						{
							lock (sync)
							{
								exceptions.Add(e);
							}
						}
					}));

				threads.Add(thread);
			}

			threads.ForEach(t => t.Start());
			threads.ForEach(t => t.Join());

			if (exceptions.Count > 0)
				throw exceptions[0];

			results.ForEach(r => Assert.That(r, Is.EqualTo(2)));
		}

		[Test]
		public void HqlIsThreadsafe_UsingPool()
		{
			List<Exception> exceptions = new List<Exception>();
			Func<int> result = FetchRowResults;
			List<IAsyncResult> results = new List<IAsyncResult>();

			for (int i=0; i<_threadCount; i++)
				results.Add(result.BeginInvoke(null, null));

			results.ForEach(r =>
				{
					try
					{
						Assert.That(result.EndInvoke(r), Is.EqualTo(2));
					}
					catch (Exception e)
					{
						exceptions.Add(e);
					}
				});

			if (exceptions.Count > 0)
				throw exceptions[0];
		}

		private int FetchRowResults()
		{
			using (var s = Sfi.OpenSession())
			{
				var count =
					s.CreateQuery("select ci from ContentItem ci where ci.Name = :v1")
						.SetParameter("v1", "Test")
						.List<ContentItem>()
						.Count;

				return count;
			}
		}
	}
}
