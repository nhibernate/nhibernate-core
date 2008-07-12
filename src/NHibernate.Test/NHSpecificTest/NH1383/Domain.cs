namespace NHibernate.Test.NHSpecificTest.NH1383
{
	public class MyNullableComponent
	{
		private int? _i1;
		private int _i2;

		public int? I1
		{
			get { return _i1; }
			set { _i1 = value; }
		}

		public int I2
		{
			get { return _i2; }
			set { _i2 = value; }
		}
	}

	public class MyComponent
	{
		private int _i1;
		private int _i2;

		public int I1
		{
			get { return _i1; }
			set { _i1 = value; }
		}

		public int I2
		{
			get { return _i2; }
			set { _i2 = value; }
		}
	}

	public class MyBO
	{
		private MyComponent _comp;
		private int _id;
		private string _name;
		private MyNullableComponent _nullableComp;

		public MyComponent Comp
		{
			get { return _comp; }
			set { _comp = value; }
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public MyNullableComponent NullableComp
		{
			get { return _nullableComp; }
			set { _nullableComp = value; }
		}
	}
}