using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.NHNewBug
{
	public class Child
	{
		private int id;
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
