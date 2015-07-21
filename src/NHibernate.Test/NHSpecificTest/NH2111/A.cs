using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2111
{
	public class A
	{
		private int? _id;
		private string _name;
		private IList<string> _lazyItems;

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public IList<string> LazyItems
		{
			get { return _lazyItems; }
			set { _lazyItems = value; }
		}
	}
}