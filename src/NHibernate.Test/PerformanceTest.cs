using System;
using System.Collections;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test 
{

	/// <summary>
	/// Does some quick & dirty Performance Tests.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The results of these tests are highly dependant upon your log4net settings. NHibernate
	/// logs alot of information and some of the log messages involve string concatenation and 
	/// calling other methods to generate a useful message.  
	/// </para>
	/// <para>
	/// On a production machine, the finest level of detail you should log is WARN.  On my machines
	/// test take about 3 times the amount of time to run with a log level of DEBUG compared to WARN.
	/// </para>
	/// <para>
	/// Currently (2004-05-26) NHibernate adds about 20% overhead with everything on the same machine/6% overhead
	/// with Sql Server on a different machine compared to a straight DataReader when
	/// the exact same sql is issued.  NHibernate's DriverConnectionProvider has not implemented a 
	/// Connection Cache or Prepared IDbCommand cache yet.  No cache was configured for the class Simple for
	/// this test.  So there are optimizations that are yet to be made, even then it is performing
	/// well compared to a DataReader.  I plan on making a seperate csproj to store Performance
	/// Test because NProf has problems with getting performance figures from dlls running in NUnit - or
	/// atleast I have not been able to get it to work. 
	/// </para>
	/// <para>
	/// Also test with a larger amount of data in the tables so they can be compared to a DataSet & Type DataSet
	/// will later be added.
	/// </para>
	/// </remarks>
	[TestFixture]
	public class PerformanceTest : TestCase 
	{
		string driverClass = null;
		IDriver driver = null;

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml"} );

			driverClass = (string)cfg.Properties["hibernate.connection.driver_class"];
			if(driverClass.IndexOf(",") < 0) 
			{
				driverClass += ", NHibernate";
			}

			driver = (IDriver)Activator.CreateInstance(System.Type.GetType(driverClass));
		}

		[Test]
		public void Many() 
		{
			IConnectionProvider cp = ConnectionProviderFactory.NewConnectionProvider( Cfg.Environment.Properties );

			long hiber = 0;
			long adonet = 0;
			
			//for(int n = 0; n < 20; n++) 
			for(int n = 0; n < 5; n++) 
			{
				Simple[] simples = new Simple[n];
				object[] ids = new object[n];

				for(int i = 0; i < n; i++) 
				{
					simples[i] = new Simple();
					simples[i].Init();
					simples[i].Count = i;
					ids[i] = (long)i;
				}

				// allow cache to settle

				ISession s = sessions.OpenSession();
				Hibernate(s, simples, ids, n, "h0");
				s.Close();

				IDbConnection c = cp.GetConnection();
				DirectAdoNet(c, simples, ids, n, "j0");
				cp.CloseConnection(c);

				s = sessions.OpenSession();
				Hibernate(s, simples, ids, n, "h0");
				s.Close();

				c = cp.GetConnection();
				DirectAdoNet(c, simples, ids, n, "j0");
				cp.CloseConnection(c);

				// now do timings

				int loops = 30;
				
				for(int runIndex = 1; runIndex < 4; runIndex++) 
				{
 
					long time = DateTime.Now.Ticks;
					for(int i = 0; i < loops; i++) 
					{
						s = sessions.OpenSession();
						Hibernate(s, simples, ids, n, "h" + runIndex.ToString());
						s.Close();
					}
					hiber += DateTime.Now.Ticks - time;

					time = DateTime.Now.Ticks;
					for(int i = 0; i < loops; i++) 
					{
						c = cp.GetConnection();
						DirectAdoNet(c, simples, ids, n, "j" + runIndex.ToString());
						cp.CloseConnection(c);
					}
					adonet += DateTime.Now.Ticks - time;


				}
			}
			System.Console.Out.Write("NHibernate: " + hiber + "ms / Direct ADO.NET: " + adonet + "ms = Ratio: " + (((float)hiber/adonet)).ToString() );
			
			cp.Close();
			System.GC.Collect();
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void Simultaneous() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void HibernateOnly() 
		{
		}

		[Test]
		[Ignore("Test not written yet.")]
		public void AdoNetOnly()
		{
		}

		private void Hibernate(ISession s, Simple[] simples, object[] ids, int N, string runname) 
		{
			ITransaction t = s.BeginTransaction();

			for(int i = 0; i < N; i++) 
			{
				s.Save(simples[i], ids[i]);
			}
			
			for(int i = 0; i < N; i++) 
			{
				simples[i].Name = "NH - " + i + N + runname + " - " + System.DateTime.Now.Ticks;
			}

			//s.Flush();
			// the results of this test are highly dependent upon
			// how many times we flush!

//			t.Commit();
//
//			t = s.BeginTransaction();
//
//			for(int i = 0; i < N; i++) 
//			{
//				s.Delete(simples[i]);
//			}
			Assert.IsTrue( s.Delete("from s in class NHibernate.DomainModel.Simple") == N);

			//s.Flush();
			t.Commit();
			
		}

		private void DirectAdoNet(IDbConnection c, Simple[] simples, object[] ids, int N, string runname) 
		{
			IDbCommand insert = InsertCommand();
			IDbCommand delete = DeleteCommand();
			IDbCommand select = SelectCommand();
			IDbCommand update = UpdateCommand();
			
			IDbTransaction t = c.BeginTransaction();

			insert.Connection = c;
			delete.Connection = c;
			select.Connection = c;
			update.Connection = c;
			
			insert.Transaction = t;
			delete.Transaction = t;
			select.Transaction = t;
			update.Transaction = t;

			insert.Prepare();
			delete.Prepare();
			select.Prepare();
			update.Prepare();

			for(int i = 0; i < N; i++) 
			{
				((IDbDataParameter)insert.Parameters[0]).Value = simples[i].Name;
				((IDbDataParameter)insert.Parameters[1]).Value = simples[i].Address;
				((IDbDataParameter)insert.Parameters[2]).Value = simples[i].Count;
				((IDbDataParameter)insert.Parameters[3]).Value = simples[i].Date;
				((IDbDataParameter)insert.Parameters[4]).Value = DBNull.Value;
				((IDbDataParameter)insert.Parameters[5]).Value = (long)ids[i];

				insert.ExecuteNonQuery();
			}
			
			for(int i = 0; i < N; i++) 
			{
				((IDbDataParameter)update.Parameters[0]).Value = "DR - " + i + N + runname + " - " + System.DateTime.Now.Ticks;
				((IDbDataParameter)update.Parameters[1]).Value = simples[i].Address;
				((IDbDataParameter)update.Parameters[2]).Value = simples[i].Count;
				((IDbDataParameter)update.Parameters[3]).Value = simples[i].Date;
				((IDbDataParameter)update.Parameters[4]).Value = DBNull.Value;
				((IDbDataParameter)update.Parameters[5]).Value = (long)ids[i];

				update.ExecuteNonQuery();
			}

			IDataReader reader = select.ExecuteReader();
			long[] keys = new long[N];
			Simple[] simplesFromReader = new Simple[N];
			int j = 0;

			long other;

			while(reader.Read()) 
			{
				//SELECT s.id_, s.name, s.address, s.count_, s.date_, s.other
				keys[j] = (long)reader[0];
				simplesFromReader[j] = new Simple();
				simplesFromReader[j].Name = (string)reader[1];
				simplesFromReader[j].Address = (string)reader[2];
				simplesFromReader[j].Count = (int)reader[3];
				simplesFromReader[j].Date = (DateTime)reader[4];
				
				if(reader.IsDBNull(5)==false) 
				{
					other = reader.GetInt64(5);
				}

				j++;
			}

			reader.Close();

			for(int i = 0; i < N; i++) 
			{
				((IDbDataParameter)delete.Parameters[0]).Value = (long)keys[i];
				delete.ExecuteNonQuery();
			}

			t.Commit();
		}

		private IDbCommand DeleteCommand() 
		{
			string sql = "delete from Simple where id_ = ";
			sql += driver.FormatNameForSql("iup0");

			IDbCommand cmd = driver.CreateCommand();
			cmd.CommandText = sql;

			IDbDataParameter prm = cmd.CreateParameter();
			prm.ParameterName = driver.FormatNameForParameter("iup0");
			prm.DbType = DbType.Int64;
			cmd.Parameters.Add(prm);

			return cmd;
		}

		private IDbCommand InsertCommand() 
		{
			string sql = "insert into Simple ( name, address, count_, date_, other, id_ ) values (";
			for(int i = 0; i < 6; i++ )
			{
				if(i > 0) sql += ", ";
				sql += driver.FormatNameForSql("iup" + i.ToString());
			}

			sql += ")";

			IDbCommand cmd = driver.CreateCommand();
			cmd.CommandText = sql;
			AppendInsertUpdateParams(cmd);		

			return cmd;
		}

		private IDbCommand SelectCommand() 
		{
			string sql = "SELECT s.id_, s.name, s.address, s.count_, s.date_, s.other FROM Simple s";
			
			IDbCommand cmd = driver.CreateCommand();
			cmd.CommandText = sql;

			return cmd;
		}

		private IDbCommand UpdateCommand() 
		{
			string sql = "update Simple set";
			sql += ( " name = " + driver.FormatNameForSql("iup0") ) ;
			sql += ( ", address = " + driver.FormatNameForSql("iup1") );
			sql += ( ", count_ = " + driver.FormatNameForSql("iup2") );
			sql += ( ", date_ = " + driver.FormatNameForSql("iup3") );
			sql += ( ", other = " + driver.FormatNameForSql("iup4") );
			sql += " where id_ = " + driver.FormatNameForSql("iup5");

			IDbCommand cmd = driver.CreateCommand();
			cmd.CommandText = sql;

			AppendInsertUpdateParams(cmd);

			return cmd;
		}

		private void AppendInsertUpdateParams(IDbCommand  cmd) 
		{
			IDbDataParameter[] prm = new IDbDataParameter[6];
			for(int j = 0; j < 6 ; j++) 
			{
				prm[j] = cmd.CreateParameter();
				prm[j].ParameterName = driver.FormatNameForParameter("iup" + j.ToString());
				cmd.Parameters.Add(prm[j]);
			}

			int i = 0;
			prm[i].DbType = DbType.String;
			prm[i].Size = 255;
			i++;

			prm[i].DbType = DbType.String;
			prm[i].Size = 200;
			i++;

			prm[i].DbType = DbType.Int32;
			i++;

			prm[i].DbType = DbType.DateTime;
			i++;

			prm[i].DbType = DbType.Int64;
			i++;

			prm[i].DbType = DbType.Int64;
			i++;
		
		}
	}
}
