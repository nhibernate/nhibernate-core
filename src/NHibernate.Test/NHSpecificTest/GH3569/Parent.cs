using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.GH3569
{
	public class Parent
	{
		private int id;
		private ContainedChild containedChild;
		private ISet<ContainedChild> containedChildren = new HashSet<ContainedChild>();

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ContainedChild ContainedChild
		{
			get { return containedChild; }
			set { containedChild = value; }
		}
		public virtual ISet<ContainedChild> ContainedChildren
		{
			get { return containedChildren; }
			set { containedChildren = value; }
		}
	}
}
