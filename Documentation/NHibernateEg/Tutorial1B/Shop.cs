/******************************************************************************\
 *
 * NHibernateEg.Tutorial1B
 * Copyright © 2006, Pierre Henri Kuaté. All rights reserved.
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/

namespace NHibernateEg.Tutorial1B
{
	/// <summary>
	/// The Repository: Used to operate on entities.
	/// Wrap the access to the data source, hiding the ORM and the database.
	/// </summary>
	public sealed class Shop
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private NHibernate.Cfg.Configuration _cfg;
		private NHibernate.ISessionFactory _sessionFactory;


		/// <summary> Configure the access to the data source. </summary>
		public Shop()
		{
			log.Info("\nConfiguration of NHibernate...\n");

			// Enable the logging of NHibernate operations
			log4net.Config.XmlConfigurator.Configure();

			// Create the object that will hold the configuration settings
			// and fill it with the information to access to the Database
			_cfg = new NHibernate.Cfg.Configuration();
			_cfg.Configure(); // Use the file "hibernate.cfg.xml"


			// Use NHibernate.Mapping.Attributes to create information about our entities
			NHibernate.Mapping.Attributes.HbmSerializer.Default.Validate = true; // Enable validation (optional)
			// Ask to NHibernate to use fields instead of properties (in entities)
			NHibernate.Mapping.Attributes.HbmSerializer.Default.HbmDefaultAccess = "field.camelcase-underscore";
			// Gather information from this assembly (can also be done class by class)
			log.Info("NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize()...\n");
			if( System.IO.Directory.Exists("../../DomainModel/") ) // Save it here
				NHibernate.Mapping.Attributes.HbmSerializer.Default.Serialize(
					"../../DomainModel/", System.Reflection.Assembly.GetExecutingAssembly() );

			// Send the mapping information to NHibernate Configuration
			// Here, we use the embedded hbm.xml files
			_cfg.AddAssembly( System.Reflection.Assembly.GetExecutingAssembly() );


			// Build the SessionFactory
			log.Info("sessionFact = cfg.BuildSessionFactory();\n\n");
			_sessionFactory = _cfg.BuildSessionFactory();


			// Log the ConnectionString
			using(NHibernate.ISession s = _sessionFactory.OpenSession())
				log.Info("ConnectionString: <" + s.Connection.ConnectionString + ">\n");
		}




		/// <summary> Create tables and relationships in the database for our entities (try to drop them before). </summary>
		public void ExportSchema()
		{
			log.Info("\nSchemaExport.Create()...\n");
			NHibernate.Tool.hbm2ddl.SchemaExport schemaExport = new NHibernate.Tool.hbm2ddl.SchemaExport(_cfg);
			schemaExport.Create(false, true);

			// Save the script used to fill the database
			if( System.IO.Directory.Exists("../../DomainModel/") )
				schemaExport.SetOutputFile("../../DomainModel/DatabaseScript.sql")
					.Create(false, false);
		}




		/// <summary> Fill the data source with some random data. </summary>
		public void FillWithRandomData()
		{
			const int n = 4;
			log.Info("\nFill the data source with some random data...");
			log.Info("\nGenerate " + n + " products and " + n + " orders with details\n");

			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;
			try
			{
				session = _sessionFactory.OpenSession();
				transaction = session.BeginTransaction();

				System.Random random = new System.Random( (int)System.DateTime.Now.Ticks );

				System.Collections.Specialized.StringCollection nameList = new System.Collections.Specialized.StringCollection();
				nameList.AddRange( new string[] {"John Smith", System.Environment.UserName, "Doe, John", System.Environment.MachineName} );
				System.Collections.ArrayList productList = new System.Collections.ArrayList();
				for(int i=0; i<n; i++)
				{
					DomainModel.Product p = new DomainModel.Product("Product_" + random.Next(100), (i+1) * 250);
					p.UnitsInStock = i * 10 + 1;
					session.Save(p);
					productList.Add(p);
				}
				for(int i=0; i<n; i++)
				{
					DomainModel.Order o = new DomainModel.Order();
					o.Customer = nameList[random.Next(nameList.Count)];
					for(int j=0; j<i+1; j++)
						o.Add( new DomainModel.OrderDetail(productList[random.Next(n)] as DomainModel.Product, j*3+1) );
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




		/// <summary> Return all products which names contain this string. </summary>
		public System.Collections.IList GetProducts(string name)
		{
			log.Info("\nLoad products where name like '" + name + "'\n");
			using(NHibernate.ISession session = _sessionFactory.OpenSession())
				return session.CreateCriteria(typeof(DomainModel.Product))
					.Add(new NHibernate.Expression.LikeExpression("Name", name, NHibernate.Expression.MatchMode.Anywhere))
					.AddOrder(new NHibernate.Expression.Order("Name", true))
					.List();
		}


		/// <summary> Return all orders which dates are between these dates. </summary>
		public System.Collections.IList GetOrders(System.DateTime dateMin, System.DateTime dateMax)
		{
			log.Info("\nLoad orders where date between '" + dateMin.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + dateMax.ToString("yyyy-MM-dd HH:mm:ss") + "'\n");
			using(NHibernate.ISession session = _sessionFactory.OpenSession())
				return session.CreateQuery("select e.Id, e.Date, e.Customer from Order e where e.Date between :dateMin and :dateMax")
					.SetDateTime("dateMin", dateMin).SetDateTime("dateMax", dateMax)
					.List();
		}




		/// <summary> Save a product. </summary>
		public void Save(DomainModel.Product e)
		{
			if(e == null)
				throw new System.ArgumentNullException("e");
			if( ! NHibernate.NHibernateUtil.IsInitialized(e) )
				return; // Useless to save it as it, obviously, hasn't been modified
			log.Info("\nSave Product: " + e.Name + "\n");

			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;
			try
			{
				session = _sessionFactory.OpenSession();
				transaction = session.BeginTransaction();

				e.Validate();
				session.SaveOrUpdate(e);

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


		/// <summary> Save an order. </summary>
		public void Save(DomainModel.Order e)
		{
			if(e == null)
				throw new System.ArgumentNullException("e");
			log.Info("\nSave Order N°" + e.Id + " - " + e.Date.ToString("u") + " from customer: " + e.Customer + "\n");

			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;
			try
			{
				session = _sessionFactory.OpenSession();
				transaction = session.BeginTransaction();

				// Attach the entity if the Details is uninitialized
				bool entityAttached = false;
				try
				{
					e.DetailsCount.ToString();
				}
				catch
				{
					session.Lock(e, NHibernate.LockMode.None);
					entityAttached = true;
				}

				if( ! entityAttached ) // Loop through orderDetails' products and attach uninitialized ones
					for(int i=0; i<e.DetailsCount; i++) // e.Details is initialized
						if( ! NHibernate.NHibernateUtil.IsInitialized( e.GetDetailAt(i).Product ) )
							session.Lock(e.GetDetailAt(i).Product, NHibernate.LockMode.None);

				e.Validate();

				if(entityAttached)
					session.Evict(e); // Required because SaveOrUpdate() will do nothing if the entity hasn't changed since its locking

				session.SaveOrUpdate(e);

				log.Debug("transaction.Commit()...\n");
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


		/// <summary> Delete a product. </summary>
		public void Delete(DomainModel.Product e)
		{
			log.Info("\nDelete Product: " + e.Name + ", Id=" + e.Id + "\n");

			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;
			try
			{
				session = _sessionFactory.OpenSession();
				transaction = session.BeginTransaction();

				// Get all OrderDetails referring to this product
				System.Collections.IList list = session.CreateQuery( "from OrderDetail e where e.Product = ?" )
					.SetEntity( 0, e ).List();
//				System.Collections.IList list = session.Find( "from OrderDetail e where e.Product = ?",
//					e, NHibernate.NHibernateUtil.Entity( NHibernate.Proxy.NHibernateProxyHelper.GetClass(e) ) );
				foreach(DomainModel.OrderDetail detail in list)
				{
					// Remove the reference to this product so that it can be deleted
					detail.Product = null;
					// Note that we don't need to explicitly update it because the transparent persistence :)
				}

				session.Delete(e);

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


		public void DeleteOrder(int id)
		{
			log.Info("\nDelete Order N°" + id + "\n");

			NHibernate.ISession session = null;
			NHibernate.ITransaction transaction = null;
			try
			{
				session = _sessionFactory.OpenSession();
				transaction = session.BeginTransaction();

				// With this query, the details are loaded in a separated query
//				session.Delete("from Order e where e.Id = :Id", id, NHibernate.NHibernateUtil.Int32);
				// And with this one, the session load the details in the same query (faster)
				session.Delete("from Order e left join fetch e.Details where e.Id = :Id", id, NHibernate.NHibernateUtil.Int32);

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


		/// <summary> Return the order N° "id". </summary>
		public DomainModel.Order LoadOrder(int id)
		{
			log.Info("\nLoad Order N°" + id + "\n");
			using(NHibernate.ISession session = _sessionFactory.OpenSession())
				return session.Load(typeof(DomainModel.Order), id) as DomainModel.Order;
		}


		/// <summary> Return the order N° "id" (with its details). </summary>
		public DomainModel.Order LoadOrderWithDetails(int id)
		{
			log.Info("\nLoad Order N°" + id + " (with its details)\n");

			using(NHibernate.ISession session = _sessionFactory.OpenSession())
			{
				DomainModel.Order order = session.CreateCriteria(typeof(DomainModel.Order))
					.Add( new NHibernate.Expression.EqExpression("Id", id) )
					.SetFetchMode("Details", NHibernate.FetchMode.Eager)
					.UniqueResult() as DomainModel.Order;

				if(order == null)
					throw new System.ArgumentException("Can't find the Order N°" + id, "id");
				return order;
			}
		}


		/// <summary> Fill the Details collection of this order. </summary>
		public void LazyLoadDetails(DomainModel.Order order)
		{
			if(order == null)
				throw new System.ArgumentNullException("order");

			log.Info("\nLazy load the details of the order: N°" + order.Id + "\n");

			using(NHibernate.ISession session = _sessionFactory.OpenSession())
			{
				// Attach the order to this opened session
				session.Lock(order, NHibernate.LockMode.None);
				// Lazy loading (may) occurs here
				order.DetailsCount.ToString();
			}
		}
	}
}
