using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NHNewBug
{
	public class ContainedChild
	{
		private int id;
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
		private Child child;

		public ContainedChild()
		{
		}

		public ContainedChild(Child child)
		{
			this.child = child;
		}
		
		public virtual Child Child
		{
			get { return child; }
			set { child = value; }
		}
	}
}
