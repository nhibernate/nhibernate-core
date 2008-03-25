using System;
using System.Collections;
using System.Data;

using NHibernate.Connection;
using NHibernate.DomainModel;

using NUnit.Framework;

using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.Performance
{
	/// <summary>
	/// Summary description for NewerPerformanceTest.
	/// </summary>
	[TestFixture]
	public class NewerPerformanceTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"Simple.hbm.xml"}; }
		}

		[Test]
		public void Many()
		{
			IConnectionProvider cp = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);

			long hiber = 0;
			long adonet = 0;
			ISession s;
			for (int n = 0; n < 20; n++)
			{
				s = OpenSession();
				s.Delete("from Simple");
				s.Flush();
				Simple[] simples = new Simple[n];
				object[] ids = new object[n];
				for (int i = 0; i < n; i++)
				{
					simples[i] = new Simple();
					simples[i].Init();
					simples[i].Count = i;
					ids[i] = (long) i;
					s.Save(simples[i], ids[i]);
				}
				s.Flush();
				s.Close();

				//allow cache to settle

				s = OpenSession();
				NHibernate(s, simples, ids, n, "h0");
				s.Close();

				IDbConnection c = cp.GetConnection();
				DirectAdoNet(c, simples, ids, n, "j0");
				cp.CloseConnection(c);

				s = OpenSession();
				NHibernate(s, simples, ids, n, "h0");
				s.Close();

				c = cp.GetConnection();
				DirectAdoNet(c, simples, ids, n, "j0");
				cp.CloseConnection(c);

				//Now do timings

				int N = 30;

				long time = DateTime.Now.Ticks;
				for (int i = 0; i < N; i++)
				{
					s = OpenSession();
					NHibernate(s, simples, ids, n, "h1");
					s.Close();
				}
				hiber += DateTime.Now.Ticks - time;

				time = DateTime.Now.Ticks;
				for (int i = 0; i < N; i++)
				{
					c = cp.GetConnection();
					DirectAdoNet(c, simples, ids, n, "j1");
					cp.CloseConnection(c);
				}
				adonet += DateTime.Now.Ticks - time;

				time = DateTime.Now.Ticks;
				for (int i = 0; i < N; i++)
				{
					s = OpenSession();
					NHibernate(s, simples, ids, n, "h2");
					s.Close();
				}
				hiber += DateTime.Now.Ticks - time;

				time = DateTime.Now.Ticks;
				for (int i = 0; i < N; i++)
				{
					c = cp.GetConnection();
					DirectAdoNet(c, simples, ids, n, "j2");
					cp.CloseConnection(c);
				}
				adonet += DateTime.Now.Ticks - time;

				time = DateTime.Now.Ticks;
				for (int i = 0; i < N; i++)
				{
					s = OpenSession();
					NHibernate(s, simples, ids, n, "h1");
					s.Close();
				}
				hiber += DateTime.Now.Ticks - time;

				time = DateTime.Now.Ticks;
				for (int i = 0; i < N; i++)
				{
					c = cp.GetConnection();
					DirectAdoNet(c, simples, ids, n, "j1");
					cp.CloseConnection(c);
				}
				adonet += DateTime.Now.Ticks - time;
			}

			Console.Out.Write("Hibernate: " + hiber + "ms / Direct JDBC: " + adonet + "ms = Ratio: " + ((float) hiber) / adonet);
			s = OpenSession();
			s.Delete("from Simple");
			s.Flush();
			s.Close();
			cp.Dispose();
			GC.Collect();
		}

		[Test]
		public void Simultaneous()
		{
			IConnectionProvider cp = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);

			ISession s;
			for (int n = 2; n < 4000; n *= 2)
			{
				s = OpenSession();
				s.Delete("from Simple");
				s.Flush();
				Simple[] simples = new Simple[n];
				object[] ids = new object[n];
				for (int i = 0; i < n; i++)
				{
					simples[i] = new Simple();
					simples[i].Init();
					simples[i].Count = i;
					ids[i] = (long) i;
					s.Save(simples[i], ids[i]);
				}
				s.Flush();
				s.Close();

				//allow cache to settle

				s = OpenSession();
				NHibernate(s, simples, ids, n, "h0");
				s.Close();

				IDbConnection c = cp.GetConnection();
				DirectAdoNet(c, simples, ids, n, "j0");
				cp.CloseConnection(c);

				s = OpenSession();
				NHibernate(s, simples, ids, n, "h0");
				s.Close();

				c = cp.GetConnection();
				DirectAdoNet(c, simples, ids, n, "j0");
				cp.CloseConnection(c);

				//Now do timings

				s = OpenSession();
				long time = DateTime.Now.Ticks;
				NHibernate(s, simples, ids, n, "h1");
				long hiber = DateTime.Now.Ticks - time;
				s.Close();

				c = cp.GetConnection();
				time = DateTime.Now.Ticks;
				DirectAdoNet(c, simples, ids, n, "j1");
				long adonet = DateTime.Now.Ticks - time;
				cp.CloseConnection(c);

				s = OpenSession();
				time = DateTime.Now.Ticks;
				NHibernate(s, simples, ids, n, "h2");
				hiber += DateTime.Now.Ticks - time;
				s.Close();

				c = cp.GetConnection();
				time = DateTime.Now.Ticks;
				DirectAdoNet(c, simples, ids, n, "j2");
				adonet += DateTime.Now.Ticks - time;
				cp.CloseConnection(c);

				s = OpenSession();
				time = DateTime.Now.Ticks;
				NHibernate(s, simples, ids, n, "h2");
				hiber += DateTime.Now.Ticks - time;
				s.Close();

				c = cp.GetConnection();
				time = DateTime.Now.Ticks;
				DirectAdoNet(c, simples, ids, n, "j2");
				adonet += DateTime.Now.Ticks - time;
				cp.CloseConnection(c);

				Console.Out.WriteLine("Objects: " + n + " - NHibernate: " + hiber + "ms / Direct AdoNet: " + adonet + "ms = Ratio: " +
				                      ((float) hiber) / adonet);
			}

			cp.Dispose();
			s = OpenSession();
			s.Delete("from Simple");
			s.Flush();
			s.Close();
			GC.Collect();
		}

		[Test]
		public void NHibernateOnly()
		{
			ISession s;
			for (int n = 2; n < 4000; n *= 2)
			{
				s = OpenSession();
				s.Delete("from Simple");
				s.Flush();
				Simple[] simples = new Simple[n];
				object[] ids = new object[n];
				for (int i = 0; i < n; i++)
				{
					simples[i] = new Simple();
					simples[i].Init();
					simples[i].Count = i;
					ids[i] = (long) i;
					s.Save(simples[i], ids[i]);
				}
				s.Flush();
				s.Close();

				//Now do timings

				s = OpenSession();
				long time = DateTime.Now.Ticks;
				NHibernate(s, simples, ids, n, "h1");
				long hiber = DateTime.Now.Ticks - time;
				s.Close();

				s = OpenSession();
				time = DateTime.Now.Ticks;
				NHibernate(s, simples, ids, n, "h2");
				hiber += DateTime.Now.Ticks - time;
				s.Close();

				s = OpenSession();
				time = DateTime.Now.Ticks;
				NHibernate(s, simples, ids, n, "h2");
				hiber += DateTime.Now.Ticks - time;
				s.Close();

				Console.Out.WriteLine("Objects: " + n + "\t - Hibernate: \t" + hiber);
			}
			s = OpenSession();
			s.Delete("from Simple");
			s.Flush();
			s.Close();
			GC.Collect();
		}

		[Test]
		public void AdoNetOnly()
		{
			IConnectionProvider cp = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);

			ISession s;
			for (int n = 2; n < 4000; n *= 2)
			{
				s = OpenSession();
				s.Delete("from Simple");
				s.Flush();
				Simple[] simples = new Simple[n];
				object[] ids = new object[n];
				for (int i = 0; i < n; i++)
				{
					simples[i] = new Simple();
					simples[i].Init();
					simples[i].Count = i;
					ids[i] = (long) i;
					s.Save(simples[i], ids[i]);
				}
				s.Flush();
				s.Close();


				//Now do timings

				IDbConnection c = cp.GetConnection();
				long time = DateTime.Now.Ticks;
				DirectAdoNet(c, simples, ids, n, "j1");
				long adonet = DateTime.Now.Ticks - time;
				cp.CloseConnection(c);

				c = cp.GetConnection();
				time = DateTime.Now.Ticks;
				DirectAdoNet(c, simples, ids, n, "j2");
				adonet += DateTime.Now.Ticks - time;
				cp.CloseConnection(c);

				c = cp.GetConnection();
				time = DateTime.Now.Ticks;
				DirectAdoNet(c, simples, ids, n, "j2");
				adonet += DateTime.Now.Ticks - time;
				cp.CloseConnection(c);
				Console.Out.WriteLine("Objects: " + n + "\t Direct AdoNet: " + adonet);
			}
			cp.Dispose();
			s = OpenSession();
			s.Delete("from Simple");
			s.Flush();
			s.Close();
			GC.Collect();
		}

		private void NHibernate(ISession s, Simple[] simples, object[] ids, int N, string runname)
		{
			ITransaction trans = s.BeginTransaction();
			s.Find("from Simple s");
			trans.Commit();
		}

		private void DirectAdoNet(IDbConnection c, Simple[] simples, object[] ids, int N, string runname)
		{
			IList result = new ArrayList();
			IDbCommand select = c.CreateCommand();
			IDbTransaction trans = c.BeginTransaction();
			select.CommandText = "SELECT id_, name, address, count_, date_, pay, other FROM Simple";
			select.Transaction = trans;
			IDataReader reader = select.ExecuteReader();
			while (reader.Read())
			{
				/*new Long( rs.getLong(1) );
				rs.getString(2);
				rs.getString(3);
				rs.getInt(4);
				rs.getDate(5);
				rs.getFloat(6);
				rs.getLong(7);*/
				Simple s = new Simple();
				if (! reader.IsDBNull(0))
				{
					reader.GetInt64(0);
				}
				if (! reader.IsDBNull(1))
				{
					s.Name = reader.GetString(1);
				}
				if (! reader.IsDBNull(2))
				{
					s.Address = reader.GetString(2);
				}
				if (! reader.IsDBNull(3))
				{
					s.Count = reader.GetInt32(3);
				}
				if (! reader.IsDBNull(4))
				{
					s.Date = reader.GetDateTime(4);
				}
				if (! reader.IsDBNull(5))
				{
					s.Pay = reader.GetFloat(5);
				}
				if (! reader.IsDBNull(6))
				{
					reader.GetInt64(6);
				}
				s.Other = null;
				result.Add(s);
			}
			reader.Close();
			select.Dispose();
			trans.Commit();
			trans.Dispose();
		}
	}
}