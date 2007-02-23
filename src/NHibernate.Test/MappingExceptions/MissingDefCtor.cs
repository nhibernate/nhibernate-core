using System;

namespace NHibernate.Test.MappingExceptions
{
	/// <summary>
	/// Summary description for MissingDefCtor.
	/// </summary>
	public class MissingDefCtor
	{
		private int _id;
		private string _something;

		public MissingDefCtor(string something)
		{
			_something = something;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Something
		{
			get { return _something; }
			set { _something = value; }
		}
	}
}