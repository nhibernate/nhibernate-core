namespace NHibernate.Test.NHSpecificTest.NH1101
{
	public class A
	{
#pragma warning disable 649
		private int id;
#pragma warning restore 649
		private string descript;
		private B b;
		public A() {}
		public A(string descript)
		{
			this.descript = descript;
		}

		public virtual int Id
		{
			get { return id; }
		}

		public virtual string Descript
		{
			get { return descript; }
			set { descript = value; }
		}

		public virtual B B
		{
			get
			{
				if (b == null)
					b = new B();
				return b;
			}
			set { b = value; }
		}
	}
	public class B
	{
		private string str;
		private int? nInt;

		public virtual string Str
		{
			get { return str; }
			set { str = value; }
		}

		public virtual int? NInt
		{
			get { return nInt; }
			set { nInt = value; }
		}
	}
}