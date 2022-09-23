using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NHibernate.Test.Extralazy
{
	[TestFixture]
	public class ExtraLazyFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] {"Extralazy.UserGroup.hbm.xml"}; }
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListAdd(bool initialize)
		{
			User gavin;
			var addedItems = new List<Company>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}
				
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Gavin's companies count after get");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after companies count");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after count");

				// Test adding companies with ICollection interface
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Gavin's companies count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding companies");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after adding companies");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after adding");

				// Test adding companies with IList interface
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					// Returned value is not valid with lazy list, no check for it.
					((IList) gavin.Companies).Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Gavin's companies count after adding through IList");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding through IList");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after adding through IList");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after adding through IList");

				// Check existence of added companies
				Sfi.Statistics.Clear();
				// Have to skip unloaded (non-queued indeed) elements to avoid triggering existence queries on them.
				foreach (var item in addedItems.Skip(5))
				{
					Assert.That(gavin.Companies.Contains(item), Is.True, "Company '{0}' existence", item.Name);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking existence of non-flushed");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after checking existence of non-flushed");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after checking existence of non-flushed");

				// Check existence of not loaded companies
				Assert.That(gavin.Companies.Contains(addedItems[0]), Is.True, "First company existence");
				Assert.That(gavin.Companies.Contains(addedItems[1]), Is.True, "Second company existence");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking existence of unloaded");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after checking existence of unloaded");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after checking existence of unloaded");

				// Check existence of not existing companies
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Contains(new Company("test1", 15, gavin)), Is.False, "First non-existent test");
				Assert.That(gavin.Companies.Contains(new Company("test2", 16, gavin)), Is.False, "Second non-existent test");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking non-existence");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after checking non-existence");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after checking non-existence");

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumerating");
					Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Companies count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Companies count after loading again Gavin");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after loading again");

				t.Commit();
			}
		}

		[Explicit]
		[TestCase(false, false)]
		[TestCase(false, true)]
		[TestCase(true, false)]
		[TestCase(true, true)]
		public void ListAddDuplicated(bool initialize, bool flush)
		{
			User gavin;
			var addedItems = new List<Company>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
					gavin.Companies.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Companies count before flush");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<Company>(addedItems[i].Id);
				}

				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Companies count after get");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statement count after count");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after get");

				// Re-add items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					gavin.Companies.Add(addedItems[i]);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Companies count after re-adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statement count after re-adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after re-adding");

				if (flush)
				{
					s.Flush();
					Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Companies count after second flush");
					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after second flush");
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumeration");
					Assert.That(gavin.Companies.Count, Is.EqualTo(flush ? 5 : 10), "Companies count after enumeration");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Companies count after loading Gavin again");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after loading Gavin again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListInsert(bool initialize)
		{
			User gavin;
			var addedItems = new List<Company>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Companies count after get");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after get");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after get");

				// Test inserting companies at the start
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Insert(0, item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Companies count after insert");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after insert");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after insert");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after insert");

				// Test inserting companies at the end
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Insert(i, item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Companies count after tail insert");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after tail insert");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after tail insert");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after tail insert");

				// Try insert invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(
					() => gavin.Companies.Insert(-1, new Company("c-1", -1, gavin)), "inserting at -1");
				Assert.Throws<ArgumentOutOfRangeException>(
					() => gavin.Companies.Insert(20, new Company("c20", 20, gavin)), "inserting too far");

				// Check existence of added companies
				Sfi.Statistics.Clear();
				// Have to skip unloaded (non-queued indeed) elements to avoid triggering existence queries on them.
				foreach (var item in addedItems.Skip(5))
				{
					Assert.That(gavin.Companies.Contains(item), Is.True, "Company '{0}' existence", item.Name);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after existence check");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after existence check");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after existence check");

				// Check existence of not loaded companies
				Assert.That(gavin.Companies.Contains(addedItems[0]), Is.True, "First company existence");
				Assert.That(gavin.Companies.Contains(addedItems[1]), Is.True, "Second company existence");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after unloaded existence check");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after unloaded existence check");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after unloaded existence check");

				// Check existence of not existing companies
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Contains(new Company("test1", 15, gavin)), Is.False, "First non-existence test");
				Assert.That(gavin.Companies.Contains(new Company("test2", 16, gavin)), Is.False, "Second non-existence test");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after non-existence check");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after non-existence check");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after non-existence check");

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumeration");
					Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Companies count after enumeration");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Companies count after loading again");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after loading again");

				t.Commit();
			}
		}

		[Explicit]
		[TestCase(false, false)]
		[TestCase(false, true)]
		[TestCase(true, false)]
		[TestCase(true, true)]
		public void ListInsertDuplicated(bool initialize, bool flush)
		{
			User gavin;
			var addedItems = new List<Company>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Insert(i, item);
					gavin.Companies.Insert(i, item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Companies count before flush");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<Company>(addedItems[i].Id);
				}

				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Companies count after get");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after count");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after count");

				// Re-add items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					gavin.Companies.Insert(4 - i, addedItems[i]);
				}

				Assert.That(gavin.Companies[0].ListIndex, Is.EqualTo(4), "Company at 0");
				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Companies count after re-insert");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-insert");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after re-insert");

				if (flush)
				{
					s.Flush();
					Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Companies count after flush");
					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after flush");
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumeration");
					Assert.That(gavin.Companies.Count, Is.EqualTo(flush ? 5 : 10), "Companies count after enumeration");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Companies count after loading again");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListRemoveAt(bool initialize)
		{
			User gavin;
			var addedItems = new List<Company>();
			var finalIndexOrder = new List<int> {0, 1, 2, 6, 8, 9};

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<Company>(addedItems[i].Id);
				}

				// Add transient companies
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Insert(i, item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Gavin's companies count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding companies");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after adding companies");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after adding");

				// Remove transient companies
				Sfi.Statistics.Clear();
				gavin.Companies.RemoveAt(5);
				gavin.Companies.RemoveAt(6);

				Assert.That(gavin.Companies.Count, Is.EqualTo(8), "Gavin's companies count after removing 2 transient companies");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing transient companies");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing transient companies");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after removing transient companies");

				// Remove persisted companies
				Sfi.Statistics.Clear();
				gavin.Companies.RemoveAt(3);
				gavin.Companies.RemoveAt(3);

				Assert.That(gavin.Companies.Count, Is.EqualTo(6), "Gavin's companies count after removing 2 persisted companies");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing persisted companies");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after removing persisted companies");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after removing persisted companies");

				// Try remove invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies.RemoveAt(-1), "Removing at -1");
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies.RemoveAt(8), "Removing too far");

				// Check existence of companies
				Sfi.Statistics.Clear();
				var removedIndexes = new HashSet<int> {3, 4, 5, 7};
				for (var i = 0; i < addedItems.Count; i++)
				{
					Assert.That(
						gavin.Companies.Contains(addedItems[i]),
						removedIndexes.Contains(i) ? Is.False : (IResolveConstraint) Is.True,
						$"Element at index {i} was {(removedIndexes.Contains(i) ? "not " : "")}removed");
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Gavin's companies count after checking existence");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3), "Flushes count after checking existence");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization after checking existence");

				// Check existence of not existing companies
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Contains(new Company("test1", 15, gavin)), Is.False, "Checking existence of non-existence");
				Assert.That(gavin.Companies.Contains(new Company("test2", 16, gavin)), Is.False, "Checking existence of non-existence");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Gavin's companies count after checking non-existence");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Flushes count after checking non-existence");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization after checking non-existence");

				gavin.UpdateCompaniesIndexes();

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumerating");
					Assert.That(gavin.Companies.Count, Is.EqualTo(6), "Companies count after enumerating");
					Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(6), "Companies count after loading again Gavin");
				Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after loading again");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListGetSet(bool initialize)
		{
			User gavin;
			var addedItems = new List<Company>();
			var finalIndexOrder = new List<int> {9, 8, 7, 6, 5, 4, 3, 2, 1, 0};

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<Company>(addedItems[i].Id);
				}

				// Add transient companies
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Insert(i, item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Gavin's companies count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding companies");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after adding companies");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after adding");

				// Compare all items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 10; i++)
				{
					Assert.That(gavin.Companies[i], Is.EqualTo(addedItems[i]), "Comparing added company at index {0}", i);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding comparing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding comparing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after comparing");

				// Try get invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					var item = gavin.Companies[10];
				}, "Get too far");
				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					var item = gavin.Companies[-1];
				}, "Get at -1");

				// Try set invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies[10] = addedItems[0], "Set too far");
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies[-1] = addedItems[0], "Set at -1");

				// Swap transient and persisted indexes
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var hiIndex = 9 - i;
					var tmp = gavin.Companies[i];
					gavin.Companies[i] = gavin.Companies[hiIndex];
					gavin.Companies[hiIndex] = tmp;
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Gavin's companies count after swapping");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after swapping");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(10), "Statements count after adding swapping");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after swapping");

				// Check indexes
				Sfi.Statistics.Clear();
				for (var i = 0; i < 10; i++)
				{
					Assert.That(gavin.Companies[i].ListIndex, Is.EqualTo(finalIndexOrder[i]), "Comparing company ListIndex at index {0}", i);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after comparing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after comparing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after comparing");

				gavin.UpdateCompaniesIndexes();

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumerating");
					Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Companies count after enumerating");
					Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Companies count after loading again Gavin");
				Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after loading again");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListFlush(bool initialize)
		{
			User gavin;
			var addedItems = new List<Company>();
			var finalIndexOrder = Enumerable.Range(0, 13).ToList();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<Company>(addedItems[i].Id);
				}

				// Add transient companies with Add
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Gavin's companies count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after adding");

				// Add transient companies with Insert
				Sfi.Statistics.Clear();
				using (var sqlLog = new SqlLogSpy())
				{
					for (var i = 10; i < 15; i++)
					{
						var item = new Company($"c{i}", i, gavin);
						addedItems.Add(item);
						gavin.Companies.Insert(i, item);
					}

					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "INSERT \n    INTO"), Is.EqualTo(5), "Statements count after inserting");
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Gavin's companies count after inserting 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after inserting");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after inserting");

				// Add transient companies with Add
				Sfi.Statistics.Clear();
				for (var i = 15; i < 20; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(20), "Gavin's companies count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after adding");

				// Remove last 5 transient companies
				Sfi.Statistics.Clear();
				using (var sqlLog = new SqlLogSpy())
				{
					for (var i = 15; i < 20; i++)
					{
						Assert.That(gavin.Companies.Remove(addedItems[i]), Is.True, "Removing transient company at index {0}", i);
					}

					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "INSERT \n    INTO"), Is.EqualTo(10), "Statements count after removing");
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Gavin's companies count after removing 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after removing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after removing");

				// Remove last 5 transient companies
				Sfi.Statistics.Clear();
				using (var sqlLog = new SqlLogSpy())
				{
					for (var i = 10; i < 15; i++)
					{
						gavin.Companies.RemoveAt(10);
					}

					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "DELETE \n    FROM"), Is.EqualTo(5), "Statements count after second removing");
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10), "Gavin's companies count after second removing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after second removing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(7), "Statements count after second removing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after second removing");

				// Add transient companies with Add
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems[i] = item;
					// NOTE: the returned index is currently invalid due to extra-lazy avoiding to query the count or initializing the collection
					((IList) gavin.Companies).Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15), "Gavin's companies count after adding through IList");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding through IList");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after adding through IList");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after adding through IList");

				// Remove last transient company
				Sfi.Statistics.Clear();
				using (var sqlLog = new SqlLogSpy())
				{
					Assert.That(gavin.Companies.Remove(addedItems[14]), Is.EqualTo(true), "Removing last transient company");
					var log = sqlLog.GetWholeLog();
					Assert.That(FindAllOccurrences(log, "DELETE \n    FROM"), Is.EqualTo(5), "Delete statements count after removing last transient company");
					Assert.That(FindAllOccurrences(log, "INSERT \n    INTO"), Is.EqualTo(5), "Insert statements count after removing last transient company");
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(14), "Gavin's companies count after adding removing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after removing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after removing");

				// Test index getter
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies[0], Is.EqualTo(addedItems[0]), "Comparing first item with index getter");

				Assert.That(gavin.Companies.Count, Is.EqualTo(14), "Gavin's companies count after adding comparing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after comparing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3), "Statements count after comparing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after comparing");

				// Remove last transient company
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Remove(addedItems[13]), Is.EqualTo(true), "Removing last transient company");

				Assert.That(gavin.Companies.Count, Is.EqualTo(13), "Gavin's companies count after adding repeated removing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after repeated removing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after repeated removing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after repeated removing");

				// Test index setter
				Sfi.Statistics.Clear();
				gavin.Companies[0] = addedItems[0];

				Assert.That(gavin.Companies.Count, Is.EqualTo(13), "Gavin's companies count after setting first item");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after setting first item");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3), "Statements count after setting first item");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after setting first item");

				// Test manual flush after remove
				Sfi.Statistics.Clear();
				gavin.Companies.RemoveAt(12);
				using (var sqlLog = new SqlLogSpy())
				{
					s.Flush();
					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "DELETE \n    FROM"), Is.EqualTo(1), "Delete statements count after removing at 12 index");
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(12), "Gavin's companies count after removing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after removing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after removing");

				// Test manual flush after insert
				Sfi.Statistics.Clear();
				gavin.Companies.Add(new Company("c12", 12, gavin));
				using (var sqlLog = new SqlLogSpy())
				{
					s.Flush();
					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "INSERT \n    INTO"), Is.EqualTo(1), "Insert statements count after flushing");
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(13), "Gavin's companies count after flushing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1), "Flushes count after flushing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after flushing");

				for (var i = 0; i < gavin.Companies.Count; i++)
				{
					Assert.That(gavin.Companies[i].ListIndex, Is.EqualTo(i), "Comparing company ListIndex at index {0}", i);
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumerating");
					Assert.That(gavin.Companies.Count, Is.EqualTo(13), "Companies count after enumerating");
					Assert.That(gavin.Companies.Select(o => o.ListIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(13), "Companies count after loading again Gavin");
				Assert.That(gavin.Companies.Select(o => o.ListIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after loading again");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListClear(bool initialize)
		{
			User gavin;
			var addedItems = new List<CreditCard>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new CreditCard($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.CreditCards.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.FlushMode = FlushMode.Commit;

				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<CreditCard>(addedItems[i].Id);
				}

				var collection = gavin.CreditCards;

				// Add transient credit cards
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new CreditCard($"c{i}", i, gavin);
					addedItems.Add(item);
					collection.Insert(i, item);
				}

				Assert.That(collection.Count, Is.EqualTo(10), "Gavin's credit cards count after inserting 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after inserting");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after inserting");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Credit cards initialization status after inserting");

				Sfi.Statistics.Clear();
				collection.Clear();

				Assert.That(collection.Count, Is.EqualTo(0), "Gavin's credit cards count after clearing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after clearing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after clearing");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Credit cards initialization status after clearing");

				// Re-add two not loaded and two transient credit cards
				collection.Add(addedItems[0]);
				collection.Add(addedItems[1]);
				collection.Add(addedItems[5]);
				collection.Add(addedItems[6]);

				Assert.That(collection.Count, Is.EqualTo(4), "Gavin's credit cards count after re-adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Credit cards initialization status after re-adding");

				// Remove one not loaded and one transient credit cards
				Assert.That(collection.Remove(addedItems[1]), Is.True, "Removing not loaded credit card");
				Assert.That(collection.Remove(addedItems[6]), Is.True, "Removing transient credit card");

				Assert.That(collection.Count, Is.EqualTo(2), "Gavin's credit cards count after removing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Credit cards initialization status after removing");

				// Remove not existing items
				Assert.That(collection.Remove(addedItems[1]), Is.False, "Removing not-existing credit card");
				Assert.That(collection.Remove(addedItems[6]), Is.False, "Removing not-existing credit card");

				Assert.That(collection.Count, Is.EqualTo(2), "Gavin's credit cards count after not-existing removing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after not-existing removing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after not-existing removing");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Credit cards initialization status after not-existing removing");

				if (initialize)
				{
					using (var e = collection.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(collection), Is.True, "Credit cards initialization status after enumerating");
					Assert.That(collection.Count, Is.EqualTo(2), "Credit cards count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				var collection = gavin.CreditCards;
				// As the cascade option is set to all, the clear operation will only work on
				// transient credit cards
				Assert.That(collection.Count, Is.EqualTo(6), "Credit cards count after loading again Gavin");
				for (var i = 0; i < 10; i++)
				{
					Assert.That(collection.Contains(addedItems[i]), i < 6 ? Is.True : (IResolveConstraint) Is.False, "Checking existence for item at {0}", i);
				}

				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Credit cards initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListIndexOperations(bool initialize)
		{
			User gavin;
			var finalIndexOrder = new List<int> {6, 0, 4};

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					gavin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Current tracker state:
				// Indexes: 0,1,2,3,4
				// Queue: /
				// RemoveDbIndexes: /

				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Count, Is.EqualTo(5), "Gavin's companies count");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status");

				Sfi.Statistics.Clear();
				gavin.Companies.Insert(1, new Company("c5", 5, gavin));
				// Current tracker state:
				// Indexes: 0,5,1,2,3,4
				// Queue: {1, 5}
				// RemoveDbIndexes: /

				gavin.Companies.Insert(0, new Company("c6", 6, gavin));
				// Current tracker state:
				// Indexes: 6,0,5,1,2,3,4
				// Queue: {0, 6}, {2, 5}
				// RemoveDbIndexes: /

				gavin.Companies.RemoveAt(4);
				// Current tracker state:
				// Indexes: 6,0,5,1,3,4
				// Queue: {0, 6}, {2, 5}
				// RemoveDbIndexes: 2

				gavin.Companies.RemoveAt(3);
				// Current tracker state:
				// Indexes: 6,0,5,3,4
				// Queue: {0, 6}, {2, 5}
				// RemoveDbIndexes: 1,2

				gavin.Companies.RemoveAt(3);
				// Current tracker state:
				// Indexes: 6,0,5,4
				// Queue: {0, 6}, {2, 5}
				// RemoveDbIndexes: 1,2,3

				gavin.Companies.RemoveAt(2);
				// Current tracker state:
				// Indexes: 6,0,4
				// Queue: {0, 6}
				// RemoveDbIndexes: 1,2,3

				Assert.That(gavin.Companies.Count, Is.EqualTo(3), "Gavin's companies count after remove/insert operations");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after remove/insert operations");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3), "Statements count after remove/insert operations");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False, "Companies initialization status after remove/insert operations");

				gavin.UpdateCompaniesIndexes();

				for (var i = 0; i < gavin.Companies.Count; i++)
				{
					Assert.That(gavin.Companies[i].OriginalIndex, Is.EqualTo(finalIndexOrder[i]), "Comparing company index at {0}", i);
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after enumerating");
					Assert.That(gavin.Companies.Count, Is.EqualTo(3), "Companies count after enumerating");
					Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(3), "Companies count after loading again Gavin");
				Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder), "Companies indexes after loading again");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True, "Companies initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void SetAdd(bool initialize)
		{
			User gavin;
			Document hia;
			Document hia2;
			var addedDocuments = new List<Document>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				hia = new Document("HiA", "blah blah blah", gavin);
				hia2 = new Document("HiA2", "blah blah blah blah", gavin);
				gavin.Documents.Add(hia);
				gavin.Documents.Add(hia2);
				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Documents.Count, Is.EqualTo(2), "Gavin's documents count after adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after adding documents");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after adding");

				// Test adding documents with ISet interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"document{i}", $"content{i}", gavin);
					addedDocuments.Add(document);
					Assert.That(gavin.Documents.Add(document), Is.True, "Adding document through ISet");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(7), "Gavin's documents count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding documents");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding documents");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after adding");

				// Test adding documents with ICollection interface
				Sfi.Statistics.Clear();
				var documents = (ICollection<Document>) gavin.Documents;
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"document2{i}", $"content{i}", gavin);
					addedDocuments.Add(document);
					documents.Add(document);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(12), "Gavin's documents count after adding through ICollection<>");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding through ICollection<>");
				// In this case we cannot determine whether the entities are transient or not so
				// we are forced to check the database
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding through ICollection<>");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after adding through ICollection<>");

				// Test re-adding documents with ISet interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Add(addedDocuments[i]), Is.False, "Re-add document through ISet<>");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(12), "Gavin's documents count after re-adding");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after re-adding");

				// Test re-adding documents with ICollection interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					documents.Add(addedDocuments[i]);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(12), "Gavin's documents count after re-adding through ICollection<>");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding through ICollection<>");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding through ICollection<>");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after re-adding through ICollection<>");

				// Check existence of added documents
				Sfi.Statistics.Clear();
				foreach (var document in addedDocuments)
				{
					Assert.That(gavin.Documents.Contains(document), Is.True, "Checking existence of an existing document");
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking existence");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after checking existence");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after checking existence");

				// Check existence of not loaded documents
				Assert.That(gavin.Documents.Contains(hia), Is.True, "Checking existence of not loaded document");
				Assert.That(gavin.Documents.Contains(hia2), Is.True, "Checking existence of not loaded document");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking existence");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after checking existence");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after checking existence");

				// Check existence of not existing documents
				Sfi.Statistics.Clear();
				Assert.That(gavin.Documents.Contains(new Document("test1", "content", gavin)), Is.False, "Checking existence of not-existing document");
				Assert.That(gavin.Documents.Contains(new Document("test2", "content", gavin)), Is.False, "Checking existence of not-existing document");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking non-existence");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after checking non-existence");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after checking non-existence");

				// Test adding not loaded documents
				Sfi.Statistics.Clear();
				Assert.That(gavin.Documents.Add(hia), Is.False, "Adding not loaded element");
				documents.Add(hia);

				Assert.That(gavin.Documents.Count, Is.EqualTo(12), "Gavin's documents count after adding not loaded element");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding not loaded element");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after adding not loaded element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after adding not loaded element");

				if (initialize)
				{
					using (var e = gavin.Documents.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True, "Documents initialization status after enumerating");
					Assert.That(gavin.Documents.Count, Is.EqualTo(12), "Documents count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents.Count, Is.EqualTo(12), "Documents count after loading again Gavin");
				Assert.That(gavin.Documents.Contains(hia2), Is.True, "Checking not loaded element");
				Assert.That(gavin.Documents.Contains(hia), Is.True, "Checking not loaded element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after loading again");

				t.Commit();
			}
		}

		[Test]
		public void SetAddWithOverrideEquals()
		{
			User gavin;
			User robert;
			User tom;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				robert = new User("robert", "secret");
				tom = new User("tom", "secret");
				s.Persist(gavin);
				s.Persist(robert);
				s.Persist(tom);

				gavin.Followers.Add(new UserFollower(gavin, robert));
				gavin.Followers.Add(new UserFollower(gavin, tom));
				robert.Followers.Add(new UserFollower(robert, tom));

				Assert.That(gavin.Followers.Count, Is.EqualTo(2), "Gavin's documents count after adding 2");
				Assert.That(robert.Followers.Count, Is.EqualTo(1), "Robert's followers count after adding one");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				robert = s.Get<User>("robert");
				tom = s.Get<User>("tom");

				// Re-add
				Assert.That(gavin.Followers.Add(new UserFollower(gavin, robert)), Is.False, "Re-adding element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Followers), Is.True, "Documents initialization status after re-adding");
				Assert.That(gavin.Followers, Has.Count.EqualTo(2), "Gavin's followers count after re-adding");

				// Add new
				Assert.That(robert.Followers.Add(new UserFollower(robert, gavin)), Is.True, "Adding element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Followers), Is.True, "Documents initialization status after adding");
				Assert.That(gavin.Followers, Has.Count.EqualTo(2), "Robert's followers count after re-adding");
			}
		}

		[TestCase(false, false)]
		[TestCase(false, true)]
		[TestCase(true, false)]
		[TestCase(true, true)]
		public void SetAddDuplicated(bool initialize, bool flush)
		{
			User gavin;
			var addedItems = new List<Document>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Document($"d{i}", $"c{i}", gavin);
					addedItems.Add(item);
					gavin.Documents.Add(item);
					gavin.Documents.Add(item);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after adding 5");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<Document>(addedItems[i].Title);
				}

				Sfi.Statistics.Clear();
				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after reload");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after reload");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after reload");

				// Re-add items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Add(addedItems[i]), Is.False, "Re-adding element");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after re-adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after re-adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after re-adding");

				if (flush)
				{
					s.Flush();
					Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after flushing");
					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after flushing");
				}

				if (initialize)
				{
					using (var e = gavin.Documents.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True, "Documents initialization status after enumerating");
					Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Documents count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Documents count after loading again Gavin");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void SetAddTransient(bool initialize)
		{
			User gavin;
			var addedItems = new List<UserPermission>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p{i}", gavin);
					addedItems.Add(item);
					gavin.Permissions.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.FlushMode = FlushMode.Commit;

				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Permissions.Count, Is.EqualTo(5), "Gavin's permissions count after adding 5");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after adding");

				// Test adding permissions with ICollection interface
				Sfi.Statistics.Clear();
				var items = (ICollection<UserPermission>) gavin.Permissions;
				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p2{i}", gavin);
					addedItems.Add(item);
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10), "Gavin's permissions count after adding through ICollection<>");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding through ICollection<>");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after adding through ICollection<>");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after adding through ICollection<>");

				// Test re-adding permissions with ICollection interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Skip(5))
				{
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10), "Gavin's permissions count after re-adding");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after re-adding");

				// Test adding not loaded permissions with ICollection interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Take(5))
				{
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10), "Gavin's permissions count after re-adding not loaded elements");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding not loaded elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after re-adding not loaded elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after re-adding not loaded elements");

				// Test adding loaded permissions with ICollection interface
				Sfi.Statistics.Clear();
				foreach (var item in s.Query<UserPermission>())
				{
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10), "Gavin's permissions count after re-adding loaded elements");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding loaded elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(6), "Statements count after re-adding loaded elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after re-adding loaded elements");

				// Test adding permissions with ISet interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p3{i}", gavin);
					addedItems.Add(item);
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15), "Gavin's permissions count after adding through ISet<>");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding through ISet<>");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after adding through ISet<>");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after adding through ISet<>");

				// Test re-adding permissions with ISet interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Skip(10))
				{
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15), "Gavin's permissions count after re-adding through ISet<>");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding through ISet<>");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding through ISet<>");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after re-adding through ISet<>");

				// Test adding not loaded permissions with ISet interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Take(5))
				{
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15), "Gavin's permissions count after re-adding not loaded permissions through ISet<>");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding not loaded permissions through ISet<>");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after re-adding not loaded permissions through ISet<>");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after re-adding not loaded permissions through ISet<>");

				// Test adding loaded permissions with ISet interface
				Sfi.Statistics.Clear();
				foreach (var item in s.Query<UserPermission>())
				{
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15), "Gavin's permissions count after re-adding loaded permissions through ISet<>");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding loaded permissions through ISet<>");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(6), "Statements count after re-adding loaded permissions through ISet<>");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after re-adding loaded permissions through ISet<>");

				if (initialize)
				{
					using (var e = gavin.Permissions.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.True, "Permissions initialization status after enumerating");
					Assert.That(gavin.Permissions.Count, Is.EqualTo(15), "Permissions count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Permissions.Count, Is.EqualTo(15), "Permissions count after loading again Gavin");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False, "Permissions initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void SetRemove(bool initialize)
		{
			User gavin;
			var addedDocuments = new List<Document>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"document{i}", $"content{i}", gavin);
					addedDocuments.Add(document);
					gavin.Documents.Add(document);
				}

				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedDocuments[i] = s.Get<Document>(addedDocuments[i].Title);
				}

				Sfi.Statistics.Clear();
				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after refresh");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after refresh");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after refresh");

				// Add new documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"document2{i}", $"content{i}", gavin);
					addedDocuments.Add(document);
					((ICollection<Document>)gavin.Documents).Add(document);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(10), "Gavin's documents count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after adding");

				// Test removing existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.True, "Removing existing document");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after removing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after removing");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after removing");

				// Test removing removed existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Contains(addedDocuments[i]), Is.False, "Checking existence of a removed document");
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.False, "Removing removed document");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after removing removed documents");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing removed documents");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing removed documents");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after removing removed documents");

				// Test removing not existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"test{i}", "content", gavin);
					Assert.That(gavin.Documents.Remove(document), Is.False, "Removing not existing document");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5), "Gavin's documents count after removing not existing documents");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing not existing documents");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after removing not existing documents");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after removing not existing documents");

				// Test removing newly added documents
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					Assert.That(gavin.Documents.Contains(addedDocuments[i]), Is.True, "Checking existence of an added document");
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.True, "Removing added document");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(0), "Gavin's documents count after removing added documents");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing added documents");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing added documents");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after removing added documents");

				// Test removing removed newly added documents
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					Assert.That(gavin.Documents.Contains(addedDocuments[i]), Is.False, "Checking existence of a removed document");
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.False, "Removing removed document");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(0), "Gavin's documents count after removing removed documents");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing removed documents");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing removed documents");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after removing removed documents");

				// Test removing not existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"test{i}", "content", gavin);
					Assert.That(gavin.Documents.Remove(document), Is.False, "Removing not existing document");
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(0), "Gavin's documents count after removing not existing documents");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing not existing documents");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after removing not existing documents");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after removing not existing documents");

				if (initialize)
				{
					using (var e = gavin.Documents.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True, "Documents initialization status after enumerating");
					Assert.That(gavin.Documents.Count, Is.EqualTo(0), "Documents count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents.Count, Is.EqualTo(0), "Documents count after loading again Gavin");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Documents initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void SetClear(bool initialize)
		{
			User gavin;
			var addedItems = new List<UserPermission>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p{i}", gavin);
					addedItems.Add(item);
					gavin.Permissions.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.FlushMode = FlushMode.Commit;

				gavin = s.Get<User>("gavin");
				// Refresh added items
				for (var i = 0; i < 5; i++)
				{
					addedItems[i] = s.Get<UserPermission>(addedItems[i].Id);
				}

				var collection = gavin.Permissions;

				Sfi.Statistics.Clear();
				Assert.That(collection.Count, Is.EqualTo(5), "Gavin's permissions count after refresh");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after refresh");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Permissions initialization status after refresh");

				// Add transient permissions
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p2{i}", gavin);
					addedItems.Add(item);
					collection.Add(item);
				}

				Assert.That(collection.Count, Is.EqualTo(10), "Gavin's permissions count after adding 5");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Permissions initialization status after adding");

				Sfi.Statistics.Clear();
				collection.Clear();

				Assert.That(collection.Count, Is.EqualTo(0), "Gavin's permissions count after flushing");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after flushing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after flushing");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Permissions initialization status after flushing");

				// Re-add two not loaded and two transient permissions
				Assert.That(collection.Add(addedItems[0]), Is.True, "Re-adding not loaded element");
				Assert.That(collection.Add(addedItems[1]), Is.True, "Re-adding not loaded element");
				Assert.That(collection.Add(addedItems[5]), Is.True, "Re-adding transient element");
				Assert.That(collection.Add(addedItems[6]), Is.True, "Re-adding transient element");

				Assert.That(collection.Count, Is.EqualTo(4), "Gavin's permissions count after re-adding");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Permissions initialization status after re-adding");

				// Remove one not loaded and one transient permissions
				Assert.That(collection.Remove(addedItems[1]), Is.True, "Removing not loaded element");
				Assert.That(collection.Remove(addedItems[6]), Is.True, "Removing transient element");

				Assert.That(collection.Count, Is.EqualTo(2), "Gavin's permissions count after removing");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Permissions initialization status after removing");

				// Remove not existing items
				Assert.That(collection.Remove(addedItems[1]), Is.False, "Removing removed element");
				Assert.That(collection.Remove(addedItems[6]), Is.False, "Removing removed element");

				Assert.That(collection.Count, Is.EqualTo(2), "Gavin's permissions count after removing removed elements");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing removed elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing removed elements");
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Permissions initialization status after removing removed elements");

				if (initialize)
				{
					using (var e = collection.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(collection), Is.True, "Permissions initialization status after enumerating");
					Assert.That(collection.Count, Is.EqualTo(2), "Permissions count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				var collection = gavin.Permissions;
				// As the cascade option is set to all, the clear operation will only work on
				// transient permissions
				Assert.That(collection.Count, Is.EqualTo(6), "Permissions count after loading again Gavin");
				for (var i = 0; i < 10; i++)
				{
					Assert.That(collection.Contains(addedItems[i]), i < 6 ? Is.True : (IResolveConstraint) Is.False,
						"Checking existence of added element at {0}", i);
				}

				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False, "Permissions initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void MapAdd(bool initialize)
		{
			User gavin;
			UserSetting setting;
			var addedSettings = new List<UserSetting>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s{i}", $"data{i}", gavin);
					addedSettings.Add(setting);
					gavin.Settings.Add(setting.Name, setting);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Count, Is.EqualTo(5), "Gavin's user settings count after load");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after load");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after load");

				// Test adding settings with Add method
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					addedSettings.Add(setting);
					gavin.Settings.Add(setting.Name, setting);
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "Gavin's user settings count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after adding");

				// Test adding settings with []
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s3{i}", $"data{i}", gavin);
					addedSettings.Add(setting);

					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(15), "Gavin's user settings count after adding 5 through indexer");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding through indexer");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding through indexer");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after adding through indexer");

				// Check existence of added settings
				Sfi.Statistics.Clear();
				foreach (var item in addedSettings.Skip(5))
				{
					Assert.That(gavin.Settings.ContainsKey(item.Name), Is.True, "Checking existence of added element");
					Assert.That(gavin.Settings.Contains(new KeyValuePair<string, UserSetting>(item.Name, item)), Is.True, "Checking existence of added element using KeyValuePair<,>");
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking existence of added elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after checking existence of added elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after checking existence of added elements");

				// Check existence of not loaded settings
				foreach (var item in addedSettings.Take(5))
				{
					Assert.That(gavin.Settings.ContainsKey(item.Name), Is.True, "Checking key existence of not loaded elements");
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking existence of not loaded elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after checking existence of not loaded elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after checking existence of not loaded elements");

				// Check existence of not existing settings
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.ContainsKey("test"), Is.False, "Checking existence of not existing element");
				Assert.That(gavin.Settings.ContainsKey("test2"), Is.False, "Checking existence of not existing element");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after checking existence of not existing elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after checking existence of not existing elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after checking existence of not existing elements");

				// Try to add an existing setting
				Assert.Throws<ArgumentException>(() => gavin.Settings.Add("s0", new UserSetting("s0", "data", gavin)), "Adding an existing key");
				Assert.Throws<ArgumentException>(() => gavin.Settings.Add("s20", new UserSetting("s20", "data", gavin)), "Adding an existing key");
				Assert.Throws<ArgumentException>(() => gavin.Settings.Add("s30", new UserSetting("s30", "data", gavin)), "Adding an existing key");

				// Get values of not loaded keys
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.TryGetValue("s0", out setting), Is.True, "Getting value of not loaded key");
				Assert.That(setting.Id, Is.EqualTo(addedSettings[0].Id), "Comparing retrieved element id");
				Assert.That(gavin.Settings["s0"].Id, Is.EqualTo(addedSettings[0].Id), "Comparing retrieved element id by indexer");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after reading elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2), "Statements count after reading elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after reading elements");

				// Get values of newly added keys
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.TryGetValue("s20", out setting), Is.True, "Getting value of a newly added key");
				Assert.That(setting, Is.EqualTo(addedSettings[5]), "Comparing retrieved element");
				Assert.That(gavin.Settings["s20"], Is.EqualTo(addedSettings[5]), "Comparing retrieved element by indexer");
				Assert.That(gavin.Settings.TryGetValue("s30", out setting), Is.True, "Getting value of a newly added key");
				Assert.That(setting, Is.EqualTo(addedSettings[10]), "Comparing retrieved element");
				Assert.That(gavin.Settings["s30"], Is.EqualTo(addedSettings[10]), "Getting value of a newly added key");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after reading elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after reading elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after reading elements");

				// Try to get a non existing setting
				Assert.That(gavin.Settings.TryGetValue("test", out setting), Is.False, "Try to get a not existing key");
				Assert.That(gavin.Settings.TryGetValue("test2", out setting), Is.False, "Try to get a not existing key");
				Assert.Throws<KeyNotFoundException>(() =>
				{
					setting = gavin.Settings["test"];
				}, "Getting a not existing key");
				Assert.Throws<KeyNotFoundException>(() =>
				{
					setting = gavin.Settings["test2"];
				}, "Getting a not existing key");

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after reading not existing elements");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(4), "Statements count after reading not existing elements");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after reading not existing elements");

				if (initialize)
				{
					using (var e = gavin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.True, "User settings initialization status after enumerating");
					Assert.That(gavin.Settings.Count, Is.EqualTo(15), "User settings count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Settings.Count, Is.EqualTo(15), "User settings count after loading again Gavin");
				Assert.That(gavin.Settings.ContainsKey(addedSettings[0].Name), Is.True, "Checking key existence");
				Assert.That(gavin.Settings.ContainsKey(addedSettings[5].Name), Is.True, "Checking key existence");
				Assert.That(gavin.Settings.ContainsKey(addedSettings[10].Name), Is.True, "Checking key existence");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void MapSet(bool initialize)
		{
			User gavin;
			UserSetting setting;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s{i}", $"data{i}", gavin);
					gavin.Settings.Add(setting.Name, setting);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Count, Is.EqualTo(5), "Gavin's user settings count after load");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after load");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after load");

				// Set a key that does not exist in db and is not in the queue
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "Gavin's user settings count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after adding");

				// Set a key that does not exist in db and it is in the queue
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "Gavin's user settings count after re-adding existing keys");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding existing keys");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding existing keys");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after re-adding existing keys");

				// Set a key that exists in db and is not in the queue
				Sfi.Statistics.Clear();
				gavin.Settings["s0"] = new UserSetting("s0", "s0", gavin);

				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "Gavin's user settings count after re-adding a not loaded key");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding a not loaded key");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after re-adding a not loaded key");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after re-adding a not loaded key");

				// Set a key that exists in db and it is in the queue
				Sfi.Statistics.Clear();
				gavin.Settings["s0"] = new UserSetting("s0", "s0", gavin);

				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "Gavin's user settings count after re-adding a loaded key");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after re-adding a loaded key");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after re-adding a loaded key");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after re-adding a loaded key");

				// Set a key that exists in db and it is in the removal queue
				Assert.That(gavin.Settings.Remove("s1"), Is.True, "Removing an existing key");
				Sfi.Statistics.Clear();
				gavin.Settings["s1"] = new UserSetting("s1", "s1", gavin);

				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "Gavin's user settings count after removing an existing key");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing an existing key");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing an existing key");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after removing an existing key");

				if (initialize)
				{
					using (var e = gavin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.True, "User settings initialization status after enumerating");
					Assert.That(gavin.Settings.Count, Is.EqualTo(10), "User settings count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "User settings count after loading again Gavin");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after loading again");

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void MapRemove(bool initialize)
		{
			User gavin;
			UserSetting setting;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s{i}", $"data{i}", gavin);
					gavin.Settings.Add(setting.Name, setting);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Count, Is.EqualTo(5), "Gavin's user settings count after loading");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after loading");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after loading");

				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10), "Gavin's user settings count after adding 5");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after adding");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5), "Statements count after adding");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after adding");

				// Remove a key that exists in db and is not in the queue and removal queue
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s0"), Is.True, "Removing an existing element");

				Assert.That(gavin.Settings.Count, Is.EqualTo(9), "Gavin's user settings count after removing a not loaded element");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing a not loaded element");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after removing a not loaded element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after removing a not loaded element");

				// Remove a key that exists in db and it is in the queue
				var item = gavin.Settings["s1"];
				Assert.That(gavin.Settings.Remove("s1"), Is.True, "Removing an existing element");
				gavin.Settings.Add(item.Name, item);
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s1"), Is.True, "Removing a re-added element");

				Assert.That(gavin.Settings.Count, Is.EqualTo(8), "Gavin's user settings count after removing a re-added element");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing a re-added element");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing a re-added element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after removing a re-added element");

				// Remove a key that does not exist in db and is not in the queue
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("test"), Is.False, "Removing not existing element");

				Assert.That(gavin.Settings.Count, Is.EqualTo(8), "Gavin's user settings count after removing not existing element");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing not existing element");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1), "Statements count after removing not existing element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after removing not existing element");

				// Remove a key that does not exist in db and it is in the queue
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s20"), Is.True);

				Assert.That(gavin.Settings.Count, Is.EqualTo(7), "Gavin's user settings count after removing an existing element");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing an existing element");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing an existing element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after removing an existing element");

				// Remove a key that exists in db and it is in the removal queue
				Assert.That(gavin.Settings.Remove("s2"), Is.True, "Removing not loaded element");
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s2"), Is.False, "Removing removed element");

				Assert.That(gavin.Settings.Count, Is.EqualTo(6), "Gavin's user settings count after removing a not loaded element");
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0), "Flushes count after removing a not loaded element");
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Statements count after removing a not loaded element");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after removing a not loaded element");

				if (initialize)
				{
					using (var e = gavin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.True, "User settings initialization status after enumerating");
					Assert.That(gavin.Settings.Count, Is.EqualTo(6), "User settings count after enumerating");
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Settings.Count, Is.EqualTo(6), "User settings count after loading again Gavin");
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False, "User settings initialization status after loading again");

				t.Commit();
			}
		}

		[Test]
		public void ExtraLazyWithWhereClause()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateSQLQuery("insert into Users (Name,Password) values('gavin','secret')")
					.UniqueResult();
				s.CreateSQLQuery("insert into Photos (Title,Owner) values('PRVaaa','gavin')")
					.UniqueResult();
				s.CreateSQLQuery("insert into Photos (Title,Owner) values('PUBbbb','gavin')")
					.UniqueResult();
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var gavin = s.Get<User>("gavin");
				Assert.AreEqual(1, gavin.Photos.Count);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateSQLQuery("delete from Photos")
					.UniqueResult();
				s.CreateSQLQuery("delete from Users")
					.UniqueResult();

				t.Commit();
			}
			Sfi.Evict(typeof (User));
			Sfi.Evict(typeof (Photo));
		}

		[Test]
		public void OrphanDelete()
		{
			User gavin = null;
			Document hia = null;
			Document hia2 = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				hia = new Document("HiA", "blah blah blah", gavin);
				hia2 = new Document("HiA2", "blah blah blah blah", gavin);
				gavin.Documents.Add(hia); // NH: added ; I don't understand how can work in H3.2.5 without add
				gavin.Documents.Add(hia2); // NH: added 
				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.AreEqual(2, gavin.Documents.Count);
				gavin.Documents.Remove(hia2);
				// Do not convert Documents.Contains to Does.Contain/Does.Not.Contain: NUnit constraints will trigger
				// initialization of the collection. Moreover, with extra-lazy, collection.Contains works even if
				// the entity does not properly overrides Equals and GetHashCode and a detached instance is used,
				// provided the collection is not yet initialized, which is the case here.
				Assert.IsFalse(gavin.Documents.Contains(hia2));
				Assert.IsTrue(gavin.Documents.Contains(hia));
				Assert.AreEqual(1, gavin.Documents.Count);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.AreEqual(1, gavin.Documents.Count);
				Assert.IsFalse(gavin.Documents.Contains(hia2));
				Assert.IsTrue(gavin.Documents.Contains(hia));
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
				Assert.That(s.Get<Document>("HiA2"), Is.Null);
				gavin.Documents.Clear();
				Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Documents));
				s.Delete(gavin);
				t.Commit();
			}
		}

		[Test]
		public void OrphanDeleteWithEnumeration()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var gavin = new User("gavin", "secret");
				gavin.Documents.Add(new Document("HiA", "blah blah blah", gavin));
				gavin.Documents.Add(new Document("HiA2", "blah blah blah blah", gavin));
				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var hia2 = s.Get<Document>("HiA2");
				var hia = s.Get<Document>("HiA");
				var gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents, Has.Count.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False, "Expecting non initialized collection");
				gavin.Documents.Remove(hia2);
				// Force an enumeration
				using (var e = gavin.Documents.GetEnumerator())
					e.MoveNext();
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True, "Expecting initialized collection");
				Assert.That(gavin.Documents, Does.Not.Contain(hia2).And.Contain(hia).And.Count.EqualTo(1));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var hia = s.Get<Document>("HiA");
				var gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents, Has.Count.EqualTo(1).And.Contain(hia));
				Assert.That(s.Get<Document>("HiA2"), Is.Null);
				t.Commit();
			}
		}

		[Test]
		public void Get()
		{
			User gavin = null;
			User turin = null;
			Group g = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				s.Persist(g);
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				gavin = (User) g.Users["gavin"];
				turin = (User) g.Users["turin"];
				Assert.That(gavin, Is.Not.Null);
				Assert.That(turin, Is.Not.Null);
				Assert.That(g.Users.ContainsKey("emmanuel"), Is.False);
				Assert.IsFalse(NHibernateUtil.IsInitialized(g.Users));
				Assert.That(gavin.Session["foo"], Is.Not.Null);
				Assert.That(turin.Session.ContainsKey("foo"), Is.False);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Session));
				Assert.IsFalse(NHibernateUtil.IsInitialized(turin.Session));
				s.Delete(gavin);
				s.Delete(turin);
				s.Delete(g);
				t.Commit();
			}
		}

		[Test]
		public void RemoveClear()
		{
			User gavin = null;
			User turin = null;
			Group g = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				s.Persist(g);
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				gavin = (User) g.Users["gavin"];
				turin = (User) g.Users["turin"];
				Assert.IsFalse(NHibernateUtil.IsInitialized(g.Users));
				g.Users.Clear();
				gavin.Session.Remove("foo");
				Assert.IsTrue(NHibernateUtil.IsInitialized(g.Users));
				Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Session));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				//Assert.IsTrue( g.Users.IsEmpty() );
				//Assert.IsFalse( NHibernateUtil.IsInitialized( g.getUsers() ) );
				gavin = s.Get<User>("gavin");
				Assert.IsFalse(gavin.Session.ContainsKey("foo"));
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Session));
				s.Delete(gavin);
				s.Delete(turin);
				s.Delete(g);
				t.Commit();
			}
		}

		[Test]
		public void IndexFormulaMap()
		{
			User gavin = null;
			User turin = null;
			Group g = null;
			IDictionary<string, SessionAttribute> smap = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				s.Persist(g);
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				Assert.AreEqual(2, g.Users.Count);
				g.Users.Remove("turin");
				smap = ((User) g.Users["gavin"]).Session;
				Assert.AreEqual(2, smap.Count);
				smap.Remove("bar");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				Assert.AreEqual(1, g.Users.Count);
				smap = ((User) g.Users["gavin"]).Session;
				Assert.AreEqual(1, smap.Count);
				gavin = (User) g.Users["gavin"]; // NH: put in JAVA return the previous value
				g.Users["gavin"] = turin;
				s.Delete(gavin);
				Assert.AreEqual(0, s.CreateQuery("select count(*) from SessionAttribute").UniqueResult<long>());
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = s.Get<Group>("developers");
				Assert.AreEqual(1, g.Users.Count);
				turin = (User) g.Users["turin"];
				smap = turin.Session;
				Assert.AreEqual(0, smap.Count);
				Assert.AreEqual(1L, s.CreateQuery("select count(*) from User").UniqueResult<long>());
				s.Delete(g);
				s.Delete(turin);
				Assert.AreEqual(0, s.CreateQuery("select count(*) from User").UniqueResult<long>());
				t.Commit();
			}
		}

		[Test]
		public void SQLQuery()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				User gavin = new User("gavin", "secret");
				User turin = new User("turin", "tiger");
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				s.Persist(gavin);
				s.Persist(turin);
				s.Flush();
				s.Clear();

				IList results = s.GetNamedQuery("UserSessionData").SetParameter("uname", "%in").List();
				Assert.AreEqual(2, results.Count);
				// NH Different behavior : NH1612, HHH-2831
				gavin = (User) results[0];
				Assert.AreEqual("gavin", gavin.Name);
				Assert.AreEqual(2, gavin.Session.Count);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from SessionAttribute");
				s.Delete("from User");
				t.Commit();
			}
		}

		[Test]
		public void AddToUninitializedSetWithLaterLazyLoad()
		{
			User gavin;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				var hia = new Document("HiA", "blah blah blah", gavin);
				gavin.Documents.Add(hia);
				s.Persist(gavin);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				var hia2 = new Document("HiA2", "blah blah blah blah", gavin);
				gavin.Documents.Add(hia2);

				foreach (var _ in gavin.Documents)
				{
					//Force Iteration
				}

				Assert.That(gavin.Documents.Contains(hia2));
				s.Delete(gavin);
				t.Commit();
			}
		}

		private int FindAllOccurrences(string source, string substring)
		{
			if (source == null)
			{
				return 0;
			}
			int n = 0, count = 0;
			while ((n = source.IndexOf(substring, n, StringComparison.InvariantCulture)) != -1)
			{
				n += substring.Length;
				++count;
			}
			return count;
		}
	}
}
