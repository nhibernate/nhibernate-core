using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NHibernate.Example.Web.Infrastructure;
using NHibernate.Example.Web.Models;
using NHibernate.Tool.hbm2ddl;

namespace NHibernate.Example.Web.Pages
{
	public class SchemaModel : PageModel
	{
		[TempData]
		public string Status { get; set; }

		private readonly AppSessionFactory _applicationSession;
		private readonly ISession _session;

		public SchemaModel(ISession session, AppSessionFactory applicationSession)
		{
			_applicationSession = applicationSession;
			_session = session ?? throw new ArgumentNullException(nameof(session));
		}

		public void OnGet()
		{
		}

		public IActionResult OnPostCreateSchema()
		{
			var export = new SchemaExport(_applicationSession.Configuration);
			export.Create(false, true);

			using (var tx = _session.BeginTransaction())
			{
				var item1 = new Item
				{
					Description = "First item",
					Price = 100m
				};
				_session.Save(item1);

				var item2 = new Item
				{
					Description = "Second item",
					Price = 150m
				};
				_session.Save(item2);

				tx.Commit();
			}

			Status = "Schema created";
			return RedirectToPage();
		}

		public IActionResult OnPostDropSchema()
		{
			SchemaExport export = new SchemaExport(_applicationSession.Configuration);
			export.Drop(false, true);

			Status = "Schema dropped";
			return RedirectToPage();
		}
	}
}
