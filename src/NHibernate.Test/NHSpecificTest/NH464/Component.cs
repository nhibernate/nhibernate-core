using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH464
{
	public class Component
	{
		private IList dates = new ArrayList();

		public IList Dates
		{
			get { return dates; }
		}
	}
}
