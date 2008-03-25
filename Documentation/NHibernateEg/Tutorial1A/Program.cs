/******************************************************************************\
 *
 * NHibernateEg.Tutorial1A
 * Copyright © 2006, Pierre Henri Kuaté. All rights reserved.
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/

[assembly: log4net.Config.XmlConfigurator(Watch=true)]

namespace NHibernateEg.Tutorial1A
{
	/// <summary>
	/// Contains the main entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary> Instantiation forbidden. </summary>
		private Program()
		{
			throw new System.NotSupportedException();
		}



		/// <summary> The main entry point for the application. </summary>
		[System.STAThread]
		static void Main()
		{
			System.Console.Out.WriteLine("Application is starting...\n");

			try
			{
				string database = System.Configuration.ConfigurationSettings.AppSettings["Database"];
				string connectionString = System.Configuration.ConfigurationSettings.AppSettings[database + ".ConnectionString"];

				Shop shop = new Shop(database, connectionString);

				shop.GenerateRandomOrders(3);

				shop.WriteAllOrders();

				Order o = shop.LoadOrder(1);

				o.Product += " (updated)";
				shop.Write(o);
				shop.Save(o);

				shop.Save( new Order("New", 4, 2) );

				shop.ChangeTimeZone(25);

				shop.Delete(2);

				shop.WriteAllOrders();
			}
			catch(System.Exception ex)
			{
				System.Console.Error.WriteLine("\n\n" + ex.ToString() + "\n\n");
			}
			finally
			{
				System.Console.Out.WriteLine("\nApplication is closed!\n");
			}
		}
	}
}
