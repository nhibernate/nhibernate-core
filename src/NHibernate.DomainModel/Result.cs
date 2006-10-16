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
		private long _count;// changed from int to long (H3.2)

		private FooStatus _status;

		public Result(string name, long amount, int count) 
		{
			_name = name;
			_amount = amount;
			_count = count;
		}

		// added for H3.2
		public Result(string name, long amount, long count)
		{
			_name = name;
			_amount = amount;
			_count = count;
		}

		// added this ctor to test Enums in ctor for query results
		public Result(string name, long amount, int count, FooStatus status)
			: this(name, amount, count)
		{
			_status = status;
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

		public long Count
		{
			get { return _count; }
			set { _count = value; }
		}

		public FooStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		

	}
}
