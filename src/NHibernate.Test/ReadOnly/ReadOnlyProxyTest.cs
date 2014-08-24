using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.ReadOnly
{
	[TestFixture]
	public class ReadOnlyProxyTest : AbstractReadOnlyTest
	{
		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"ReadOnly.DataPoint.hbm.xml",
						//"ReadOnly.TextHolder.hbm.xml"
					};
			}
		}

		[Test]
		public void ReadOnlyViaSessionDoesNotInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.SetReadOnly(dp, false);
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Flush();
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Transaction.Commit();
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyViaLazyInitializerDoesNotInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			ILazyInitializer dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			dpLI.ReadOnly = true;
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			dpLI.ReadOnly = false;
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Flush();
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Transaction.Commit();
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyViaSessionNoChangeAfterInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			CheckReadOnly(s, dp, true);
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.SetReadOnly(dp, false);
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyViaLazyInitializerNoChangeAfterInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			ILazyInitializer dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			CheckReadOnly(s, dp, false);
			Assert.That(dpLI.IsUninitialized);
			NHibernateUtil.Initialize(dp);
			Assert.That(dpLI.IsUninitialized, Is.False);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			dpLI.ReadOnly = true;
			CheckReadOnly(s, dp, true);
			Assert.That(dpLI.IsUninitialized);
			NHibernateUtil.Initialize(dp);
			Assert.That(dpLI.IsUninitialized, Is.False);
			CheckReadOnly(s, dp, true);
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			dpLI.ReadOnly = true;
			CheckReadOnly(s, dp, true);
			Assert.That(dpLI.IsUninitialized);
			dpLI.ReadOnly = false;
			CheckReadOnly(s, dp, false);
			Assert.That(dpLI.IsUninitialized);
			NHibernateUtil.Initialize(dp);
			Assert.That(dpLI.IsUninitialized, Is.False);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void ReadOnlyViaSessionBeforeInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			s.SetReadOnly(dp, true);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, true);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void ModifiableViaSessionBeforeInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, false);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyViaSessionBeforeInitByModifiableQuery()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			DataPoint dpFromQuery = s.CreateQuery("from DataPoint where id = " + dpOrig.Id).SetReadOnly(false).UniqueResult<DataPoint>();
			Assert.That(NHibernateUtil.IsInitialized(dpFromQuery), Is.True);
			Assert.That(dpFromQuery, Is.SameAs(dp));
			CheckReadOnly(s, dp, true);
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void ReadOnlyViaSessionBeforeInitByReadOnlyQuery()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			DataPoint dpFromQuery = s.CreateQuery( "from DataPoint where Id = " + dpOrig.Id).SetReadOnly(true).UniqueResult<DataPoint>();
			Assert.That(NHibernateUtil.IsInitialized(dpFromQuery), Is.True);
			Assert.That(dpFromQuery, Is.SameAs(dp));
			CheckReadOnly(s, dp, true);
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ModifiableViaSessionBeforeInitByModifiableQuery()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			DataPoint dpFromQuery = s.CreateQuery("from DataPoint where Id = " + dpOrig.Id).SetReadOnly(false).UniqueResult<DataPoint>();
			Assert.That(NHibernateUtil.IsInitialized(dpFromQuery), Is.True);
			Assert.That(dpFromQuery, Is.SameAs(dp));
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ModifiableViaSessionBeforeInitByReadOnlyQuery()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			DataPoint dpFromQuery = s.CreateQuery("from DataPoint where id=" + dpOrig.Id).SetReadOnly(true).UniqueResult<DataPoint>();
			Assert.That(NHibernateUtil.IsInitialized(dpFromQuery), Is.True);
			Assert.That(dpFromQuery, Is.SameAs(dp));
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void ReadOnlyViaLazyInitializerBeforeInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			ILazyInitializer dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			Assert.That(dpLI.IsUninitialized);
			CheckReadOnly(s, dp, false);
			dpLI.ReadOnly = true;
			CheckReadOnly(s, dp, true);
			dp.Description = "changed";
			Assert.That(dpLI.IsUninitialized, Is.False);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, true);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void ModifiableViaLazyInitializerBeforeInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			ILazyInitializer dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(dpLI.IsUninitialized);
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(dpLI.IsUninitialized, Is.False);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, false);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyViaLazyInitializerAfterInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			ILazyInitializer dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			Assert.That(dpLI.IsUninitialized);
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(dpLI.IsUninitialized, Is.False);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, false);
			dpLI.ReadOnly = true;
			CheckReadOnly(s, dp, true);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ModifiableViaLazyInitializerAfterInit()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			ILazyInitializer dpLI = ((INHibernateProxy)dp).HibernateLazyInitializer;
			Assert.That(dpLI.IsUninitialized);
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(dpLI.IsUninitialized, Is.False);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, false);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		[Ignore("Failing test. See HHH-4642")]
		public void ModifyToReadOnlyToModifiableIsUpdatedFailureExpected()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			s.SetReadOnly(dp, false);
			CheckReadOnly(s, dp, false);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			
			try
			{
				Assert.That(dp.Description, Is.EqualTo("changed"));
				// should fail due to HHH-4642
			}
			finally
			{
				s.Transaction.Rollback();
				s.Close();
				s = OpenSession();
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		[Ignore("Failing test. See HHH-4642")]
		public void ReadOnlyModifiedToModifiableIsUpdatedFailureExpected()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.SetReadOnly(dp, false);
			CheckReadOnly(s, dp, false);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			
			try
			{
				Assert.That(dp.Description, Is.EqualTo("changed"));
				// should fail due to HHH-4642
			}
			finally
			{
				s.Transaction.Rollback();
				s.Close();
				s = OpenSession();
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}

		[Test]
		public void ReadOnlyChangedEvictedUpdate()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Evict(dp);
			Assert.That(s.Contains(dp), Is.False);
			s.Update(dp);
			CheckReadOnly(s, dp, false);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyToModifiableInitWhenModifiedIsUpdated()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			s.SetReadOnly(dp, false);
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyInitToModifiableModifiedIsUpdated()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			CheckReadOnly(s, dp, true);
			s.SetReadOnly(dp, false);
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyModifiedUpdate()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, true);
			s.Update(dp);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyDelete()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Delete(dp);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.Null);
			s.Transaction.Commit();
			s.Close();
		}

		[Test]
		public void ReadOnlyRefresh()
		{
			DataPoint dp = this.CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			ITransaction t = s.BeginTransaction();
			dp = s.Load<DataPoint>(dp.Id);
			s.SetReadOnly(dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			s.Refresh(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			Assert.That(dp.Description, Is.EqualTo("original"));
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(s.IsReadOnly(dp), Is.True);
			Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.True);
			s.Refresh(dp);
			Assert.That(dp.Description, Is.EqualTo("original"));
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(s.IsReadOnly(dp), Is.True);
			Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.True);
			t.Commit();
	
			s.Clear();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			Assert.That(dp.Description, Is.EqualTo("original"));
			s.Delete(dp);
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyRefreshDeleted()
		{
			DataPoint dp = this.CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			ITransaction t = s.BeginTransaction();
			INHibernateProxy dpProxy = (INHibernateProxy)s.Load<DataPoint>(dp.Id);
			Assert.That(NHibernateUtil.IsInitialized(dpProxy), Is.False);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			s.Delete(dp);
			s.Flush();
			
			try
			{
				s.Refresh(dp);
				Assert.Fail("should have thrown UnresolvableObjectException" );
			}
			catch (UnresolvableObjectException)
			{
				// expected
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.CacheMode = CacheMode.Ignore;
			DataPoint dpProxyInit = s.Load<DataPoint>(dp.Id);
			Assert.That(dp.Description, Is.EqualTo("original"));
			s.Delete(dpProxyInit);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Assert.That(dpProxyInit, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dpProxyInit), Is.True);

			try
			{
				s.Refresh(dpProxyInit);
				Assert.Fail("should have thrown UnresolvableObjectException");
			}
			catch (UnresolvableObjectException)
			{
				// expected
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			Assert.That(dpProxyInit, Is.InstanceOf<INHibernateProxy>());
			
			try
			{
				s.Refresh(dpProxy);
				Assert.That(NHibernateUtil.IsInitialized(dpProxy), Is.False);
				NHibernateUtil.Initialize(dpProxy);
				Assert.Fail("should have thrown UnresolvableObjectException");
			}
			catch (UnresolvableObjectException)
			{
				// expected
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
		}
	
		[Test]
		public void ReadOnlyRefreshDetached()
		{
			DataPoint dp = this.CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			ITransaction t = s.BeginTransaction();
			dp = s.Load<DataPoint>(dp.Id);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			Assert.That(s.IsReadOnly(dp), Is.False);
			s.SetReadOnly(dp, true);
			Assert.That(s.IsReadOnly(dp), Is.True);
			s.Evict(dp);
			s.Refresh(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			Assert.That(s.IsReadOnly(dp), Is.False);
			dp.Description = "changed";
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			s.SetReadOnly(dp, true);
			s.Evict(dp);
			s.Refresh(dp);
			Assert.That(dp.Description, Is.EqualTo("original"));
			Assert.That(s.IsReadOnly(dp), Is.False);
			t.Commit();
	
			s.Clear();
			t = s.BeginTransaction();
			dp = s.Get<DataPoint>(dp.Id);
			Assert.That(dp.Description, Is.EqualTo("original"));
			s.Delete(dp);
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyProxyMergeDetachedProxyWithChange()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			s.Transaction.Commit();
			s.Close();
	
			// modify detached proxy
			dp.Description = "changed";
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dpLoaded = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dpLoaded, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dpLoaded, false);
			s.SetReadOnly(dpLoaded, true);
			CheckReadOnly(s, dpLoaded, true);
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.False);
			DataPoint dpMerged = (DataPoint)s.Merge(dp);
			Assert.That(dpMerged, Is.SameAs(dpLoaded));
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.True);
			Assert.That(dpLoaded.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dpLoaded, true);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyProxyInitMergeDetachedProxyWithChange()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			s.Transaction.Commit();
			s.Close();
	
			// modify detached proxy
			dp.Description = "changed";
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dpLoaded = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dpLoaded, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.False);
			NHibernateUtil.Initialize(dpLoaded);
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.True);
			CheckReadOnly(s, dpLoaded, false);
			s.SetReadOnly(dpLoaded, true);
			CheckReadOnly(s, dpLoaded, true);
			DataPoint dpMerged = (DataPoint)s.Merge(dp);
			Assert.That(dpMerged, Is.SameAs(dpLoaded));
			Assert.That(dpLoaded.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dpLoaded, true);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyProxyMergeDetachedEntityWithChange()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			s.Transaction.Commit();
			s.Close();
	
			// modify detached proxy target
			DataPoint dpEntity = (DataPoint)((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation();
			dpEntity.Description = "changed";
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dpLoaded = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dpLoaded, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dpLoaded, false);
			s.SetReadOnly(dpLoaded, true);
			CheckReadOnly(s, dpLoaded, true);
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.False);
			DataPoint dpMerged = (DataPoint)s.Merge(dpEntity);
			Assert.That(dpMerged, Is.SameAs(dpLoaded));
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.True);
			Assert.That(dpLoaded.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dpLoaded, true);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyProxyInitMergeDetachedEntityWithChange()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			s.Transaction.Commit();
			s.Close();
	
			// modify detached proxy target
			DataPoint dpEntity = (DataPoint)((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation();
			dpEntity.Description = "changed";
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dpLoaded = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dpLoaded, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.False);
			NHibernateUtil.Initialize(dpLoaded);
			Assert.That(NHibernateUtil.IsInitialized(dpLoaded), Is.True);
			CheckReadOnly(s, dpLoaded, false);
			s.SetReadOnly(dpLoaded, true);
			CheckReadOnly(s, dpLoaded, true);
			DataPoint dpMerged = (DataPoint)s.Merge(dpEntity);
			Assert.That(dpMerged, Is.SameAs(dpLoaded));
			Assert.That(dpLoaded.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dpLoaded, true);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void ReadOnlyEntityMergeDetachedProxyWithChange()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			CheckReadOnly(s, dp, false);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			NHibernateUtil.Initialize(dp);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			s.Transaction.Commit();
			s.Close();
	
			// modify detached proxy
			dp.Description = "changed";
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			DataPoint dpEntity = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dpEntity, Is.Not.InstanceOf<INHibernateProxy>());
			Assert.That(s.IsReadOnly(dpEntity), Is.False);
			s.SetReadOnly(dpEntity, true);
			Assert.That(s.IsReadOnly(dpEntity), Is.True);
			DataPoint dpMerged = (DataPoint)s.Merge(dp);
			Assert.That(dpMerged, Is.SameAs(dpEntity));
			Assert.That(dpEntity.Description, Is.EqualTo("changed"));
			Assert.That(s.IsReadOnly(dpEntity), Is.True);
			s.Flush();
			s.Transaction.Commit();
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void SetReadOnlyInTwoTransactionsSameSession()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Flush();
			s.Transaction.Commit();
	
			CheckReadOnly(s, dp, true);
	
			s.BeginTransaction();
			CheckReadOnly(s, dp, true);
			dp.Description = "changed again";
			Assert.That(dp.Description, Is.EqualTo("changed again"));
			s.Flush();
			s.Transaction.Commit();
	
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void SetReadOnlyBetweenTwoTransactionsSameSession()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, false);
			s.Flush();
			s.Transaction.Commit();
	
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
	
			s.BeginTransaction();
			CheckReadOnly(s, dp, true);
			dp.Description = "changed again";
			Assert.That(dp.Description, Is.EqualTo("changed again"));
			s.Flush();
			s.Transaction.Commit();
	
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void SetModifiableBetweenTwoTransactionsSameSession()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.SetReadOnly(dp, true);
			CheckReadOnly(s, dp, true);
			dp.Description = "changed";
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			CheckReadOnly(s, dp, true);
			s.Flush();
			s.Transaction.Commit();
	
			CheckReadOnly(s, dp, true);
			s.SetReadOnly(dp, false);
			CheckReadOnly(s, dp, false);
	
			s.BeginTransaction();
			CheckReadOnly(s, dp, false);
			Assert.That(dp.Description, Is.EqualTo("changed"));
			s.Refresh(dp);
			Assert.That(dp.Description, Is.EqualTo(dpOrig.Description));
			CheckReadOnly(s, dp, false);
			dp.Description = "changed again";
			Assert.That(dp.Description, Is.EqualTo("changed again"));
			s.Flush();
			s.Transaction.Commit();
	
			s.Close();
	
			s = OpenSession();
			s.BeginTransaction();
			dp = s.Get<DataPoint>(dpOrig.Id);
			Assert.That(dp.Id, Is.EqualTo(dpOrig.Id));
			Assert.That(dp.Description, Is.EqualTo("changed again"));
			Assert.That(dp.X, Is.EqualTo(dpOrig.X));
			Assert.That(dp.Y, Is.EqualTo(dpOrig.Y));
			s.Delete(dp);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void IsReadOnlyAfterSessionClosed()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			s.Close();
	
			try
			{
	 			s.IsReadOnly(dp);
				Assert.Fail("should have failed because session was closed");
			}
			catch (ObjectDisposedException) // SessionException in Hibernate
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s = OpenSession();
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void IsReadOnlyAfterSessionClosedViaLazyInitializer()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			Assert.That(s.Contains(dp), Is.True);
			s.Close();
	
			Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.Session, Is.Null);
			try
			{
	 			var value = ((INHibernateProxy)dp).HibernateLazyInitializer.ReadOnly;
				Assert.Fail("should have failed because session was detached");
			}
			catch (TransientObjectException)
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s = OpenSession();
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void DetachedIsReadOnlyAfterEvictViaSession()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			Assert.That(s.Contains(dp), Is.True);
			s.Evict(dp);
			Assert.That(s.Contains(dp), Is.False);
			Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.Session, Is.Null);
	
			try
			{
	 			s.IsReadOnly(dp);
				Assert.Fail("should have failed because proxy was detached");
			}
			catch (TransientObjectException)
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void DetachedIsReadOnlyAfterEvictViaLazyInitializer()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.Evict(dp);
			Assert.That(s.Contains(dp), Is.False);
			Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.Session, Is.Null);
			try
			{
	 			var value = ((INHibernateProxy)dp).HibernateLazyInitializer.ReadOnly;
				Assert.Fail("should have failed because proxy was detached");
			}
			catch (TransientObjectException)
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void SetReadOnlyAfterSessionClosed()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			s.Close();
	
			try
			{
	 			s.SetReadOnly(dp, true);
				Assert.Fail("should have failed because session was closed");
			}
			catch (ObjectDisposedException) // SessionException in Hibernate
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s = OpenSession();
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void SetReadOnlyAfterSessionClosedViaLazyInitializer()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			Assert.That(s.Contains(dp), Is.True);
			s.Close();
	
			Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.Session, Is.Null);
			try
			{
	 			((INHibernateProxy)dp).HibernateLazyInitializer.ReadOnly = true;
				Assert.Fail("should have failed because session was detached");
			}
			catch (TransientObjectException)
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s = OpenSession();
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void SetClosedSessionInLazyInitializer()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.Transaction.Commit();
			Assert.That(s.Contains(dp), Is.True);
			s.Close();
	
			Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.Session, Is.Null);
			Assert.That(((ISessionImplementor)s).IsClosed, Is.True);
			
			try
			{
				((INHibernateProxy)dp).HibernateLazyInitializer.SetSession((ISessionImplementor)s);
				Assert.Fail("should have failed because session was closed");
			}
			catch (ObjectDisposedException) // SessionException in Hibernate
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s = OpenSession();
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
		
		[Test]
		public void DetachedSetReadOnlyAfterEvictViaSession()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			Assert.That(s.Contains(dp), Is.True);
			s.Evict(dp);
			Assert.That(s.Contains(dp), Is.False);
			Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.Session, Is.Null);
	
			try
			{
	 			s.SetReadOnly(dp, true);
				Assert.Fail("should have failed because proxy was detached");
			}
			catch (TransientObjectException)
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void DetachedSetReadOnlyAfterEvictViaLazyInitializer()
		{
			DataPoint dpOrig = CreateDataPoint(CacheMode.Ignore);
	
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
	
			s.BeginTransaction();
			DataPoint dp = s.Load<DataPoint>(dpOrig.Id);
			Assert.That(dp, Is.InstanceOf<INHibernateProxy>());
			Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
			CheckReadOnly(s, dp, false);
			s.Evict(dp);
			Assert.That(s.Contains(dp), Is.False);
			Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.Session, Is.Null);
			
			try
			{
	 			((INHibernateProxy)dp).HibernateLazyInitializer.ReadOnly = true;
				Assert.Fail("should have failed because proxy was detached");
			}
			catch (TransientObjectException)
			{
				// expected
				Assert.That(((INHibernateProxy)dp).HibernateLazyInitializer.IsReadOnlySettingAvailable, Is.False);
			}
			finally
			{
				s.BeginTransaction();
				s.Delete(dp);
				s.Transaction.Commit();
				s.Close();
			}
		}
		
		private DataPoint CreateDataPoint(CacheMode mode)
		{
			DataPoint dp = null;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = new DataPoint();
					dp.X = 0.1M;
					dp.Y = (decimal)System.Math.Cos((double)dp.X);
					dp.Description = "original";
					s.Save(dp);
					t.Commit();
				}
			}
			
			return dp;
		}
	
		private void CheckReadOnly(ISession s, object proxy, bool expectedReadOnly)
		{
			Assert.That(proxy, Is.InstanceOf<INHibernateProxy>());
			ILazyInitializer li = ((INHibernateProxy)proxy).HibernateLazyInitializer;
			Assert.That(s, Is.SameAs(li.Session));
			Assert.That(s.IsReadOnly(proxy), Is.EqualTo(expectedReadOnly));
			Assert.That(li.ReadOnly, Is.EqualTo(expectedReadOnly));
			Assert.That(NHibernateUtil.IsInitialized(proxy), Is.Not.EqualTo(li.IsUninitialized));
			if (NHibernateUtil.IsInitialized(proxy))
			{
				Assert.That(s.IsReadOnly(li.GetImplementation()), Is.EqualTo(expectedReadOnly));
			}
		}
	}
}
