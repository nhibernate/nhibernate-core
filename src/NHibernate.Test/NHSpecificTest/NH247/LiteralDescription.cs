using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH247
{
	public class LiteralDescription
	{
		public LiteralDescription() : base() { }
		public LiteralDescription(string description)
			: this()
		{
			_description = description;
		}

		private int _id;
		public int Id
		{
			get { return _id; }
		}

		private string _description = string.Empty;
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

	}
}
