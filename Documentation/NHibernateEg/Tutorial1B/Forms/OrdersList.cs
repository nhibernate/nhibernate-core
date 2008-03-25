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
	/// Form used to display the list of orders, do basic searchs and add/edit/delete them (with OrderForm).
	/// </summary>
	public class OrdersList : System.Windows.Forms.Form
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private System.Windows.Forms.Label lblDateInterval;
		private System.Windows.Forms.DateTimePicker dateMin;
		private System.Windows.Forms.DateTimePicker dateMax;
		private System.Windows.Forms.Label lblMin;
		private System.Windows.Forms.Label lblMax;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.ListView listOrders;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.ColumnHeader listColumnId;
		private System.Windows.Forms.ColumnHeader listColumnDate;
		private System.Windows.Forms.ColumnHeader listColumnCustomer;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary> Default Constructor. </summary>
		public OrdersList()
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
			this.lblDateInterval = new System.Windows.Forms.Label();
			this.dateMin = new System.Windows.Forms.DateTimePicker();
			this.dateMax = new System.Windows.Forms.DateTimePicker();
			this.lblMin = new System.Windows.Forms.Label();
			this.lblMax = new System.Windows.Forms.Label();
			this.btnSearch = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.listOrders = new System.Windows.Forms.ListView();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.listColumnId = new System.Windows.Forms.ColumnHeader();
			this.listColumnDate = new System.Windows.Forms.ColumnHeader();
			this.listColumnCustomer = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// lblDateInterval
			// 
			this.lblDateInterval.Location = new System.Drawing.Point(10, 25);
			this.lblDateInterval.Name = "lblDateInterval";
			this.lblDateInterval.Size = new System.Drawing.Size(110, 20);
			this.lblDateInterval.TabIndex = 0;
			this.lblDateInterval.Text = "Order\'s date interval";
			this.lblDateInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dateMin
			// 
			this.dateMin.CustomFormat = "yyyy-MM-dd     HH:mm";
			this.dateMin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateMin.Location = new System.Drawing.Point(150, 10);
			this.dateMin.Name = "dateMin";
			this.dateMin.Size = new System.Drawing.Size(140, 20);
			this.dateMin.TabIndex = 1;
			this.dateMin.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
			// 
			// dateMax
			// 
			this.dateMax.CustomFormat = "yyyy-MM-dd     HH:mm";
			this.dateMax.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateMax.Location = new System.Drawing.Point(150, 40);
			this.dateMax.Name = "dateMax";
			this.dateMax.Size = new System.Drawing.Size(140, 20);
			this.dateMax.TabIndex = 2;
			// 
			// lblMin
			// 
			this.lblMin.Location = new System.Drawing.Point(120, 10);
			this.lblMin.Name = "lblMin";
			this.lblMin.Size = new System.Drawing.Size(30, 20);
			this.lblMin.TabIndex = 0;
			this.lblMin.Text = "Min";
			this.lblMin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblMax
			// 
			this.lblMax.Location = new System.Drawing.Point(120, 40);
			this.lblMax.Name = "lblMax";
			this.lblMax.Size = new System.Drawing.Size(30, 20);
			this.lblMax.TabIndex = 0;
			this.lblMax.Text = "Max";
			this.lblMax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(310, 25);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.TabIndex = 3;
			this.btnSearch.Text = "&Search";
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(415, 25);
			this.btnExit.Name = "btnExit";
			this.btnExit.TabIndex = 4;
			this.btnExit.Text = "E&xit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// listOrders
			// 
			this.listOrders.Activation = System.Windows.Forms.ItemActivation.TwoClick;
			this.listOrders.AutoArrange = false;
			this.listOrders.BackColor = System.Drawing.SystemColors.ControlLight;
			this.listOrders.CheckBoxes = true;
			this.listOrders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.listColumnId,
																						 this.listColumnDate,
																						 this.listColumnCustomer});
			this.listOrders.FullRowSelect = true;
			this.listOrders.GridLines = true;
			this.listOrders.Location = new System.Drawing.Point(10, 80);
			this.listOrders.MultiSelect = false;
			this.listOrders.Name = "listOrders";
			this.listOrders.Size = new System.Drawing.Size(480, 250);
			this.listOrders.TabIndex = 5;
			this.listOrders.View = System.Windows.Forms.View.Details;
			this.listOrders.DoubleClick += new System.EventHandler(this.listOrders_DoubleClick);
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(40, 345);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(100, 23);
			this.btnAdd.TabIndex = 6;
			this.btnAdd.Text = "&Add new";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Location = new System.Drawing.Point(200, 345);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(100, 23);
			this.btnEdit.TabIndex = 7;
			this.btnEdit.Text = "&Edit current";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Location = new System.Drawing.Point(360, 345);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(100, 23);
			this.btnDelete.TabIndex = 8;
			this.btnDelete.Text = "&Delete selection";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// listColumnId
			// 
			this.listColumnId.Text = "Id";
			// 
			// listColumnDate
			// 
			this.listColumnDate.Text = "Date";
			this.listColumnDate.Width = 200;
			// 
			// listColumnCustomer
			// 
			this.listColumnCustomer.Text = "Customer";
			this.listColumnCustomer.Width = 200;
			// 
			// OrdersList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(494, 378);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.listOrders);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.lblMax);
			this.Controls.Add(this.lblMin);
			this.Controls.Add(this.dateMax);
			this.Controls.Add(this.dateMin);
			this.Controls.Add(this.lblDateInterval);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "OrdersList";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Orders List";
			this.ResumeLayout(false);

		}
		#endregion



		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			try
			{
				System.Collections.IList list = Program.Shop.GetOrders( dateMin.Value, dateMax.Value );

				listOrders.Items.Clear();

				System.Windows.Forms.ListViewItem[] listViewItems = new System.Windows.Forms.ListViewItem[list.Count];
				for(int i=0; i<list.Count; i++)
				{
					System.Collections.IList order = list[i] as System.Collections.IList;
					listViewItems[i] = new System.Windows.Forms.ListViewItem();
					listViewItems[i].Text = order[0].ToString();
					listViewItems[i].SubItems.Add( order[1].ToString() );
					listViewItems[i].SubItems.Add( order[2].ToString() );
				}
				this.listOrders.Items.AddRange(listViewItems);
				UpdateItemsColors();
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
			try
			{
				new OrderForm().ShowDialog();
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}


		private void btnEdit_Click(object sender, System.EventArgs e)
		{
			try
			{
				if( this.listOrders.SelectedItems.Count == 0 )
					return;

				bool editDetails = System.Windows.Forms.MessageBox.Show(
						"Do you want to edit the details of this order?\n\nYou can select 'No' and lazy load it later in the Form.", "NHibernateEg.Tutorial1B",
						System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question )
					== System.Windows.Forms.DialogResult.Yes;

				new OrderForm( int.Parse(this.listOrders.SelectedItems[0].SubItems[0].Text), editDetails )
					.ShowDialog();
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}


		private void listOrders_DoubleClick(object sender, System.EventArgs e)
		{
			// Change back its checked state and edit it
			if(this.listOrders.SelectedItems.Count>0)
				this.listOrders.SelectedItems[0].Checked = !this.listOrders.SelectedItems[0].Checked;
			this.btnEdit_Click(sender, e);
		}


		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			try
			{
				bool AskConfirmation = true;
				for(int i=this.listOrders.Items.Count-1; i>=0; i--)
					if(this.listOrders.Items[i].Checked)
					{
						if(AskConfirmation)
						{
							if(System.Windows.Forms.DialogResult.No == System.Windows.Forms.MessageBox.Show("Are you sure that you want to delete these order(s) ?", "NHibernateEg.Tutorial1B", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question))
								return; // Cancel
							AskConfirmation = false;
						}

						Program.Shop.DeleteOrder( int.Parse(this.listOrders.Items[i].SubItems[0].Text) );

						this.listOrders.Items.RemoveAt(i);
					}
				UpdateItemsColors();
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}



		private void UpdateItemsColors()
		{
			// Alternate the color :)
			for(int i=0; i<this.listOrders.Items.Count; i++)
				this.listOrders.Items[i].BackColor = (i%2==0) ? System.Drawing.Color.White : System.Drawing.Color.FromArgb(248, 245, 250);
		}
	}
}
