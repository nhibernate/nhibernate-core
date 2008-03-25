/******************************************************************************\
 *
 * NHibernateEg.Tutorial1B
 * Copyright © 2006, Pierre Henri Kuaté. All rights reserved.
 *
 * This product is under the terms of the GNU Lesser General Public License.
 * Read the file "license.txt" for more details.
 *
\*/

namespace NHibernateEg.Tutorial1B.Forms
{
	/// <summary>
	/// Form used to display the list of available products, do basic searchs and add/edit/delete them.
	/// </summary>
	public class ProductsList : System.Windows.Forms.Form
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private System.Collections.IList _list = new System.Collections.ArrayList(); // List of Products

		private System.Windows.Forms.Label lblProductName;
		private System.Windows.Forms.TextBox editProductName;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.DataGrid dataGrid;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary> Default Constructor. </summary>
		public ProductsList()
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
			this.dataGrid = new System.Windows.Forms.DataGrid();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.lblProductName = new System.Windows.Forms.Label();
			this.editProductName = new System.Windows.Forms.TextBox();
			this.btnSearch = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGrid
			// 
			this.dataGrid.CaptionText = "Products";
			this.dataGrid.DataMember = "";
			this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid.Location = new System.Drawing.Point(8, 40);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.Size = new System.Drawing.Size(440, 300);
			this.dataGrid.TabIndex = 7;
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(8, 350);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(100, 23);
			this.btnAdd.TabIndex = 4;
			this.btnAdd.Text = "&Add new";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(376, 8);
			this.btnExit.Name = "btnExit";
			this.btnExit.TabIndex = 3;
			this.btnExit.Text = "E&xit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(180, 350);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(100, 23);
			this.btnSave.TabIndex = 5;
			this.btnSave.Text = "&Save current";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(348, 350);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(100, 23);
			this.btnDelete.TabIndex = 6;
			this.btnDelete.Text = "&Delete current";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// lblProductName
			// 
			this.lblProductName.Location = new System.Drawing.Point(8, 8);
			this.lblProductName.Name = "lblProductName";
			this.lblProductName.Size = new System.Drawing.Size(90, 20);
			this.lblProductName.TabIndex = 0;
			this.lblProductName.Text = "Product\'s Name:";
			this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editProductName
			// 
			this.editProductName.Location = new System.Drawing.Point(100, 8);
			this.editProductName.Name = "editProductName";
			this.editProductName.TabIndex = 1;
			this.editProductName.Text = "";
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(220, 7);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.TabIndex = 2;
			this.btnSearch.Text = "S&earch";
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// ProductsList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(454, 378);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.editProductName);
			this.Controls.Add(this.lblProductName);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.dataGrid);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ProductsList";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Products List";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion



		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			try
			{
				dataGrid.DataSource = _list = Program.Shop.GetProducts( editProductName.Text );
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}


		private void btnExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			_list.Add(new DomainModel.Product());
			RefreshDataGrid();
		}


		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(0 <= dataGrid.CurrentRowIndex && dataGrid.CurrentRowIndex < _list.Count)
				{
					Program.Shop.Save( _list[dataGrid.CurrentRowIndex] as DomainModel.Product );
					RefreshDataGrid();
				}
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}


		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(0 <= dataGrid.CurrentRowIndex && dataGrid.CurrentRowIndex < _list.Count)
				{
					int currentRow = dataGrid.CurrentRowIndex;
					dataGrid.CurrentRowIndex = 0; // Avoid IndexOutOfRangeException when deleting the last row

					if( (_list[currentRow] as DomainModel.Product).Id != System.Guid.Empty ) // Not transient
						Program.Shop.Delete( _list[currentRow] as DomainModel.Product );

					_list.RemoveAt( currentRow );
					RefreshDataGrid();
				}
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}


		/// <summary> Force DataGrid to read the list to show any added/updated/deleted entity. </summary>
		private void RefreshDataGrid()
		{
			// TODO: RefreshDataGrid - Is there a better way?
			dataGrid.DataSource = null;
			dataGrid.DataSource = _list;
		}
	}
}
