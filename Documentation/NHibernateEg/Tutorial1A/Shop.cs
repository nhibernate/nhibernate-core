/******************************************************************************\
 *
 * NHibernateEg.Tutorial1A
 * Copyright © 2005, Pierre Henri Kuaté. All rights reserved.
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/

namespace NHibernateEg.Tutorial1A
{
	/// <summary>
	/// Shop where orders are done.
	/// </summary>
	public sealed class Shop
	{
		private NHibernate.ISessionFactory _sessionFact;


		/// <summary> Configure the access to the data source. </summary>
		public Shop(string database, string connectionString)
		{
			System.Console.Out.WriteLine("Configuration of NHibernate...\n");

			// Enable the logging of NHibernate operations
			log4net.Config.XmlConfigurator.Configure();

			// Create the object that will hold the configuration settings
			// and fill it with the information to access to the Database
			NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
			cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");

			System.Console.Out.WriteLine("Use database: <" + database + ">\n");

			if("MSSQL" == database)
			{
				cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSql2000Dialect");
				cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");
				cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);
			}
			else if("MySQL" == database)
			{
				cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MySQLDialect");
				cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.MySqlDataDriver");
				cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);
			}
			else
				throw new System.InvalidOperationException("The database '" + database + "' is not valid.");


			// Use NHibernate.Mapping.Attributes to create information about our entities
			System.IO.MemoryStream stream = new System.IO.MemoryStream(); // Where the information will be written
			NHibernate.Mapping.Attributes.HbmSerializer.Default.Validate = true; // Enable validation (optional)
			// Ask to NHibernate to use fields instead of properties
			NHibernate.Mapping.Attributes.HbmSerializer.Default.HbmDefaultAccess = "field.camelcase-underscore";
			// Gather information from this assembly (can also be done class by class)
			System.Console.Out.WriteLine("NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize()...\n");
			NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize(stream, System.Reflection.Assembly.GetExecutingAssembly());
			stream.Position = 0;
			cfg.AddInputStream(stream); // Send the Mapping information to NHibernate Configuration
			stream.Close();


			// Create table(s) in the database for our entities
			System.Console.Out.WriteLine("new NHibernate.Tool.hbm2ddl.SchemaExport(cfg).Create()...");
			new NHibernate.Tool.hbm2ddl.SchemaExport(cfg).Create(true, true);


			// Build the SessionFactory
			System.Console.Out.WriteLine("\n\nsessionFact = cfg.BuildSessionFactory();\n\n");
			_sessionFact = cfg.BuildSessionFactory();
		}




		/// <summary> Create "n" random orders and save them. </summary>
		public void GenerateRandomOrders(int n)
		{
			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;

			System.Console.Out.WriteLine("\nSaving " + n + " aleatory orders...");
			try
			{
				session = _sessionFact.OpenSession();
				transaction = session.BeginTransaction();

				for(int i=0; i<n; i++)
				{
					Order o = new Order();

					o.Product = "P" + (i+1).ToString();
					o.Quantity = n - i;
					o.ComputeTotalPrice(i * 10 + n);

					session.Save(o);
				}

				// Commit modifications (=> Build and execute queries)
				transaction.Commit();
			}
			catch
			{
				if(transaction != null)
					transaction.Rollback(); // Error => we MUST roll back modifications
				throw; // Here, we throw the same exception so that it is handled (printed)
			}
			finally
			{
				if(session != null)
					session.Close();
			}
		}


		/// <summary> For each order (in the database), write the identifier, the date and the product name. </summary>
		public void WriteAllOrders()
		{
			using(NHibernate.ISession session = _sessionFact.OpenSession())
			{
				System.Collections.IList result = session.Find("select o.Id, o.Date, o.Product from Order o");

				System.Console.Out.WriteLine("\n" + result.Count + " orders found!");
				foreach(System.Collections.IList l in result)
					System.Console.Out.WriteLine("  Order N°"
						+ l[0] + ",  Date=" + ((System.DateTime)l[1]).ToString("u") + ",  Product=" + l[2]);
			} // finally { session.Close(); }	is done by using()
		}


		/// <summary> Return the order N° "id". </summary>
		public Order LoadOrder(int id)
		{
			System.Console.Out.WriteLine("\nLoading order N° " + id + "...");
			using(NHibernate.ISession session = _sessionFact.OpenSession())
				return session.Load(typeof(Order), id) as Order;
			// finally { session.Close(); }	is done by using()
		}


		/// <summary> Write the order on the console. </summary>
		public void Write(Order o)
		{
			System.Console.Out.WriteLine("\nOrder N°"
				+ o.Id + "\n Date = " + o.Date.ToString("U") + "\n Product=" + o.Product
				+ ",  Quantity=" + o.Quantity + ",  TotalPrice=" + o.TotalPrice);
		}


		/// <summary> Save or Update the order (in the database). </summary>
		public void Save(Order o)
		{
			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;

			// That's how the Session decide to save or to update; set NHMA.Id(UnsavedValue=x) to replace 0
			System.Console.Out.Write("\n"  +  (o.Id == 0  ?  "Save"  :  "Update"));
			System.Console.Out.WriteLine(" the order N° " + o.Id + "...");
			try
			{
				session = _sessionFact.OpenSession();
				transaction = session.BeginTransaction();

				// NHibernate Session will automatically find out if it has to build an INSERT or an UPDATE
				session.SaveOrUpdate(o);

				// Commit modifications (=> Build and execute queries)
				transaction.Commit();
			}
			catch
			{
				if(transaction != null)
					transaction.Rollback(); // Error => we MUST roll back modifications
				throw; // Here, we throw the same exception so that it is handled (printed)
			}
			finally
			{
				if(session != null)
					session.Close();
			}
		}


		/// <summary> Add 'n' hours to all orders. </summary>
		public void ChangeTimeZone(int n)
		{
			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;

			System.Console.Out.WriteLine("\nChange the time zone of all orders: n=" + n + "...");
			try
			{
				session = _sessionFact.OpenSession();
				transaction = session.BeginTransaction();

				System.Collections.IList commandes = session.CreateCriteria(typeof(Order)).List();//session.Find("from Order");
				foreach(Order o in commandes)
					o.ChangeTimeZone(n);
				// It is useless to call Update(), the Session will automatically
				// detect modified entities (as long as it loaded them)
				System.Console.Out.WriteLine(commandes.Count + " updated orders!");

				// Commit modifications (=> Build and execute queries)
				transaction.Commit();
			}
			catch
			{
				if(transaction != null)
					transaction.Rollback(); // Error => we MUST roll back modifications
				throw; // Here, we throw the same exception so that it is handled (printed)
			}
			finally
			{
				if(session != null)
					session.Close();
			}
		}


		/// <summary> Delete the order (in the database). </summary>
		public void Delete(int id)
		{
			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;

			System.Console.Out.WriteLine("\nDeleting the order N° " + id + "...");
			try
			{
				session = _sessionFact.OpenSession();
				transaction = session.BeginTransaction();

				session.Delete("from Order o where o.Id = :Id", id, NHibernate.NHibernateUtil.Int32);

				// Commit modifications (=> Build and execute queries)
				transaction.Commit();
			}
			catch
			{
				if(transaction != null)
					transaction.Rollback(); // Error => we MUST roll back modifications
				throw; // Here, we throw the same exception so that it is handled (printed)
			}
			finally
			{
				if(session != null)
					session.Close();
			}
		}
	}
}
