using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1264
{
   public class Name
	{
		private string first;
		private string last;
		private string display;
		
		public string First
		{
			get { return first; }
			set { first = value; }
		}

		public string Last
		{
			get { return last; }
			set { last = value; }
		}

		public string Display
		{
			get { return display ?? first + " " + last; }
			set { display = value; }
		}	
	}
}
