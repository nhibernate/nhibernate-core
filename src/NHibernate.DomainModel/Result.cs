using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Result.
	/// </summary>
	public class Result 
	{
		private string _name;
		private long _amount;
		private int _count;
		
		public Result(string name, long amount, int count) 
		{
		}
		
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public long Amount
		{
			get { return _amount; }
			set { _amount = value; }
		}

		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

	}
}
