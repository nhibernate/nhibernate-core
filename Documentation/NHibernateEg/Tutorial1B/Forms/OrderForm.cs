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
	/// Form used to edit an order (and its details).
	/// </summary>
	public class OrderForm : System.Windows.Forms.Form
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private DomainModel.Order _order;
		System.Data.DataSet _detailsDataSet;
		private bool _editDetails;

		bool _autoChangingProduct = false;
		bool _updatingDetailsDatagrid = false;
		private int currentRow = -1; // In the details dataGrid

		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblDate;
		private System.Windows.Forms.TextBox editId;
		private System.Windows.Forms.Label lblCustomer;
		private System.Windows.Forms.TextBox editCustomer;
		private System.Windows.Forms.Label lblTotalPrice;
		private System.Windows.Forms.TextBox editTotalPrice;
		private System.Windows.Forms.DateTimePicker date;
		private System.Windows.Forms.Label lblId;
		private System.Windows.Forms.DataGrid gridDetails;
		private System.Windows.Forms.Button btnLazyLoad;
		private System.Windows.Forms.ComboBox comboProduct;
		private System.Windows.Forms.Label lblDetailProduct;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary> Edit a new order. </summary>
		public OrderForm() : this(0, true)
		{
		}


		/// <summary> Edit an existing order (or a new one if id==0). </summary>
		public OrderForm(int id, bool editDetails)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			editCustomer.MaxLength = 256;

			// Get the order
			if(id == 0)
			{
				_editDetails = true;
				_order = new DomainModel.Order();
				Text += " (new)";
			}
			else
			{
				_editDetails = editDetails;
				if(_editDetails)
					_order = Program.Shop.LoadOrderWithDetails(id);
				else
					_order = Program.Shop.LoadOrder(id);

				Text += " N°" + id;
			}

			// Bind the properties
			editId.DataBindings.Add("Text", _order, "Id");
			date.DataBindings.Add("Value", _order, "Date");
			editCustomer.DataBindings.Add("Text", _order, "Customer");
			editTotalPrice.DataBindings.Add("Text", _order, "TotalPrice");

			// Load the products
			comboProduct.DataSource = Program.Shop.GetProducts(null);
			comboProduct.DisplayMember = "Name";
			comboProduct.ValueMember = "Id"; // TODO: Fails if the Products' list is empty
			comboProduct.Text = string.Empty;

			btnLazyLoad.Enabled = !_editDetails;
			if(_editDetails)
				UpdateDetailsDataGrid();
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
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblDate = new System.Windows.Forms.Label();
			this.date = new System.Windows.Forms.DateTimePicker();
			this.editId = new System.Windows.Forms.TextBox();
			this.lblId = new System.Windows.Forms.Label();
			this.lblCustomer = new System.Windows.Forms.Label();
			this.editCustomer = new System.Windows.Forms.TextBox();
			this.lblTotalPrice = new System.Windows.Forms.Label();
			this.editTotalPrice = new System.Windows.Forms.TextBox();
			this.gridDetails = new System.Windows.Forms.DataGrid();
			this.btnLazyLoad = new System.Windows.Forms.Button();
			this.comboProduct = new System.Windows.Forms.ComboBox();
			this.lblDetailProduct = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.gridDetails)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(290, 10);
			this.btnSave.Name = "btnSave";
			this.btnSave.TabIndex = 1;
			this.btnSave.Text = "&Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(390, 10);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lblDate
			// 
			this.lblDate.Location = new System.Drawing.Point(300, 50);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(30, 20);
			this.lblDate.TabIndex = 0;
			this.lblDate.Text = "Date";
			this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// date
			// 
			this.date.CustomFormat = "yyyy-MM-dd     HH:mm";
			this.date.Enabled = false;
			this.date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.date.Location = new System.Drawing.Point(335, 50);
			this.date.Name = "date";
			this.date.Size = new System.Drawing.Size(140, 20);
			this.date.TabIndex = 4;
			// 
			// editId
			// 
			this.editId.Location = new System.Drawing.Point(70, 50);
			this.editId.Name = "editId";
			this.editId.ReadOnly = true;
			this.editId.Size = new System.Drawing.Size(80, 20);
			this.editId.TabIndex = 3;
			this.editId.Text = "";
			// 
			// lblId
			// 
			this.lblId.Location = new System.Drawing.Point(50, 50);
			this.lblId.Name = "lblId";
			this.lblId.Size = new System.Drawing.Size(15, 20);
			this.lblId.TabIndex = 0;
			this.lblId.Text = "Id";
			this.lblId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblCustomer
			// 
			this.lblCustomer.Location = new System.Drawing.Point(10, 80);
			this.lblCustomer.Name = "lblCustomer";
			this.lblCustomer.Size = new System.Drawing.Size(55, 20);
			this.lblCustomer.TabIndex = 0;
			this.lblCustomer.Text = "Customer";
			this.lblCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editCustomer
			// 
			this.editCustomer.Location = new System.Drawing.Point(70, 80);
			this.editCustomer.Name = "editCustomer";
			this.editCustomer.Size = new System.Drawing.Size(140, 20);
			this.editCustomer.TabIndex = 5;
			this.editCustomer.Text = "";
			// 
			// lblTotalPrice
			// 
			this.lblTotalPrice.Location = new System.Drawing.Point(270, 80);
			this.lblTotalPrice.Name = "lblTotalPrice";
			this.lblTotalPrice.Size = new System.Drawing.Size(60, 20);
			this.lblTotalPrice.TabIndex = 0;
			this.lblTotalPrice.Text = "Total Price";
			this.lblTotalPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editTotalPrice
			// 
			this.editTotalPrice.Location = new System.Drawing.Point(335, 80);
			this.editTotalPrice.Name = "editTotalPrice";
			this.editTotalPrice.ReadOnly = true;
			this.editTotalPrice.Size = new System.Drawing.Size(140, 20);
			this.editTotalPrice.TabIndex = 6;
			this.editTotalPrice.Text = "";
			// 
			// gridDetails
			// 
			this.gridDetails.AllowSorting = false;
			this.gridDetails.CaptionText = "Details";
			this.gridDetails.DataMember = "";
			this.gridDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.gridDetails.Location = new System.Drawing.Point(10, 120);
			this.gridDetails.Name = "gridDetails";
			this.gridDetails.Size = new System.Drawing.Size(480, 200);
			this.gridDetails.TabIndex = 7;
			this.gridDetails.CurrentCellChanged += new System.EventHandler(this.gridDetails_CurrentCellChanged);
			// 
			// btnLazyLoad
			// 
			this.btnLazyLoad.Location = new System.Drawing.Point(10, 330);
			this.btnLazyLoad.Name = "btnLazyLoad";
			this.btnLazyLoad.Size = new System.Drawing.Size(100, 23);
			this.btnLazyLoad.TabIndex = 8;
			this.btnLazyLoad.Text = "Lazy load details";
			this.btnLazyLoad.Click += new System.EventHandler(this.btnLazyLoad_Click);
			// 
			// comboProduct
			// 
			this.comboProduct.Location = new System.Drawing.Point(360, 330);
			this.comboProduct.Name = "comboProduct";
			this.comboProduct.Size = new System.Drawing.Size(130, 21);
			this.comboProduct.TabIndex = 9;
			this.comboProduct.Validating += new System.ComponentModel.CancelEventHandler(this.comboProduct_Validating);
			// 
			// lblDetailProduct
			// 
			this.lblDetailProduct.Location = new System.Drawing.Point(155, 330);
			this.lblDetailProduct.Name = "lblDetailProduct";
			this.lblDetailProduct.Size = new System.Drawing.Size(200, 21);
			this.lblDetailProduct.TabIndex = 0;
			this.lblDetailProduct.Text = "Select the product of the current detail:";
			this.lblDetailProduct.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// OrderForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(499, 358);
			this.Controls.Add(this.editTotalPrice);
			this.Controls.Add(this.editCustomer);
			this.Controls.Add(this.editId);
			this.Controls.Add(this.lblDetailProduct);
			this.Controls.Add(this.comboProduct);
			this.Controls.Add(this.btnLazyLoad);
			this.Controls.Add(this.gridDetails);
			this.Controls.Add(this.lblTotalPrice);
			this.Controls.Add(this.lblCustomer);
			this.Controls.Add(this.lblId);
			this.Controls.Add(this.lblDate);
			this.Controls.Add(this.date);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "OrderForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Order";
			((System.ComponentModel.ISupportInitialize)(this.gridDetails)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion



		private void UpdateDetailsDataGrid()
		{
			if(_updatingDetailsDatagrid)
				return; // Avoid recursive calls
			_updatingDetailsDatagrid = true;
			try
			{
				if(_detailsDataSet == null)
				{
					// Initialization of the DataSet holding the details collection
					_detailsDataSet = new System.Data.DataSet("Details");
					System.Data.DataTable table = new System.Data.DataTable("OrderDetail");
					table.Columns.Add("Id", typeof(int));
					table.Columns.Add("ProductName", typeof(string));
					table.Columns.Add("UnitPrice", typeof(float));
					table.Columns.Add("Quantity", typeof(int));
					table.Columns.Add("TotalPrice", typeof(float));
					_detailsDataSet.Tables.Add(table);

					table.RowChanged += new System.Data.DataRowChangeEventHandler(OnDetailRowChanged);
					table.RowDeleting += new System.Data.DataRowChangeEventHandler(OnDetailRowChanged);


					// Bind the DataGrid to the DataSet, displaying the details.
					gridDetails.SetDataBinding(_detailsDataSet, "OrderDetail");
				}

				// Allow edition of columns
				foreach(System.Data.DataColumn column in _detailsDataSet.Tables["OrderDetail"].Columns)
					column.ReadOnly = false;

				// Fill the dataSet with the order details
				int i=0;
				System.Data.DataRowCollection rows = _detailsDataSet.Tables["OrderDetail"].Rows;
				// Update existing rows; don't replace them to avoid triggering OnDetailRowChanged()
				for(; i<_order.DetailsCount; i++)
				{
					DomainModel.OrderDetail od = _order.GetDetailAt(i);
					object[] itemArray = new object[] { od.Id, od.ProductName, od.UnitPrice, od.Quantity, od.TotalPrice };
					if( i < rows.Count )
						rows[i].ItemArray = itemArray;
					else
						rows.Add( itemArray );
				}
				// Remove remaining rows (have been deleted)
				while(i < rows.Count)
					rows[ rows.Count-1 ].Delete();

				// Set back columns to read-only (except the Quantity column)
				foreach(System.Data.DataColumn column in _detailsDataSet.Tables["OrderDetail"].Columns)
					column.ReadOnly = true;
				_detailsDataSet.Tables["OrderDetail"].Columns["Quantity"].ReadOnly = false;


				// TODO: Better way to   Refresh editTotalPrice
				editTotalPrice.DataBindings.Clear();
				editTotalPrice.DataBindings.Add("Text", _order, "TotalPrice");
			}
			finally
			{
				_updatingDetailsDatagrid = false;
			}
		}



		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Program.Shop.Save(_order);
				Close();
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}



		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}



		private void btnLazyLoad_Click(object sender, System.EventArgs e)
		{
			try
			{
				// Do the lazy loading
				Program.Shop.LazyLoadDetails(_order);

				_editDetails = true;
				btnLazyLoad.Enabled = !_editDetails;
				UpdateDetailsDataGrid();
			}
			catch(System.Exception ex)
			{
				log.Error("Unexpected Exception", ex);
				System.Windows.Forms.MessageBox.Show(ex.ToString(), "NHibernateEg.Tutorial1B",
					System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}



		private void OnDetailRowChanged(object sender, System.Data.DataRowChangeEventArgs e)
		{
			if( _updatingDetailsDatagrid || _autoChangingProduct )
				return;
			if( comboProduct.SelectedValue == null )
			{
				// Don't go without selecting the product
				comboProduct.Focus();
				return;
			}

			// Synchronize the Order's Details collection with the DataGrid

			if( e.Action == System.Data.DataRowAction.Add )
			{
				int id = 0;
				if( e.Row["Id"] is int ) // Can be null
					id = (int) e.Row["Id"];
				if( id==0 || !_order.ContainsDetail( id ) )
				{
					DomainModel.OrderDetail detail = new DomainModel.OrderDetail();
					if( e.Row["Quantity"] is int && 0<(int)e.Row["Quantity"])
						detail.Quantity = (int) e.Row["Quantity"];
					_order.Add( detail );
					UpdateDetailsDataGrid();
				}
			}
			if( e.Action == System.Data.DataRowAction.Change )
			{
				if(_order.GetDetailAt(currentRow).Quantity != (int) e.Row["Quantity"])
				{
					_order.GetDetailAt(currentRow).Quantity = (int) e.Row["Quantity"];
					UpdateDetailsDataGrid();
				}
			}
			if( e.Action == System.Data.DataRowAction.Delete )
			{
				if(currentRow == -1) currentRow = 0; // Occurs when you directly delete the first row (without a click inside the grid)
				_order.RemoveDetailAt(currentRow);
				// TODO: Better way to   Refresh editTotalPrice
				editTotalPrice.DataBindings.Clear();
				editTotalPrice.DataBindings.Add("Text", _order, "TotalPrice");
				if(_order.DetailsCount == 0)
					currentRow = 1; // Required to be sure that rowChanged will be true (to add a new line)
			}
		}


		private void gridDetails_CurrentCellChanged(object sender, System.EventArgs e)
		{
			// Ignore if updating the dataGrid or the Product is not selected (=> will not lose its focus)
			if( _updatingDetailsDatagrid )
				return;

			if( currentRow == gridDetails.CurrentCell.RowNumber )
				return; // The row hasn't changed
			currentRow = gridDetails.CurrentCell.RowNumber;

			if( comboProduct.SelectedValue == null )
			{
				// Don't go without selecting the product
				comboProduct.Focus();
				return;
			}

			// Set the product in the comboBox (select the right item)
			_autoChangingProduct = true;
			try 
			{
				if(currentRow < _order.DetailsCount)
				{
					comboProduct.SelectedIndex = -1; // First, no selection
					if(_order.GetDetailAt(currentRow).Product != null)
					{
						// Find this product in the list and select it
						System.Collections.IList products = comboProduct.DataSource as System.Collections.IList;
						for(int i=0; i<products.Count; i++)
							if( ((DomainModel.Product)products[i]).Id == _order.GetDetailAt(currentRow).Product.Id )
								comboProduct.SelectedIndex = i;
					}
				}
				else
				{
					// Create a new row
					comboProduct.SelectedIndex = -1;
					// TODO: I don't know why, but sometimes, SelectedIndex go back to 0 :D
					if(comboProduct.SelectedIndex != -1)
						comboProduct.SelectedIndex = -1;
					_order.Add( new DomainModel.OrderDetail() );
					_detailsDataSet.Tables["OrderDetail"].Rows.Add( new object[] {0, "", 0, 0, 0} );
				}
			}
			finally { _autoChangingProduct = false; }
		}


		private void comboProduct_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(_autoChangingProduct)
				return;
			if(comboProduct.SelectedValue == null)
			{
				if(_order.GetDetailAt(currentRow).ProductName == null || _order.GetDetailAt(currentRow).ProductName == string.Empty)
				{
					System.Windows.Forms.MessageBox.Show("You must select a product.", "NHibernateEg.Tutorial1B");
					e.Cancel = true;
				} // else the product has probably been deleted
				return;
			}
			if(0 > currentRow || currentRow >= _order.DetailsCount)
			{
				// Occurs when the details aren't loaded yet
				// May also occurs when deleting all details
				return;
			}


			// Set the product to the detail
			foreach(DomainModel.Product product in comboProduct.DataSource as System.Collections.IList)
				if( product.Id == (System.Guid)comboProduct.SelectedValue )
					_order.GetDetailAt(currentRow).Product = product;

			UpdateDetailsDataGrid();
		}
	}
}
