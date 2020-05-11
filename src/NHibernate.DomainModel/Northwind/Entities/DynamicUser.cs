using System.Collections;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class DynamicUser : IEnumerable
	{
		public virtual int Id { get; set; }

		public virtual dynamic Properties { get; set; }

		public virtual IDictionary Settings { get; set; }

		public virtual IEnumerator GetEnumerator()
		{
			throw new System.NotImplementedException();
		}
	}
}
