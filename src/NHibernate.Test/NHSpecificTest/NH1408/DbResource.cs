using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1408
{
	public abstract class DbResource : Entity
	{
		private readonly IList<DbResourceKey> keys = new List<DbResourceKey>();
		private string rawValue;

		protected virtual string RawValue
		{
			get { return rawValue; }
			set { rawValue = value; }
		}

		public virtual IEnumerable<DbResourceKey> Keys
		{
			get { return keys; }
		}

		public abstract object Value { get; set; }

		public virtual void AddKey(DbResourceKey key)
		{
			if (keys.Contains(key))
			{
				return;
			}
			keys.Add(key);
			key.Resource = this;
		}
	}

	internal class StringDbResource : DbResource
	{
		protected StringDbResource() {}

		public StringDbResource(string key, string language, string value)
		{
			AddKey(new DbResourceKey(key, language));
			Value = value;
		}

		public override object Value
		{
			get { return RawValue; }
			set { RawValue = (string) value; }
		}
	}

	internal class IntDbResource : DbResource
	{
		protected IntDbResource() {}

		public IntDbResource(string key, int xml)
		{
			AddKey(new DbResourceKey(key));
			Value = xml;
		}

		public override object Value
		{
			get { return int.Parse(RawValue); }
			set { RawValue = ((int) value).ToString(); }
		}
	}

	internal class DecimalDbResource : DbResource
	{
		protected DecimalDbResource() {}

		public DecimalDbResource(string key, decimal xml)
		{
			AddKey(new DbResourceKey(key));
			Value = xml;
		}

		public override object Value
		{
			get { return decimal.Parse(RawValue); }
			set { RawValue = ((decimal) value).ToString(); }
		}
	}
}