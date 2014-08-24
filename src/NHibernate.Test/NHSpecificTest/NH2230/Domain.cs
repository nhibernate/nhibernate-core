using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2230
{
	public class MyEntity
	{
		public virtual MyComponentWithParent Component { get; set; }
		public virtual ICollection<MyComponentWithParent> Children { get; set; }
	}

	public class MyComponentWithParent
	{
		private MyEntity parent;

		protected MyComponentWithParent()
		{
		}

		public MyComponentWithParent(MyEntity parent)
		{
			this.parent = parent;
		}

		public MyEntity Parent
		{
			get { return parent; }
		}

		public string Something { get; set; }
	}
}