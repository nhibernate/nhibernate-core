using System;
using NHibernate.Example.Web;

public partial class ViewData : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		ExampleApplication.SessionFactory.GetCurrentSession().BeginTransaction();
	}

	protected override void OnUnload(EventArgs e)
	{
		ExampleApplication.SessionFactory.GetCurrentSession().Transaction.Commit();
		base.OnUnload(e);
	}
}
