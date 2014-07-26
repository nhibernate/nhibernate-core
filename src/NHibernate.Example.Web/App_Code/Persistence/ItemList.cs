using System;
using System.Collections.Generic;
using NHibernate.Example.Web.Domain;

namespace NHibernate.Example.Web.Persistence
{
	public class ItemList
	{
		public IList<Item> GetAllItems()
		{
			return ExampleApplication.GetCurrentSession().CreateQuery("from Item").List<Item>();
		}
		
		public void UpdateItem(Item item)
		{
			ExampleApplication.GetCurrentSession().Merge(item);
		}
		
		public void DeleteItem(Item item)
		{
			ISession session = ExampleApplication.GetCurrentSession();
			session.Delete(session.Load(typeof(Item), item.Id));
		}
		
		public void InsertItem(Item item)
		{
			ExampleApplication.GetCurrentSession().Save(item);
		}
	}
}