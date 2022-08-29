using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NHibernate.Example.Web.Models;

namespace NHibernate.Example.Web.Pages
{
	public class InsertItemModel : PageModel
	{
		[BindProperty]
		public InsertItemEvent NewItem { get; set; }

		private readonly ISession _session;

		public InsertItemModel(ISession session)
		{
			_session = session;
		}

		public void OnGet()
		{
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			using (var tx = _session.BeginTransaction())
			{
				var item = new Item
				{
					Price = NewItem.Price,
					Description = NewItem.Description,
				};
				_session.Save(item);

				tx.Commit();
			}

			return RedirectToPage("/ViewData");
		}

		public IActionResult OnPostCancel()
		{
			return RedirectToPage("/ViewData");
		}

		public class InsertItemEvent
		{
			[Required, Range(0.0, double.MaxValue)]
			public decimal Price { get; set; }

			public string Description { get; set; }
		}
	}
}
