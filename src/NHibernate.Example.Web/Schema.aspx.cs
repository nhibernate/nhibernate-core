using System;
using NHibernate;
using NHibernate.Example.Web;
using NHibernate.Example.Web.Domain;
using NHibernate.Tool.hbm2ddl;

public partial class Schema : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	protected void CreateSchema_Click(object sender, EventArgs e)
	{
		SchemaExport export = new SchemaExport(ExampleApplication.Configuration);
		export.Create(false, true);

		ISession session = ExampleApplication.SessionFactory.GetCurrentSession();
		session.BeginTransaction();
		Item item1 = new Item();
		item1.Description = "First item";
		item1.Price = 100m;
		session.Save(item1);
		
		Item item2 = new Item();
		item2.Description = "Second item";
		item2.Price = 150m;
		session.Save(item2);

		session.Transaction.Commit();

		Status.Text = "Schema created";
	}

	protected void DropSchema_Click(object sender, EventArgs e)
	{
		SchemaExport export = new SchemaExport(ExampleApplication.Configuration);
		export.Drop(false, true);
		Status.Text = "Schema dropped";
	}
}
