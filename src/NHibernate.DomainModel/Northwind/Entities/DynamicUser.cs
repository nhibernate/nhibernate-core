using System.Collections;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class DynamicUser
	{
		public virtual int Id { get; set; }

		public virtual dynamic Properties { get; set; }

		public virtual IDictionary Settings { get; set; }
	}
}
