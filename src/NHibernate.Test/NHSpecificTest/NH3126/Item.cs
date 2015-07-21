using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3126
{
	public class Item
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IDictionary<Guid, PropertyValue> PropertyValues { get; set; }
	}
}