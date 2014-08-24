using System;

namespace NHibernate.Test.FilterTest
{
	public class BinaryFiltered
	{
		private int id;
		private byte[] binValue;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual byte[] BinValue
		{
			get { return binValue; }
			set { binValue = value; }
		}
	}
}