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
		public void MapAddChildSaveChangeParent(bool initialize)
		{
			User gavin;
			User turin;
			var gavinItems = new List<UserSetting>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				s.Persist(gavin);
				s.Persist(turin);

				for (var i = 0; i < 5; i++)
				{
					var item = new UserSetting($"g{i}", $"data{i}", gavin);
					gavinItems.Add(item);
					gavin.Settings.Add(item.Name, item);

					item = new UserSetting($"t{i}", $"data{i}", turin);
					turin.Settings.Add(item.Name, item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Count, Is.EqualTo(5));
				Assert.That(turin.Settings.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.Settings), Is.False);

				// Save companies and then add them to the collection
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new UserSetting($"c{i}", $"data{i}", gavin);
					s.Save(item);
					gavinItems.Add(item);
				}

				for (var i = 5; i < 10; i++)
				{
					gavin.Settings.Add(gavinItems[i].Name, gavinItems[i]);
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Add companies to the collection and then save them
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new UserSetting($"c{i}", $"data{i}", gavin);
					gavin.Settings.Add(item.Name, item);
					gavinItems.Add(item);
					s.Save(item);
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Remove added items from the collection and add them to a different parent
				foreach (var item in gavinItems.Skip(5).Take(5))
				{
					gavin.Settings.Remove(item.Name);

					item.Owner = turin;
					turin.Settings.Add(item.Name, item);
				}

				// Remove added items from the collection
				for (var i = 10; i < 15; i++)
				{
					var item = gavinItems[i];
					gavin.Settings.Remove(item.Name);
					// When identity is used for the primary key the item will be already inserted in the database,
					// so the RemoveAt method will mark it as an orphan which will be deleted on flush.
					// The same would work for an initialized collection as the collection snapshot would contain the item.
					// When dealing with an id generator that supports a delayed insert, we have to trigger a delete
					// for the item as it is currently scheduled for insertion.
					if (Dialect.SupportsIdentityColumns)
					{
						if (i % 2 != 0)
						{
							item.Owner = null;
						}
					}
					else
					{
						s.Delete(item);
					}
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(5));
				Assert.That(turin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.Settings), Is.False);

				if (initialize)
				{
					using (var e = gavin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}
					using (var e = turin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(turin.Settings), Is.True);
					Assert.That(gavin.Settings.Count, Is.EqualTo(5));
					Assert.That(turin.Settings.Count, Is.EqualTo(10));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");
				Assert.That(gavin.Settings.Count, Is.EqualTo(5));
				Assert.That(turin.Settings.Count, Is.EqualTo(10));

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListInsertChildSaveChangeParent(bool initialize)
		{
			User gavin;
			User turin;
			var gavinItems = new List<Company>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				s.Persist(gavin);
				s.Persist(turin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"g{i}", i, gavin);
					gavinItems.Add(item);
					gavin.Companies.Add(item);

					item = new Company($"t{i}", i, turin);
					turin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(turin.Companies.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.Companies), Is.False);

				// Save companies and then add them to the collection
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					s.Save(item);
					gavinItems.Add(item);
				}

				for (var i = 5; i < 10; i++)
				{
					if (i % 2 != 0)
					{
						gavin.Companies.Insert(i, gavinItems[i]);
					}
					else
					{
						gavin.Companies.Add(gavinItems[i]);
					}
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Add companies to the collection and then save them
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					if (i % 2 != 0)
					{
						gavin.Companies.Insert(i, item);
					}
					else
					{
						gavin.Companies.Add(item);
					}

					gavinItems.Add(item);
					s.Save(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove added items from the collection and add them to a different parent
				foreach (var item in gavinItems.Skip(5).Take(5))
				{
					gavin.Companies.RemoveAt(5);

					item.Owner = turin;
					turin.Companies.Insert(item.ListIndex, item);
				}

				// Remove added items from the collection
				for (var i = 10; i < 15; i++)
				{
					var item = gavinItems[i];
					gavin.Companies.RemoveAt(5);
					// When identity is used for the primary key the item will be already inserted in the database,
					// so the RemoveAt method will mark it as an orphan which will be deleted on flush.
					// The same would work for an initialized collection as the collection snapshot would contain the item.
					// When dealing with an id generator that supports a delayed insert, we have to trigger a delete
					// for the item as it is currently scheduled for insertion.
					if (Dialect.SupportsIdentityColumns)
					{
						if (i % 2 != 0)
						{
							item.Owner = null;
						}
					}
					else
					{
						s.Delete(item);
					}
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(turin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.Companies), Is.False);

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}
					using (var e = turin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(turin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(5));
					Assert.That(turin.Companies.Count, Is.EqualTo(10));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(turin.Companies.Count, Is.EqualTo(10));

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListClearChildSaveChangeParent(bool initialize)
		{
			User gavin;
			User turin;
			var gavinItems = new List<CreditCard>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				s.Persist(gavin);
				s.Persist(turin);

				for (var i = 0; i < 5; i++)
				{
					var item = new CreditCard($"g{i}", i, gavin);
					gavin.CreditCards.Add(item);

					item = new CreditCard($"t{i}", i, turin);
					turin.CreditCards.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.CreditCards.Count, Is.EqualTo(5));
				Assert.That(turin.CreditCards.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.CreditCards), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.CreditCards), Is.False);

				Sfi.Statistics.Clear();
				gavin.CreditCards.Clear();
				turin.CreditCards.Clear();
				Assert.That(gavin.CreditCards.Count, Is.EqualTo(0));
				Assert.That(turin.CreditCards.Count, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.CreditCards), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.CreditCards), Is.False);

				// Save credit cards and then add them to the collection
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var item = new CreditCard($"c2{i}", i, gavin);
					s.Save(item);
					gavinItems.Add(item);
				}

				for (var i = 0; i < 5; i++)
				{
					gavin.CreditCards.Add(gavinItems[i]);
				}

				Assert.That(gavin.CreditCards.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.CreditCards), Is.False);

				// Add credit cards to the collection and then save them
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new CreditCard($"c2{i}", i, gavin);
					Assert.That(((IList) gavin.CreditCards).Add(item), Is.EqualTo(i));
					gavinItems.Add(item);
					s.Save(item);
				}

				Assert.That(gavin.CreditCards.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.CreditCards), Is.False);

				// Remove added items from the collection and add them to a different parent
				foreach (var item in gavinItems.Take(5))
				{
					gavin.CreditCards.Remove(item);

					item.Owner = turin;
					item.ListIndex += 5;
					turin.CreditCards.Add(item);
				}

				Assert.That(gavin.CreditCards.Count, Is.EqualTo(5));
				Assert.That(turin.CreditCards.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.CreditCards), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.CreditCards), Is.False);

				if (initialize)
				{
					using (var e = gavin.CreditCards.GetEnumerator())
					{
						e.MoveNext();
					}
					using (var e = turin.CreditCards.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.CreditCards), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(turin.CreditCards), Is.True);
					Assert.That(gavin.CreditCards.Count, Is.EqualTo(5));
					Assert.That(turin.CreditCards.Count, Is.EqualTo(5));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");
				Assert.That(gavin.CreditCards.Count, Is.EqualTo(10));
				Assert.That(turin.CreditCards.Count, Is.EqualTo(10));

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListAddChildSaveChangeParent(bool initialize)
		{
			User gavin;
			User turin;
			var gavinItems = new List<Company>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				s.Persist(gavin);
				s.Persist(turin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"g{i}", i, gavin);
					gavinItems.Add(item);
					gavin.Companies.Add(item);

					item = new Company($"t{i}", i, turin);
					turin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");

				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(turin.Companies.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.Companies), Is.False);

				// Save companies and then add them to the collection
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					s.Save(item);
					gavinItems.Add(item);
				}

				for (var i = 5; i < 10; i++)
				{
					gavin.Companies.Add(gavinItems[i]);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Add companies to the collection and then save them
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					Assert.That(((IList) gavin.Companies).Add(item), Is.EqualTo(i));
					gavinItems.Add(item);
					s.Save(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove added items from the collection and add them to a different parent
				foreach (var item in gavinItems.Skip(5).Take(5))
				{
					gavin.Companies.Remove(item);

					item.Owner = turin;
					turin.Companies.Add(item);
				}

				// Remove added items from the collection
				for (var i = 10; i < 15; i++)
				{
					var item = gavinItems[i];
					gavin.Companies.Remove(item);
					// When identity is used for the primary key the item will be already inserted in the database,
					// so the RemoveAt method will mark it as an orphan which will be deleted on flush.
					// The same would work for an initialized collection as the collection snapshot would contain the item.
					// When dealing with an id generator that supports a delayed insert, we have to trigger a delete
					// for the item as it is currently scheduled for insertion.
					if (Dialect.SupportsIdentityColumns)
					{
						if (i % 2 != 0)
						{
							item.Owner = null;
						}
					}
					else
					{
						s.Delete(item);
					}
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(turin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(turin.Companies), Is.False);

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}
					using (var e = turin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(turin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(5));
					Assert.That(turin.Companies.Count, Is.EqualTo(10));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				turin = s.Get<User>("turin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(turin.Companies.Count, Is.EqualTo(10));

				t.Commit();
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ListAddChildSave(bool initialize)
		{
			User gavin;
			var gavinItems = new List<Company>();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				s.Persist(gavin);

				for (var i = 0; i < 5; i++)
				{
					var item = new Company($"g{i}", i, gavin);
					gavinItems.Add(item);
					gavin.Companies.Add(item);
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");

				Sfi.Statistics.Clear();

				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Save companies and then add them to the collection
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					s.Save(item);
					gavinItems.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(Dialect.SupportsIdentityColumns ? 10 : 5));

				for (var i = 5; i < 10; i++)
				{
					gavin.Companies.Add(gavinItems[i]);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Add companies to the collection and then save them
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					Assert.That(((IList) gavinItems).Add(item), Is.EqualTo(i));
					gavin.Companies.Add(item);
					s.Save(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

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
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test adding companies with ICollection interface
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test adding companies with IList interface
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					Assert.That(((IList)addedItems).Add(item), Is.EqualTo(i));
					gavin.Companies.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);


				// Check existance of added companies
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Skip(5))
				{
					Assert.That(gavin.Companies.Contains(item), Is.True);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Check existance of not loaded companies
				Assert.That(gavin.Companies.Contains(addedItems[0]), Is.True);
				Assert.That(gavin.Companies.Contains(addedItems[1]), Is.True);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Check existance of not existing companies
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Contains(new Company("test1", 15, gavin)), Is.False);
				Assert.That(gavin.Companies.Contains(new Company("test2", 16, gavin)), Is.False);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				t.Commit();
			}
		}

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

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));

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
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Readd items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					gavin.Companies.Add(addedItems[i]);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				if (flush)
				{
					s.Flush();
					Assert.That(gavin.Companies.Count, Is.EqualTo(5));
					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(flush ? 5 : 10));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

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
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test inserting companies at the start
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Insert(0, item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test inserting companies at the end
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Insert(i, item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Try insert invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies.RemoveAt(-1));
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies.RemoveAt(20));

				// Check existance of added companies
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Skip(5))
				{
					Assert.That(gavin.Companies.Contains(item), Is.True);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Check existance of not loaded companies
				Assert.That(gavin.Companies.Contains(addedItems[0]), Is.True);
				Assert.That(gavin.Companies.Contains(addedItems[1]), Is.True);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Check existance of not existing companies
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Contains(new Company("test1", 15, gavin)), Is.False);
				Assert.That(gavin.Companies.Contains(new Company("test2", 16, gavin)), Is.False);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				t.Commit();
			}
		}

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

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));

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
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Readd items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					gavin.Companies.Insert(4 - i, addedItems[i]);
				}

				Assert.That(gavin.Companies[0].ListIndex, Is.EqualTo(4));
				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				if (flush)
				{
					s.Flush();
					Assert.That(gavin.Companies.Count, Is.EqualTo(5));
					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(flush ? 5 : 10));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

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

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove transient companies
				Sfi.Statistics.Clear();
				gavin.Companies.RemoveAt(5);
				gavin.Companies.RemoveAt(6);

				Assert.That(gavin.Companies.Count, Is.EqualTo(8));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove persisted companies
				Sfi.Statistics.Clear();
				gavin.Companies.RemoveAt(3);
				gavin.Companies.RemoveAt(3);

				Assert.That(gavin.Companies.Count, Is.EqualTo(6));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Try remove invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies.RemoveAt(-1));
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies.RemoveAt(8));

				// Check existance of companies
				Sfi.Statistics.Clear();
				var removedIndexes = new HashSet<int> {3, 4, 5, 7};
				for (var i = 0; i < addedItems.Count; i++)
				{
					Assert.That(
						gavin.Companies.Contains(addedItems[i]),
						removedIndexes.Contains(i) ? Is.False : (IResolveConstraint) Is.True,
						$"Element at index {i} was {(removedIndexes.Contains(i) ? "not " : "")}removed");
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Check existance of not existing companies
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Contains(new Company("test1", 15, gavin)), Is.False);
				Assert.That(gavin.Companies.Contains(new Company("test2", 16, gavin)), Is.False);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				gavin.UpdateCompaniesIndexes();

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(6));
					Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(6));
				Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);

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

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Compare all items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 10; i++)
				{
					Assert.That(gavin.Companies[i], Is.EqualTo(addedItems[i]));
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Try get invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					var item = gavin.Companies[10];
				});
				Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					var item = gavin.Companies[-1];
				});

				// Try set invalid indexes
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies[10] = addedItems[0]);
				Assert.Throws<ArgumentOutOfRangeException>(() => gavin.Companies[-1] = addedItems[0]);

				// Swap transient and persisted indexes
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var hiIndex = 9 - i;
					var tmp = gavin.Companies[i];
					gavin.Companies[i] = gavin.Companies[hiIndex];
					gavin.Companies[hiIndex] = tmp;
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(10));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Check indexes
				Sfi.Statistics.Clear();
				for (var i = 0; i < 10; i++)
				{
					Assert.That(gavin.Companies[i].ListIndex, Is.EqualTo(finalIndexOrder[i]));
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				gavin.UpdateCompaniesIndexes();

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(10));
					Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);

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

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

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

					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "INSERT \n    INTO"), Is.EqualTo(5));
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Add transient companies with Add
				Sfi.Statistics.Clear();
				for (var i = 15; i < 20; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems.Add(item);
					gavin.Companies.Add(item);
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(20));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove last 5 transient companies
				Sfi.Statistics.Clear();
				using (var sqlLog = new SqlLogSpy())
				{
					for (var i = 15; i < 20; i++)
					{
						Assert.That(gavin.Companies.Remove(addedItems[i]), Is.True);
					}

					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "INSERT \n    INTO"), Is.EqualTo(10));
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove last 5 transient companies
				Sfi.Statistics.Clear();
				using (var sqlLog = new SqlLogSpy())
				{
					for (var i = 10; i < 15; i++)
					{
						gavin.Companies.RemoveAt(10);
					}

					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "DELETE \n    FROM"), Is.EqualTo(5));
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(7));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Add transient companies with Add
				Sfi.Statistics.Clear();
				for (var i = 10; i < 15; i++)
				{
					var item = new Company($"c{i}", i, gavin);
					addedItems[i] = item;
					Assert.That(((IList)gavin.Companies).Add(item), Is.EqualTo(i));
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove last transient company
				Sfi.Statistics.Clear();
				using (var sqlLog = new SqlLogSpy())
				{
					Assert.That(gavin.Companies.Remove(addedItems[14]), Is.EqualTo(true));
					var log = sqlLog.GetWholeLog();
					Assert.That(FindAllOccurrences(log, "DELETE \n    FROM"), Is.EqualTo(5));
					Assert.That(FindAllOccurrences(log, "INSERT \n    INTO"), Is.EqualTo(5));
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(14));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test index getter
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies[0], Is.EqualTo(addedItems[0]));

				Assert.That(gavin.Companies.Count, Is.EqualTo(14));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Remove last transient company
				Sfi.Statistics.Clear();
				Assert.That(gavin.Companies.Remove(addedItems[13]), Is.EqualTo(true));

				Assert.That(gavin.Companies.Count, Is.EqualTo(13));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test index setter
				Sfi.Statistics.Clear();
				gavin.Companies[0] = addedItems[0];

				Assert.That(gavin.Companies.Count, Is.EqualTo(13));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test manual flush after remove
				Sfi.Statistics.Clear();
				gavin.Companies.RemoveAt(12);
				using (var sqlLog = new SqlLogSpy())
				{
					s.Flush();
					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "DELETE \n    FROM"), Is.EqualTo(1));
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(12));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				// Test manual flush after insert
				Sfi.Statistics.Clear();
				gavin.Companies.Add(new Company($"c{12}", 12, gavin));
				using (var sqlLog = new SqlLogSpy())
				{
					s.Flush();
					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "INSERT \n    INTO"), Is.EqualTo(1));
				}

				Assert.That(gavin.Companies.Count, Is.EqualTo(13));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				for (var i = 0; i < gavin.Companies.Count; i++)
				{
					Assert.That(gavin.Companies[i].ListIndex, Is.EqualTo(i));
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(13));
					Assert.That(gavin.Companies.Select(o => o.ListIndex), Is.EquivalentTo(finalIndexOrder));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(13));
				Assert.That(gavin.Companies.Select(o => o.ListIndex), Is.EquivalentTo(finalIndexOrder));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);

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

				// Add transient permissions
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					var item = new CreditCard($"c{i}", i, gavin);
					addedItems.Add(item);
					collection.Insert(i, item);
				}

				Assert.That(collection.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				Sfi.Statistics.Clear();
				collection.Clear();

				Assert.That(collection.Count, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				// Readd two not loaded and two transient permissions
				collection.Add(addedItems[0]);
				collection.Add(addedItems[1]);
				collection.Add(addedItems[5]);
				collection.Add(addedItems[6]);

				Assert.That(collection.Count, Is.EqualTo(4));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				// Remove one not loaded and one transient permissions
				Assert.That(collection.Remove(addedItems[1]), Is.True);
				Assert.That(collection.Remove(addedItems[6]), Is.True);

				Assert.That(collection.Count, Is.EqualTo(2));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				// Remove not existing items
				Assert.That(collection.Remove(addedItems[1]), Is.False);
				Assert.That(collection.Remove(addedItems[6]), Is.False);

				Assert.That(collection.Count, Is.EqualTo(2));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				if (initialize)
				{
					using (var e = collection.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(collection), Is.True);
					Assert.That(collection.Count, Is.EqualTo(2));
				}


				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				var collection = gavin.CreditCards;
				// As the cascade option is set to all, the clear operation will only work on
				// transient permissions
				Assert.That(collection.Count, Is.EqualTo(6));
				for (var i = 0; i < 10; i++)
				{
					Assert.That(collection.Contains(addedItems[i]), i < 6 ? Is.True : (IResolveConstraint) Is.False);
				}

				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

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
				Assert.That(gavin.Companies.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

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

				Assert.That(gavin.Companies.Count, Is.EqualTo(3));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(3));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.False);

				gavin.UpdateCompaniesIndexes();

				for (var i = 0; i < gavin.Companies.Count; i++)
				{
					Assert.That(gavin.Companies[i].OriginalIndex, Is.EqualTo(finalIndexOrder[i]));
				}

				if (initialize)
				{
					using (var e = gavin.Companies.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);
					Assert.That(gavin.Companies.Count, Is.EqualTo(3));
					Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Companies.Count, Is.EqualTo(3));
				Assert.That(gavin.Companies.Select(o => o.OriginalIndex), Is.EquivalentTo(finalIndexOrder));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Companies), Is.True);

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
				Assert.That(gavin.Documents.Count, Is.EqualTo(2));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test adding documents with ISet interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"document{i}", $"content{i}", gavin);
					addedDocuments.Add(document);
					Assert.That(gavin.Documents.Add(document), Is.True);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(7));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test adding documents with ICollection interface
				Sfi.Statistics.Clear();
				var documents = (ICollection<Document>) gavin.Documents;
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"document2{i}", $"content{i}", gavin);
					addedDocuments.Add(document);
					documents.Add(document);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(12));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				// In this case we cannot determine whether the entities are transient or not so
				// we are forced to check the database
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test readding documents with ISet interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Add(addedDocuments[i]), Is.False);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(12));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test readding documents with ICollection interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					documents.Add(addedDocuments[i]);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(12));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Check existance of added documents
				Sfi.Statistics.Clear();
				foreach (var document in addedDocuments)
				{
					Assert.That(gavin.Documents.Contains(document), Is.True);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Check existance of not loaded documents
				Assert.That(gavin.Documents.Contains(hia), Is.True);
				Assert.That(gavin.Documents.Contains(hia2), Is.True);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Check existance of not existing documents
				Sfi.Statistics.Clear();
				Assert.That(gavin.Documents.Contains(new Document("test1", "content", gavin)), Is.False);
				Assert.That(gavin.Documents.Contains(new Document("test2", "content", gavin)), Is.False);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test adding not loaded documents
				Sfi.Statistics.Clear();
				Assert.That(gavin.Documents.Add(hia), Is.False);
				documents.Add(hia);

				Assert.That(gavin.Documents.Count, Is.EqualTo(12));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				if (initialize)
				{
					using (var e = gavin.Documents.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True);
					Assert.That(gavin.Documents.Count, Is.EqualTo(12));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents.Count, Is.EqualTo(12));
				Assert.That(gavin.Documents.Contains(hia2), Is.True);
				Assert.That(gavin.Documents.Contains(hia), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				t.Commit();
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

				Assert.That(gavin.Documents.Count, Is.EqualTo(5));

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
				Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Readd items
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Add(addedItems[i]), Is.False);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				if (flush)
				{
					s.Flush();
					Assert.That(gavin.Documents.Count, Is.EqualTo(5));
					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);
				}

				if (initialize)
				{
					using (var e = gavin.Documents.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True);
					Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

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
				Assert.That(gavin.Permissions.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				// Test adding permissions with ICollection interface
				Sfi.Statistics.Clear();
				var items = (ICollection<UserPermission>) gavin.Permissions;
				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p2{i}", gavin);
					addedItems.Add(item);
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				// Test readding permissions with ICollection interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Skip(5))
				{
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				// Test adding not loaded permissions with ICollection interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Take(5))
				{
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				// Test adding loaded permissions with ICollection interface
				Sfi.Statistics.Clear();
				foreach (var item in s.Query<UserPermission>())
				{
					items.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(6));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);


				// Test adding permissions with ISet interface
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p3{i}", gavin);
					addedItems.Add(item);
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				// Test readding permissions with ISet interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Skip(10))
				{
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				// Test adding not loaded permissions with ISet interface
				Sfi.Statistics.Clear();
				foreach (var item in addedItems.Take(5))
				{
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				// Test adding loaded permissions with ISet interface
				Sfi.Statistics.Clear();
				foreach (var item in s.Query<UserPermission>())
				{
					gavin.Permissions.Add(item);
				}

				Assert.That(gavin.Permissions.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(6));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

				if (initialize)
				{
					using (var e = gavin.Permissions.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.True);
					Assert.That(gavin.Permissions.Count, Is.EqualTo(15));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Permissions.Count, Is.EqualTo(15));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Permissions), Is.False);

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
				Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Add new documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"document2{i}", $"content{i}", gavin);
					addedDocuments.Add(document);
					((ICollection<Document>)gavin.Documents).Add(document);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test removing existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.True);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test removing removed existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					Assert.That(gavin.Documents.Contains(addedDocuments[i]), Is.False);
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.False);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test removing not existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"test{i}", "content", gavin);
					Assert.That(gavin.Documents.Remove(document), Is.False);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test removing newly added documents
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					Assert.That(gavin.Documents.Contains(addedDocuments[i]), Is.True);
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.True);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test removing removed newly added documents
				Sfi.Statistics.Clear();
				for (var i = 5; i < 10; i++)
				{
					Assert.That(gavin.Documents.Contains(addedDocuments[i]), Is.False);
					Assert.That(gavin.Documents.Remove(addedDocuments[i]), Is.False);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				// Test removing not existing documents
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var document = new Document($"test{i}", "content", gavin);
					Assert.That(gavin.Documents.Remove(document), Is.False);
				}

				Assert.That(gavin.Documents.Count, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

				if (initialize)
				{
					using (var e = gavin.Documents.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.True);
					Assert.That(gavin.Documents.Count, Is.EqualTo(0));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Documents.Count, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Documents), Is.False);

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
				Assert.That(collection.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				// Add transient permissions
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					var item = new UserPermission($"p2{i}", gavin);
					addedItems.Add(item);
					collection.Add(item);
				}

				Assert.That(collection.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				Sfi.Statistics.Clear();
				collection.Clear();

				Assert.That(collection.Count, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				// Readd two not loaded and two transient permissions
				Assert.That(collection.Add(addedItems[0]), Is.True);
				Assert.That(collection.Add(addedItems[1]), Is.True);
				Assert.That(collection.Add(addedItems[5]), Is.True);
				Assert.That(collection.Add(addedItems[6]), Is.True);

				Assert.That(collection.Count, Is.EqualTo(4));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				// Remove one not loaded and one transient permissions
				Assert.That(collection.Remove(addedItems[1]), Is.True);
				Assert.That(collection.Remove(addedItems[6]), Is.True);

				Assert.That(collection.Count, Is.EqualTo(2));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				// Remove not existing items
				Assert.That(collection.Remove(addedItems[1]), Is.False);
				Assert.That(collection.Remove(addedItems[6]), Is.False);

				Assert.That(collection.Count, Is.EqualTo(2));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

				if (initialize)
				{
					using (var e = collection.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(collection), Is.True);
					Assert.That(collection.Count, Is.EqualTo(2));
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
				Assert.That(collection.Count, Is.EqualTo(6));
				for (var i = 0; i < 10; i++)
				{
					Assert.That(collection.Contains(addedItems[i]), i < 6 ? Is.True : (IResolveConstraint) Is.False);
				}

				Assert.That(NHibernateUtil.IsInitialized(collection), Is.False);

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
				Assert.That(gavin.Settings.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Test adding settings with Add method
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					addedSettings.Add(setting);
					gavin.Settings.Add(setting.Name, setting);
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Test adding settings with []
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s3{i}", $"data{i}", gavin);
					addedSettings.Add(setting);

					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(15));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Check existance of added settings
				Sfi.Statistics.Clear();
				foreach (var item in addedSettings.Skip(5))
				{
					Assert.That(gavin.Settings.ContainsKey(item.Name), Is.True);
					Assert.That(gavin.Settings.Contains(new KeyValuePair<string, UserSetting>(item.Name, item)), Is.True);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Check existance of not loaded settings
				foreach (var item in addedSettings.Take(5))
				{
					Assert.That(gavin.Settings.ContainsKey(item.Name), Is.True);
				}

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Check existance of not existing settings
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.ContainsKey("test"), Is.False);
				Assert.That(gavin.Settings.ContainsKey("test2"), Is.False);

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Try to add an existing setting
				Assert.Throws<ArgumentException>(() => gavin.Settings.Add("s0", new UserSetting("s0", "data", gavin)));
				Assert.Throws<ArgumentException>(() => gavin.Settings.Add("s20", new UserSetting("s20", "data", gavin)));
				Assert.Throws<ArgumentException>(() => gavin.Settings.Add("s30", new UserSetting("s30", "data", gavin)));

				// Get values of not loaded keys
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.TryGetValue("s0", out setting), Is.True);
				Assert.That(setting.Id, Is.EqualTo(addedSettings[0].Id));
				Assert.That(gavin.Settings["s0"].Id, Is.EqualTo(addedSettings[0].Id));

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Get values of newly added keys
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.TryGetValue("s20", out setting), Is.True);
				Assert.That(setting, Is.EqualTo(addedSettings[5]));
				Assert.That(gavin.Settings["s20"], Is.EqualTo(addedSettings[5]));
				Assert.That(gavin.Settings.TryGetValue("s30", out setting), Is.True);
				Assert.That(setting, Is.EqualTo(addedSettings[10]));
				Assert.That(gavin.Settings["s30"], Is.EqualTo(addedSettings[10]));

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Try to get a non existing setting
				Assert.That(gavin.Settings.TryGetValue("test", out setting), Is.False);
				Assert.That(gavin.Settings.TryGetValue("test2", out setting), Is.False);
				Assert.Throws<KeyNotFoundException>(() =>
				{
					setting = gavin.Settings["test"];
				});
				Assert.Throws<KeyNotFoundException>(() =>
				{
					setting = gavin.Settings["test2"];
				});

				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(4));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				if (initialize)
				{
					using (var e = gavin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.True);
					Assert.That(gavin.Settings.Count, Is.EqualTo(15));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Settings.Count, Is.EqualTo(15));
				Assert.That(gavin.Settings.ContainsKey(addedSettings[0].Name), Is.True);
				Assert.That(gavin.Settings.ContainsKey(addedSettings[5].Name), Is.True);
				Assert.That(gavin.Settings.ContainsKey(addedSettings[10].Name), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

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
				Assert.That(gavin.Settings.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Set a key that does not exist in db and it is not in the queue
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Set a key that does not exist in db and it is in the queue
				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Set a key that exists in db and it is not in the queue
				Sfi.Statistics.Clear();
				gavin.Settings["s0"] = new UserSetting("s0", "s0", gavin);

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Set a key that exists in db and it is in the queue
				Sfi.Statistics.Clear();
				gavin.Settings["s0"] = new UserSetting("s0", "s0", gavin);

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Set a key that exists in db and it is in the removal queue
				Assert.That(gavin.Settings.Remove("s1"), Is.True);
				Sfi.Statistics.Clear();
				gavin.Settings["s1"] = new UserSetting("s1", "s1", gavin);

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				if (initialize)
				{
					using (var e = gavin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.True);
					Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

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
				Assert.That(gavin.Settings.Count, Is.EqualTo(5));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				Sfi.Statistics.Clear();
				for (var i = 0; i < 5; i++)
				{
					setting = new UserSetting($"s2{i}", $"data{i}", gavin);
					gavin.Settings[setting.Name] = setting;
				}

				Assert.That(gavin.Settings.Count, Is.EqualTo(10));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(5));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Remove a key that exists in db and it is not in the queue and removal queue
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s0"), Is.True);

				Assert.That(gavin.Settings.Count, Is.EqualTo(9));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Remove a key that exists in db and it is in the queue
				var item = gavin.Settings["s1"];
				Assert.That(gavin.Settings.Remove("s1"), Is.True);
				gavin.Settings.Add(item.Name, item);
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s1"), Is.True);

				Assert.That(gavin.Settings.Count, Is.EqualTo(8));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Remove a key that does not exist in db and it is not in the queue
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("test"), Is.False);

				Assert.That(gavin.Settings.Count, Is.EqualTo(8));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Remove a key that does not exist in db and it is in the queue
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s20"), Is.True);

				Assert.That(gavin.Settings.Count, Is.EqualTo(7));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				// Remove a key that exists in db and it is in the removal queue
				Assert.That(gavin.Settings.Remove("s2"), Is.True);
				Sfi.Statistics.Clear();
				Assert.That(gavin.Settings.Remove("s2"), Is.False);

				Assert.That(gavin.Settings.Count, Is.EqualTo(6));
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

				if (initialize)
				{
					using (var e = gavin.Settings.GetEnumerator())
					{
						e.MoveNext();
					}

					Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.True);
					Assert.That(gavin.Settings.Count, Is.EqualTo(6));
				}

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = s.Get<User>("gavin");
				Assert.That(gavin.Settings.Count, Is.EqualTo(6));
				Assert.That(NHibernateUtil.IsInitialized(gavin.Settings), Is.False);

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
