using System;

namespace NHibernate.Test.NHSpecificTest.NH247
{
	public class LiteralDescription
	{
		public LiteralDescription() : base()
		{
		}

		public LiteralDescription(string description)
			: this()
		{
			_description = description;
		}

#pragma warning disable 649
		private int _id;
#pragma warning restore 649

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