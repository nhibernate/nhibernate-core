/******************************************************************************\
 *
 * NHibernateEg.Tutorial1B
 * Copyright © 2006, Pierre Henri Kuaté. All rights reserved.
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/

[assembly: log4net.Config.XmlConfigurator(Watch=true)]

namespace NHibernateEg.Tutorial1B
{
	/// <summary>
	/// Contains the main entry point.
	/// </summary>
	internal sealed class Program
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private static Shop _shop;

		/// <summary> Shop. </summary>
		public static Shop Shop
		{
			get { return _shop; }
		}


		/// <summary> Instantiation forbidden. </summary>
		private Program()
		{
			throw new System.NotSupportedException();
		}


		/// <summary> The main entry point for the application. </summary>
		[System.STAThread]
		static void Main()
		{
			try
			{
				log.Info("Application is starting...\n");
				_shop = new Shop();
				log.Info("\nThe application is ready!\n");

				System.Windows.Forms.Application.Run( new MainForm() );
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}
	}
}
