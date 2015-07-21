using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NHibernate.Example.Web;

public partial class InsertItem : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}

	protected void OKButton_Click(object sender, EventArgs e)
	{
		ExampleApplication.GetCurrentSession().BeginTransaction();
		InsertForm.InsertItem(true);
		ExampleApplication.GetCurrentSession().Transaction.Commit();
		Response.Redirect("~/ViewData.aspx");
	}
	protected void CancelButton_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/ViewData.aspx");
	}
}
