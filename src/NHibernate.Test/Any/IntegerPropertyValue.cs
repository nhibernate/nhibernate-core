namespace NHibernate.Test.Any
{
	public class IntegerPropertyValue : IPropertyValue
	{
		private long id;
		private int value;
		public IntegerPropertyValue() {}

		public IntegerPropertyValue(int value)
		{
			this.value = value;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual int Value
		{
			get { return value; }
			set { this.value = value; }
		}

		#region IPropertyValue Members

		public virtual string AsString()
		{
			return value.ToString();
		}

		#endregion
	}
}