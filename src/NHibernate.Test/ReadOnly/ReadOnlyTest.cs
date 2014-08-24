using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.ReadOnly
{
	[TestFixture]
	public class ReadOnlyTest : AbstractReadOnlyTest
	{
		protected override IList Mappings
		{
			get
			{
				var mappings = new List<string> { "ReadOnly.DataPoint.hbm.xml" };

				if (TextHolder.SupportedForDialect(Dialect))
					mappings.Add("ReadOnly.TextHolder.hbm.xml");

				return mappings;
			}
		}
		
		[Test]
		public void ReadOnlyOnProxies()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			s.BeginTransaction();
			DataPoint dp = new DataPoint();
			dp.X = 0.1M;
			dp.Y = (decimal)System.Math.Cos((double)dp.X);
			dp.Description = "original";
			s.Save(dp);
			long dpId = dp.Id;
			s.Transaction.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Load<DataPoint>(dpId);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False, "was initialized");
			s.SetReadOnly(dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False, "was initialized during SetReadOnly");
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True, "was not initialized during mod");
			Assert.That(dp.Description, Is.EqualTo("changed"), "desc not changed in memory");
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			IList list = s.CreateQuery("from DataPoint where Description = 'changed'").List();
			Assert.That(list.Count, Is.EqualTo(0), "change written to database");
			Assert.That(s.CreateQuery("delete from DataPoint").ExecuteUpdate(), Is.EqualTo(1));
			s.Transaction.Commit();
			s.Close();
			
			AssertUpdateCount(0);
			//deletes from Query.executeUpdate() are not tracked
			//AssertDeleteCount(1);
		}
	
		public void ReadOnlyMode()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			for (int i = 0; i < 100; i++)
			{
				DataPoint dp = new DataPoint();
				dp.X = i * 0.1M;
				dp.Y = (decimal)System.Math.Cos((double)dp.X);
				s.Save(dp);
			}
			t.Commit();
			s.Close();
	
			AssertInsertCount(100);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();

			// NH-specific: Replace use of Scroll with List
			IList<DataPoint> sr = s.CreateQuery("from DataPoint dp order by dp.X asc")
					.SetReadOnly(true)
					.List<DataPoint>();
			
			int index = 0;

			foreach (DataPoint dp in sr)
			{
				if (++index == 50)
				{
					s.SetReadOnly(dp, false);
				}
				dp.Description = "done!";
			}
			t.Commit();
	
			AssertUpdateCount(1);
			ClearCounts();
	
			s.Clear();
			t = s.BeginTransaction();
			IList single = s.CreateQuery("from DataPoint where description='done!'").List();
			Assert.That(single.Count, Is.EqualTo(1));
			Assert.That(s.CreateQuery("delete from DataPoint").ExecuteUpdate(), Is.EqualTo(100));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			//deletes from Query.executeUpdate() are not tracked
			//AssertDeleteCount(100);
		}
	
		[Test]
		public void ReadOnlyModeAutoFlushOnQuery()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			for (int i = 0; i < 100; i++)
			{
				DataPoint dp = new DataPoint();
				dp.X = i * 0.1M;
				dp.Y = (decimal)System.Math.Cos((double)dp.X);
				s.Save(dp);
			}
	
			AssertInsertCount(0);
			AssertUpdateCount(0);

			// NH-specific: Replace use of Scroll with List
			IList<DataPoint> sr = s.CreateQuery("from DataPoint dp order by dp.X asc")
					.SetReadOnly(true)
					.List<DataPoint>();
	
			AssertInsertCount(100);
			AssertUpdateCount(0);
			ClearCounts();
	
			foreach(DataPoint dp in sr)
			{
				Assert.That(s.IsReadOnly(dp), Is.False);
				s.Delete(dp);
			}
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(100);
		}
	
		[Test]
		public void SaveReadOnlyModifyInSaveTransaction()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = new DataPoint();
			dp.Description = "original";
			dp.X = 0.1M;
			dp.Y = (decimal)System.Math.Cos((double)dp.X);
			s.Save(dp);
			s.SetReadOnly(dp, true);
			dp.Description = "different";
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			s.SetReadOnly(dp, true);
			Assert.That(dp.Description, Is.EqualTo("original"));
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Refresh(dp);
			Assert.That(dp.Description, Is.EqualTo("original"));
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			t.Commit();
	
			AssertInsertCount(0);
			AssertUpdateCount(0);
	
			s.Clear();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			Assert.That(dp.Description, Is.EqualTo("original"));
			s.Delete(dp);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
			ClearCounts();
		}
	
		[Test]
		public void ReadOnlyRefresh()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = new DataPoint();
			dp.Description = "original";
			dp.X = 0.1M;
			dp.Y = (decimal)System.Math.Cos((double)dp.X);
			s.Save(dp);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			s.SetReadOnly(dp, true);
			Assert.That(dp.Description, Is.EqualTo("original"));
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Refresh(dp);
			Assert.That(dp.Description, Is.EqualTo("original"));
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			t.Commit();
	
			AssertInsertCount(0);
			AssertUpdateCount(0);
	
			s.Clear();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			Assert.That(dp.Description, Is.EqualTo("original"));
			s.Delete(dp);;
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
			ClearCounts();
		}
	
		[Test]
		public void ReadOnlyRefreshDetached()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = new DataPoint();
			dp.Description = "original";
			dp.X = 0.1M;
			dp.Y = (decimal)System.Math.Cos((double)dp.X);
			s.Save(dp);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Refresh(dp);
			Assert.That(dp.Description, Is.EqualTo("original"));
			Assert.That(s.IsReadOnly(dp), Is.False);
			s.SetReadOnly(dp, true);
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Evict(dp);
			s.Refresh(dp);
			Assert.That(dp.Description, Is.EqualTo("original"));
			Assert.That(s.IsReadOnly(dp), Is.False);
			t.Commit();
	
			AssertInsertCount(0);
			AssertUpdateCount(0);
	
			s.Clear();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			Assert.That(dp.Description, Is.EqualTo("original"));
			s.Delete(dp);;
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void ReadOnlyDelete()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = new DataPoint();
			dp.X = 0.1M;
			dp.Y = (decimal)System.Math.Cos((double)dp.X);
			s.Save(dp);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			s.SetReadOnly(dp, true);
			s.Delete(dp);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
	
			s = OpenSession();
			t = s.BeginTransaction();
			IList list = s.CreateQuery("from DataPoint where Description='done!'").List();
			Assert.That(list.Count, Is.EqualTo(0));
			t.Commit();
			s.Close();
		}

		[Test]
		public void ReadOnlyGetModifyAndDelete()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = new DataPoint();
			dp.X = 0.1M;
			dp.Y = (decimal)System.Math.Cos((double)dp.X);
			s.Save(dp);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			s.SetReadOnly(dp, true);
			dp.Description = "a DataPoint";
			s.Delete(dp);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			IList list = s.CreateQuery("from DataPoint where Description='done!'").List();
			Assert.That(list.Count, Is.EqualTo(0));
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyModeWithExistingModifiableEntity()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = null;
			for (int i = 0; i < 100; i++)
			{
				dp = new DataPoint();
				dp.X = i * 0.1M;
				dp.Y = (decimal)System.Math.Cos((double)dp.X);
				s.Save(dp);
			}
			t.Commit();
			s.Close();
	
			AssertInsertCount(100);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			DataPoint dpLast = s.Get<DataPoint>(dp.Id);
			Assert.That(s.IsReadOnly(dpLast), Is.False);
			
			// NH-specific: Replace use of Scroll with List
			IList<DataPoint> sr = s.CreateQuery("from DataPoint dp order by dp.X asc")
					.SetReadOnly(true)
					.List<DataPoint>();
			
			int nExpectedChanges = 0;
			int index = 0;
			
			foreach(DataPoint nextDp in sr)
			{
				if (nextDp.Id == dpLast.Id)
				{
					//dpLast existed in the session before executing the read-only query
					Assert.That(s.IsReadOnly(nextDp), Is.False);
				}
				else
				{
					Assert.That(s.IsReadOnly(nextDp), Is.True);
				}
				if (++index == 50)
				{
					s.SetReadOnly(nextDp, false);
					nExpectedChanges = (nextDp == dpLast ? 1 : 2);
				}
				nextDp.Description = "done!";
			}
			t.Commit();
			s.Clear();
	
			AssertInsertCount(0);
			AssertUpdateCount(nExpectedChanges);
			ClearCounts();
	
			t = s.BeginTransaction();
			IList list = s.CreateQuery("from DataPoint where Description='done!'").List();
			Assert.That(list.Count, Is.EqualTo(nExpectedChanges));
			Assert.That(s.CreateQuery("delete from DataPoint").ExecuteUpdate(), Is.EqualTo(100));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
		}
	
		[Test]
		public void ModifiableModeWithExistingReadOnlyEntity()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = null;
			for (int i = 0; i < 100; i++)
			{
				dp = new DataPoint();
				dp.X = i * 0.1M;
				dp.Y = (decimal)System.Math.Cos((double)dp.X);
				s.Save(dp);
			}
			t.Commit();
			s.Close();
	
			AssertInsertCount(100);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			DataPoint dpLast = s.Get<DataPoint>(dp.Id);
			Assert.That(s.IsReadOnly(dpLast), Is.False);
			s.SetReadOnly(dpLast, true);
			Assert.That(s.IsReadOnly(dpLast), Is.True);
			dpLast.Description = "oy";

			AssertUpdateCount(0);
	
			// NH-specific: Replace use of Scroll with List
			IList<DataPoint> sr = s.CreateQuery("from DataPoint dp order by dp.X asc")
					.SetReadOnly(false)
					.List<DataPoint>();

			int nExpectedChanges = 0;
			int index = 0;
			
			foreach(DataPoint nextDp in sr)
			{
				if (nextDp.Id == dpLast.Id )
				{
					//dpLast existed in the session before executing the read-only query
					Assert.That(s.IsReadOnly(nextDp), Is.True);
				}
				else
				{
					Assert.That(s.IsReadOnly(nextDp), Is.False);
				}
				if (++index == 50)
				{
					s.SetReadOnly(nextDp, true);
					nExpectedChanges = (nextDp == dpLast ? 99 : 98);
				}
				nextDp.Description = "done!";
			}
			t.Commit();
			s.Clear();
	
			AssertUpdateCount(nExpectedChanges);
			ClearCounts();
	
			t = s.BeginTransaction();
			IList list = s.CreateQuery("from DataPoint where Description='done!'").List();
			Assert.That(list.Count, Is.EqualTo(nExpectedChanges));
			Assert.That(s.CreateQuery("delete from DataPoint").ExecuteUpdate(), Is.EqualTo(100));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
		}
	
		[Test]
		public void ReadOnlyOnTextType()
		{
			if (!TextHolder.SupportedForDialect(Dialect))
				Assert.Ignore("Dialect doesn't support the 'text' data type.");

			string origText = "some huge text string";
			string newText = "some even bigger text string";
	
			ClearCounts();
	
			ISession s = OpenSession();
			s.BeginTransaction();
			TextHolder holder = new TextHolder(origText);
			s.Save(holder);
			long id = holder.Id;
			s.Transaction.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			holder = s.Get<TextHolder>(id);
			s.SetReadOnly(holder, true);
			holder.TheText = newText;
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			holder = s.Get<TextHolder>(id);
			Assert.That(origText, Is.EqualTo(holder.TheText), "change written to database");
			s.Delete(holder);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
			
		[Test]
		public void MergeWithReadOnlyEntity()
		{
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			DataPoint dp = new DataPoint();
			dp.X = 0.1M;
			dp.Y = (decimal)System.Math.Cos((double)dp.X);
			s.Save(dp);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			dp.Description = "description";
	
			s = OpenSession();
			t = s.BeginTransaction();
			DataPoint dpManaged = s.Get<DataPoint>(dp.Id);
			s.SetReadOnly(dpManaged, true);
			DataPoint dpMerged = (DataPoint)s.Merge(dp);
			Assert.That(dpManaged, Is.SameAs(dpMerged));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			dpManaged = s.Get<DataPoint>(dp.Id);
			Assert.That(dpManaged.Description, Is.Null);
			s.Delete(dpManaged);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	}
}
