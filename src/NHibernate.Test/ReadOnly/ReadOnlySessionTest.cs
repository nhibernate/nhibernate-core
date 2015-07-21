using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect;
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
	public class ReadOnlySessionTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

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
			DataPoint dp = null;
			long dpId = -1;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				s.BeginTransaction();
			
				dp = new DataPoint();
				dp.X = 0.1M;
				dp.Y = (decimal)System.Math.Cos((double)dp.X);
				dp.Description = "original";
				s.Save(dp);
				dpId = dp.Id;
				s.Transaction.Commit();
			}
		
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				s.BeginTransaction();
				s.DefaultReadOnly = true;
				Assert.That(s.DefaultReadOnly, Is.True);
				dp = (DataPoint)s.Load<DataPoint>(dpId);
				s.DefaultReadOnly = false;
				Assert.That(NHibernateUtil.IsInitialized(dp), Is.False, "was initialized");
				Assert.That(s.IsReadOnly(dp), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(dp), Is.False, "was initialized during isReadOnly");
				dp.Description = "changed";
				Assert.That(NHibernateUtil.IsInitialized(dp), Is.True, "was not initialized during mod");
				Assert.That(dp.Description, Is.StringMatching("changed"), "desc not changed in memory");
				s.Flush();
				s.Transaction.Commit();
			}
	
			using (ISession s = OpenSession())
			{
				s.BeginTransaction();
				IList list = s.CreateQuery("from DataPoint where description = 'changed'").List();
				Assert.That(list.Count, Is.EqualTo(0), "change written to database");
				s.CreateQuery("delete from DataPoint").ExecuteUpdate();
				s.Transaction.Commit();
			}
		}
		
		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void ReadOnlySessionDefaultQueryScroll()
		{
		}
		
		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void ReadOnlySessionModifiableQueryScroll()
		{
		}

		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void ModifiableSessionReadOnlyQueryScroll()
		{
		}
		
		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void ModifiableSessionDefaultQueryReadOnlySessionScroll()
		{
		}
		
		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void QueryReadOnlyScroll()
		{
		}

		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void QueryModifiableScroll()
		{
		}
		
		[Test]
		public void ReadOnlySessionDefaultQueryIterate()
		{
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				using (ITransaction t = s.BeginTransaction())
				{
					for (int i = 0; i < 100; i++)
					{
						DataPoint dp = new DataPoint();
						dp.X = 0.1M * i;
						dp.Y = (decimal)System.Math.Cos((double)dp.X);
						s.Save(dp);
					}
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;
					IEnumerable enumerable = s.CreateQuery("from DataPoint dp order by dp.X asc").Enumerable();
					s.DefaultReadOnly = false;
					
					int i = 0;
					foreach (DataPoint dp in enumerable)
					{
						if (++i == 50)
						{
							s.SetReadOnly(dp, false);
						}
						dp.Description = "done!";
					}
					t.Commit();
				}
				
				s.Clear();
			
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						IList single = s.CreateQuery("from DataPoint where Description = 'done!'").List();
						Assert.That(single.Count, Is.EqualTo(1));
					}
					finally
					{
						// cleanup
						s.CreateQuery("delete from DataPoint").ExecuteUpdate();
					}
					
					t.Commit();
				}
			}
		}
		
		[Test]
		public void ReadOnlySessionModifiableQueryIterate()
		{
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				using (ITransaction t = s.BeginTransaction())
				{
					for (int i = 0; i < 100; i++)
					{
						DataPoint dp = new DataPoint();
						dp.X = 0.1M * i;
						dp.Y = (decimal)System.Math.Cos((double)dp.X);
						s.Save(dp);
					}
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;
					IEnumerable enumerable = s.CreateQuery("from DataPoint dp order by dp.X asc")
						.SetReadOnly(false)
						.Enumerable();
					
					int i = 0;
					foreach (DataPoint dp in enumerable)
					{
						if (++i == 50)
						{
							s.SetReadOnly(dp, true);
						}
						dp.Description = "done!";
					}
					t.Commit();
				}
				
				s.Clear();
			
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						IList single = s.CreateQuery("from DataPoint where Description = 'done!'").List();
						Assert.That(single.Count, Is.EqualTo(99));
					}
					finally
					{
						// cleanup
						s.CreateQuery("delete from DataPoint").ExecuteUpdate();
					}
					
					t.Commit();
				}
			}
		}
		
		[Test]
		public void ModifiableSessionReadOnlyQueryIterate()
		{
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				using (ITransaction t = s.BeginTransaction())
				{
					for (int i = 0; i < 100; i++)
					{
						DataPoint dp = new DataPoint();
						dp.X = 0.1M * i;
						dp.Y = (decimal)System.Math.Cos((double)dp.X);
						s.Save(dp);
					}
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					Assert.That(s.DefaultReadOnly, Is.False);
					
					IEnumerable enumerable = s.CreateQuery("from DataPoint dp order by dp.X asc")
						.SetReadOnly(true)
						.Enumerable();
					
					int i = 0;
					foreach (DataPoint dp in enumerable)
					{
						if (++i == 50)
						{
							s.SetReadOnly(dp, false);
						}
						dp.Description = "done!";
					}
					t.Commit();
				}
				
				s.Clear();
			
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						IList single = s.CreateQuery("from DataPoint where Description = 'done!'").List();
						Assert.That(single.Count, Is.EqualTo(1));
					}
					finally
					{
						// cleanup
						s.CreateQuery("delete from DataPoint").ExecuteUpdate();
					}
					
					t.Commit();
				}
			}
		}
		
		[Test]
		public void ModifiableSessionDefaultQueryReadOnlySessionIterate()
		{
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				using (ITransaction t = s.BeginTransaction())
				{
					for (int i = 0; i < 100; i++)
					{
						DataPoint dp = new DataPoint();
						dp.X = 0.1M * i;
						dp.Y = (decimal)System.Math.Cos((double)dp.X);
						s.Save(dp);
					}
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = false;
					
					IQuery query = s.CreateQuery("from DataPoint dp order by dp.X asc");
					
					s.DefaultReadOnly = true;
					IEnumerable enumerable = query.Enumerable();
					s.DefaultReadOnly = false;
					
					int i = 0;
					foreach (DataPoint dp in enumerable)
					{
						if (++i == 50)
						{
							s.SetReadOnly(dp, false);
						}
						dp.Description = "done!";
					}
					t.Commit();
				}
				
				s.Clear();
			
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						IList single = s.CreateQuery("from DataPoint where Description = 'done!'").List();
						Assert.That(single.Count, Is.EqualTo(1));
					}
					finally
					{
						// cleanup
						s.CreateQuery("delete from DataPoint").ExecuteUpdate();
					}
					
					t.Commit();
				}
			}
		}
		
		[Test]
		public void QueryReadOnlyIterate()
		{
			long lastDataPointId = 0;
			int nExpectedChanges = 0;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					DataPoint dp = null;
					
					for (int i = 0; i < 100; i++)
					{
						dp = new DataPoint();
						dp.X = 0.1M * i;
						dp.Y = (decimal)System.Math.Cos((double)dp.X);
						s.Save(dp);
					}
					t.Commit();
					
					lastDataPointId = dp.Id;
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = false;

					IQuery query = s.CreateQuery("from DataPoint dp order by dp.X asc");
					
					Assert.That(query.IsReadOnly, Is.False);
					s.DefaultReadOnly = true;
					Assert.That(query.IsReadOnly, Is.True);
					s.DefaultReadOnly = false;
					Assert.That(query.IsReadOnly, Is.False);
					query.SetReadOnly(true);
					Assert.That(query.IsReadOnly, Is.True);
					s.DefaultReadOnly = true;
					Assert.That(query.IsReadOnly, Is.True);
					s.DefaultReadOnly = false;
					Assert.That(query.IsReadOnly, Is.True);
					query.SetReadOnly(false);
					Assert.That(query.IsReadOnly, Is.False);
					s.DefaultReadOnly = true;
					Assert.That(query.IsReadOnly, Is.False);
					query.SetReadOnly(true);
					Assert.That(query.IsReadOnly, Is.True);
					s.DefaultReadOnly = false;
					Assert.That(s.DefaultReadOnly, Is.False);
					IEnumerator<DataPoint> it = query.Enumerable<DataPoint>().GetEnumerator();
					Assert.That(query.IsReadOnly, Is.True);
					DataPoint dpLast = s.Get<DataPoint>(lastDataPointId);
					Assert.That(s.IsReadOnly(dpLast), Is.False);
					query.SetReadOnly(false);
					Assert.That(query.IsReadOnly, Is.False);
					Assert.That(s.DefaultReadOnly, Is.False);
		
					int i = 0;
					
					while (it.MoveNext())
					{
						Assert.That(s.DefaultReadOnly, Is.False);
						DataPoint dp = it.Current;
						Assert.That(s.DefaultReadOnly, Is.False);
						
						if (dp.Id == dpLast.Id)
						{
							//dpLast existed in the session before executing the read-only query
							Assert.That(s.IsReadOnly(dp), Is.False);
						}
						else
						{
							Assert.That(s.IsReadOnly(dp), Is.True);
						}
						
						if (++i == 50)
						{
							s.SetReadOnly(dp, false);
							nExpectedChanges = (dp == dpLast ? 1 : 2 );
						}
						
						dp.Description = "done!";
					}
					
					Assert.That(s.DefaultReadOnly, Is.False);
							
					t.Commit();
				}
				
				s.Clear();
			
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						IList single = s.CreateQuery("from DataPoint where Description = 'done!'").List();
						Assert.That(single.Count, Is.EqualTo(nExpectedChanges));
					}
					finally
					{
						// cleanup
						s.CreateQuery("delete from DataPoint").ExecuteUpdate();
					}
					
					t.Commit();
				}
			}
		}
		
		[Test]
		public void QueryModifiableIterate()
		{
			long lastDataPointId = 0;
			int nExpectedChanges = 0;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					DataPoint dp = null;
					
					for (int i = 0; i < 100; i++)
					{
						dp = new DataPoint();
						dp.X = 0.1M * i;
						dp.Y = (decimal)System.Math.Cos((double)dp.X);
						s.Save(dp);
					}
					t.Commit();
					
					lastDataPointId = dp.Id;
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;

					IQuery query = s.CreateQuery("from DataPoint dp order by dp.X asc");
					
					Assert.That(query.IsReadOnly, Is.True);
					s.DefaultReadOnly = false;
					Assert.That(query.IsReadOnly, Is.False);
					s.DefaultReadOnly = true;
					Assert.That(query.IsReadOnly, Is.True);
					query.SetReadOnly(false);
					Assert.That(query.IsReadOnly, Is.False);
					s.DefaultReadOnly = false;
					Assert.That(query.IsReadOnly, Is.False);
					s.DefaultReadOnly = true;
					Assert.That(query.IsReadOnly, Is.False);
					query.SetReadOnly(true);
					Assert.That(query.IsReadOnly, Is.True);
					s.DefaultReadOnly = false;
					Assert.That(query.IsReadOnly, Is.True);
					
					query.SetReadOnly(false);
					Assert.That(query.IsReadOnly, Is.False);
					s.DefaultReadOnly = true;
					Assert.That(s.DefaultReadOnly, Is.True);
					
					IEnumerator<DataPoint> it = query.Enumerable<DataPoint>().GetEnumerator();
					Assert.That(query.IsReadOnly, Is.False);
					DataPoint dpLast = s.Get<DataPoint>(lastDataPointId);
					Assert.That(s.IsReadOnly(dpLast), Is.True);
					query.SetReadOnly(true);
					Assert.That(query.IsReadOnly, Is.True);
					Assert.That(s.DefaultReadOnly, Is.True);
		
					int i = 0;
					
					while (it.MoveNext())
					{
						Assert.That(s.DefaultReadOnly, Is.True);
						DataPoint dp = it.Current;
						Assert.That(s.DefaultReadOnly, Is.True);
						
						if (dp.Id == dpLast.Id)
						{
							//dpLast existed in the session before executing the read-only query
							Assert.That(s.IsReadOnly(dp), Is.True);
						}
						else
						{
							Assert.That(s.IsReadOnly(dp), Is.False);
						}
						
						if (++i == 50)
						{
							s.SetReadOnly(dp, true);
							nExpectedChanges = (dp == dpLast ? 99 : 98);
						}
						
						dp.Description = "done!";
					}
					
					Assert.That(s.DefaultReadOnly, Is.True);
							
					t.Commit();
				}
				
				s.Clear();
			
				using (ITransaction t = s.BeginTransaction())
				{
					try
					{
						IList list = s.CreateQuery("from DataPoint where Description = 'done!'").List();
						Assert.That(list.Count, Is.EqualTo(nExpectedChanges));
					}
					finally
					{
						// cleanup
						s.CreateQuery("delete from DataPoint").ExecuteUpdate();
					}
					
					t.Commit();
				}
			}
		}
		
		[Test]
		public void ReadOnlyRefresh()
		{
			DataPoint dp = null;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = new DataPoint();
					dp.Description = "original";
					dp.X = 0.1M;
					dp.Y = (decimal)System.Math.Cos((double)dp.X);
					s.Save(dp);
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				s.DefaultReadOnly = true;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(dp.Description, Is.EqualTo("original"));
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					s.Refresh(dp);
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(dp.Description, Is.EqualTo("original"));
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					s.DefaultReadOnly = false;
					s.Refresh(dp);
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(dp.Description, Is.EqualTo("original"));
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					
					t.Commit();
				}
				
				s.Clear();
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					Assert.That(dp.Description, Is.EqualTo("original"));
					s.Delete(dp);
					t.Commit();
				}
			}
		}
		
		[Test]
		public void ReadOnlyRefreshDetached()
		{
			DataPoint dp = null;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = new DataPoint();
					dp.Description = "original";
					dp.X = 0.1M;
					dp.Y = (decimal)System.Math.Cos((double)dp.X);
					s.Save(dp);
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
			
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = false;
					
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					s.Refresh(dp);
					Assert.That(dp.Description, Is.EqualTo("original"));
					Assert.That(s.IsReadOnly(dp), Is.False);
					
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					s.Evict(dp);
					s.Refresh(dp);
					Assert.That(dp.Description, Is.EqualTo("original"));
					Assert.That(s.IsReadOnly(dp), Is.False);
					
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					s.DefaultReadOnly = true;
					s.Evict(dp);
					s.Refresh(dp);
					Assert.That(dp.Description, Is.EqualTo("original"));
					Assert.That(s.IsReadOnly(dp), Is.True);
					
					dp.Description = "changed";
					
					t.Commit();
				}
				
				s.Clear();
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					Assert.That(dp.Description, Is.EqualTo("original"));
					
					s.Delete(dp);
					t.Commit();
				}
			}
		}
		
		[Test]
		public void ReadOnlyProxyRefresh()
		{
			DataPoint dp = null;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = new DataPoint();
					dp.Description = "original";
					dp.X = 0.1M;
					dp.Y = (decimal)System.Math.Cos((double)dp.X);
					s.Save(dp);
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
			
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;
					
					dp = s.Load<DataPoint>(dp.Id);
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
					
					s.Refresh(dp);
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
					Assert.That(s.IsReadOnly(dp), Is.True);
					s.DefaultReadOnly = false;
					
					s.Refresh(dp);
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(dp.Description, Is.EqualTo("original"));
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.True);
					
					s.Refresh(dp);
					Assert.That(dp.Description, Is.EqualTo("original"));
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.True);
					s.DefaultReadOnly = true;
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					
					s.Refresh(dp);
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.True);
					Assert.That(dp.Description, Is.EqualTo("original"));
					dp.Description = "changed";
					
					t.Commit();
				}
				
				s.Clear();
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					Assert.That(dp.Description, Is.EqualTo("original"));
					s.Delete(dp);
					t.Commit();
				}
			}
		}
		
		[Test]
		public void ReadOnlyProxyRefreshDetached()
		{
			DataPoint dp = null;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = new DataPoint();
					dp.Description = "original";
					dp.X = 0.1M;
					dp.Y = (decimal)System.Math.Cos((double)dp.X);
					s.Save(dp);
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;
					
					dp = s.Load<DataPoint>(dp.Id);
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
					Assert.That(s.IsReadOnly(dp), Is.True);
					s.Evict(dp);

					s.Refresh(dp);
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
					s.DefaultReadOnly = false;
					Assert.That(s.IsReadOnly(dp), Is.True);
					s.Evict(dp);
	
					s.Refresh(dp);
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.False);
					Assert.That(s.IsReadOnly(dp), Is.False);
					Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.False);
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					Assert.That(NHibernateUtil.IsInitialized(dp), Is.True);
					s.Evict(dp);

					s.Refresh(dp);
					Assert.That(dp.Description, Is.EqualTo("original"));
					Assert.That(s.IsReadOnly(dp), Is.False);
					Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.False);
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));
					s.DefaultReadOnly = true;
					s.Evict(dp);

					s.Refresh(dp);
					Assert.That(dp.Description, Is.EqualTo("original"));
					Assert.That(s.IsReadOnly(dp), Is.True);
					Assert.That(s.IsReadOnly(((INHibernateProxy)dp).HibernateLazyInitializer.GetImplementation()), Is.True);
					dp.Description = "changed";
					Assert.That(dp.Description, Is.EqualTo("changed"));

					t.Commit();
				}

				s.Clear();
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					Assert.That(dp.Description, Is.EqualTo("original"));
					s.Delete(dp);
					t.Commit();
				}
			}
		}

		[Test]
		public void ReadOnlyDelete()
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
					s.Save(dp);
					t.Commit();
				}
			}
	
			using (ISession s = OpenSession())
			{
				s.DefaultReadOnly = true;
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					s.DefaultReadOnly = false;
					Assert.That(s.IsReadOnly(dp), Is.True);
					s.Delete(dp);
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					IList list = s.CreateQuery("from DataPoint where id=" + dp.Id ).List();
					Assert.That(list.Count, Is.EqualTo(0));
					t.Commit();
				}
			}
		}
	
		[Test]
		public void ReadOnlyGetModifyAndDelete()
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
					s.Save(dp);
					t.Commit();
				}
			}
	
			using (ISession s = OpenSession())
			{
				s.DefaultReadOnly = true;
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					s.DefaultReadOnly = true;
					dp.Description = "a DataPoint";
					s.Delete(dp);
					t.Commit();
				}
			}
	
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					IList list = s.CreateQuery("from DataPoint where id=" + dp.Id ).List();
					Assert.That(list.Count, Is.EqualTo(0));
					t.Commit();
				}
			}
		}
	
		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void ReadOnlyModeWithExistingModifiableEntity()
		{
		}
	
		[Test]
		[Ignore("Scrollable result sets not supported in NHibernate")]
		public void ModifiableModeWithExistingReadOnlyEntity()
		{
		}
	
		[Test]
		public void ReadOnlyOnTextType()
		{
			if (!TextHolder.SupportedForDialect(Dialect))
				Assert.Ignore("Dialect doesn't support the 'text' data type.");

			string origText = "some huge text string";
			string newText = "some even bigger text string";
			long id = 0;
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				using (ITransaction t = s.BeginTransaction())
				{
					TextHolder holder = new TextHolder(origText);
					s.Save(holder);
					id = holder.Id;
					t.Commit();
				}
			}
			
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;
					s.CacheMode = CacheMode.Ignore;
					TextHolder holder = s.Get<TextHolder>(id);
					s.DefaultReadOnly = false;
					holder.TheText = newText;
					s.Flush();
					t.Commit();
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					TextHolder holder = s.Get<TextHolder>(id);
					Assert.That(holder.TheText, Is.EqualTo(origText), "change written to database");
					s.Delete(holder);
					t.Commit();
				}
			}
		}
	
		[Test]
		public void MergeWithReadOnlyEntity()
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
					s.Save(dp);
					t.Commit();
				}
			}
	
			dp.Description = "description";
	
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;
					DataPoint dpManaged = s.Get<DataPoint>(dp.Id);
					DataPoint dpMerged = (DataPoint)s.Merge(dp);
					Assert.That(dpManaged, Is.SameAs(dpMerged));
					t.Commit();
				}
			}
	
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					DataPoint dpManaged = s.Get<DataPoint>(dp.Id);
					Assert.That(dpManaged.Description, Is.Null);
					s.Delete(dpManaged);
					t.Commit();
				}
			}
	
		}
	
		[Test]
		public void MergeWithReadOnlyProxy()
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
					s.Save(dp);
					t.Commit();
				}
			}
	
			dp.Description = "description";
			
			using (ISession s = OpenSession())
			{
				s.CacheMode = CacheMode.Ignore;
				
				using (ITransaction t = s.BeginTransaction())
				{
					s.DefaultReadOnly = true;
					DataPoint dpProxy = s.Load<DataPoint>(dp.Id);
					Assert.That(s.IsReadOnly(dpProxy), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(dpProxy), Is.False);
					s.Evict(dpProxy);
					dpProxy = (DataPoint)s.Merge(dpProxy);
					Assert.That(s.IsReadOnly(dpProxy), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(dpProxy), Is.False);
					dpProxy = (DataPoint)s.Merge(dp);
					Assert.That(s.IsReadOnly(dpProxy), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(dpProxy), Is.True);
					Assert.That(dpProxy.Description, Is.EqualTo("description"));
					s.Evict(dpProxy);
					dpProxy = (DataPoint)s.Merge(dpProxy);
					Assert.That(s.IsReadOnly(dpProxy), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(dpProxy), Is.True);
					Assert.That(dpProxy.Description, Is.EqualTo("description"));
					dpProxy.Description = null;
					dpProxy = (DataPoint)s.Merge(dp);
					Assert.That(s.IsReadOnly(dpProxy), Is.True);
					Assert.That(NHibernateUtil.IsInitialized(dpProxy), Is.True);
					Assert.That(dpProxy.Description, Is.EqualTo("description"));
					t.Commit();
				}
			}
	
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					dp = s.Get<DataPoint>(dp.Id);
					Assert.That(dp.Description, Is.Null);
					s.Delete(dp);
					t.Commit();
				}
			}
		}
	}
}
