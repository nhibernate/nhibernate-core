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
	public class ViewDataModel : PageModel
	{
		public IList<Item> Items { get; set; }
		public int? Editing { get; set; }

		private readonly ISession _session;

		public ViewDataModel(ISession session)
		{
			_session = session;
		}

		public void OnGet(int? editing)
		{
			Editing = editing;

			using (var tx = _session.BeginTransaction())
			{
				Items = _session.QueryOver<Item>().List();
			}
		}

		public IActionResult OnPostEdit(int id)
		{
			return RedirectToPage(new { editing = id });
		}

		public IActionResult OnPostDelete(int id)
		{
			using (var tx = _session.BeginTransaction())
			{
				var item = _session.Get<Item>(id);
				if (item != null)
				{
					_session.Delete(item);
				}

				tx.Commit();
			}

			return RedirectToPage();
		}

		public IActionResult OnPostUpdate(int id, UpdateItemEvent updateItem)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			using (var tx = _session.BeginTransaction())
			{
				var item = _session.Get<Item>(id);
				if (item != null)
				{
					item.Price = updateItem.Price;
					item.Description = updateItem.Description;
					_session.Update(item);
				}

				tx.Commit();
			}

			return RedirectToPage();
		}

		public IActionResult OnPostCancelUpdate()
		{
			return RedirectToPage();
		}

		public class UpdateItemEvent
		{
			[Required, Range(0.0, double.MaxValue)]
			public decimal Price { get; set; }
			public string Description { get; set; }
		}
	}
}
