using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2201
{
	public class Person
	{
		private Detail _detail;

		public Person()
		{
			this.Children = new HashSet<Person>();
		}

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Person Parent { get; set; }
		public virtual ISet<Person> Children { get; protected set; }
		public virtual Detail Details
		{
			get { return _detail; }
			set
			{
				_detail = value;

				if (_detail != null)
				{
					_detail.Person = this;
				}
			}
		}
	}
}
