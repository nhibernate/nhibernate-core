namespace NHibernate.Test.Join
{
	public class Worker
	{
		private long _Id;
		public virtual long Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private string _Name;
		public virtual string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private string _Junk = "junk";
		public virtual string Junk
		{
			get { return _Junk; }
			set { _Junk = value; }
		}
	}

	public class PaidWorker : Worker
	{
		private decimal _Wage;
		public virtual decimal Wage
		{
			get { return _Wage; }
			set { _Wage = value; }
		}
	}
}
