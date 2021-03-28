using System.Collections.Generic;

namespace NHibernate.Test.Hql
{
	public class Person
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IDictionary<int, string > Localized { get; set; } = new Dictionary<int, string>();
	}

	public class Document
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IDictionary<int, Person> Contacts { get; set; } = new Dictionary<int, Person>();
	}
}
