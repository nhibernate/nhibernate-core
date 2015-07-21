namespace NHibernate.Test.Any
{
	public class StringPropertyValue: IPropertyValue
	{
		private long id;
		private string value;

		public StringPropertyValue() {}

		public StringPropertyValue(string value)
		{
			this.value = value;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Value
		{
			get { return value; }
			set { this.value = value; }
		}

		public virtual string AsString()
		{
			return value;
		}
	}
}