using System;

namespace NHibernate.DomainModel.NHSpecific 
{

	/// <summary>
	/// Summary description for ClassWithNullColumns.
	/// </summary>
	public class ClassWithNullColumns 
	{
		
		private int _id;
		private int _firstInt32;
		private int _secondInt32;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int FirstInt32
		{
			get { return _firstInt32; }
			set { _firstInt32 = value; }
		}

		public int SecondInt32
		{
			get { return _secondInt32; }
			set { _secondInt32 = value; }
		}

	}
}
