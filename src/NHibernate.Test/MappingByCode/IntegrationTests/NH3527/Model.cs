using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3527
{
	public abstract class EntityBase
	{
		public virtual int Id { get; set; }
	}

	public abstract class Item : EntityBase
	{
	}

	public class InventoryItem : Item
	{
	}
}
