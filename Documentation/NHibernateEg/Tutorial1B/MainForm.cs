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
	/// Main Form (pseudo-MDI)
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuExit;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ToolBarButton toolBarSchemaExport;
		private System.Windows.Forms.ToolBarButton toolBarGenData;
		private System.Windows.Forms.ToolBarButton toolBarSeparator1;
		private System.Windows.Forms.ToolBarButton toolBarProducts;
		private System.Windows.Forms.ToolBarButton toolBarOrders;
		private System.Windows.Forms.ToolBarButton toolBarExit;
		private System.Windows.Forms.ToolBarButton toolBarSeparator2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary> Default Constructor. </summary>
		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuExit = new System.Windows.Forms.MenuItem();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.toolBarProducts = new System.Windows.Forms.ToolBarButton();
			this.toolBarOrders = new System.Windows.Forms.ToolBarButton();
			this.toolBarSeparator2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarExit = new System.Windows.Forms.ToolBarButton();
			this.toolBarSeparator1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarSchemaExport = new System.Windows.Forms.ToolBarButton();
			this.toolBarGenData = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFile});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuExit});
			this.menuFile.Text = "&File";
			// 
			// menuExit
			// 
			this.menuExit.Index = 0;
			this.menuExit.Text = "E&xit";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// toolBar
			// 
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.toolBarSchemaExport,
																					   this.toolBarGenData,
																					   this.toolBarSeparator1,
																					   this.toolBarProducts,
																					   this.toolBarOrders,
																					   this.toolBarSeparator2,
																					   this.toolBarExit});
			this.toolBar.Cursor = System.Windows.Forms.Cursors.Hand;
			this.toolBar.DropDownArrows = true;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(632, 42);
			this.toolBar.TabIndex = 1;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// toolBarProducts
			// 
			this.toolBarProducts.Text = "Products";
			this.toolBarProducts.ToolTipText = "Show all available products";
			// 
			// toolBarOrders
			// 
			this.toolBarOrders.Text = "Orders";
			this.toolBarOrders.ToolTipText = "Show the list of orders";
			// 
			// toolBarSeparator2
			// 
			this.toolBarSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarExit
			// 
			this.toolBarExit.Text = "Exit";
			this.toolBarExit.ToolTipText = "Close this application";
			// 
			// toolBarSeparator1
			// 
			this.toolBarSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarSchemaExport
			// 
			this.toolBarSchemaExport.Text = "SchemaExport";
			// 
			// toolBarGenData
			// 
			this.toolBarGenData.Text = "Generate Data";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(632, 453);
			this.Controls.Add(this.toolBar);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "NHibernateEg.Tutorial1B";
			this.ResumeLayout(false);

		}
		#endregion


		private void menuExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			try
			{
				if(e.Button == this.toolBarSchemaExport)
				{
					Program.Shop.ExportSchema();
					System.Windows.Forms.MessageBox.Show("SchemaExport: Creating tables and relationships in the database succeeded!", "NHibernateEg.Tutorial1B");
				}
				else if(e.Button == this.toolBarGenData)
				{
					Program.Shop.FillWithRandomData();
					System.Windows.Forms.MessageBox.Show("The database was successfully filled with some random data!", "NHibernateEg.Tutorial1B");
				}

				else if(e.Button == this.toolBarProducts)
					new Forms.ProductsList().ShowDialog();
				else if(e.Button == this.toolBarOrders)
					new Forms.OrdersList().ShowDialog();

				else if(e.Button == this.toolBarExit)
					this.menuExit_Click(sender, e);
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
